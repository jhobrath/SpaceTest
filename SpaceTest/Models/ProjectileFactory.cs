using Raylib_cs;
using GalagaFighter.Models.Players;
using System.Numerics;
using SpaceTest.Models.Projectiles;
using GalagaFighter.Models.Effects;
using GalagaFigther.Models.Projectiles;

namespace GalagaFighter.Models
{
    public static class ProjectileFactory
    {
        public static Projectile Create(ProjectileEffect effect, Rectangle rect, Vector2 speed, Player owner)
        {
            if (effect is IceShotEffect)
                return new IceProjectile(rect, speed, owner, effect);
            if (effect is WallEffect)
                return new WallProjectile(rect, speed, owner, effect);
            if (effect is NinjaShotEffect)
                return new NinjaProjectile(rect, speed, owner, effect);

            return new NormalProjectile(rect, speed, owner, effect);
        }
    }
}