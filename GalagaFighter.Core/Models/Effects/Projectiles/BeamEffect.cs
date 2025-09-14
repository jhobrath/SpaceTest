using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class BeamEffect : ProjectileEffect
    {
        public override string IconPath => "Sprites/effects/projectiles/beam.png";
        public override bool IsProjectile => true;

        private BeamProjectile? _beamProjectile;
        private readonly SpriteDecorations _decorations;

        public BeamEffect()
        {
            _decorations = new SpriteDecorations
            {
                Guns = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipBeam_Guns.png")) { FollowRotation = false }
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectiles);
            modifiers.Stats.FireRateMultiplier *= 0.5f; // Very slow fire rate - beam should be rare and powerful
            modifiers.Projectile.DeactivateOnCollision = false; // Beam stays active when hitting things
            modifiers.Projectile.IgnoreShipMovement = true; // Beam should NOT move with ship - stays stationary and grows
            modifiers.Projectile.FollowShipVertically = true; // But it should follow ship vertically
            modifiers.Decorations.Apply(_decorations);
        }

        private Projectile CreateProjectiles(IProjectileController controller, Player player, Vector2 vector, PlayerProjectile modifiers)
        {
            // If we already have an active beam projectile, reuse it instead of creating a new one
            if (_beamProjectile != null && _beamProjectile.IsActive)
            {
                _beamProjectile.Reset();
                return _beamProjectile;
            }

            // Create a new beam projectile only if we don't have an active one
            _beamProjectile = new BeamProjectile(controller, player, vector, modifiers, Color.Red);
            return _beamProjectile;
        }

        public override Vector2 GetProjectileSpeed()
        {
            return BeamProjectile._baseSpeed;
        }
    }
}
