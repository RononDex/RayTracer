
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Shaders;

namespace Rasterizer.Models;

public interface IGeometry
{
    List<Vertex> Vertices { get; }

    List<(int A, int B, int C)> Triangles { get; }

    IVertexShader VertexShader { get; }

    Matrix4x4 ModelMatrix { get; set; }

    List<IFragmentShader> FragmentShaders { get; }
}
