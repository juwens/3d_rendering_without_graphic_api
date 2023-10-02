using SkiaSharp;
using System.Windows;

namespace Gui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
