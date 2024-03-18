
using System.Numerics;
using RayTracer.Rendering;

namespace RayTracer.Materials;

public class TransparentMaterial : IMaterial
{
    public float RefractionIndex { get; private set; }
    public IMaterial DiffuseMaterial { get; private set; }

    public TransparentMaterial(float refractionIndex, IMaterial? diffuseMaterialForReflections = null)
    {
        this.RefractionIndex = refractionIndex;
        if (diffuseMaterialForReflections == null)
        {
            this.DiffuseMaterial = new LambertMaterial(new Vector3(1, 1, 1));
        }
        else
        {
            this.DiffuseMaterial = diffuseMaterialForReflections;
        }
    }

    public Vector3 BRDFS(ref Vector3 inDirection, ref Vector3 outDirection, HitPoint hitPoint)
    {
        return this.DiffuseMaterial.BRDFS(ref inDirection, ref outDirection, hitPoint);
    }

    public Vector3 GetEmission(HitPoint hitPoint)
    {
        return this.DiffuseMaterial.GetEmission(hitPoint);
    }

    public bool HasRefraction => true;
}
