using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Projectiles;
using GalagaFighter.Core2.Services.Sprites;
using System.Numerics;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public class HomingProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(0f, 1750f);
        private static Vector2 _baseSize => new(30f, 10f);

        public HomingProjectile(Guid owner)
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
            Damage = 1;
            Behaviors.Add(typeof(RotationFollowsSpeedBehavior));
            Behaviors.Add(typeof(HomingBehavior));
            Behaviors.Add(typeof(EdgeDeactivatesBehavior));
            StateModels.Add(new HomingState { Homing = 1f });
        }

        private static SpriteBase GetSprite()
        {
            return new DrawnSprite((color) => 
                DefaultProjectileSpriteGenerator.CreateProjectileSprite(color: color));
        }

        public override List<PlayerEffect> CreateEffects(Player player) => [];
    }
}
