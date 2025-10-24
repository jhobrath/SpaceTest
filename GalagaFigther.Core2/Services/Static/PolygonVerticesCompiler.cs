using GalagaFighter.Core2.GameObjects;
using System.Numerics;

namespace GalagaFighter.Core2.Services.Static
{
    public static class PolygonVerticesCompiler
    {
        public static Vector2[] GetVertices(GameObject gameObject)
        {
            var bounds = gameObject.Bounds ?? [new(0, 0), new(1, 0), new(1, 1), new(0, 1)];
            var vertices = bounds.Select(x => new Vector2(x.X * gameObject.Width, x.Y * gameObject.Height)).ToArray();
            vertices = vertices.Select(x => x + gameObject.WorldPosition).ToArray();
            var rotated = ApplyRotation(vertices, gameObject.Center, gameObject.Rotation);
            return rotated;
        }

        private static Vector2[] ApplyRotation(Vector2[] vertices, Vector2 origin, float rotation)
        {
            // Apply rotation if needed
            if (rotation == 0)
                return vertices;

            // Convert degrees to radians
            float rotationInRadians = rotation * MathF.PI / 180f;

            float cos = MathF.Cos(rotationInRadians);
            float sin = MathF.Sin(rotationInRadians);

            for (int i = 0; i < vertices.Length; i++)
            {
                // Translate to make rotation origin the center (0,0)
                float x = vertices[i].X - origin.X;
                float y = vertices[i].Y - origin.Y;
                    
                // Apply rotation
                vertices[i] = new Vector2(
                    x * cos - y * sin,
                    x * sin + y * cos
                );
                    
                // Translate back
                vertices[i] += origin;
            }

            return vertices;
        }
    }
}
