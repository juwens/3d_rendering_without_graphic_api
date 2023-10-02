using System.Collections.Immutable;
using System.Drawing;
using System.Numerics;

namespace Gui;

// size = 9 * sizeof(float) = 36 byte
struct Triangle
{
    public Triangle(Point3 a, Point3 b, Point3 c)
    {
        A = a;
        B = b;
        C = c;

        Points = ImmutableArray.Create(a, b, c);
        Normal = Vector3.Normalize(Vector3.Cross(b.Vector - a.Vector, c.Vector - a.Vector));
    }

    public Triangle(Vector3 a, Vector3 b, Vector3 c, Color color) : this(
        new Point3(a, color),
        new Point3(b, color),
        new Point3(c, color))
    {
    }

    public Point3 A { get; }
    public Point3 B { get; }
    public Point3 C { get; }
    public Vector3 Normal { get; }

    public ImmutableArray<Point3> Points { get; }

    public override string ToString()
    {
        var v2s = VecToString;
        return $"{{{v2s(A.Vector)},{v2s(B.Vector)},{v2s(C.Vector)}}};{v2s(Normal)}";
    }

    private readonly string VecToString(Vector3 v)
    {
        return $"{{{v.X},{v.Y},{v.Z}}}";
    }
}

internal struct Point3
{
    public Point3(Vector3 vec, Color color)
    {
        Vector = vec;
        Color = color;
    }

    public Vector3 Vector { get; }
    public Color Color { get; }
}