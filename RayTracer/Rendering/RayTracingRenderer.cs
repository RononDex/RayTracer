using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using RayTracer.Models;

namespace RayTracer.Rendering;

public class RayTracingRenderer(int raysPerPixel)
{
    private readonly int raysPerPixel = raysPerPixel;
    public Vector3[,] CurrentRendering { get; set; }

    public Task<Vector3[,]> RenderSceneAsync(Scene scene, int width, int height)
    {
        return Task.Run(() =>
        {
            this.CurrentRendering = new Vector3[width, height];
            var columns = this.CurrentRendering.GetUpperBound(1) + 1;

            Enumerable.Range(0, this.CurrentRendering.GetUpperBound(0) + 1)
            .AsParallel()
#if DEBUG
            .WithDegreeOfParallelism(1)
#endif
            .ForAll((row) =>
                    {
                        for (var col = 0; col < columns; col++)
                        {
                            this.CurrentRendering[col, row] = this.GetColor(scene, new Vector2(col, row), height, width);
                        }
                    });

            return this.CurrentRendering;
        });
    }

    public Ray[] CreateEyeRays(Vector3 eyePos, Vector3 lookAt, Vector3 up, float fovHorizontal, float fovVertical, Vector2 pixel, float pixelSizeX, float pixelSizeY, float gauss_sigma)
    {
        var rays = new Ray[this.raysPerPixel];
        var f = Vector3.Normalize(lookAt - eyePos);
        var r = Vector3.Cross(up, f);
        var u = Vector3.Cross(f, r);
        for (var i = 0; i < this.raysPerPixel; i++)
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
        var closestDistance = float.MaxValue;
        for (var i = 0; i < scene.Objects.Count; i++)
        {
            var hitpoint = scene.Objects[i].Intersect(ray);
            if (hitpoint == null)
            {
                continue;
            }

            var distance = Vector3.DistanceSquared(ray.Origin, hitpoint!.Position);
            if (distance < closestDistance)
            {
                closestHitpoint = hitpoint;
                closestDistance = distance;
            }
        }

        return closestHitpoint;
    }

    public Vector3 GetColor(Scene scene, Vector2 pixel, int height, int width)
    {
        var screenSpace = RenderingUtils.PixelToScreenProjectionSpace((int)pixel.X, (int)pixel.Y, width, height);
        var rays = this.CreateEyeRays(
                scene.CameraPosition,
                scene.LookAtPosition,
                scene.Up,
                scene.FieldOfView,
                scene.FieldOfView * (height / (float)width),
                screenSpace,
                2f / width,
                2f / height,
                scene.Gauss_Sigma);
        var summedColor = new Vector3();
        var pRecursion = 1f / scene.NumberOfBounces;
        foreach (var ray in rays)
        {
            summedColor += GetColorRecursive(scene, ray, pRecursion);
        }

        return summedColor / this.raysPerPixel;
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
        else
        {
            if (closestHitPoint.HitObject.Material.HasRefraction)
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

                if (!isReflection)
                {
                    var refractedDirection = quotient * ray.Direction + (((quotient * cosine) - MathF.Sqrt(1 - (quotient * quotient * (1 - (cosine * cosine))))) * closestHitPoint.SurfaceNormal);
                    var inDirection = ray.Direction;
                    var refractedRay = new Ray(closestHitPoint.Position, Vector3.Normalize(refractedDirection), closestHitPoint.HitObject);
                    return MathF.PI * closestHitPoint.HitObject.Material.BRDFS(ref inDirection, ref refractedDirection, closestHitPoint) * GetColorRecursive(scene, refractedRay, pRecursion, depth + 1);
                }
                else
                {
                    var reflectedDirection = Vector3.Normalize(Vector3.Reflect(ray.Direction, closestHitPoint.SurfaceNormal));
                    var reflectedRay = new Ray(closestHitPoint.Position, Vector3.Normalize(reflectedDirection), closestHitPoint.HitObject);
                    var inDirection = ray.Direction;
                    return MathF.PI * closestHitPoint.HitObject.Material.BRDFS(ref inDirection, ref reflectedDirection, closestHitPoint) * GetColorRecursive(scene, reflectedRay, pRecursion, depth + 1);
                }
            }
            else
            {
                var result = new Vector3();
                var randomVector = GetRandomVector();
                if (Vector3.Dot(randomVector, closestHitPoint.SurfaceNormal) < 0)
                {
                    randomVector = -randomVector;
                }

                randomVector = Vector3.Normalize(randomVector);
                var direction = ray.Direction;

                result += closestHitPoint.HitObject.Material.GetEmission(closestHitPoint) +
                    (2f * MathF.PI / (1f - pRecursion)
                    * Vector3.Dot(randomVector, closestHitPoint.SurfaceNormal)
                    * closestHitPoint.HitObject.Material.BRDFS(ref randomVector, ref direction, closestHitPoint)
                    * GetColorRecursive(scene, new Ray(closestHitPoint.Position, randomVector), pRecursion, depth + 1));

                return result;
            }
        }
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
        while (randomVect.Length() > 1)
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
