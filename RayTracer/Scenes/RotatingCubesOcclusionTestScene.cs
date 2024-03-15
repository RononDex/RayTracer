
using System;
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Models;
using Rasterizer.Shaders;

namespace Rasterizer.Scenes;

public class RotatingCubesOcclusionTestScene : BaseScene
{
    public RotatingCubesOcclusionTestScene()
    {
        this.Objects.Add(new Cube(
                            Matrix4x4.CreateTranslation(0, 0, 4),
                            new Vector3(0.9f, 0, 0),
                            new Vector3(0, 0.7f, 0),
                            new Vector3(0, 0, 0.8f),
                            new Vector3(0.9f, 0.9f, 0),
                            new Vector3(0, 0.9f, 0.9f),
                            new Vector3(0.9f, 0, 0.9f),
                            new List<IFragmentShader>() { new AmbientColorShader(1f) }));
        this.Objects.Add(new Cube(
                            Matrix4x4.CreateTranslation(0, 0, 4),
                            new Vector3(0.9f, 0, 0),
                            new Vector3(0, 0.7f, 0),
                            new Vector3(0, 0, 0.8f),
                            new Vector3(0.9f, 0.9f, 0),
                            new Vector3(0, 0.9f, 0.9f),
                            new Vector3(0.9f, 0, 0.9f),
                            new List<IFragmentShader>() { new AmbientColorShader(1f) }));

        this.Camera = Camera.LookAt(new Vector3(0, 2, -4), new Vector3(0, 0, 0), new Vector3(0, -1, 0));
    }

    public override void UpdateGameState(TimeSpan deltaT, TimeSpan totalRunTime)
    {
        this.Objects[0].ModelMatrix = Matrix4x4.CreateRotationY((float)totalRunTime.TotalSeconds) * Matrix4x4.CreateTranslation(0, 0, 2);
        this.Objects[1].ModelMatrix = Matrix4x4.CreateRotationX((float)totalRunTime.TotalSeconds / 2) * Matrix4x4.CreateTranslation(0, 0, 2);
    }
}
