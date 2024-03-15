
using System.Numerics;
using Rasterizer.Scenes;

namespace Rasterizer.Shaders;

public class LambertDiffuseFragmentShader : IFragmentShader
{
    public Vector3 CalculateColor(ref Vertex q, IScene scene)
    {
        var color = Vector3.Zero;
        foreach (var lightSource in scene.LightSources)
        {
            var qe = lightSource.WorldCoordinates - q.WorldCoordinates;
            var normal = Vector3.Normalize(q.Normal);

            var dotProduct = Vector3.Dot(Vector3.Normalize(new Vector3(qe.X, qe.Y, qe.Z)), normal);

            if (dotProduct > 0)
            {
                color += lightSource.Color * q.Color * dotProduct;
            }
        }

        return color;
    }

    private static LambertDiffuseFragmentShader _instance;

    public static LambertDiffuseFragmentShader GetSingletonInstance()
    {
        if (_instance == null)
            _instance = new LambertDiffuseFragmentShader();

        return _instance;
    }
}
