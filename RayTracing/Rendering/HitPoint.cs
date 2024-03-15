using System.Numerics;
using _02_RayTracing.Models;

namespace _02_RayTracing.Rendering;

public class HitPoint
{
    public Vector3 Position { get; private set; }

    public Vector3 SurfaceNormal { get; private set;}

    public IGeometry HitObject { get; private set; }

    public HitPoint(Vector3 position, IGeometry hitObject, Vector3 surfaceNormal)
    {
        this.Position = position;
        this.HitObject = hitObject;
        this.SurfaceNormal = surfaceNormal;
    }
}
