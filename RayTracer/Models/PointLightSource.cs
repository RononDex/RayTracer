
using System.Numerics;

namespace Rasterizer.Models;

public class PointLightSource : ILightSource
{
    public Vector3 WorldCoordinates { get; set; }

    public Vector3 Color { get; set; }

    public PointLightSource(Vector3 worldCoordinates, Vector3 color)
    {
        this.WorldCoordinates = worldCoordinates;
        this.Color = color;
    }
}
