using System.Numerics;

namespace Gui;

// size = 9 * sizeof(float) = 36 byte
struct Triangle
{
    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        A = a;
        B = b;
        C = c;
    }

    public Vector3 A { get; }
    public Vector3 B { get; }
    public Vector3 C { get; }
}