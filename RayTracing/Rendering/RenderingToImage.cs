using System;
using System.Linq;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace _02_RayTracing.Rendering;

public static class RenderingToImage
{
    public static Image RenderToImage(Vector3[,] rendering)
    {
        var image = new Image<Rgb24>(rendering.GetLength(0), rendering.GetLength(1));

        ClampColors(rendering);

        var gamma = 2.2f;
        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < image.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                for (var x = 0; x < image.Width; x++)
                {
                    var color = Vector3.Clamp(rendering[x, y], new Vector3(0f), new Vector3(1f));
                    color.X = (float)Math.Pow(color.X, 1 / gamma);
                    color.Y = (float)Math.Pow(color.Y, 1 / gamma);
                    color.Z = (float)Math.Pow(color.Z, 1 / gamma);
                    color *= 255;
                    pixelRow[x] = new Rgb24(
                                            (byte)color.X,
                                            (byte)color.Y,
                                            (byte)color.Z);
                }
            }
        });

        return image;
    }

    public static void ClampColors(Vector3[,] colors)
    {
        for (var x = 0; x < colors.GetLength(0); x++) {
            for (var y = 0; y < colors.GetLength(1); y++) {
                colors[x, y] = Vector3.Clamp(colors[x, y], Vector3.Zero, Vector3.One);
            }
        }
    }
}
