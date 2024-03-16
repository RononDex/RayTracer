using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using RayTracer.Rendering;
using SixLabors.ImageSharp;
using Color = Avalonia.Media.Color;
using Image = Avalonia.Controls.Image;

namespace RayTracer;

public partial class MainWindow : Window
{
    private readonly Timer timer;
    private readonly Task imageRenderingTask;
    private bool saved;

    public MainWindow()
    {
        InitializeComponent();

        this.Background = new SolidColorBrush(new Color(255, 0, 0, 0));

        var scene = SceneBuilder.CornellBoxReflections();

        Console.WriteLine("Starting rendering scene...");
        /* var renderer = new BasicRenderer(20000); */
        /* imageRenderingTask = renderer.RenderSceneAsync(scene, 1000, 1000); */
        /* var renderer = new BasicRenderer(20000); */
        /* imageRenderingTask = renderer.RenderSceneAsync(scene, 500, 500); */
        var renderer = new RayTracingRenderer(4000);
        this.imageRenderingTask = renderer.RenderSceneAsync(scene, 512, 512);

        this.timer = new Timer((e) => this.PeriodicFooAsync(renderer), null, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(300));
    }

    public void PeriodicFooAsync(RayTracingRenderer renderer)
    {
        if (renderer.CurrentRendering != null)
        {
            var image = RenderingToImage.RenderToImage(renderer.CurrentRendering);

            Dispatcher.UIThread.Post(() =>
            {
                var img = this.FindControl<Image>("img")!;
                using var memoryStream = new MemoryStream();
                image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder());
                _ = memoryStream.Seek(0, SeekOrigin.Begin);
                img.Source = new Bitmap(memoryStream);
            }, DispatcherPriority.Render);

            if (this.imageRenderingTask.IsCompleted && !this.saved)
            {
                this.saved = true;
                image.SaveAsPng("Rendering_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".png");
                this.timer.Dispose();
            }
        }
    }
}
