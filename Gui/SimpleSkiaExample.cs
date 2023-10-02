using SkiaSharp;

namespace Gui;

internal static class SimpleSkiaExample
{
    /// <summary>
    /// credit to https://swharden.com/csdv/maui.graphics/quickstart-wpf/
    /// 
    /// </summary>
    internal static void Draw(SKCanvas canvas, SKSize size)
    {
        canvas.Clear(new SKColor(0, 0x33, 0x66));

        var dotPaint = new SKPaint()
        {
            StrokeWidth = 2,
            IsAntialias = true,
        };

        var nrOfPixels = canvas.DeviceClipBounds.Width * canvas.DeviceClipBounds.Height;
        for (int i = 0; i < nrOfPixels / 1000; i++)
        {
            float x = Random.Shared.Next((int)size.Width);
            float y = Random.Shared.Next((int)size.Height);
            float r = Random.Shared.Next(10, 20);
            dotPaint.Color = SKColors.White.WithAlpha(byte.MaxValue / 2)
                .WithRed((byte)Random.Shared.Next(0, 255))
                .WithGreen((byte)Random.Shared.Next(0, 255))
                .WithBlue((byte)Random.Shared.Next(0, 255));
            canvas.DrawCircle(x, y, r, dotPaint);
        }
    }
}
