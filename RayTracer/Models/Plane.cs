
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Shaders;

namespace Rasterizer.Models;

public class Plane : IGeometry
{
    public List<Vertex> Vertices { get; private set; } = new List<Vertex>();

    public List<(int A, int B, int C)> Triangles { get; private set; } = new List<(int A, int B, int C)>();

    public IVertexShader VertexShader => Rasterizer.Shaders.VertexShader.GetSingletonInstance();

    public Matrix4x4 ModelMatrix { get; set; }

    public List<IFragmentShader> FragmentShaders { get; private set; }

    public Plane(Matrix4x4 modelMatrix, List<IFragmentShader> fragmentShaders, float uvWidth = 1, float uvHeight = 1)
    {
        this.ModelMatrix = modelMatrix;
        this.Vertices.Add(new Vertex(
                    new Vector3(-1, -1, 0),
                    new Vector3(0.5f),
                    new Vector2(0, 0),
                    new Vector3(0, 0, -1)));
        this.Vertices.Add(new Vertex(
                    new Vector3(1, -1, 0),
                    new Vector3(0.5f),
                    new Vector2(uvWidth, 0),
                    new Vector3(0, 0, -1)));
        this.Vertices.Add(new Vertex(
                    new Vector3(1, 1, 0),
                    new Vector3(0.5f),
                    new Vector2(uvWidth, uvHeight),
                    new Vector3(0, 0, -1)));
        this.Vertices.Add(new Vertex(
                    new Vector3(-1, 1, 0),
                    new Vector3(0.5f),
                    new Vector2(0, uvHeight),
                    new Vector3(0, 0, -1)));

        this.Triangles.Add((0, 1, 2));
        this.Triangles.Add((0, 2, 3));

        this.FragmentShaders = fragmentShaders;
    }
}
