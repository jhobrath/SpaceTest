using Raylib_cs;
using GalagaFighter.Models.Players;
using System.Numerics;

namespace GalagaFighter.Models
{
    public static class ProjectileFactory
    {
        public static Projectile Create(ProjectileType type, Rectangle rect, Vector2 speed, Player owner)
        {
            return type switch
            {
                ProjectileType.Normal => new NormalProjectile(rect, speed, owner),
                ProjectileType.Explosive => new ExplosiveProjectile(rect, speed, owner),
                ProjectileType.Ice => new IceProjectile(rect, speed, owner),
                ProjectileType.Wall => new WallProjectile(rect, speed, owner),
                _ => new NormalProjectile(rect, speed, owner),
            };
        }
    }
}