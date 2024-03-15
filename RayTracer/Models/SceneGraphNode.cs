
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Shaders;

namespace Rasterizer.Models;

public class SceneGraphNode : IGeometry
{
    public List<Vertex> Vertices { get; private set; } = new List<Vertex>();

    public List<(int A, int B, int C)> Triangles { get; private set; } = new List<(int A, int B, int C)>();

    public IVertexShader VertexShader => Rasterizer.Shaders.VertexShader.GetSingletonInstance();

    public Matrix4x4 ModelMatrix { get; set; }

    public List<IFragmentShader> FragmentShaders { get; private set; }

    public Dictionary<string, SceneGraphNodeReference> Children { get; private set; } = new Dictionary<string, SceneGraphNodeReference>();

    public SceneGraphNode()
    {
        this.ModelMatrix = Matrix4x4.Identity;
    }

    public SceneGraphNode(IGeometry obj)
    {
        this.Triangles = obj.Triangles;
        this.Vertices = obj.Vertices;
        this.ModelMatrix = obj.ModelMatrix;
    }
}
