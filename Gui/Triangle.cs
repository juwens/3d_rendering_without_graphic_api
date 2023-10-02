using System;
using System.Numerics;
using System.Windows.Media.Media3D;

namespace Gui;

// size = 9 * sizeof(float) = 36 byte
struct Triangle
{
    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        A = a;
        B = b;
        C = c;

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
        
        Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
    }

    public Vector3 A { get; }
    public Vector3 B { get; }
    public Vector3 C { get; }
    //public Vector3 Center { get; }
    ///// <summary>
    ///// Triangle is guaranteed to be within radius of <see cref="Center"/>
    ///// </summary>
    //public float Radius { get; }
    public Vector3 Normal { get; }
}