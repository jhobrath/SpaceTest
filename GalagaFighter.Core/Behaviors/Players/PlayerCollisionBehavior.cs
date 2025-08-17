using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players
{
    public class PlayerCollisionBehavior : IPlayerCollisionBehavior
    {
        private IObjectService _objectService;

        public PlayerCollisionBehavior(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Apply(Player player, Projectile projectile)
        {
            UpdatePlayer(player, projectile);
        }

        protected virtual void UpdatePlayer(Player player, Projectile projectile)
        {
            projectile.IsActive = false;
            player.Health -= projectile.Damage*player.Stats.Shield;
        }
    }
}
