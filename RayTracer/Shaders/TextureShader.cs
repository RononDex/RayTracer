using System;
using System.Numerics;
using Rasterizer.Scenes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Rasterizer.Shaders;

public class TextureShader : IFragmentShader
{

    public Image<Rgb24> Texture { get; private set; }
    public float AmbientBrightness { get; private set; }

    public TextureShader(string texturePath, float ambientBrightness = 0.03f)
    {
        Texture = Image.Load<Rgb24>(texturePath);
        AmbientBrightness = ambientBrightness;
    }

    public Vector3 CalculateColor(ref Vertex q, IScene scene)
    {
        var color = Vector3.Zero;
        var s = (q.UV.X - MathF.Floor(q.UV.X)) * Texture.Width;
        var t = (q.UV.Y - MathF.Floor(q.UV.Y)) * Texture.Height;

        var bottomLeft = Texture[(int)MathF.Floor(s) % Texture.Width, (int)MathF.Floor(t) % Texture.Height];
        var topLeft = Texture[(int)MathF.Floor(s) % Texture.Width, (int)MathF.Floor((t + 1) % Texture.Height)];
        var topRight = Texture[(int)MathF.Floor((s + 1) % Texture.Width), (int)MathF.Floor((t + 1) % Texture.Height)];
        var bottomRight = Texture[(int)MathF.Floor((s + 1) % Texture.Width), (int)MathF.Floor(t) % Texture.Height];
        var colorTexture = BilinearExtrapoltation(
                new Vector3(
                    bottomLeft.R,
                    bottomLeft.G,
                    bottomLeft.B),
                new Vector3(
                    topLeft.R,
                    topLeft.G,
                    topLeft.B),
                new Vector3(
                    topRight.R,
                    topRight.G,
                    topRight.B),
                new Vector3(
                    bottomRight.R,
                    bottomRight.G,
                    bottomRight.B),
                s,
                t);

        foreach (var lightSource in scene.LightSources)
        {
            var v = lightSource.WorldCoordinates - q.WorldCoordinates;
            var normal = Vector3.Normalize(q.Normal);

            var dotProduct = Vector3.Dot(Vector3.Normalize(new Vector3(v.X, v.Y, v.Z)), normal);
            var colorVector = new Vector3(colorTexture.X, colorTexture.Y, colorTexture.Z);

            ToLinearColor(ref colorVector, 2.2f);

            if (dotProduct > 0)
            {
                color += lightSource.Color * colorVector * dotProduct;
            }
            else
            {
                color += this.AmbientBrightness * colorVector;
            }
        }

        return color;
    }

    private static void ToLinearColor(ref Vector3 nonLinearColor, float gamma)
    {
        nonLinearColor.X = MathF.Pow(nonLinearColor.X / 255, gamma);
        nonLinearColor.Y = MathF.Pow(nonLinearColor.Y / 255, gamma);
        nonLinearColor.Z = MathF.Pow(nonLinearColor.Z / 255, gamma);
    }

    private static Vector3 BilinearExtrapoltation(Vector3 bottomLeft, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight, float s, float t)
    {
        var c1 = Vector3.Lerp(bottomLeft, topLeft, t - MathF.Floor(t));
        var c2 = Vector3.Lerp(bottomRight, topRight, t - MathF.Floor(t));

        return Vector3.Lerp(c1, c2, s - MathF.Floor(s));
    }
}
