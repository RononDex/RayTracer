using System.Numerics;

namespace Rasterizer.Shaders;

public class VertexShader : IVertexShader
{
    public Vertex Transform(Vertex v, Matrix4x4 modelViewProjectionMatrix, Matrix4x4 modelMatrix)
    {
        var worldCoordinates = Vector4.Transform(v.Position, modelMatrix);
        var projectionCoordinates = Vector4.Transform(v.Position, modelViewProjectionMatrix);
        return new Vertex(
            projectionCoordinates,
            new Vector3(worldCoordinates.X, worldCoordinates.Y, worldCoordinates.Z),
            v.Color,
            v.UV,
            Vector3.Normalize(Vector3.TransformNormal(v.Normal, modelMatrix)));
    }

    private static VertexShader _instance;

    public static VertexShader GetSingletonInstance()
    {
        if (_instance == null)
            _instance = new VertexShader();

        return _instance;
    }
}
