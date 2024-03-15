
using System.Numerics;
using _02_RayTracing.Rendering;

namespace _02_RayTracing.Materials;
public interface IMaterial
{

    Vector3 BRDFS(ref Vector3 inDirection, ref Vector3 outDirection, HitPoint hitPoint);

    Vector3 GetEmission(HitPoint hitPoint);
}
