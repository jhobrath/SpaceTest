using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Collisions
{
    public abstract class AnimatedCollision : Collision
    {
        private int _frameCount;
        private float _frameDuration;

        protected override float Duration => _frameCount * _frameDuration;

        public AnimatedCollision(Guid owner, Texture2D texture, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed, int frameCount, float frameDuration)
            : base(owner, new SpriteWrapper(texture, frameCount, frameDuration, false), initialPosition, initialSize, initialSpeed)
        {
            _frameCount = frameCount;
            _frameDuration = frameDuration;
        }
    }
}
