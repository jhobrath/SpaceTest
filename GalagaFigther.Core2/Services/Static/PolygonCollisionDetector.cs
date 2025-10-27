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
        public static Vector2? Detect(Vector2[] poly1, Vector2[] poly2)
        {
            float minOverlap = float.MaxValue;
            Vector2 minAxis = Vector2.Zero;
            bool foundCollision = true;

            // Check separation on axes from first polygon
            for (int i = 0; i < poly1.Length; i++)
            {
                Vector2 edge = poly1[(i + 1) % poly1.Length] - poly1[i];
                Vector2 axis = new Vector2(-edge.Y, edge.X); // Perpendicular axis
                axis = Vector2.Normalize(axis);

                var (separated, overlap) = CheckSeparationWithOverlap(poly1, poly2, axis);
                if (separated)
                {
                    foundCollision = false;
                    break;
                }

                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    minAxis = axis;
                }
            }

            if (!foundCollision)
                return null;

            // Check separation on axes from second polygon
            for (int i = 0; i < poly2.Length; i++)
            {
                Vector2 edge = poly2[(i + 1) % poly2.Length] - poly2[i];
                Vector2 axis = new Vector2(-edge.Y, edge.X); // Perpendicular axis
                axis = Vector2.Normalize(axis);

                var (separated, overlap) = CheckSeparationWithOverlap(poly1, poly2, axis);
                if (separated)
                {
                    foundCollision = false;
                    break;
                }

                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    minAxis = axis;
                }
            }

            if (!foundCollision)
                return null;

            // Calculate collision point using the minimum overlap axis
            return CalculateCollisionPoint(poly1, poly2, minAxis);
        }

        private static (bool separated, float overlap) CheckSeparationWithOverlap(Vector2[] poly1, Vector2[] poly2, Vector2 axis)
        {
            var proj1 = ProjectPolygon(poly1, axis);
            var proj2 = ProjectPolygon(poly2, axis);

            bool separated = proj1.max < proj2.min || proj2.max < proj1.min;
            
            if (separated)
                return (true, 0f);

            // Calculate overlap
            float overlap = Math.Min(proj1.max, proj2.max) - Math.Max(proj1.min, proj2.min);
            return (false, overlap);
        }

        private static Vector2 CalculateCollisionPoint(Vector2[] poly1, Vector2[] poly2, Vector2 axis)
        {
            // Project both polygons onto the minimum overlap axis
            var proj1 = ProjectPolygon(poly1, axis);
            var proj2 = ProjectPolygon(poly2, axis);

            // Find the overlap region
            float overlapStart = Math.Max(proj1.min, proj2.min);
            float overlapEnd = Math.Min(proj1.max, proj2.max);
            float overlapCenter = (overlapStart + overlapEnd) * 0.5f;

            // Find the point on the axis that represents the collision
            Vector2 axisPoint = axis * overlapCenter;

            // Find the closest points on each polygon to this axis point
            Vector2 center1 = GetPolygonCenter(poly1);
            Vector2 center2 = GetPolygonCenter(poly2);

            // Return a point between the two polygon centers, biased toward the collision axis
            Vector2 centerLine = (center1 + center2) * 0.5f;
            Vector2 collisionPoint = centerLine + axisPoint * 0.1f; // Small bias toward axis

            return collisionPoint;
        }

        private static Vector2 GetPolygonCenter(Vector2[] vertices)
        {
            Vector2 center = Vector2.Zero;
            foreach (var vertex in vertices)
            {
                center += vertex;
            }
            return center / vertices.Length;
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
