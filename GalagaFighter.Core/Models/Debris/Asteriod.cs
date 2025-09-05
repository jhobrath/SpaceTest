using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using GalagaFighter.Core.Models.Collisions;

namespace GalagaFighter.Core.Models.Debris
{
    public class Asteroid : GameObject
    {
        private float _rotation;

        private Vector2[] _vertices;

        public Asteroid(Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(Game.Id, GetSpriteAndVertices(out var verts), initialPosition, initialSize, initialSpeed)
        {
            var rectSize = new Vector2(verts.Max(x => x.X) - verts.Min(x => x.X), verts.Max(x => x.Y) - verts.Min(x => x.Y));
            var ty =  verts.Select(x => new Vector2(x.X / rectSize.X, x.Y / rectSize.Y)).ToArray();
            Hitbox = new HitboxVertices(ty);
            _rotation = 90f + (float)Game.Random.NextDouble() * 180f;
        }

        private static SpriteWrapper GetSpriteAndVertices(out Vector2[] vertices)
        {
            var result = Static.AsteroidSpriteFactory.CreateProceduralAsteroidSpriteWithVertices();
            vertices = result.vertices;
            return result.sprite;
        }

        public override void Update(Game game)
        {
            Move(Speed.X * Raylib.GetFrameTime(), Speed.Y * Raylib.GetFrameTime());
            Rotation += _rotation * Raylib_cs.Raylib.GetFrameTime();
        }

        public override void Draw()
        {
            //For some reason the sprites are drawn centered at <0,0> so we have to move them to the center of the rect.
            Sprite.Draw(Rect.Position + new Vector2(Rect.Width/2, Rect.Height/2), Rotation, Rect.Width, Rect.Height, Color.White);
        }

        internal void AddRotation(float rotationToAdd)
        {
            _rotation += rotationToAdd;
        }
    }
}
