using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using RayTracer.Geometry;

namespace RayTracer.Rendering;

public class RayTracingRenderer()
{
    public Vector3[,] CurrentRendering { get; set; }

    public Task<Vector3[,]> RenderSceneAsync(Scene scene, int raysPerPixel, int width, int height, int nThreads)
    {

        return Task.Run(() =>
        {
            nThreads = int.Min(nThreads, Environment.ProcessorCount);
            var workPackages = new List<(int startRow, int startColumn, int endRow, int endColumn)>();
            var curWidth = 0;
            var curHeight = 0;
            var packageSize = (int)(3.5 * MathF.Log2(width));
            while (curWidth < width)
            {
                while (curHeight < height)
                {
                    workPackages.Add((startRow: curHeight, startColumn: curWidth, endRow: int.Min(curHeight + packageSize - 1, height - 1), endColumn: int.Min(curWidth + packageSize - 1, width - 1)));
                    curHeight += packageSize;
                }
                curWidth += packageSize;
                curHeight = 0;
            }
            workPackages = [.. workPackages.OrderBy(_ => Random.Shared.Next())];

            this.CurrentRendering = new Vector3[width, height];
            workPackages
            .AsParallel()
#if DEBUG
            .WithDegreeOfParallelism(1)
#else
            .WithDegreeOfParallelism(nThreads)
#endif
            .ForAll((package) =>
                    {
                        for (var row = package.startRow; row <= package.endRow; row++)
                        {
                            for (var col = package.startColumn; col <= package.endColumn; col++)
                            {
                                this.CurrentRendering[col, row] = GetColor(scene, new Vector2(col, row), raysPerPixel, height, width);
                            }
                        }
                    });

            return this.CurrentRendering;
        });
    }

    private static Ray[] CreateEyeRays(Vector3 eyePos, Vector3 lookAt, Vector3 up, float fovHorizontal, float fovVertical, Vector2 pixel, float pixelSizeX, float pixelSizeY, float gauss_sigma, int raysPerPixel)
    {
        var rays = new Ray[raysPerPixel];
        var f = Vector3.Normalize(lookAt - eyePos);
        var r = Vector3.Cross(up, f);
        var u = Vector3.Cross(f, r);
        for (var i = 0; i < raysPerPixel; i++)
        {
            var locX = pixel.X + (pixelSizeX * RandomGaussian(Random.Shared, 0, MathF.Pow(gauss_sigma, 2)));
            var locY = pixel.Y + (pixelSizeY * RandomGaussian(Random.Shared, 0, MathF.Pow(gauss_sigma, 2)));
            var d = f +
                    (locY * (float)Math.Tan(fovVertical / 2) * u) +
                    (locX * (float)Math.Tan(fovHorizontal / 2) * r);
            rays[i] = new Ray(eyePos, Vector3.Normalize(d));
        }

        return rays;
    }

    public static HitPoint? FindClosestHitPoint(Scene scene, Ray ray)
    {
        HitPoint? closestHitpoint = null;
        var closestDistanceSquared = float.MaxValue;
        for (var i = 0; i < scene.Objects.Count; i++)
        {
            var hitpoint = scene.Objects[i].Intersect(ray);
            if (hitpoint == null)
            {
                continue;
            }

            var distance = Vector3.DistanceSquared(ray.Origin, hitpoint!.Position);
            if (distance < closestDistanceSquared)
            {
                closestHitpoint = hitpoint;
                closestDistanceSquared = distance;
            }
        }

        return closestHitpoint;
    }

    public static Vector3 GetColor(Scene scene, Vector2 pixel, int raysPerPixel, int height, int width)
    {
        var screenSpace = RenderingUtils.PixelToScreenProjectionSpace((int)pixel.X, (int)pixel.Y, width, height);
        var rays = CreateEyeRays(
                scene.CameraPosition,
                scene.LookAtPosition,
                scene.Up,
                scene.FieldOfView,
                scene.FieldOfView * (height / (float)width),
                screenSpace,
                2f / width,
                2f / height,
                scene.Gauss_Sigma,
                raysPerPixel);
        var summedColor = new Vector3();
        var pRecursion = 1f / scene.NumberOfBounces;
        foreach (var ray in rays)
        {
            summedColor += GetColorRecursive(scene, ray, pRecursion);
        }

        return summedColor / raysPerPixel;
    }

