using SkiaSharp;
using System.Windows;
using System.Windows.Threading;

namespace Gui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;

    public MainWindow()
    {
        _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Normal, Callback, Dispatcher);
        InitializeComponent();
    }

    private void Callback(object? sender, EventArgs e)
    {
        _skElm.InvalidateVisual();
    }

    private void SKElement_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
    {
        bool skiaTest = false;
        if (skiaTest)
        {
            SimpleSkiaExample.Draw(e.Surface.Canvas, new SKSize((float)_skElm.ActualWidth, (float)_skElm.ActualHeight));
            return;
        }

        Simple3dRenderer.Draw(e);
    }
}
