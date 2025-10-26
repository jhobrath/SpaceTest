using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Models.Particles
{
    public class ParticleInstance : GameObject
    {
        public float CurrentLifetime { get; set; }

        public ParticleEffectConfig Config { get; set; }

        private static Random _random = new Random();

        public Color StartColor { get; set; }
        public Color EndColor { get; set; }

        public ParticleInstance(Guid owner, Vector2 position, Vector2 speed, 
            SpriteBase sprite, ParticleEffectConfig config) 
            : base(owner, position, Vector2.One*config.StartSize, speed, sprite)
        {
            Config = config;
            CurrentLifetime = config.Lifetime;

            StartColor = RecalculateColor(config.StartColor);
            EndColor = RecalculateColor(config.EndColor);
        }

        private Color RecalculateColor(Color color)
        {
            return new Color(
                (float)(Math.Clamp(color.R + (Config.ColorVariation * (1 - 2 * _random.NextDouble())), 0, 255)/255),
                (float)(Math.Clamp(color.G + (Config.ColorVariation * (1 - 2 * _random.NextDouble())), 0, 255)/255),
                (float)(Math.Clamp(color.B + (Config.ColorVariation * (1 - 2 * _random.NextDouble())), 0, 255)/255),
                (float)color.A);
        }
    }
}