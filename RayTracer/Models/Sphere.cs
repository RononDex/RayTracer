using System;
using System.Collections.Generic;
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

    public List<HitPoint> Intersect(Ray ray)
    {
        var hitpoints = new List<HitPoint>();
        var b = 2 * Vector3.Dot(ray.Origin - this.Position, ray.Direction);
        var c = Vector3.DistanceSquared(ray.Origin, this.Position) - MathF.Pow(this.Radius, 2);
        var b_squared = b * b;
        var quotient = 4 * c;
        if (b_squared < quotient)
        {
            return hitpoints;
        }
        else
        {
            var sqrt = MathF.Sqrt(b_squared - quotient);
            var lambda_1 = (-b + sqrt) / 2;
            if (lambda_1 > 0 && lambda_1 > EPSILON)
            {
                var hitLocation1 = ray.Origin + ((lambda_1 - EPSILON) * ray.Direction);
                hitpoints.Add(new HitPoint(hitLocation1, this, Vector3.Normalize(hitLocation1 - this.Position)));
            }
            if (b_squared > quotient)
            {
                var lambda_2 = (-b - sqrt) / 2;
                if (lambda_2 > 0 && lambda_2 > EPSILON)
                {
                    var hitLocation2 = ray.Origin + ((lambda_2 - EPSILON) * ray.Direction);
                    hitpoints.Add(new HitPoint(hitLocation2, this, Vector3.Normalize(hitLocation2 - this.Position)));
                }
            }

            return hitpoints;
        }
    }
}
