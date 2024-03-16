
using System.Numerics;
using RayTracer.Rendering;

namespace RayTracer.Materials;

public interface IMaterial
{
    Vector3 BRDFS(ref Vector3 inDirection, ref Vector3 outDirection, HitPoint hitPoint);

    Vector3 GetEmission(HitPoint hitPoint);
}
