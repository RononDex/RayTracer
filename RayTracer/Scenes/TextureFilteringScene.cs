
using System;
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Models;
using Rasterizer.Shaders;
using Plane = Rasterizer.Models.Plane;

namespace Rasterizer.Scenes;

public class TextureFilteringScene : BaseScene
{
    public TextureFilteringScene()
    {
        this.Objects.Add(new Plane(
                    Matrix4x4.CreateScale(100, 100, 100) * Matrix4x4.CreateTranslation(0, -3, 60),
                    new List<IFragmentShader>()
                    {
                            new TextureShader("Textures/flat-temple-stonework.png")
                    },
                    uvWidth: 5,
                    uvHeight: 5));

        this.Camera = Camera.LookAt(new Vector3(0, 0, -1), new Vector3(0, 0, 0), new Vector3(0, -1, 0));

        this.LightSources.Add(new PointLightSource(new Vector3(0, 0, -5), new Vector3(1.0f, 1.0f, 1.0f)));
    }

    public override void UpdateGameState(TimeSpan deltaT, TimeSpan totalRunTime)
    {
        if (totalRunTime.TotalSeconds * 2 < 60)
        {
            this.Objects[0].ModelMatrix =  Matrix4x4.CreateScale(100) * Matrix4x4.CreateTranslation(0, -3, 60 - (float)totalRunTime.TotalSeconds * 2);
        }

        // this.LightSources[0].WorldCoordinates = Vector3.Transform(
        //          Vector3.Zero,
        //          Matrix4x4.CreateTranslation(0, 5, (float)totalRunTime.TotalSeconds));
    }
}
