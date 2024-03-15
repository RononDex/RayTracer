
using System.Numerics;

namespace Rasterizer.Scenes;

public class Camera
{
    public Vector3 Position { get; private set; }

    public Matrix4x4 ViewMatrix { get; private set; }

    private Camera(Vector3 position, Matrix4x4 viewMatrix)
    {
        this.Position = position;
        this.ViewMatrix = viewMatrix;
    }

    public static Camera LookAt(Vector3 cameraPosition, Vector3 lookAt, Vector3 up)
    {
        return new Camera(cameraPosition, Matrix4x4.CreateLookAt(cameraPosition, lookAt, up));
    }
}
