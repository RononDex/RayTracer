
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Shaders;

namespace Rasterizer.Models;

public class Sphere : IGeometry
{
    public List<Vertex> Vertices { get; private set; } = new List<Vertex>();

    public List<(int A, int B, int C)> Triangles { get; private set; } = new List<(int A, int B, int C)>();

    public IVertexShader VertexShader => Rasterizer.Shaders.VertexShader.GetSingletonInstance();

    public Matrix4x4 ModelMatrix { get; set; }

    public List<IFragmentShader> FragmentShaders { get; private set; }

    public Sphere(int patches, Vector3 color, Matrix4x4 modelMatrix, List<IFragmentShader> fragmentShaders)
    {
        MeshGenerator.AddSphere(this.Vertices, this.Triangles, patches, color);

        this.FragmentShaders = fragmentShaders;
        this.ModelMatrix = modelMatrix;
    }
}
