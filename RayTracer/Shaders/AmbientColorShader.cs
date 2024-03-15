
using System.Numerics;
using Rasterizer.Scenes;

namespace Rasterizer.Shaders;

public class AmbientColorShader : IFragmentShader
{
    private readonly float strength;

    public AmbientColorShader(float strength) {
        this.strength = strength;
    }

    public Vector3 CalculateColor(ref Vertex q, IScene scene) => strength * q.Color;
}
