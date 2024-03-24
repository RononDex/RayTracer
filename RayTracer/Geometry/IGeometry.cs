using System.Numerics;
using RayTracer.Rendering;

namespace RayTracer.Geometry;
public interface IGeometry
{
    HitPoint? Intersect(Ray ray);

    IntersectionLambda[] GetAllIntersectionLambdas(Ray ray);
}

public class IntersectionLambda(float lambda, Vector3 surfaceNormal)
{
    public float Lambda { get; } = lambda;
    public Vector3 SurfaceNormal { get; } = surfaceNormal;
}
