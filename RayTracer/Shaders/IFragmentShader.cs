using System.Numerics;
using Rasterizer.Scenes;

namespace Rasterizer.Shaders;

public interface IFragmentShader
{
    Vector3 CalculateColor(ref Vertex q, IScene scene);
}
