using SkiaSharp;

namespace Gui.Extensions;

internal static class SKColorExtensions
{
    public static SKColor ScaleLuminescenceByDeterminant(this SKColor color, float determinant)
    {
        color.ToHsl(out float h, out float s, out float l);
        l = l * (determinant + 0.3f);
        return SKColor.FromHsl(h, s, l);
    }
}
