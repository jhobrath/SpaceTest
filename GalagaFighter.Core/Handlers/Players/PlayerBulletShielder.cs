using GalagaFighter.Core.Handlers.Collisions;
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
        private readonly IObjectService _objectService;
        private readonly ICollisionCreationService _collisionCreationService;
        private const float MaxDistance = 200f;

        private readonly Dictionary<Guid, int> _projectileShields = [];

        public PlayerBulletShielder(IObjectService objectService, ICollisionCreationService collisionCreationService)
        {
            _objectService = objectService;
            _collisionCreationService = collisionCreationService;
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
                projectile.Modifiers.RotationOffset = player.IsPlayer1 ? 90 : -90f;
                projectile.Modifiers.RotationOffsetIncrement = 360f * ((float)Game.Random.NextDouble()+.5f);
                projectile.Modifiers.Homing = 0f;


                var tunnelProofCollideRect = new Rectangle(projectile.Center.X + (player.IsPlayer1 ? -60 : 0), projectile.Center.Y - 15, 65, 30);
                var newTexture = SpriteGenerationService.CreateProjectileSprite(ProjectileType.Normal, (int)projectile.Rect.Size.X, (int)projectile.Rect.Size.Y, Color.Gray);
                projectile.Sprite = new SpriteWrapper(newTexture);
                _projectileShields.Add(projectile.Id, 0);

                projectile.Modifiers.OnNearProjectile!.Add((Projectile current, Projectile collidingProjectile) =>
                {
                    if (!_projectileShields.ContainsKey(current.Id))
                        return;

                    if (collidingProjectile.Owner == current.Owner)
                        return;

                    if (!collidingProjectile.IsActive || !current.IsActive)
                        return;

                    if (!Raylib.CheckCollisionRecs(current.Rect, collidingProjectile.Rect))
                        return;


                    current.HurryTo(current.Speed.X + (player.IsPlayer1 ? -1f : 1f)*20, 0);

                    collidingProjectile.IsActive = false;

                    var collision = _collisionCreationService.Create(new Vector2(current.Center.X, collidingProjectile.Center.Y), new Vector2(25, 25), collidingProjectile.Speed/20);
                    collision.Rotation += 360f * (float)Game.Random.NextDouble();
                    _projectileShields[current.Id]++;

                    if (_projectileShields[current.Id] >= 10)
                    {
                        current.IsActive = false;

                        //Must clear OnNearProjectile otherwise GC will not collect bullet since its a capture
                        current.Modifiers.OnNearProjectile = null;
                    }
                    else
                    {
                        current.Modifiers.Opacity = (float)Math.Clamp((10f - (float)_projectileShields[current.Id]) / 10f, 0, 1);
                    }
                });
                projectile.SetOwner(player.Id);
            }
        }
    }
}
