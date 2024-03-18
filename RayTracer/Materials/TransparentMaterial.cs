
using System.Numerics;
using RayTracer.Rendering;

namespace RayTracer.Materials;

public class TransparentMaterial : IMaterial
{
    public float RefractionIndex { get; private set; }

    public TransparentMaterial(float refractionIndex)
    {
        this.RefractionIndex = refractionIndex;
    }

    public Vector3 BRDFS(ref Vector3 inDirection, ref Vector3 outDirection, HitPoint hitPoint) => new();
    public Vector3 GetEmission(HitPoint hitPoint) => new();
    public bool HasRefraction => true;
}
