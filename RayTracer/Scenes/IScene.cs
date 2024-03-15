
using System;
using System.Collections.Generic;
using Rasterizer.Models;

namespace Rasterizer.Scenes;

public interface IScene
{

    public Camera Camera { get; }

    public List<IGeometry> Objects { get; }

    public List<ILightSource> LightSources { get; }

    void UpdateGameState(TimeSpan deltaT, TimeSpan totalRunTime);
}
