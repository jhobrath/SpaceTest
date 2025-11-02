using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public abstract class Projectile : GameObject
    {
        public float Lifetime { get; set; } = -1f;
        public virtual List<ParticleEffectConfig> EmitterConfigurations => [];

        public Projectile(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite)
            : base(owner, position, size, speed, sprite)
        {
            Owner = owner;
        }

        public virtual bool IsTransformChild => false;
        public abstract float Damage { get; }
        public virtual float Homing { get; } = 0f;
        public virtual float Veer { get; } = 0f;
        public virtual bool Destroys { get; } = false;

        public virtual List<PlayerEffect> CreateEffects(Player player) => [];
    }
}
