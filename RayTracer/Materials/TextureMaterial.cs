
using System;
using System.Numerics;
using RayTracer.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracer.Materials;

public class TextureMaterial : IMaterial
{
    public float RefractionIndex => 1.0f;

    public string? PathToTexture { get; private set; }

    public Image<Rgb24> Texture { get; private set; }

    public string? PathToEmissionTexture { get; private set; }

    public Image<Rgb24>? EmissionTexture { get; private set; }

    public float Gamma { get; private set; }

    public float EmissionMultiplier { get; private set; }

    public float SpecularMultiplier { get; private set; }

    public Vector3? SpecularColor { get; private set; }

    public float ReflectionEpsilon { get; private set; }

    public Matrix4x4? TextureTransformation { get; private set; }


    public TextureMaterial(
            string? pathToTexture,
            string? pathToEmissionTexture = null,
            float emissionMultiplier = 2,
            float specularMultiplier = 5f,
            float reflectionEpsilon = 0.01f,
            Vector3? specularColor = null,
            Matrix4x4? textureTransformation = null,
            float gamma = 2.2f)
    {
        this.PathToTexture = pathToTexture;
        if (pathToTexture != null)
        {
            Texture = Image.Load<Rgb24>(this.PathToTexture);
        }
        this.Gamma = gamma;
        this.EmissionMultiplier = emissionMultiplier;
        this.PathToEmissionTexture = pathToEmissionTexture;

        if (this.PathToEmissionTexture != null)
        {
            this.EmissionTexture = Image.Load<Rgb24>(this.PathToEmissionTexture);
        }

        this.SpecularMultiplier = specularMultiplier;
        this.SpecularColor = specularColor;
        this.ReflectionEpsilon = reflectionEpsilon;
        this.TextureTransformation = textureTransformation;
    }

    public Vector3 BRDFS(ref Vector3 inDirection, ref Vector3 outDirection, HitPoint hitPoint)
    {
        var colorVector = new Vector3();
        var normal = hitPoint.SurfaceNormal;
        if (this.PathToTexture != null)
        {
            (var x, var y) = SphericalProjection(normal, this.Texture);

            var color = Texture[x, y];
            colorVector.X = color.R;
            colorVector.Y = color.G;
            colorVector.Z = color.B;

            // reverse gamma correction
            ToLinearColor(ref colorVector, this.Gamma);
            colorVector /= MathF.PI;
        }

        if (this.SpecularColor != null)
        {
            var reflectionDirection = Vector3.Normalize(Vector3.Reflect(outDirection, normal));

            if (Vector3.Dot(inDirection, reflectionDirection) > (1f - this.ReflectionEpsilon))
            {
                colorVector += this.SpecularMultiplier * SpecularColor.Value;
            }
        }

        return colorVector;
    }

    private Vector3 noEmission = new Vector3(0);

    public Vector3 GetEmission(HitPoint hitPoint)
    {
        if (this.EmissionTexture == null)
            return noEmission;

        var normal = hitPoint.SurfaceNormal;
        (var x, var y) = SphericalProjection(normal, this.EmissionTexture!);

        var color = EmissionTexture[x, y];
        var colorVector = new Vector3(color.R, color.G, color.B);

        // reverse gamma correction
        ToLinearColor(ref colorVector, this.Gamma);

        return this.EmissionMultiplier * colorVector;
    }

    protected (int x, int y) SphericalProjection(Vector3 surfaceNormal, Image texture)
    {
        if (this.TextureTransformation != null)
        {
            surfaceNormal = Vector3.Transform(surfaceNormal, this.TextureTransformation.Value);
        }
        var x = (int)(MathF.Atan2(surfaceNormal.X, surfaceNormal.Z) / (2 * MathF.PI) * texture.Width);
        x = x % texture.Width;
        if (x < 0)
            x = x + texture.Width;

        var y = (int)(MathF.Acos(surfaceNormal.Y) / MathF.PI * texture.Height);
        y = y % texture.Height;
        if (y < 0)
            y = y + texture.Height;

        return (x, y);
    }

    protected static void ToLinearColor(ref Vector3 nonLinearColor, float gamma)
    {
        nonLinearColor.X = MathF.Pow(nonLinearColor.X / 255, gamma);
        nonLinearColor.Y = MathF.Pow(nonLinearColor.Y / 255, gamma);
        nonLinearColor.Z = MathF.Pow(nonLinearColor.Z / 255, gamma);
    }

    public bool HasRefraction => false;
}
