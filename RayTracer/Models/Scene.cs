
using System.Collections.Generic;
using System.Numerics;

namespace _02_RayTracing.Models;

public class Scene
{
    public List<IGeometry> Objects { get; private set; }

    public float FieldOfView { get; private set; }

    public Vector3 CameraPosition { get; private set; }

    public Vector3 LookAtPosition { get; private set; }

    public Vector3 Up { get; private set; }

    public Scene(List<IGeometry> objects, Vector3 cameraPosition, Vector3 lookAtPosition, float fov, Vector3 up)
    {
        this.Objects = objects;
        this.FieldOfView = fov;
        this.CameraPosition = cameraPosition;
        this.LookAtPosition = lookAtPosition;
        this.Up = up;
    }
}
