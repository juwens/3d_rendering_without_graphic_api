using System.Collections.Immutable;
using System.Numerics;

namespace Gui.Data;

internal static class Models
{
    public static ImmutableArray<Vector3> Teapot(Matrix4x4 transform) => GetVertices(Data.Teapot.RawVertices, transform, true);

    public static ImmutableArray<Vector3> Cube(Matrix4x4 transform) => GetVertices(Data.Cube.RawVertices, transform, false);
    
    public static ImmutableArray<Vector3> SemiCube(Matrix4x4 transform) => GetVertices(Data.SemiCube.Vertices, transform, false);

    public static ImmutableArray<Vector3> GetVertices(ImmutableArray<float> vertices, Matrix4x4 transform, bool openGlStyle)
    {
        var yInverter = openGlStyle ? -1.0f : 1.0f;

        return vertices.Chunk(3).Select(x =>
        {
            return openGlStyle 
                ? new Vector3(x[2], yInverter * x[1], x[0])
                : new Vector3(x[0], yInverter * x[1], x[2]);
        })
            .Select(x => Vector3.Transform(x, transform))
            .ToImmutableArray();
    }
}
