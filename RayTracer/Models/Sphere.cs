using System;
using System.Numerics;
using RayTracer.Materials;
using RayTracer.Rendering;

namespace RayTracer.Models;

public class Sphere : IGeometry
{
    private const float EPSILON = 0.001f;

    public float Radius { get; set; }
    public Vector3 Position { get; set; }

    public IMaterial Material { get; private set; }

    public Sphere(Vector3 position, float radius, IMaterial material)
    {
        this.Radius = radius;
        this.Position = position;
        this.Material = material;
    }

    public HitPoint? Intersect(Ray ray)
    {
        float? smallestValidLambda = null;
        var b = 2 * Vector3.Dot(ray.Origin - this.Position, ray.Direction);
        var c = Vector3.DistanceSquared(ray.Origin, this.Position) - MathF.Pow(this.Radius, 2);
        var b_squared = b * b;
        var quotient = 4 * c;
        if (b_squared < quotient)
        {
            return null;
        }

        var sqrt = MathF.Sqrt(b_squared - quotient);
        var lambda = (-b + sqrt) / 2;
        if (lambda > 0 && lambda > EPSILON)
        {
            smallestValidLambda = lambda;
        }

        if (b_squared > quotient)
        {
            var lambda_2 = (-b - sqrt) / 2;
            if (lambda_2 > 0 && lambda_2 > EPSILON)
            {
                smallestValidLambda = MathF.Min(smallestValidLambda ?? float.MaxValue, lambda_2);
            }
        }

        if (smallestValidLambda != null)
        {
            var hitLocation = ray.Origin + ((smallestValidLambda!.Value - EPSILON) * ray.Direction);
            return new HitPoint(hitLocation, this, Vector3.Normalize(hitLocation - this.Position));
        }
        return null;
    }
}
