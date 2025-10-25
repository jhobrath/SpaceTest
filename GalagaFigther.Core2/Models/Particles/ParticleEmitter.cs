using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core2.Models.Particles
{
    public class ParticleEmitter : GameObject
    {
        public Vector2 Offset { get; set; }
        public List<ParticleInstance> Particles { get; set; } = [];

        public ParticleEmitter(Guid owner, Vector2 position, Vector2 size, Vector2 offset, SpriteBase sprite) 
            : base(owner, position, size, Vector2.Zero, sprite)
        {
            Offset = offset;
        }
    }
}