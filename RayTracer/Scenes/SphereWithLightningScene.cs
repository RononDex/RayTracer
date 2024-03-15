

using System;
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Models;
using Rasterizer.Shaders;

namespace Rasterizer.Scenes;

public class SphereWithLightningScene : BaseScene
{
    public SphereWithLightningScene()
    {

        this.Objects.Add(new Sphere(
                    6,
                    new Vector3(0.9f, 0f, 0f),
                    Matrix4x4.CreateTranslation(0, 0, 4),
                    new List<IFragmentShader>()
                        {
                                    new AmbientColorShader(0.03f),
                                    LambertDiffuseFragmentShader.GetSingletonInstance(),
                                    new SpecularPhongShader(100, 1f, new Vector3(1))
                        }));


        this.LightSources.Add(new PointLightSource(new Vector3(10, 5, 20), new Vector3(1.0f, 1.0f, 1.0f)));

        this.Camera = Camera.LookAt(new Vector3(0, 2, -6), new Vector3(0, 0, 0), new Vector3(0, -1, 0));

    }

    public override void UpdateGameState(TimeSpan deltaT, TimeSpan totalRunTime)
    {
        this.Objects[0].ModelMatrix =
            Matrix4x4.CreateScale(3f)
            * Matrix4x4.CreateRotationY((float)totalRunTime.TotalSeconds)
            * Matrix4x4.CreateTranslation(0, 0, 2);
        this.LightSources[0].WorldCoordinates = Vector3.Transform(
                Vector3.Zero,
                Matrix4x4.CreateTranslation(0, 18, 20) * Matrix4x4.CreateRotationY((float)-totalRunTime.TotalSeconds));
    }
}
