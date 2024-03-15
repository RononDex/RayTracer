
using System;
using System.Numerics;
using Rasterizer.Scenes;

namespace Rasterizer.Shaders;

public class SpecularPhongShader : IFragmentShader
{
    private readonly int k;
    private readonly float strength;
    private readonly Vector3 specularColor;

    public SpecularPhongShader(int k, float strength, Vector3 specularColor)
    {
        this.k = k;
        this.strength = strength;
        this.specularColor = specularColor;
    }

    public Vector3 CalculateColor(ref Vertex q, IScene scene)
    {
        var qe = Vector3.Normalize(scene.Camera.Position - q.WorldCoordinates);
        var n = Vector3.Normalize(q.Normal);
        var color = Vector3.Zero;
        foreach (var lightSource in scene.LightSources)
        {
            var ql = Vector3.Normalize(lightSource.WorldCoordinates - q.WorldCoordinates);

            var qlDotProduct = Vector3.Dot(n, ql);
            var reflDir = Vector3.Normalize(Vector3.Reflect(ql, n));
            var dotRefl = Vector3.Dot(-reflDir, qe);

            if (qlDotProduct > 0 && dotRefl > 0)
            {
                color += lightSource.Color * specularColor * MathF.Pow(dotRefl, k) * strength;
            }
        }

        return color;
    }
}
