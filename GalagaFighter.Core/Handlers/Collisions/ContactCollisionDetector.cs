using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Collisions
{
    public class ContactCollisionDetector
    {
        // Cache for rotated percentage vertices to avoid recalculating same rotations
        private static readonly Dictionary<(float rotation, int verticesHash), Vector2[]> _rotationCache = [];

        public bool HasCollision(GameObject obj1, GameObject obj2)
        {
            if (obj1.Hitbox == null && obj2.Hitbox == null )
                return BoundsComparer.CompareRectRect(obj1.Rect, obj2.Rect);

            if (obj1.Hitbox ==  null && obj2.Hitbox != null)
                return BoundsComparer.CompareRectVertices(obj1.Rect, GetActualBounds(obj2));

            if (obj1.Hitbox != null && obj2.Hitbox == null)
                return BoundsComparer.CompareRectVertices(obj2.Rect, GetActualBounds(obj1));

            if (obj1.Hitbox != null && obj2.Hitbox != null)
                return BoundsComparer.CompareVerticesVertices(GetActualBounds(obj1), GetActualBounds(obj2));

            return false;
        }

        private Vector2[] GetActualBounds(GameObject obj)
        {
            var worldVertices = ConvertPercentagesToWorldCoordinates(
                obj.Hitbox!.Vertices,
                obj.Rect,
                obj.Rotation
            );

            return worldVertices;
        }

        private Vector2[] ConvertPercentagesToWorldCoordinates(Vector2[] percentageVertices, Raylib_cs.Rectangle rect, float rotation)
        {
            // First, convert percentages to local coordinates relative to center
            Vector2[] localVertices = new Vector2[percentageVertices.Length];
            for (int i = 0; i < percentageVertices.Length; i++)
            {
                localVertices[i] = new Vector2(
                    rect.Width * percentageVertices[i].X - rect.Width / 2f,
                    rect.Height * percentageVertices[i].Y - rect.Height / 2f
                );
            }

            // Apply rotation if needed (use cache for performance)
            Vector2[] rotatedVertices;
            if (rotation != 0f)
            {
                rotatedVertices = GetRotatedVertices(localVertices, rotation);
            }
            else
            {
                rotatedVertices = localVertices;
            }

            // Translate to world position (this is cheap, no need to cache)
            var center = new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
            Vector2[] worldVertices = new Vector2[rotatedVertices.Length];
            for (int i = 0; i < rotatedVertices.Length; i++)
            {
                worldVertices[i] = rotatedVertices[i] + center;
            }

            return worldVertices;
        }

        private Vector2[] GetRotatedVertices(Vector2[] localVertices, float rotation)
        {
            // Create cache key
            int verticesHash = GetVerticesHash(localVertices);
            var cacheKey = (rotation, verticesHash);

            // Check cache first
            if (_rotationCache.TryGetValue(cacheKey, out var cachedVertices))
            {
                return cachedVertices;
            }

            // Calculate rotation
            float radians = rotation * (MathF.PI / 180f);
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);

            Vector2[] rotatedVertices = new Vector2[localVertices.Length];
            for (int i = 0; i < localVertices.Length; i++)
            {
                Vector2 point = localVertices[i];
                rotatedVertices[i] = new Vector2(
                    point.X * cos - point.Y * sin,
                    point.X * sin + point.Y * cos
                );
            }

            // Cache the result
            _rotationCache[cacheKey] = rotatedVertices;

            return rotatedVertices;
        }

        private int GetVerticesHash(Vector2[] vertices)
        {
            // Simple hash combining all vertex coordinates
            int hash = 17;
            foreach (var vertex in vertices)
            {
                hash = hash * 31 + vertex.X.GetHashCode();
                hash = hash * 31 + vertex.Y.GetHashCode();
            }
            return hash;
        }
    }
}