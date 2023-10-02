using System.Collections.Immutable;

namespace Gui.Data;

internal class Pyramid
{
    public static readonly ImmutableArray<float> RawVertices = ImmutableArray.Create<float>(
       // front
        -1f, 0f, -1f,
        0f, 2f, 0f,
        1f, 0f, -1f,

        // back,
        1f, 0f, 1f,
        0f, 2f, 0f,
        -1f, 0f, 1f,

        // left
        -1f, 0f, -1f,
        -1f, 0f, 1f,
        0f, 2f, 0f,

        // right
        1f, 0f, -1f,
        0f, 2f, 0f,
        1f, 0f, 1f
    );
}
