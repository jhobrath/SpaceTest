using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerProjectileCollisionService
    {
        void HandleCollisions();
    }

    public class PlayerProjectileCollisionService : IPlayerProjectileCollisionService
    {
        private readonly IObjectService _objectService;
        private readonly IPlayerEffectManager _playerEffectManager;

        public PlayerProjectileCollisionService(IObjectService objectService, IPlayerEffectManager playerEffectManager)
        {
            _objectService = objectService;
            _playerEffectManager = playerEffectManager;
        }

        public void HandleCollisions()
        {
            var players = _objectService.GetGameObjects<Player>();
            var projectiles = _objectService.GetGameObjects<Projectile>();

            foreach(var projectile in projectiles)
            {
                var target = players.Single(x => projectile.Owner != x.Id);

                if (!Raylib.CheckCollisionRecs(target.Rect, projectile.Rect))
                    continue;

                Collide(target, projectile);
            }
        }

        private void Collide(Player player, Projectile projectile)
        {
            var effects = projectile.CreateEffects();
            foreach (var effect in effects)
                player.Effects.AddRange(effects);

            player.Health -= projectile.BaseDamage;

            CreateCollision(player, projectile);

            projectile.IsActive = false;
        }

        private void CreateCollision(Player player, Projectile projectile)
        {
            var rect = projectile.Rect;
            var speed = projectile.Speed;

            bool useRight = speed.X > 0;
            bool useLeft = speed.X < 0;

            Vector2 position;
            if (useRight)
                position = new Vector2(rect.X + rect.Width, rect.Y + rect.Height / 2f);
            else if (useLeft)
                position = new Vector2(rect.X, rect.Y + rect.Height / 2f);
            else
                position = new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);

            var size = new Vector2(rect.Width, rect.Height);

            var collisions = projectile.CreateCollisions(player.Id, position, size, speed);

            foreach (var collision in collisions)
            {
                collision.Move(x: -collision.Rect.Size.X / 2);
                _objectService.AddGameObject(collision);
            }
        }
    }
}
