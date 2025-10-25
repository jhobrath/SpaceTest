using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Models.Particles
{
    public class ParticleInstance : GameObject
    {
        public float MaxLifetime { get; private set; }
        public float CurrentLifetime { get; set; }
        public float StartSize { get; private set; }
        public float EndSize { get; private set; }
        public Color StartColor { get; private set; }
        public Color EndColor { get; private set; }

        public ParticleInstance(Guid owner, Vector2 position, Vector2 size, Vector2 speed, 
            SpriteBase sprite, float lifetime, float startSize, float endSize, 
            Color startColor, Color endColor) 
            : base(owner, position, size, speed, sprite)
        {
            MaxLifetime = lifetime;
            CurrentLifetime = lifetime;
            StartSize = startSize;
            EndSize = endSize;
            StartColor = startColor;
            EndColor = endColor;
        }
    }
}