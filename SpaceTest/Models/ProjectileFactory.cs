using Raylib_cs;
using GalagaFighter.Models.Players;
using System.Numerics;
using SpaceTest.Models.Projectiles;
using GalagaFighter.Models.Effects;

namespace GalagaFighter.Models
{
    public static class ProjectileFactory
    {
        public static Projectile Create(ProjectileEffect effect, Rectangle rect, Vector2 speed, Player owner)
        {
            if (effect is IceShotEffect)
                return new IceProjectile(rect, speed, owner);
            if (effect is WallEffect)
                return new WallProjectile(rect, speed, owner);
            
            return new NormalProjectile(rect, speed, owner);
        }
    }
}