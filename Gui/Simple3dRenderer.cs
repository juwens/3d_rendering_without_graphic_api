using SkiaSharp;
using System.Numerics;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp.Views.Desktop;

namespace Gui;

class Simple3dRenderer
{
    static Vector3 eye = new(0, 0, -5);
    static float h_fov = 65;
    static SKSizeI renderResolution = new(200, 200);
    static Vector3 eyeToRenderPlane = new(0, 0, 1);
    static float renderPlaneWidth = (float)Math.Tan(h_fov / 2) * eyeToRenderPlane.Length() * 2;
    static float renderPlaneHeight = renderPlaneWidth / renderResolution.Width * renderResolution.Height;

    public static void Draw(SKPaintSurfaceEventArgs e)
    {

        var pxTriangles = GetPixelTriangles();
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.LightGray);
        canvas.Translate(1, 1);
        canvas.DrawRect(0, 0, renderResolution.Width, renderResolution.Height, new SKPaint() { StrokeWidth = 2, Color = SKColors.Black, IsStroke = true });

        var scaling = renderResolution.Width / renderPlaneWidth;
        foreach (var triangle in pxTriangles)
        {
            SKPoint[] points = new[]
            {
                new SKPoint(triangle.A.X * scaling, triangle.A.Y * scaling),
                new SKPoint(triangle.B.X * scaling, triangle.B.Y * scaling),
                new SKPoint(triangle.C.X * scaling, triangle.C.Y * scaling),
            };
            var colors = new SKColor[] { SKColors.Red, SKColors.Green, SKColors.Blue };
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


        // clockwise
        Triangle triangle = new(new(0, 0, 0), new(0, 1, 0), new(1, 0, 0));

        var geometryTriangles = new[] { triangle };
        List<Triangle> pixelTriangles = new();

        foreach (var tri in geometryTriangles)
        {
            Vector3? i_a = Intersection(eye, renderPlane, tri.A);
            Vector3? i_b = Intersection(eye, renderPlane, tri.B);
            Vector3? i_c = Intersection(eye, renderPlane, tri.C);

            if (i_a is null || i_b is null || i_c is null)
            {
                Debug.WriteLine("no pixel triangle found");
                Debugger.Break();
            }

            var pixelTriangle = new Triangle(i_a.Value, i_b.Value, i_c.Value);
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