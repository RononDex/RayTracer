
using System;
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Models;
using Rasterizer.Shaders;

namespace Rasterizer.Scenes;
public class TexturedCubeScene : BaseScene
{
    public TexturedCubeScene()
    {
        this.Objects.Add(new Cube(
                            Matrix4x4.CreateTranslation(0, 0, 4),
                            new Vector3(0.9f, 0, 0),
                            new Vector3(0, 0.7f, 0),
                            new Vector3(0, 0, 0.8f),
                            new Vector3(0.9f, 0.9f, 0),
                            new Vector3(0, 0.9f, 0.9f),
                            new Vector3(0.9f, 0, 0.9f),
                            new List<IFragmentShader>()
                            {
                                new SpecularPhongShader(500, 1f, new Vector3(1)),
                                new TextureShader("Textures/flat-temple-stonework.png")
                            }));

        this.Camera = Camera.LookAt(new Vector3(0, 2, -2), new Vector3(0, 1, 0), new Vector3(0, -1, 0));

        this.LightSources.Add(new PointLightSource(new Vector3(0, 5, -10), new Vector3(1.0f, 1.0f, 1.0f)));
    }

    public override void UpdateGameState(TimeSpan deltaT, TimeSpan totalRunTime)
    {
        this.Objects[0].ModelMatrix = Matrix4x4.CreateRotationY((float)totalRunTime.TotalSeconds) * Matrix4x4.CreateTranslation(0, 0, 2);

        /* this.LightSources[0].WorldCoordinates = Vector3.Transform( */
        /*         Vector3.Zero, */
        /*         Matrix4x4.CreateTranslation(0, 3, 15) * Matrix4x4.CreateRotationY((float)-totalRunTime.TotalSeconds / 2)); */
    }

}
