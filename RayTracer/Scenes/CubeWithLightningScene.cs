using System;
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Models;
using Rasterizer.Shaders;

namespace Rasterizer.Scenes;

public class CubeWithLightningScene : BaseScene
{
    public CubeWithLightningScene()
    {
        this.Objects.Add(new Cube(
                            Matrix4x4.CreateTranslation(0, 0, 4),
                            new Vector3(255, 0, 0),
                            new Vector3(0, 192, 0),
                            new Vector3(0, 0, 164),
                            new Vector3(255, 255, 0),
                            new Vector3(0, 255, 255),
                            new Vector3(255, 0, 255),
                            new List<IFragmentShader>()
                            {
                                new AmbientColorShader(0.01f),
                                LambertDiffuseFragmentShader.GetSingletonInstance(),
                                new SpecularPhongShader(100, 1f, new Vector3(1))
                            }));

        // Change vertex colors to demonstrate color interpolation
        for (var i = 0; i < this.Objects[0].Vertices.Count; i += 4)
        {
            var oldVertex1 = this.Objects[0].Vertices[i];
            var oldVertex2 = this.Objects[0].Vertices[i + 1];
            var oldVertex3 = this.Objects[0].Vertices[i + 2];
            var oldVertex4 = this.Objects[0].Vertices[i + 3];

            this.Objects[0].Vertices[i] = new Vertex(oldVertex1.Position, oldVertex1.WorldCoordinates, new Vector3(0.9f, 0, 0), oldVertex1.UV, oldVertex1.Normal);
            this.Objects[0].Vertices[i + 1] = new Vertex(oldVertex2.Position, oldVertex2.WorldCoordinates, new Vector3(0, 0.9f, 0), oldVertex2.UV, oldVertex2.Normal);
            this.Objects[0].Vertices[i + 2] = new Vertex(oldVertex3.Position, oldVertex3.WorldCoordinates, new Vector3(0, 0, 0.9f), oldVertex3.UV, oldVertex3.Normal);
            this.Objects[0].Vertices[i + 3] = new Vertex(oldVertex4.Position, oldVertex4.WorldCoordinates, new Vector3(0.9f, 0.9f, 0), oldVertex4.UV, oldVertex4.Normal);
        }

        this.LightSources.Add(new PointLightSource(new Vector3(5, 15, 20), new Vector3(1.0f, 1.0f, 1.0f)));

        this.Camera = Camera.LookAt(new Vector3(0, 2, -6), new Vector3(0, 0, 0), new Vector3(0, -1, 0));
    }

    public override void UpdateGameState(TimeSpan deltaT, TimeSpan totalRunTime)
    {
        this.Objects[0].ModelMatrix =
            Matrix4x4.CreateScale(2f)
            * Matrix4x4.CreateRotationY((float)totalRunTime.TotalSeconds / 2)
            * Matrix4x4.CreateTranslation(0, 0, 2);
        this.LightSources[0].WorldCoordinates = Vector3.Transform(
                Vector3.Zero,
                Matrix4x4.CreateTranslation(0, 1, 20) * Matrix4x4.CreateRotationY((float)-totalRunTime.TotalSeconds / 2));
    }
}
