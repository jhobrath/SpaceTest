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
                if (IsPointInPolygon(point, vertices))
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
            // Check if any vertex of vertices1 is inside vertices2
            foreach (var vertex in vertices1)
            {
                if (IsPointInPolygon(vertex, vertices2))
                    return true;
            }

            // Check if any vertex of vertices2 is inside vertices1
            foreach (var vertex in vertices2)
            {
                if (IsPointInPolygon(vertex, vertices1))
                    return true;
            }

            return false;
        }

        // General point-in-polygon test (ray casting)
        private static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
        {
            int n = polygon.Length;
            bool inside = false;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                    (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / ((polygon[j].Y - polygon[i].Y) + 0.0001f) + polygon[i].X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }
    }
}
