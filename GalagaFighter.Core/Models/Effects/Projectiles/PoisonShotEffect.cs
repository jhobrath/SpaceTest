using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class PoisonShotEffect : ProjectileEffect
    {
        public override string IconPath => "Sprites/Effects/Projectiles/IceShot.png";

        private readonly SpriteDecorations _decorations;
        private float _lastShotTime = 2f;
        private PoisonProjectile _bubbleProjectile;

        public PoisonShotEffect()
        {
            _decorations = new SpriteDecorations
            {
                Guns = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipPoisonGuns.png"))
            };
        }

        private Projectile CreateProjectile(IProjectileController controller, Player owner, Vector2 position, PlayerProjectile modifiers)
        {
            if(_lastShotTime >= 1f)
            { 
                _bubbleProjectile = new PoisonProjectile(controller, owner, new SpriteWrapper("Purposefully Missing Image"), position, modifiers);
                _lastShotTime = 0f;
            }

            return _bubbleProjectile;
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectile);
            modifiers.Decorations.Apply(_decorations);
        }

        public override void OnUpdate(float frameTime)
        {
            _lastShotTime += Raylib.GetFrameTime();
            base.OnUpdate(frameTime);
        }

        public override Vector2 GetProjectileSpeed()
        {
            //TODO: Replace with bubble projectile
            return PoisonProjectile._baseSpeed;
        }
    }
}
