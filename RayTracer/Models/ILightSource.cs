
using System.Numerics;

namespace Rasterizer.Models;

public interface ILightSource
{
    public Vector3 WorldCoordinates { get; set; }

    public Vector3 Color { get; set; }
}
