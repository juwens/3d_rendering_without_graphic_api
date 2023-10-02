using SkiaSharp;

namespace Gui.Extensions
{
    internal static class SKColorExtensions
    {
        public static SKColor WithBrightness(this SKColor color, byte brightness)
        {
            return new SKColor(
                (byte)(1f * color.Red * brightness / 255),
                (byte)(1f * color.Green * brightness / 255),
                (byte)(1f * color.Blue * brightness / 255));
        }
    }
}
