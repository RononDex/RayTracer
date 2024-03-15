
using System;
using System.Threading.Tasks;
using Rasterizer.Scenes;
using SixLabors.ImageSharp;

namespace Rasterizer;

public class GameLoop
{
    private DateTime lastRenderingTime = DateTime.Now;
    private DateTime? firstRenderingTime = null;

    public IScene Scene { get; private set; }

    private readonly Renderer renderer;

    public event Action<Image> FrameRendered;

    public GameLoop(IScene scene, Renderer renderer)
    {
        this.Scene = scene;
        this.renderer = renderer;
    }

    public Task StartLoopAsync()
    {
        return Task.Run(() =>
        {
            if (firstRenderingTime == null)
            {
                firstRenderingTime = DateTime.Now;
            }
            while (true)
            {
                var deltaT = DateTime.Now - lastRenderingTime;
                lastRenderingTime = DateTime.Now;
                this.Scene.UpdateGameState(deltaT, DateTime.Now - firstRenderingTime.Value);

                var frame = renderer.RenderToImage(this.Scene);

                FrameRendered?.Invoke(frame);
            }
        });
    }
}
