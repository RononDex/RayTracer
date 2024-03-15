
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Shaders;

namespace Rasterizer.Models;

public class Cube : IGeometry
{
    public List<Vertex> Vertices { get; private set; } = new List<Vertex>();

    public List<(int A, int B, int C)> Triangles { get; private set; } = new List<(int A, int B, int C)>();

    public IVertexShader VertexShader => Rasterizer.Shaders.VertexShader.GetSingletonInstance();

    public Matrix4x4 ModelMatrix { get; set; }

    public List<IFragmentShader> FragmentShaders { get; private set; }

    public Cube(
                    Matrix4x4 modelMatrix,
                    Vector3 frontColor,
                    Vector3 backColor,
                    Vector3 topColor,
                    Vector3 bottomColor,
                    Vector3 leftColor,
                    Vector3 rightColor,
                    List<IFragmentShader> fragmentShaders)
    {
        MeshGenerator.AddCube(this.Vertices, this.Triangles, frontColor, backColor, topColor, bottomColor, rightColor, leftColor);

        this.ModelMatrix = modelMatrix;
        this.FragmentShaders = fragmentShaders;
    }
}
