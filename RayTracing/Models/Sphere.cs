using System;
using System.Collections.Generic;
using System.Numerics;
using _02_RayTracing.Materials;
using _02_RayTracing.Rendering;

namespace _02_RayTracing.Models;

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
        var b = 2 * Vector3.Dot(ray.Origin - Position, ray.Direction);
        var c = Vector3.DistanceSquared(ray.Origin, Position) - MathF.Pow(Radius, 2);
        var b_squared = b * b;
        var quotient = 4 * c;
        var sqrt = MathF.Sqrt(b_squared - quotient);
        if (b_squared < quotient)
        {
            return hitpoints;
        }
        else
        {
            var lambda_1 = (-b + sqrt) / 2;
            var hitLocation1 = ray.Origin + (lambda_1 - EPSILON) * ray.Direction;
            if (lambda_1 > 0)
                hitpoints.Add(new HitPoint(hitLocation1, this, Vector3.Normalize(hitLocation1 - this.Position)));

            if (b_squared > quotient)
            {
                var lambda_2 = (-b - sqrt) / 2;
                var hitLocation2 = ray.Origin + (lambda_2 - EPSILON) * ray.Direction;
                if (lambda_2 > 0)
                    hitpoints.Add(new HitPoint(hitLocation2, this, Vector3.Normalize(hitLocation2 - this.Position)));
            }

            return hitpoints;
        }
    }
}
