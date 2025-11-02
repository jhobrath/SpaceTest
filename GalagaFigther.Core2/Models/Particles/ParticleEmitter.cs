using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core2.Models.Particles
{
    public class ParticleEmitter : CollectibleGameObject
    {
        public ParticleEffectConfig Config { get; set; }

        public bool Enabled { get; set; }
        public bool IsFirstFrame { get; set; }

        public ParticleEmitter(Guid owner, Vector2 position, float size) 
            : base(owner, position, Vector2.One*size, Vector2.Zero, new StillImageSprite("DoesntExist"))
        {
            Enabled = true;
            IsFirstFrame = true;
        }
    }
}