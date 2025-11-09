using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using System.Numerics;

namespace GalagaFighter.Core2.Services.Static
{
    public static class PolygonVerticesCompiler
    {
        public static Vector2[] GetVertices(GameObject gameObject)
        {
            if(gameObject is GameObjects.Projectiles.ShotGunShellProjectile && gameObject.Owner == Game.Player2Id)
            {
                var s = "";
            }

            var bounds = gameObject.Bounds ?? [new(0, 0), new(1, 0), new(1, 1), new(0, 1)];
            var vertices = bounds.Select(x => new Vector2(x.X * gameObject.Width, x.Y * gameObject.Height)).ToArray();
            vertices = vertices.Select(x => x + gameObject.WorldPosition - gameObject.Rect.Size/2).ToArray();

            var rotated = (gameObject is Projectile projectile && projectile.IsTransformChild)
                ? RotatePoints(vertices, gameObject.Center, gameObject.WorldRotation - 90f)
                : RotatePoints(vertices, gameObject.Center, gameObject.WorldRotation);

            return rotated;
        }

        public static Vector2[] RotatePoints(Vector2[] vertices, Vector2 origin, float rotation)
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
                vertices[i] = RotatePoint(vertices[i], origin, cos, sin);
            }

            return vertices;
        }

        public static Vector2 RotatePoint(Vector2 point, Vector2 origin, float rotation)
        {
            // Apply rotation if needed
            if (rotation == 0)
                return point;

            // Convert degrees to radians
            float rotationInRadians = rotation * MathF.PI / 180f;

            float cos = MathF.Cos(rotationInRadians);
            float sin = MathF.Sin(rotationInRadians);

            return RotatePoint(point, origin, cos, sin);
        }

        public static Vector2 RotatePoint(Vector2 point, Vector2 origin, float cos, float sin)
        {
            // Translate to make rotation origin the center (0,0)
            float x = point.X - origin.X;
            float y = point.Y - origin.Y;

            // Apply rotation
            point = new Vector2(
                x * cos - y * sin,
                x * sin + y * cos
            );

            // Translate back
            point += origin;

            return point;
        }
    }
}
