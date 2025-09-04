using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class FirethrowerEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/beam.png";
        public override bool IsProjectile => true;

        private BeamProjectile? _beamProjectile;
        private readonly SpriteDecorations _decorations;

        public FirethrowerEffect()
        {
            _decorations = new SpriteDecorations
            {
                Guns = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipFireGuns.png"))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectiles);
            modifiers.Stats.FireRateMultiplier *= .235f;
            modifiers.Projectile.DeactivateOnCollision = false;
            modifiers.Projectile.Untouchable = true;
            modifiers.Projectile.IgnoreShipMovement = true;
            modifiers.Decorations = _decorations;
        }

        private Projectile CreateProjectiles(IProjectileController controller, Player player, Vector2 vector, PlayerProjectile modifiers)
        {
            _beamProjectile =  new BeamProjectile(controller, player, vector, modifiers, Color.Red);
            return _beamProjectile;
        }
    }
}
