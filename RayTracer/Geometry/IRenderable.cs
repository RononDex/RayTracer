using RayTracer.Materials;

namespace RayTracer.Geometry;
public interface IRenderable : IGeometry
{
    public IMaterial Material { get; }
}
