
using System;
using System.Numerics;
using RayTracer.Rendering;

namespace RayTracer.Materials;

public class ReflectiveSpecularMaterial : IMaterial
{
    public Vector3 DiffuseColor { get; private set; }

    public Vector3 SpecularColor { get; private set; }

    public Vector3 EmissionColor { get; private set; }

    private float ReflectionMultiplier { get; set; }
    private float ReflectionEpsilon { get; set; }

    public ReflectiveSpecularMaterial(
            Vector3 diffuseColor,
            Vector3 specularColor,
            Vector3? emissionColor = null,
            float reflectionMultiplier = 10f,
            float reflectionEpsilon = 0.01f)
    {
        this.DiffuseColor = diffuseColor;
        this.SpecularColor = specularColor;
        this.ReflectionMultiplier = reflectionMultiplier;
        this.ReflectionEpsilon = reflectionEpsilon;

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
        var surfaceNormal = hitPoint.SurfaceNormal;
        var reflectionDirection = Vector3.Normalize(Vector3.Reflect(outDirection, surfaceNormal));

        if (Vector3.Dot(inDirection, reflectionDirection) > (1f - this.ReflectionEpsilon))
        {
            return this.DiffuseColor / MathF.PI + this.ReflectionMultiplier * SpecularColor;
        }
        else
        {
            return this.DiffuseColor / MathF.PI;
        }
    }

    public Vector3 GetEmission(HitPoint hitPoint) => EmissionColor;
}
