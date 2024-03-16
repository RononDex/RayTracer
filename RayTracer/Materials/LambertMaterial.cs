
using System;
using System.Numerics;
using RayTracer.Rendering;

namespace RayTracer.Materials;

public class LambertMaterial : IMaterial
{
    public Vector3 DiffuseColor { get; private set; }
    public Vector3 EmissionColor { get; private set; }

    public LambertMaterial(Vector3 diffuseColor, Vector3? emissionColor = null)
    {
        this.DiffuseColor = diffuseColor;
        if (emissionColor == null)
        {
            this.EmissionColor = new Vector3(0);
        }
        else
        {
            this.EmissionColor = emissionColor.Value;
        }
    }

    public Vector3 BRDFS(ref Vector3 inDirection, ref Vector3 outDirection, HitPoint hitPoint)
    {
        return this.DiffuseColor / MathF.PI;
    }

    public Vector3 GetEmission(HitPoint hitPoint) { return this.EmissionColor; }
}