    public static Vector3 GetColorRecursive(Scene scene, Ray ray, float pRecursion, int depth = 0)
    {
        var closestHitPoint = FindClosestHitPoint(scene, ray);
        var rand = Random.Shared.NextSingle();

        if (closestHitPoint == null)
        {
            return new Vector3();
        }

        if (depth > 0 && rand < pRecursion)
        {
            return closestHitPoint.HitObject.Material.GetEmission(closestHitPoint);
        }

        if (closestHitPoint.HitObject.Material.HasRefraction)
        {
            return HandleRefraction(scene, ray, pRecursion, depth, closestHitPoint!);
        }

        var result = new Vector3();
        var randomVector = GetRandomVector();
        var dot = Vector3.Dot(randomVector, closestHitPoint.SurfaceNormal);
        if (dot < 0)
        {
            randomVector = -randomVector;
            dot = -dot;
        }

        randomVector = Vector3.Normalize(randomVector);
        var direction = ray.Direction;

        result += closestHitPoint.HitObject.Material.GetEmission(closestHitPoint) +
            (2f * MathF.PI / (1f - pRecursion)
            * dot
            * closestHitPoint.HitObject.Material.BRDFS(ref randomVector, ref direction, closestHitPoint)
            * GetColorRecursive(scene, new Ray(closestHitPoint.Position, randomVector), pRecursion, depth + 1));

        return result;
    }

    private static Vector3 HandleRefraction(Scene scene, Ray ray, float pRecursion, int depth, HitPoint closestHitPoint)
    {
        var isReflection = false;
        var cosine = Vector3.Dot(ray.Direction, closestHitPoint.SurfaceNormal);
        var oldRefractionIndex = 1.0f; // air
        if (ray.ComingFromGeometry != null)
        {
            oldRefractionIndex = ray.ComingFromGeometry.Material.RefractionIndex;
        }
        var newRefractionIndex = closestHitPoint.HitObject.Material.RefractionIndex;
        if (cosine > 0)
        {
            newRefractionIndex = 1.0f; //air
        }
        var quotient = oldRefractionIndex / newRefractionIndex;
        if (newRefractionIndex > oldRefractionIndex)
        {
            var criticalAngle = MathF.Asin(newRefractionIndex / oldRefractionIndex);
            var isTotalInternalReflection = MathF.Abs(criticalAngle) < MathF.Abs(MathF.Acos(cosine));
            isReflection = GetIsReflection(oldRefractionIndex, newRefractionIndex, isTotalInternalReflection, cosine);
        }

        var inDirection = ray.Direction;
        if (!isReflection)
        {
            var refractedDirection = quotient * ray.Direction + (((quotient * cosine) - MathF.Sqrt(1 - (quotient * quotient * (1 - (cosine * cosine))))) * closestHitPoint.SurfaceNormal);
            var refractedRay = new Ray(closestHitPoint.Position, Vector3.Normalize(refractedDirection), closestHitPoint.HitObject);
            return MathF.PI * closestHitPoint.HitObject.Material.BRDFS(ref inDirection, ref refractedDirection, closestHitPoint) * GetColorRecursive(scene, refractedRay, pRecursion, depth + 1);
        }

        var reflectedDirection = Vector3.Normalize(Vector3.Reflect(ray.Direction, closestHitPoint.SurfaceNormal));
        var reflectedRay = new Ray(closestHitPoint.Position, Vector3.Normalize(reflectedDirection), closestHitPoint.HitObject);

        return GetColorRecursive(scene, reflectedRay, pRecursion, depth + 1);
    }

    private static bool GetIsReflection(float oldRefractionIndex, float newRefractionIndex, bool isTotalInternalReflection, float cosIncoming)
    {
        if (isTotalInternalReflection)
        {
            return true;
        }

        var cosTransmittance = MathF.Cos(MathF.Asin(oldRefractionIndex / newRefractionIndex * MathF.Sin(MathF.Acos(cosIncoming))));
        var cosine = oldRefractionIndex <= newRefractionIndex ? cosIncoming : cosTransmittance;
        var r0 = MathF.Pow((oldRefractionIndex - newRefractionIndex) / (oldRefractionIndex + newRefractionIndex), 2);
        var rSchlick = r0 + ((1 - r0) * MathF.Pow((1 - MathF.Abs(cosine)), 5));

        return Random.Shared.NextSingle() < rSchlick;
    }

    private static Vector3 GetRandomVector()
    {
        var randomVect = new Vector3(
                (2 * Random.Shared.NextSingle()) - 1,
                (2 * Random.Shared.NextSingle()) - 1,
                (2 * Random.Shared.NextSingle()) - 1);
        while (randomVect.LengthSquared() > 1)
        {
            randomVect = new Vector3(
                    (2 * Random.Shared.NextSingle()) - 1,
                    (2 * Random.Shared.NextSingle()) - 1,
                    (2 * Random.Shared.NextSingle()) - 1);
        }

        return randomVect;
    }

    public static float RandomGaussian(Random random, float mean, float stddev)
    {
        var x1 = 1 - random.NextSingle();
        var x2 = 1 - random.NextSingle();

        var y1 = MathF.Sqrt(-2.0f * MathF.Log(x1)) * MathF.Cos(2.0f * MathF.PI * x2);
        return y1 * stddev + mean;
    }
}

