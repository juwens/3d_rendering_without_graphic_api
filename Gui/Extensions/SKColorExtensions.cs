using SkiaSharp;

namespace Gui.Extensions;

internal static class SKColorExtensions
{
    private static readonly float Pi = (float)Math.PI;

    public static SKColor ScaleLuminescenceByDeg(this SKColor color, float angleRad)
    {
        color.ToHsl(out float h, out float s, out float l);
        return SKColor.FromHsl(h, s, l * (angleRad / Pi));
    }
}
