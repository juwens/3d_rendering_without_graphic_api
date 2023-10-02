using System.Collections.Immutable;

namespace Gui.Data;

internal static class SemiCube
{
    public static ImmutableArray<float> Vertices = ImmutableArray.Create<float>(

        // front
        0, 0, 0,
        0, 1, 0,
        1, 0, 0,

        // back
        0, 0, 1,
        1, 0, 1,
        1, 1, 1,
        
        // left
        0, 0, 0,
        0, 0, 1,
        0, 1, 0,

        // right
        1, 0, 0,
        1, 1, 0,
        1, 0, 1);
}
