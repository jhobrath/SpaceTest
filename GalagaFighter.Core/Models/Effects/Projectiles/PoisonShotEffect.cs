using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
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

        public PoisonShotEffect()
        {
            _decorations = new SpriteDecorations
            {
                Guns = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipPoisonGuns.png"))
            };
        }

        private Projectile CreateProjectile(IProjectileController controller, Player owner, Vector2 position, PlayerProjectile modifiers)
            => new PoisonProjectile(controller, owner, position, modifiers);


        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Decorations.Apply(_decorations);
        }

        public override Vector2 GetProjectileSpeed()
        {
            //TODO: Replace with bubble projectile
            return PoisonProjectile._baseSpeed;
        }
    }
}
