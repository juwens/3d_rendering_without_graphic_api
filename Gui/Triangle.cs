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

        //Center = new Vector3(
        //    (a.X + b.X + c.X) / 3,
        //    (a.Y + b.Y + c.Y) / 3,
        //    (a.Z + b.Z + c.Z) / 3);

        //float radius = 0;

        //foreach (var point in new[] { a, b, c })
        //{
        //    var distance = (Center - point).Length();
        //    if (distance > radius)
        //    {
        //        radius = distance;
        //    }
        //}
        //Radius = radius;

        Normal = Vector3.Cross(b.Vector - a.Vector, c.Vector - a.Vector);
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

    //public Vector3 Center { get; }
    ///// <summary>
    ///// Triangle is guaranteed to be within radius of <see cref="Center"/>
    ///// </summary>
    //public float Radius { get; }
    public Vector3 Normal { get; }

    public ImmutableArray<Point3> Points { get; }
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