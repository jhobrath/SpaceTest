using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Statuses;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public class NinjaProjectile : Projectile
    {
        public static readonly Vector2 _baseSpeed = new(2080f, 40f);
        public static readonly Vector2 _baseSize = new(38.8f, 27.77f);

        public override float Damage => 1f;
        public override float Veer => 1000f;

        public NinjaProjectile(Guid owner) 
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
            AngularVelocity = 1000f;
        }

        private static SpriteBase GetSprite()
        {
            return new AnimatedImageSprite("Sprites/Projectiles/Ninja.png", 3, 945 / 3, 250, .05f);
        }

        public override List<PlayerEffect> CreateEffects(Player player)
        {
            return [];
        }
    }
}
