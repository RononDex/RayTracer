
using System;
using System.Collections.Generic;
using Rasterizer.Models;

namespace Rasterizer.Scenes;

public abstract class BaseScene : IScene
{
    public Camera Camera { get; protected set; }

    public List<IGeometry> Objects { get; private set; } = new List<IGeometry>();

    public List<ILightSource> LightSources {get; private set;} = new List<ILightSource>();

    public abstract void UpdateGameState(TimeSpan deltaT, TimeSpan totalRunTime);
}
