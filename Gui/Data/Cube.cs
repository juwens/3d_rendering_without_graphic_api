using System.Collections.Immutable;
using System.Numerics;

namespace Gui.Data;

internal class Cube
{
    public static ImmutableArray<Vector3> Vertices = new Vector3[]
    {
        // front:
        new(0, 0, 0),
        new(0, 1, 1),
        new(1, 0, 0),
        new(0.1f, 1f, 0),
        new(1.1f, 1f, 0),
        new(1.1f, 0, 0),

        // right:
        new (1.1f, 0, 0),
        new (1.1f, 1, 0),
        new (1.1f, 0, 1),

        new (1.1f, 0, 1),
        new (1.1f, 1, 0),
        new (1.1f, 1, 1),
    }.ToImmutableArray();
}
