using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Projectiles
{
    public interface IParryProjectileService
    {
        void Parry(Player player);
    }
    public class ParryProjectileService : IParryProjectileService
    {
        private readonly IObjectService _objectService;

        private const float MaxDistance = 200f;

        public ParryProjectileService(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Parry(Player player)
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

                var xDist = distance;
                var yDist = distance/2;

                var xSpeedFactor = Math.Max((MaxDistance - xDist) / MaxDistance, .5f);
                var ySpeedFactor = Math.Max((MaxDistance - yDist) / MaxDistance, .5f);

                // Apply the repulsion magnitude in the calculated direction
                // For the "really good" effect, flip the X direction
                var parrySpeedX = projectile.Speed.X * projectile.Modifiers.SpeedMultiplier * -1;
                var parrySpeedY = ySpeedFactor;

                projectile.HurryTo(x: parrySpeedX, y: parrySpeedY);
                projectile.Modifiers.Homing = 2f;
                projectile.SetOwner(player.Id);
            }
        }
    }
}
