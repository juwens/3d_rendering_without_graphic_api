using System.Collections.Immutable;

namespace Gui.Data;

internal class Cube
{
    public static readonly ImmutableArray<float> RawVertices = new float[]
    {
        // front:
        0f, 0f, 0f,
        0f, 1f, 0f,
        1f, 0f, 0f,
        0.1f, 1f, 0f,
        1.1f, 1f, 0f,
        1.1f, 0f, 0f,

        // back:
        0f, 0f, 1f,
        0f, 1f, 1f,
        1f, 0f, 1f,
        0.1f, 1f, 1f,
        1.1f, 1f, 1f,
        1.1f, 0f, 1f,

        // right:
        1.1f, 0f, 0f,
        1.1f, 1f, 0f,
        1.1f, 0f, 1f,

        1.1f, 0f, 1f,
        1.1f, 1f, 0f,
        1.1f, 1f, 1f,

        // left:
        0, 0f, 0f,
        0, 1f, 0f,
        0, 0f, 1f,

        0, 0f, 1f,
        0, 1f, 0f,
        0, 1f, 1f,

    }.ToImmutableArray();
}
