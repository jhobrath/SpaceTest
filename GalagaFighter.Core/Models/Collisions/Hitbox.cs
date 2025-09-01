using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Collisions
{
    public abstract class Hitbox
    {
        public abstract Vector2[] Vertices { get; }
        public abstract Vector2 Center { get; }
    }

    public class HitboxTriangle : Hitbox
    {
        private readonly Vector2[] _vertices;

        public HitboxTriangle(Vector2[] vertices)
        {
            if (vertices.Length != 3)
                throw new ArgumentException("Triangle hitbox requires exactly 3 vertices");

            _vertices = vertices;
        }

        public override Vector2 Center =>
            new(
                (_vertices[0].X + _vertices[1].X + _vertices[2].X) / 3f,
                (_vertices[0].Y + _vertices[1].Y + _vertices[2].Y) / 3f
            );

        public override Vector2[] Vertices => _vertices;
    }

    public class HitboxRectangle : Hitbox
    {
        private readonly Rectangle _rect;

        public HitboxRectangle(Rectangle rectangle)
        {
            _rect = rectangle;
        }

        public override Vector2 Center => GetCenter();
        public override Vector2[] Vertices => GetVertices();

        private Vector2 GetCenter()
        {
            return new Vector2(_rect.X + _rect.Width/2, _rect.Y + _rect.Height /2);
        }

        private Vector2[] GetVertices()
        {
            return [Vector2.Zero, new Vector2(0, _rect.Height), new Vector2(_rect.Width, _rect.Height), new Vector2(_rect.Width, 0)];
        }
    }
}