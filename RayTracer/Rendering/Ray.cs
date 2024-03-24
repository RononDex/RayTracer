using System.Numerics;
using RayTracer.Geometry;

namespace RayTracer.Rendering;

public class Ray(Vector3 origin, Vector3 direction, IRenderable? comingFromGeometry = null)
{

    public Vector3 Origin { get; private set; } = origin;
    public Vector3 Direction { get; private set; } = direction;
    public IRenderable? ComingFromGeometry { get; private set; } = comingFromGeometry;
}
