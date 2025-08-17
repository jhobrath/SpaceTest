using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players
{
    public class PlayerCollisionBehavior : IPlayerCollisionBehavior
    {
        public PlayerCollisionUpdate Apply(Player player, PlayerCollisionUpdate collisionUpdate)
        {
            foreach (var projectile in collisionUpdate.Hits)
            {
                HandleHit(projectile, collisionUpdate);
                CreateCollision(projectile, collisionUpdate);
            }

            return collisionUpdate;
        }

        protected virtual void HandleHit(Projectile projectile, PlayerCollisionUpdate collisionUpdate)
        {
            collisionUpdate.Destroy.Add(projectile);
            collisionUpdate.DamageDealt += projectile.Damage;
        }

        protected virtual void CreateCollision(Projectile projectile, PlayerCollisionUpdate collisionUpdate)
        {
            var collisions = projectile.Collide();
            foreach (var collision in collisions)
                collisionUpdate.Create.Add(collision);
        }
    }
}
