using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Collisions
{
    public interface ICollisionCreationService
    {
        void Create(Player player, Projectile projectile, Vector2? speedOverride = null);
    }
    public class CollisionCreationService : ICollisionCreationService
    {
        private IObjectService _objectService;

        public CollisionCreationService(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Create(Player player, Projectile projectile, Vector2? speedOverride = null)
        {
            //TODO: Find a way to see if a projectile has collisions before
            //bothering to calculate their spawn point
            var rect = projectile.CurrentFrameRect;
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

            var collisions = projectile.CreateCollisions(player, position, size, speedOverride ?? new Vector2(speed.X, 0f));

            foreach (var collision in collisions)
            {
                _objectService.AddGameObject(collision);

                if (collision.AnimateManually)
                    continue;

                collision.Move(x: -collision.Rect.Size.X / 2, y: -collision.Rect.Size.Y / 2);

                var positionXWithinShipBounds = (float)Math.Clamp(collision.Rect.X,
                    player.Rect.X,
                    player.Rect.X + player.Rect.Width - collision.Rect.Width);
                var positionYWithinShipBounds = (float)Math.Clamp(collision.Rect.Y,
                    player.Rect.Y,
                    player.Rect.Y + player.Rect.Height - collision.Rect.Height);

                collision.MoveTo(x: positionXWithinShipBounds, y: positionYWithinShipBounds);
                collision.GlueVerticallyTo(player);
            }
        }
    }
}
