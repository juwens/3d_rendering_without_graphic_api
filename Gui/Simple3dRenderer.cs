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
    static Vector3 eye = new(0, 0, -5);
    static float h_fov = 65;
    static SKSizeI renderResolution = new(200, 200);
    static Vector3 eyeToRenderPlane = new(0, 0, 1);
    static float renderPlaneWidth = (float)Math.Tan(h_fov / 2) * eyeToRenderPlane.Length() * 2;
    static float renderPlaneHeight = renderPlaneWidth / renderResolution.Width * renderResolution.Height;
    static readonly bool UseRandomColor = false;
    static readonly Vector3 Light = new Vector3(5, 5, -5);
    static readonly Vector3 LightNormal = new Vector3(-1, -1, 1);

    public static void Draw(SKPaintSurfaceEventArgs e)
    {
        var sw = Stopwatch.StartNew();

        var pxTriangles = GetPixelTriangles();
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.LightGray);
        canvas.Scale(1, -1);
        canvas.Translate(renderResolution.Width / 2, -renderResolution.Height - 20);

        canvas.DrawRect(0, 0, renderResolution.Width, renderResolution.Height, new SKPaint() { StrokeWidth = 2, Color = SKColors.Black, IsStroke = true });

        var scaling = renderResolution.Width / renderPlaneWidth;
        float maxDistance = pxTriangles.SelectMany(x => x.Points).Select(x => Vector3.Distance(eye, x.Vector)).Max();

        foreach (var triangle in pxTriangles)
        {
            SKPoint[] points = triangle.Points.Select(x => new SKPoint(x.Vector.X * scaling + (scaling / 2), x.Vector.Y * scaling + (scaling / 2))).ToArray();

            var colors = triangle.Points.Select(x => GetColor(x, triangle)).ToArray();
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
                return new SKColor((byte)Random.Shared.Next(255), (byte)Random.Shared.Next(255), (byte)Random.Shared.Next(255));
            }

            float lightDet = Vector3.Dot(tr.Normal, LightNormal);
            if (lightDet < 0.00001f)
            {
                return SKColors.Magenta;
            }

            return pt.Color.ToSKColor().ScaleLuminescenceByDeterminant(lightDet);
        }

        sw.Stop();

        Debug.WriteLine($"frame time {sw.Elapsed.TotalMilliseconds:F0} ms");
    }

    private static IReadOnlyList<Triangle> GetPixelTriangles()
    {
        // assumption: left handed 3d space
        // unit is meter
        Vector3 viewDirection = new(0, 0, 1);

        Vector3 renderPlaneTopLeft = eye + eyeToRenderPlane + new Vector3(renderPlaneWidth / 2, renderPlaneHeight / 2, 0);
        //Triangle renderPlane = ;

        SKSize pixelSize = new SKSize(renderPlaneWidth / renderResolution.Width, renderPlaneHeight / renderResolution.Height);

        Vector3 renderPlaneCenter = eye + eyeToRenderPlane;
        Plane renderPlane = Plane.CreateFromVertices(renderPlaneCenter, renderPlaneCenter + Vector3.UnitX, renderPlaneCenter + Vector3.UnitY);

        var geometryTriangles = Models.Teapot
            .Chunk(3)
            .Select(x => new Triangle(x[0], x[1], x[2], Color.FromArgb(255, 237, 32)))
            .OrderByDescending(x => Vector3.Distance(eye, x.A.Vector)) // we have no z-buffer, so we draw every triangle from back to forth
            .ToArray();

        List<Triangle> pixelTriangles = new();

        foreach (var tri in geometryTriangles)
        {
            var determinant = Vector3.Dot(tri.Normal, eyeToRenderPlane);
            if (determinant < 0)
            {
                continue;
            }

            Vector3? i_a = Intersection(eye, renderPlane, tri.A.Vector);
            Vector3? i_b = Intersection(eye, renderPlane, tri.B.Vector);
            Vector3? i_c = Intersection(eye, renderPlane, tri.C.Vector);

            if (i_a is null || i_b is null || i_c is null)
            {
                Debug.WriteLine("no pixel triangle found");
                Debugger.Break();
            }

            var pixelTriangle = new Triangle(
                new(i_a.Value, tri.A.Color),
                new(i_b.Value, tri.B.Color),
                new(i_c.Value, tri.C.Color));

            pixelTriangles.Add(pixelTriangle);
        }

        return pixelTriangles;
    }

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