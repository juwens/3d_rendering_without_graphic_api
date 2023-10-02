using SkiaSharp;
using System.Numerics;
using System.Diagnostics;
using System.Drawing;
using SkiaSharp.Views.Desktop;
using Gui.Data;
using Gui.Extensions;

namespace Gui;

class Simple3dRenderer
{
    static Vector3 Eye = new(0, 1, -5);
    static float h_fov = 65;
    static SKSizeI renderResolution = new(200, 200);
    static Vector3 eyeToRenderPlane = new(0, 0, 1);
    static float renderPlaneWidth = (float)Math.Tan(h_fov / 2) * eyeToRenderPlane.Length() * 2;
    static float renderPlaneHeight = renderPlaneWidth / renderResolution.Width * renderResolution.Height;
    static readonly bool UseRandomColor = false;
    static readonly Vector3 Light = new Vector3(5, 5, -5);
    static readonly Vector3 LightNormal = Vector3.Normalize(new Vector3(-1, -1, 1));
    static readonly float Pi = (float)Math.PI;


    public static void Draw(SKPaintSurfaceEventArgs e)
    {
        var sw = Stopwatch.StartNew();

        var rand = new Random(1);
        var deg = ((float)DateTime.UtcNow.TimeOfDay.TotalSeconds) % 360;
        var transform = RotateY(-deg / 10);

        var fragments = GetFragments(transform);
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.LightGray);
        canvas.Scale(1, -1);
        canvas.Translate(renderResolution.Width / 2, -renderResolution.Height - 20);

        canvas.DrawRect(0, 0, renderResolution.Width, renderResolution.Height, new SKPaint() { StrokeWidth = 2, Color = SKColors.Black, IsStroke = true });

        var scaling = renderResolution.Width / renderPlaneWidth;
        float maxDistance = fragments.Count == 0 ? 0 : fragments.SelectMany(x => x.SourceTriangle.Points).Select(x => Vector3.Distance(Eye, x.Vector)).Max();

        List<float> determinants = new List<float>();
        foreach (var fragment in fragments)
        {
            SKPoint[] points = fragment.PixelTriangle.Points.Select(x => new SKPoint(x.Vector.X * scaling + (scaling / 2), x.Vector.Y * scaling + (scaling / 2))).ToArray();

            var colors = fragment.SourceTriangle.Points.Select(x => GetColor(x, fragment.SourceTriangle)).ToArray();
            canvas.DrawVertices(SKVertexMode.Triangles, points, colors, new SKPaint() { Color = SKColors.Magenta });
        }

        var axisPaint = new SKPaint()
        {
            Color = SKColors.Black,
            IsStroke = true,
            StrokeWidth = 1,
        };
        canvas.DrawLine(0, renderResolution.Height / 2, renderResolution.Width, renderResolution.Height / 2, axisPaint);
        canvas.DrawLine(renderResolution.Width / 2, 0, renderResolution.Width / 2, renderResolution.Height, axisPaint);

        SKColor GetColor(Point3 pt, Triangle tr)
        {
            if (UseRandomColor)
            {
                return new SKColor((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));
            }

            float dotProd = Vector3.Dot(tr.Normal, LightNormal);
            determinants.Add(dotProd);

            return pt.Color.ToSKColor().ScaleLuminescenceByDeg(Acos(dotProd));
        }

        sw.Stop();

        Debug.WriteLine($"frame time {sw.Elapsed.TotalMilliseconds:F0} ms");

