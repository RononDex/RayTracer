
using System.Collections.Generic;
using System.Numerics;
using _02_RayTracing.Materials;
using _02_RayTracing.Rendering;

namespace _02_RayTracing.Models;
public interface IGeometry
{
    public Vector3 Position { get; set; }

    public IMaterial Material { get; }

    List<HitPoint> Intersect(Ray ray);
}
