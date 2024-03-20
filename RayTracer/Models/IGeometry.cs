using System.Numerics;
using RayTracer.Materials;
using RayTracer.Rendering;

namespace RayTracer.Models;
public interface IGeometry
{
    public Vector3 Position { get; set; }

    public IMaterial Material { get; }

    HitPoint? Intersect(Ray ray);
}
