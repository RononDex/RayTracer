using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using RayTracer.Models;
using RayTracer.Rendering;
using SixLabors.ImageSharp;
using Color = Avalonia.Media.Color;
using Image = Avalonia.Controls.Image;

namespace RayTracer;

public partial class MainWindow : Window
{
    private readonly Timer timer;
    private readonly Task imageRenderingTask;
    private readonly DateTime renderingStart;
    private bool saved;

    public MainWindow()
    {
        this.InitializeComponent();

        this.Background = new SolidColorBrush(new Color(255, 0, 0, 0));

        var scene = SceneBuilder.CornellBoxTransparency();

        Console.WriteLine("Starting rendering scene...");
        var renderer = new RayTracingRenderer();
        const int width = 512;
        const int height = 512;
        const int raysPerPixel = 1000;

        this.timer = new Timer((e) => this.UpdateUIWithRendering(renderer, scene, raysPerPixel, width, height), state: null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(200));
        this.renderingStart = DateTime.Now;
        this.imageRenderingTask = renderer.RenderSceneAsync(scene, raysPerPixel, width, height, Environment.ProcessorCount - 1);
    }

    public void UpdateUIWithRendering(RayTracingRenderer renderer, Scene scene, int raysPerPixel, int width, int height)
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
            }, DispatcherPriority.MaxValue);

            if (this.imageRenderingTask.IsCompleted && !this.saved)
            {
                var renderingTime = DateTime.Now - this.renderingStart;
                Console.WriteLine($"Rendering finished after {renderingTime.TotalMinutes:0.###} minutes");
                Console.WriteLine($"Averaged {(int)(raysPerPixel * width * height / renderingTime.TotalSeconds)} rays/s and {(int)(raysPerPixel * scene.NumberOfBounces * width * height / renderingTime.TotalSeconds)} bounces/s");
                this.saved = true;
                image.SaveAsPng("Rendering_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".png");
                this.timer.Dispose();
            }
        }
    }
}
