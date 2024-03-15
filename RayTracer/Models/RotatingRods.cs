
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Shaders;

namespace Rasterizer.Models;

public class RotatingRods : SceneGraphObject
{
    public RotatingRods(Matrix4x4 modelMatrix, List<IFragmentShader> fragmentShaders)
    {
        this.FragmentShaders = fragmentShaders;

        this.RootNode = new SceneGraphNode();
        this.RootNode.ModelMatrix = Matrix4x4.Identity;

        this.RootNode.Children.Add("FirstBlock", new SceneGraphNodeReference(new SceneGraphNode(new Cube(
                            Matrix4x4.Identity,
                            new Vector3(0.9f, 0, 0),
                            new Vector3(0.9f, 0.9f, 0),
                            new Vector3(0.9f, 0, 0.9f),
                            new Vector3(0f, 0.0f, 0),
                            new Vector3(0.9f, 0.9f, 0.9f),
                            new Vector3(0f, 0, 0.9f),
                            new List<IFragmentShader>()
                            {
                            })), Matrix4x4.Identity));
        this.RootNode.Children.Add("Connector1", new SceneGraphNodeReference(new SceneGraphNode(new Cube(
                            Matrix4x4.CreateScale(0.5f, 3, 0.5f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new List<IFragmentShader>()
                            {
                            })), Matrix4x4.CreateTranslation(0f, -3, 0f)));

        var node = new SceneGraphNode();
        this.RootNode.Children.Add("SecondBlockGroup", new SceneGraphNodeReference(node, Matrix4x4.CreateTranslation(0, -4, 0f)));

        node.Children.Add("SecondBlock", new SceneGraphNodeReference(new SceneGraphNode(new Cube(
                            Matrix4x4.CreateScale(0.8f, 0.8f, 0.8f),
                            new Vector3(0f, 0, 0.9f),
                            new Vector3(0.9f, 0.9f, 0.9f),
                            new Vector3(0f, 0.0f, 0),
                            new Vector3(0.9f, 0, 0.9f),
                            new Vector3(0.9f, 0.9f, 0),
                            new Vector3(0.9f, 0, 0),
                            new List<IFragmentShader>()
                            {
                            })), Matrix4x4.Identity));
        node.Children.Add("Connector2", new SceneGraphNodeReference(new SceneGraphNode(new Cube(
                            Matrix4x4.CreateScale(2f, 0.3f, 0.3f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new Vector3(0.4f, 0.4f, 0.4f),
                            new List<IFragmentShader>()
                            {
                            })), Matrix4x4.CreateTranslation(-2f, -0, -0.0f)));
        node.Children.Add("Sphere1", new SceneGraphNodeReference(new SceneGraphNode(new Sphere(
                            3,
                            new Vector3(0.9f, 0.45f, 0.0f),
                            Matrix4x4.CreateScale(1f),
                            new List<IFragmentShader>()
                            {
                            })), Matrix4x4.CreateTranslation(-4f, -0, -0.0f)));


        this.ModelMatrix = modelMatrix;
    }
}
