using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services.Static
{
    public static class PolygonCollisionDetector
    {
        public static bool Detect(Vector2[] poly1, Vector2[] poly2)
        {
            // Check separation on axes from first polygon
            for (int i = 0; i < poly1.Length; i++)
            {
                Vector2 edge = poly1[(i + 1) % poly1.Length] - poly1[i];
                Vector2 axis = new Vector2(-edge.Y, edge.X); // Perpendicular axis
                axis = Vector2.Normalize(axis);

                if (IsSeparatedOnAxis(poly1, poly2, axis))
                    return false;
            }

            // Check separation on axes from second polygon
            for (int i = 0; i < poly2.Length; i++)
            {
                Vector2 edge = poly2[(i + 1) % poly2.Length] - poly2[i];
                Vector2 axis = new Vector2(-edge.Y, edge.X); // Perpendicular axis
                axis = Vector2.Normalize(axis);

                if (IsSeparatedOnAxis(poly1, poly2, axis))
                    return false;
            }

            return true; // No separation found, polygons collide
        }

        private static bool IsSeparatedOnAxis(Vector2[] poly1, Vector2[] poly2, Vector2 axis)
        {
            var proj1 = ProjectPolygon(poly1, axis);
            var proj2 = ProjectPolygon(poly2, axis);

            return proj1.max < proj2.min || proj2.max < proj1.min;
        }

        private static (float min, float max) ProjectPolygon(Vector2[] vertices, Vector2 axis)
        {
            float min = Vector2.Dot(vertices[0], axis);
            float max = min;

            for (int i = 1; i < vertices.Length; i++)
            {
                float projection = Vector2.Dot(vertices[i], axis);
                if (projection < min) min = projection;
                if (projection > max) max = projection;
            }

            return (min, max);
        }
    }
}
