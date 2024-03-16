
using System.Collections.Generic;
using System.Numerics;

namespace RayTracer.Models;

public class Scene
{
    public List<IGeometry> Objects { get; private set; }

    public float FieldOfView { get; private set; }

    public Vector3 CameraPosition { get; private set; }

    public Vector3 LookAtPosition { get; private set; }

    public Vector3 Up { get; private set; }

    public float NumberOfBounces { get; set; } = 4f;
    public float Gauss_Sigma { get; set; } = 0.5f;

    public Scene(List<IGeometry> objects, Vector3 cameraPosition, Vector3 lookAtPosition, float fov, Vector3 up)
    {
        this.Objects = objects;
        this.FieldOfView = fov;
        this.CameraPosition = cameraPosition;
        this.LookAtPosition = lookAtPosition;
        this.Up = up;
    }
}
