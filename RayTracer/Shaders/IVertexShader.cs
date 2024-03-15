
using System.Numerics;

namespace Rasterizer.Shaders;

public interface IVertexShader
{
    Vertex Transform(Vertex vertex, Matrix4x4 modelViewProjectionMatrix, Matrix4x4 modelMatrix);
}
