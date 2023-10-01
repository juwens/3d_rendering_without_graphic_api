using SkiaSharp;
using System.Windows;
using System;
using System.Numerics;

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
        SimpleSkiaExample.Draw(e.Surface.Canvas, new SKSize((float)_skElm.ActualWidth, (float)_skElm.ActualHeight));
    }
}
