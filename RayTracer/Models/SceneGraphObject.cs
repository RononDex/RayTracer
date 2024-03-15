
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Rasterizer.Shaders;

namespace Rasterizer.Models;

public abstract class SceneGraphObject : IGeometry
{
    public List<Vertex> Vertices { get; private set; } = new List<Vertex>();

    public List<(int A, int B, int C)> Triangles { get; private set; } = new List<(int A, int B, int C)>();

    public IVertexShader VertexShader => Rasterizer.Shaders.VertexShader.GetSingletonInstance();

    public Matrix4x4 ModelMatrix { get; set; } = Matrix4x4.Identity;

    public List<IFragmentShader> FragmentShaders { get; protected set; }

    public SceneGraphNode RootNode { get; protected set; }

    public void FlattenVerticesAndTriangles()
    {
        List<Vertex> vertices = new List<Vertex>();
        List<(int A, int B, int C)> triangles = new List<(int A, int B, int C)>();
        GetAllVerticesAndTrianglesRecursive(this.RootNode, this.RootNode.ModelMatrix, vertices, triangles);

        this.Vertices = vertices;
        this.Triangles = triangles;
    }

    private void GetAllVerticesAndTrianglesRecursive(SceneGraphNode curNode, Matrix4x4 curModelMatrix, List<Vertex> vertices, List<(int A, int B, int C)> triangles)
    {
        foreach (var child in curNode.Children)
        {
            var curOffset = vertices.Count == 0 ? 0 : vertices.Count;
            var modelMatrix = child.Value.GroupMatrix * curModelMatrix;

            vertices.AddRange(child.Value.Node.Vertices.Select(v => new Vertex(
                            Vector4.Transform(v.Position, child.Value.Node.ModelMatrix * modelMatrix),
                            new Vector3(),
                            v.Color,
                            v.UV,
                            Vector3.Normalize(Vector3.TransformNormal(v.Normal, child.Value.Node.ModelMatrix * modelMatrix)))));
            triangles.AddRange(child.Value.Node.Triangles.Select(t => (t.A + curOffset, t.B + curOffset, t.C + curOffset)));

            if (child.Value.Node.Children.Count > 0)
            {
                GetAllVerticesAndTrianglesRecursive(child.Value.Node, modelMatrix, vertices, triangles);
            }
        }
    }
}

