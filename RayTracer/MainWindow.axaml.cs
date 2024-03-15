using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Rasterizer.Scenes;
using Image = SixLabors.ImageSharp.Image;

namespace Rasterizer;

public partial class MainWindow : Window
{

    Avalonia.Controls.Image uiImage;

    public MainWindow()
    {
        InitializeComponent();

        var scene = new SphereWithLightningScene();

        this.uiImage = this.FindControl<Avalonia.Controls.Image>("image")!;
        var renderer = new Renderer(
                700,
                700,
                MathF.PI / 2,
                0.1f,
                100f);
        var gameLoop = new GameLoop(scene, renderer);

        gameLoop.FrameRendered += RenderImage;
        gameLoop.StartLoopAsync();
    }

    private void RenderImage(Image image)
    {
        Dispatcher.UIThread.Post(() =>
        {
            using (var memoryStream = new MemoryStream())
            {

                image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder());
                memoryStream.Seek(0, SeekOrigin.Begin);
                this.uiImage.Source = new Bitmap(memoryStream);
            }
        }, DispatcherPriority.MaxValue);
    }
}
