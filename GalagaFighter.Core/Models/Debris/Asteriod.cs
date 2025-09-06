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
        private float _opacity = 1f;

        public Asteroid(Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(Game.Id, GetSpriteAndVertices(out var verts), initialPosition, initialSize, initialSpeed)
        {
            var rectSize = new Vector2(verts.Max(x => x.X) - verts.Min(x => x.X), verts.Max(x => x.Y) - verts.Min(x => x.Y));
            _vertices = verts.Select(x => new Vector2(x.X / rectSize.X, x.Y / rectSize.Y)).ToArray();
            Hitbox = new HitboxVertices(_vertices);
            _rotation = -25f + (float)Game.Random.NextDouble() * 50f;

            CreateParticleEffects();
        }

        public Asteroid((Image image, Vector2[] vertices) result, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(Game.Id, GetFromResult(result, out var verts), initialPosition, initialSize, initialSpeed)
        {
            var rectSize = new Vector2(verts.Max(x => x.X) - verts.Min(x => x.X), verts.Max(x => x.Y) - verts.Min(x => x.Y));
            _vertices = verts.Select(x => new Vector2(x.X / rectSize.X, x.Y / rectSize.Y)).ToArray();
            Hitbox = new HitboxVertices(_vertices);
            _rotation = -25f + (float)Game.Random.NextDouble() * 50f;

            CreateParticleEffects();
        }

        private static SpriteWrapper GetSpriteAndVertices(out Vector2[] vertices)
        {
            var result = Static.AsteroidSpriteFactory.CreateProceduralAsteroidSpriteWithVertices();
            var result1 = GetFromResult(result, out var verts);
            vertices = result.vertices;
            return result1;
        }

        private static SpriteWrapper GetFromResult((Image image, Vector2[] vertices) result, out Vector2[] verts)
        {
             var asteroidTexture = Raylib.LoadTextureFromImage(result.image);
            var sprite = new SpriteWrapper(asteroidTexture);
            Raylib.UnloadImage(result.image);
            verts = result.vertices;
            return sprite;
        }

        public override void Update(Game game)
        {
            if((Rect.Width + Rect.Height)/2 < 50)
            { 
                _opacity = _opacity - Raylib.GetFrameTime()/1.5f;
                if (_opacity < 0)
                    IsActive = false;
            }

            Move(Speed.X * Raylib.GetFrameTime(), Speed.Y * Raylib.GetFrameTime());
            Rotation += _rotation * Raylib_cs.Raylib.GetFrameTime();
        }

        public override void Draw()
        {
            //For some reason the sprites are drawn centered at <0,0> so we have to move them to the center of the rect.
            Sprite.Draw(Rect.Position + new Vector2(Rect.Width/2, Rect.Height/2), Rotation, Rect.Width, Rect.Height, new Color((int)255,(int)255,(int)255, (int)(Math.Clamp(_opacity * 255, 0, 255))));
        }

        internal void AddRotation(float rotationToAdd)
        {
            _rotation += rotationToAdd;
        }

        private void CreateParticleEffects()
        {
            if ((Rect.Width + Rect.Height) / 2 < 85)
                return;


            int particleCount = Game.Random.Next(4, 8);
            for (int i = 0; i < particleCount; i++)
            {
                // Pick a random vertex
                var vertex = _vertices[Game.Random.Next(_vertices.Length)];
                // Scale vertex to asteroid size
                var localPos = new Vector2(vertex.X * Rect.Width, vertex.Y * Rect.Height);
                // Correct offset: randomize and center
                var offset = localPos
                    + new Vector2(
                        (float)(Game.Random.NextDouble() - 0.5) * Rect.Width * 0.2f,
                        (float)(Game.Random.NextDouble() - 0.5) * Rect.Height * 0.2f
                      )
                    - new Vector2(Rect.Width / 2f, Rect.Height / 2f);

                // Get dust effect template from library and clone it
                var effect = new ParticleEffect(ParticleEffectsLibrary.Get("AsteroidDust"));
                effect.Offset = offset;
                effect.Duration = 0.3f + (float)Game.Random.NextDouble() * 0.3f;
                effect.MaxParticles = Game.Random.Next(8, 16);

                effect.FollowRotation = true;

                AddParticleEffect(effect);
            }
        }
    }
}
