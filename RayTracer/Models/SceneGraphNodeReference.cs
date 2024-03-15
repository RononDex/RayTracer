using System.Numerics;

namespace Rasterizer.Models;

public class SceneGraphNodeReference
{
    public SceneGraphNode Node { get; private set; }
    public Matrix4x4 GroupMatrix { get; set; }

    public SceneGraphNodeReference(SceneGraphNode node, Matrix4x4 groupMatrix) {
        this.Node = node;
        this.GroupMatrix = groupMatrix;
    }

    public override bool Equals(object obj) => this.Node.Equals(obj);
    public override int GetHashCode() => Node.GetHashCode();
    public override string ToString() => base.ToString();
}
