
using System;
using System.Collections.Generic;
using System.Numerics;
using Rasterizer.Models;
using Rasterizer.Shaders;

namespace Rasterizer.Scenes;

public class GraphDemonstrationScene : BaseScene
{
    Matrix4x4? originalSecondGroupModelMatrix;
    public GraphDemonstrationScene()
    {
        var obj = new RotatingRods(
                            Matrix4x4.CreateTranslation(0, 0, 2),
                            new List<IFragmentShader>()
                            {
                                new AmbientColorShader(0.01f),
                                LambertDiffuseFragmentShader.GetSingletonInstance(),
                                new SpecularPhongShader(100, 1f, new Vector3(1)),
                            });
        this.Objects.Add(obj);

        this.LightSources.Add(new PointLightSource(new Vector3(5, 15, 20), new Vector3(1.0f, 1.0f, 1.0f)));

        this.Camera = Camera.LookAt(new Vector3(0, 2, -6), new Vector3(0, 0, 0), new Vector3(0, -1, 0));
    }

    public override void UpdateGameState(TimeSpan deltaT, TimeSpan totalRunTime)
    {
        var sceneGraphObject = (SceneGraphObject)this.Objects[0];
        if (originalSecondGroupModelMatrix == null)
            originalSecondGroupModelMatrix = sceneGraphObject.RootNode.Children["SecondBlockGroup"].GroupMatrix;
        this.Objects[0].ModelMatrix = Matrix4x4.CreateTranslation(0, 0, 3) * Matrix4x4.CreateRotationZ((float)totalRunTime.TotalSeconds);

        sceneGraphObject.RootNode.Children["SecondBlockGroup"].GroupMatrix = Matrix4x4.CreateRotationY((float)totalRunTime.TotalSeconds * 3) * originalSecondGroupModelMatrix.Value;

        this.LightSources[0].WorldCoordinates = Vector3.Transform(
                Vector3.Zero,
                Matrix4x4.CreateTranslation(0, 5, 10) * Matrix4x4.CreateRotationY((float)-totalRunTime.TotalSeconds / 2));
    }
}
