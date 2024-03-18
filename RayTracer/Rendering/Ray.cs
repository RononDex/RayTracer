using System.Numerics;
using RayTracer.Models;

namespace RayTracer.Rendering;

public class Ray(Vector3 origin, Vector3 direction, IGeometry? comingFromGeometry = null)
{

    public Vector3 Origin { get; private set; } = origin;
    public Vector3 Direction { get; private set; } = direction;
    public IGeometry? ComingFromGeometry { get; private set; } = comingFromGeometry;
}
