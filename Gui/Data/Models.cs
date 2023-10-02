using System.Collections.Immutable;
using System.Numerics;

namespace Gui.Data;

internal static class Models
{
    public static ImmutableArray<Vector3> Teapot => Data.Teapot.Vertices;
    public static ImmutableArray<Vector3> Cube => Data.Cube.Vertices;
}
