using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using _02_RayTracing.Rendering;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using SixLabors.ImageSharp;
using Color = Avalonia.Media.Color;
using Image = Avalonia.Controls.Image;

namespace _02_RayTracing;

public partial class MainWindow : Window
{
    Timer timer;
    Task imageRenderingTask;
    bool saved = false;
    public MainWindow()
    {
        InitializeComponent();

        this.Background = new SolidColorBrush(new Color(255, 0, 0, 0));

        var scene = SceneBuilder.CornellBox();

        Console.WriteLine("Starting rendering scene...");
        /* var renderer = new BasicRenderer(20000); */
        /* imageRenderingTask = renderer.RenderSceneAsync(scene, 1000, 1000); */
        /* var renderer = new BasicRenderer(20000); */
        /* imageRenderingTask = renderer.RenderSceneAsync(scene, 500, 500); */
        var renderer = new BasicRenderer(10000);
        imageRenderingTask = renderer.RenderSceneAsync(scene, 100, 100);

        timer = new System.Threading.Timer((e) =>
        {
            PeriodicFooAsync(renderer);
        }, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200));
    }

    public void PeriodicFooAsync(BasicRenderer renderer)
    {
        if (renderer.CurrentRendering != null)
        {
            var image = RenderingToImage.RenderToImage(renderer.CurrentRendering);

            Dispatcher.UIThread.Post(() =>
            {
                var img = this.FindControl<Image>("img")!;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder());
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    img.Source = new Bitmap(memoryStream);
                }
            }, DispatcherPriority.Render);

            if (imageRenderingTask.IsCompleted && !saved)
            {
                saved = true;
                image.SaveAsPng("Rendering_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".png");
            }
        }
    }
}
