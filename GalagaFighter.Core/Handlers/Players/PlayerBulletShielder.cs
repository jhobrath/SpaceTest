using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerBulletShielder
    {
        void Shield(Player player, EffectModifiers modifiers);
    }
    public class PlayerBulletShielder : IPlayerBulletShielder
    {
        private IObjectService _objectService;
        private const float MaxDistance = 200f;

        private readonly Dictionary<Guid, int> _projectileShields = [];

        public PlayerBulletShielder(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Shield(Player player, EffectModifiers modifiers)
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();

            foreach (var projectile in projectiles)
            {
                if (projectile.Owner == player.Id)
                    continue;

                var offset = (player.Center.X - projectile.Center.X);

                var distance = Vector2.Distance(player.Center, projectile.Center);
                if (distance > MaxDistance)
                    continue;

                projectile.Hurry(0f, 0f);
                projectile.Modifiers.RotationOffset = 90;

                var tunnelProofCollideRect = new Rectangle(projectile.Center.X + (player.IsPlayer1 ? -105 : 0), projectile.Center.Y - 15, 120, 30);
                var newTexture = SpriteGenerationService.CreateProjectileSprite(ProjectileType.Normal, (int)projectile.Rect.Size.X, (int)projectile.Rect.Size.Y, Color.Gray);
                projectile.Sprite = new SpriteWrapper(newTexture);
                _projectileShields.Add(projectile.Id, 0);

                projectile.Modifiers.OnNearProjectile!.Add((Projectile current, Projectile collidingProjectile) =>
                {
                    if (collidingProjectile.Owner == projectile.Owner)
                        return;

                    if (!Raylib.CheckCollisionRecs(tunnelProofCollideRect, collidingProjectile.Rect))
                        return;

                    collidingProjectile.IsActive = false;
                    _projectileShields[projectile.Id]++;

                    if (_projectileShields[projectile.Id] >= 10)
                    { 
                        projectile.IsActive = false;

                        //Must clear OnNearProjectile otherwise GC will not collect bullet since its a capture
                        projectile.Modifiers.OnNearProjectile = null;
                    }
                    else
                    { 
                        projectile.Modifiers.Opacity = (float)Math.Clamp((10f - (float)_projectileShields[projectile.Id]) / 10f, 0, 1);
                    }
                });
                projectile.SetOwner(player.Id);
            }
        }
    }
}
