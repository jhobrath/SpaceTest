using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Statuses;
using GalagaFighter.Core2.Handlers.Projectiles;
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
    public class IceProjectile : Projectile
    {
        public override List<ParticleEffectConfig> EmitterConfigurations => GetEmitter();

        private List<ParticleEffectConfig> GetEmitter()
        {
            var emitter = ParticleEffectTemplates.Get("SnowTrail");
            emitter.Duration = .8f;
            emitter.Loop = false;
            return [emitter];
        }

        private static Vector2 _baseSpeed => new(1420f, 0f);
        private static Vector2 _baseSize => new(95f, 42f);

        private static Vector2[] _bounds = new Vector2[]
        {
            new(.04f, .04f),
            new(.91f, .465f),
            new(.04f, .96f)
        };

        public IceProjectile(Guid owner) 
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
            Bounds = _bounds;
            Damage = 0;

            Behaviors.Add(typeof(RotationFollowsSpeedBehavior));
            Behaviors.Add(typeof(EdgeDeactivatesBehavior));
        }

        private static SpriteBase GetSprite()
        {
            return new NonRepeatingAnimatedImageSprite("Sprites/Projectiles/ice.png", 6, 570 / 6, 42, .2f);
        }

        public override List<PlayerEffect> CreateEffects(Player player)
        {
            return [new FrozenEffect()];
        }
    }
}
