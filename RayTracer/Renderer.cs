using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Rasterizer.Models;
using Rasterizer.Scenes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Rasterizer;

public class Renderer
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float ZNear { get; private set; }
    public float ZFar { get; private set; }
    public Matrix4x4 ProjectionMatrix { get; private set; }

    public Renderer(int width, int height, float fieldOfView, float zNear, float zFar)
    {
        this.Width = width;
        this.Height = height;
        this.ZFar = zFar;
        this.ZNear = zNear;

        this.ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fieldOfView, (float)width / (float)height, zNear, zFar);
    }

    public Image RenderToImage(IScene scene)
    {
        var image = new Image<Rgb24>(Width, Height);
        var zBuffer = new float[Width, Height];
        InitZBuffer(zBuffer);

        var viewProjectionMatrix = scene.Camera.ViewMatrix * this.ProjectionMatrix;

        // Update Coordinates on GraphObjects
        scene.Objects.Where(o => o is SceneGraphObject).ToList().ForEach(o => ((SceneGraphObject)o).FlattenVerticesAndTriangles());

        var triangleVertices = scene.Objects.AsParallel()
                .ToDictionary(
                                o => o,
                                o => o.Triangles
                                        .Select(t => new Vertex[] { o.Vertices[t.A], o.Vertices[t.B], o.Vertices[t.C] }
                                                .Select(v => o.VertexShader.Transform(v, o.ModelMatrix * viewProjectionMatrix, o.ModelMatrix))
                                                .Select(v => v * (1f / v.Position.W))));
        var trianglesForRendering = triangleVertices.AsParallel()
                .ToDictionary(
                                o => o.Key,
                                o => o.Value
                                    .ToDictionary(
                                        t => t.ToArray(),
                                        t => t
                                            .Select(v => new Vector2(v.Position.X * (Width / 2f) + (Width / 2f), v.Position.Y * (Width / 2f) + (Height / 2f)))
                                            .ToArray())
                                        .Where(x => !IsObstructed(x.Value)).ToDictionary(x => x.Key, x => new Triangle(x.Value)));

        var boundingBoxPerObject = CalculateBoundingBoxes(trianglesForRendering);

        Parallel.ForEach(Enumerable.Range(0, image.Height),
            // new ParallelOptions { MaxDegreeOfParallelism = 1 },
            y => image.ProcessPixelRows(accessor =>
                {
                    var row = accessor.GetRowSpan(y);
                    for (var x = 0; x < image.Width; x++)
                    {
                        var pixelVector = new Vector2(x, y);
                        var hitObjectsFromBoundingBoxes = boundingBoxPerObject.Where(
                                b =>
                                    b.Value.MinX <= pixelVector.X
                                    && b.Value.MinY <= pixelVector.Y
                                    && b.Value.MaxX >= pixelVector.X
                                    && b.Value.MaxY >= pixelVector.Y).Select(x => x.Key);

                        // Else iterate over every triangle to find which one we are hitting
                        foreach (var obj in hitObjectsFromBoundingBoxes)
                        {
                            var objTriangles = trianglesForRendering[obj];
                            var trianglesInsideBoundingBox = objTriangles.Where(t =>
                                    t.Value.BoundingBox.MinX <= pixelVector.X
                                    && t.Value.BoundingBox.MinY <= pixelVector.Y
                                    && t.Value.BoundingBox.MaxX >= pixelVector.X
                                    && t.Value.BoundingBox.MaxY >= pixelVector.Y);

                            foreach (var triangle in trianglesInsideBoundingBox)
                            {
                                var lerpParams = PointInsideTriangle(triangle.Value.Corners[0], triangle.Value.InvertedMatrix, pixelVector);
                                if (lerpParams != null && lerpParams.Value.u >= 0 && lerpParams.Value.v >= 0 && lerpParams.Value.u + lerpParams.Value.v <= 1)
                                {
                                    var interpolatedVertex = InterpolateVertex(triangle.Key, lerpParams.Value.u, lerpParams.Value.v);
                                    var color = CalculateColor(obj, interpolatedVertex, scene, ref zBuffer, x, y);

                                    if (color != null)
                                    {
                                        color = ConvertToNonLinear(color.Value);

                                        row[x] = new Rgb24((byte)color.Value.X, (byte)color.Value.Y, (byte)color.Value.Z);
                                    }
                                }

                            }
                        }
                    }
                }));

        return image;
    }

    private static Dictionary<IGeometry, BoundingBox> CalculateBoundingBoxes(Dictionary<IGeometry, Dictionary<Vertex[], Triangle>> trianglesForRendering)
    {
        var boundingBoxPerObject = new Dictionary<IGeometry, BoundingBox>();
        foreach (var obj in trianglesForRendering)
        {
            var allVertices = obj.Value.SelectMany(v => v.Value.Corners);
            var minX = allVertices.Min(v => v.X);
            var minY = allVertices.Min(v => v.Y);
            var maxX = allVertices.Max(v => v.X);
            var maxY = allVertices.Max(v => v.Y);

            boundingBoxPerObject.Add(obj.Key, new BoundingBox(minX, minY, maxX, maxY));
        }

        return boundingBoxPerObject;
    }

    private void InitZBuffer(float[,] zBuffer)
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                zBuffer[x, y] = this.ZFar;
            }
        }
    }

    private Vector3 ConvertToNonLinear(Vector3 color)
    {
        var gamma = 2.2f;
        var nonLinear = Vector3.Clamp(color, new Vector3(0f), new Vector3(1f));
        nonLinear.X = (float)Math.Pow(nonLinear.X, 1 / gamma);
        nonLinear.Y = (float)Math.Pow(nonLinear.Y, 1 / gamma);
        nonLinear.Z = (float)Math.Pow(nonLinear.Z, 1 / gamma);

        return nonLinear * 255;
    }

    private Vertex InterpolateVertex(Vertex[] vertices, float u, float v)
    {
        var ac = vertices[2] - vertices[0];
        var ab = vertices[1] - vertices[0];

        var interpolatedVertex = vertices[0] + u * ab + v * ac;

        return interpolatedVertex;
    }

    private bool IsObstructed(Span<Vector2> onScreenCoordinates)
    {
        return false;
        // Backface culling
        var edge1 = new Vector2((float)onScreenCoordinates[1].X, (float)onScreenCoordinates[1].Y)
            - new Vector2((float)onScreenCoordinates[0].X, (float)onScreenCoordinates[0].Y);
        var edge2 = new Vector2((float)onScreenCoordinates[2].X, (float)onScreenCoordinates[2].Y)
            - new Vector2((float)onScreenCoordinates[1].X, (float)onScreenCoordinates[1].Y);

        return edge1.X * edge2.Y - edge1.Y * edge2.X > 0;
    }

    private (float u, float v)? PointInsideTriangle(Vector2 a, Matrix3x2? invertedMatrix, Vector2 q)
    {
        var aq = q - a;

        if (invertedMatrix == null)
        {
            return null;
        }
        var solution = Vector2.Transform(aq, invertedMatrix.Value);

        return (solution.X, solution.Y);
    }

    private Vector3? CalculateColor(IGeometry obj, Vertex q, IScene scene, ref float[,] zBuffer, int x, int y)
    {
        // Convert back to camera space
        var z = ZFar * ZNear / (ZFar + (ZNear - ZFar) * q.Position.Z);
        if (Single.IsInfinity(z))
        {
            return null;
        }
        var qNormalized = q * z;
        if (z <= ZNear || q.Position.Z >= zBuffer[x, y])
            return null;

        zBuffer[x, y] = q.Position.Z;

        var color = obj.FragmentShaders.Select(s => s.CalculateColor(ref qNormalized, scene)).Aggregate(Vector3.Zero, (c1, c2) => c1 + c2);
        return color;
    }
}

public record BoundingBox(float MinX, float MinY, float MaxX, float MaxY);

public class Triangle
{
    public Vector2[] Corners { get; private set; } = new Vector2[3];
    public BoundingBox BoundingBox { get; private set; }
    public Matrix3x2? InvertedMatrix { get; private set; }

    public Triangle(Vector2[] corners)
    {
        this.Corners = corners;

        var minX = Corners.Min(v => v.X);
        var minY = Corners.Min(v => v.Y);
        var maxX = Corners.Max(v => v.X);
        var maxY = Corners.Max(v => v.Y);
        this.BoundingBox = new BoundingBox(minX, minY, maxX, maxY);

        var ab = Corners[1] - Corners[0];
        var ac = Corners[2] - Corners[0];

        var matrix = new Matrix3x2(ab.X, ab.Y, ac.X, ac.Y, 0, 0);
        var isInvertible = Matrix3x2.Invert(matrix, out var invertedMatrix);
        if (isInvertible)
        {
            this.InvertedMatrix = invertedMatrix;
        }
    }
}
