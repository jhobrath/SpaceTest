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
}