        var foo = determinants.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()).ToList();
    }

    // https://de.mathworks.com/help/phased/ref/rotz.html
    private static Matrix4x4 RotateZ(float phi)
    {
        var rad = (phi % 360f) * 2f * Pi;

        return new Matrix4x4(
            Cos(rad), -Sin(rad), 0, 0,
            Sin(rad), Cos(rad), 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);
    }

    // https://de.mathworks.com/help/phased/ref/rotx.html
    private static Matrix4x4 RotateX(float alpha)
    {
        var rad = (alpha % 360f) * 2f * Pi;

        return new Matrix4x4(
            1, 0, 0, 0,
            0, Cos(rad), -Sin(rad), 0,
            0, Sin(rad), Cos(rad), 0,
            0, 0, 0, 1);
    }

    // https://de.mathworks.com/help/phased/ref/roty.html
    private static Matrix4x4 RotateY(float beta)
    {
        var rad = (beta % 360f) * 2f * Pi;

        return new Matrix4x4(
            Cos(rad), 0, Sin(rad), 0,
            0, 1, 0, 0,
            -Sin(rad), 0, Cos(rad), 0,
            0, 0, 0, 1);
    }

    static float Acos(float rad) => (float)Math.Acos(rad);
    static float Cos(float rad) => (float)Math.Cos(rad);
    static float Sin(float rad) => (float)Math.Sin(rad);

    private static IReadOnlyList<Fragment> GetFragments(Matrix4x4 transform)
    {
        // assumption: left handed 3d space
        // unit is meter
        Vector3 viewDirection = new(0, 0, 1);

        Vector3 renderPlaneTopLeft = Eye + eyeToRenderPlane + new Vector3(renderPlaneWidth / 2, renderPlaneHeight / 2, 0);
        //Triangle renderPlane = ;

        SKSize pixelSize = new SKSize(renderPlaneWidth / renderResolution.Width, renderPlaneHeight / renderResolution.Height);

        Vector3 renderPlaneCenter = Eye + eyeToRenderPlane;
        Plane renderPlane = Plane.CreateFromVertices(renderPlaneCenter, renderPlaneCenter + Vector3.UnitX, renderPlaneCenter + Vector3.UnitY);

        var geometryTriangles = Models.GetVertices(Teapot.RawVertices, transform, true)
            .Chunk(3)
            .Select(x => new Triangle(x[0], x[1], x[2], Color.FromArgb(255, 237, 32)))
            .OrderByDescending(x => Vector3.Distance(Eye, x.A.Vector)) // we have no z-buffer, so we draw every triangle from back to forth
            .ToArray();

        List<Fragment> fragments = new();

        foreach (var tri in geometryTriangles)
        {
            var dotProd = Vector3.Dot(tri.Normal, eyeToRenderPlane);
            // culling
            if (dotProd > 0)
            {
                continue;
            }

            Vector3? isectA = Intersection(Eye, renderPlane, tri.A.Vector);
            Vector3? isectB = Intersection(Eye, renderPlane, tri.B.Vector);
            Vector3? isectC = Intersection(Eye, renderPlane, tri.C.Vector);

            if (isectA is null || isectB is null || isectC is null)
            {
                Debug.WriteLine("no pixel triangle found");
                Debugger.Break();
            }

            var fragment = new Fragment()
            {
                PixelTriangle = new Triangle(
                    new(isectA.Value, tri.A.Color),
                    new(isectB.Value, tri.B.Color),
                    new(isectC.Value, tri.C.Color)),

                SourceTriangle = tri,
            };
            fragments.Add(fragment);
        }

        return fragments;
    }

    /// <summary>
    /// https://www.habrador.com/tutorials/math/4-plane-ray-intersection/
    /// </summary>
    private static Vector3? Intersection(Vector3 l_0, Plane renderPlane, Vector3 p_triangle)
    {
        Vector3 p_0 = renderPlane.Normal * renderPlane.D;
        Vector3 n = renderPlane.Normal;
        Vector3 l = p_triangle - l_0; //l is the direction of the ray

        //t is the length of the ray, which we can get by combining the above equations:
        //t = ((p_0 - l_0) . n) / (l . n)
        //But there's a chance that the line doesn't intersect with the plane, and we can check this by first
        //calculating the denominator and see if it's not small. 
        //We are also checking that the denominator is positive or we are looking in the opposite direction
        float denominator = Vector3.Dot(l, n);

        if (denominator <= 0.00001f)
        {
            return null;
        }

        // The distance to the plane
        float t = Vector3.Dot(p_0 - l_0, n) / denominator;

        //Where the ray intersects with the plane
        return l_0 + l * t;
    }
}