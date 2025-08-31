using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Collisions
{
    public static class BoundsComparer
    {
        public static bool CompareRectRect(Rectangle rect1, Rectangle rect2)
        {
            return Raylib.CheckCollisionRecs(rect1, rect2);
        }

        public static bool CompareRectVertices(Rectangle rect, Vector2[] vertices)
        {
            // Quick bounding rect check
            var triangleBounds = GetTriangleBoundingRect(vertices);
            if (!Raylib.CheckCollisionRecs(rect, triangleBounds))
                return false;

            // Rectangle points
            var rectPoints = new Vector2[]
            {
                new(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f), // Center
                new(rect.X, rect.Y), // Top-left
                new(rect.X + rect.Width, rect.Y), // Top-right
                new(rect.X, rect.Y + rect.Height), // Bottom-left
                new(rect.X + rect.Width, rect.Y + rect.Height) // Bottom-right
            };

            // Check if any rect point is inside triangle
            foreach (var point in rectPoints)
            {
                if (IsPointInTriangle(point, vertices[0], vertices[1], vertices[2]))
                    return true;
            }

            // Check if any triangle vertex is inside the rectangle
            foreach (var vertex in vertices)
            {
                if (vertex.X >= rect.X && vertex.X <= rect.X + rect.Width &&
                    vertex.Y >= rect.Y && vertex.Y <= rect.Y + rect.Height)
                    return true;
            }

            return false;
        }

        public static bool CompareVerticesVertices(Vector2[] vertices1, Vector2[] vertices2)
        {
            // For triangle-triangle collision, check if any vertex of either triangle is inside the other
            foreach (var vertex in vertices1)
            {
                if (IsPointInTriangle(vertex, vertices2[0], vertices2[1], vertices2[2]))
                    return true;
            }

            foreach (var vertex in vertices2)
            {
                if (IsPointInTriangle(vertex, vertices1[0], vertices1[1], vertices1[2]))
                    return true;
            }

            return false;
        }

        private static Rectangle GetTriangleBoundingRect(Vector2[] vertices)
        {
            float minX = Math.Min(vertices[0].X, Math.Min(vertices[1].X, vertices[2].X));
            float maxX = Math.Max(vertices[0].X, Math.Max(vertices[1].X, vertices[2].X));
            float minY = Math.Min(vertices[0].Y, Math.Min(vertices[1].Y, vertices[2].Y));
            float maxY = Math.Max(vertices[0].Y, Math.Max(vertices[1].Y, vertices[2].Y));

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        private static bool IsPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            // Using barycentric coordinates for point-in-triangle test
            var v0 = c - a;
            var v1 = b - a;
            var v2 = point - a;

            var dot00 = Vector2.Dot(v0, v0);
            var dot01 = Vector2.Dot(v0, v1);
            var dot02 = Vector2.Dot(v0, v2);
            var dot11 = Vector2.Dot(v1, v1);
            var dot12 = Vector2.Dot(v1, v2);

            var invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
            var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            return u >= 0 && v >= 0 && u + v <= 1;
        }
    }
}
