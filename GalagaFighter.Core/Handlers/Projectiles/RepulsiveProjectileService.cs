using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Projectiles
{
    public interface IRepulsionProjectileService
    {
        void Repulse(Player player);
    }
    public class RepulsionProjectileService : IRepulsionProjectileService
    {
        private IObjectService _objectService;

        public RepulsionProjectileService(IObjectService objectService, IInputService inputService)
        {
            _objectService = objectService;
        }

        public void Repulse(Player player)
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();
    
            foreach (var projectile in projectiles)
            {
                if (projectile.Owner == player.Id)
                    continue;

                var offset = (player.Center.X - projectile.Center.X);

                var distance = Vector2.Distance(player.Center, projectile.Center);
                if (distance > 350f)
                    continue;

                var xDist = player.Center.X - projectile.Center.X;
                var yDist = player.Center.Y - projectile.Center.Y;
                
                // Calculate the repulsion force magnitude based on distance
                var repulsionMagnitude = ((500f - distance) / 500f) * 2000;
                
                // Calculate the normalized direction vector from player to projectile
                var directionX = -xDist / distance; // Negative because we want to push away from player
                var directionY = -yDist / distance; // Negative because we want to push away from player
                
                // Apply the repulsion magnitude in the calculated direction
                // For the "really good" effect, flip the X direction
                var repulseSpeedX = repulsionMagnitude * directionX * -1;
                var repulseSpeedY = repulsionMagnitude * directionY;

                var newSpeedX = repulseSpeedX;
                var newSpeedY = repulseSpeedY;

                if (newSpeedX < 0 && projectile.Speed.X > 0 || newSpeedX > 0 && projectile.Speed.X < 0)
                    projectile.SetOwner(player.Id);

                projectile.HurryTo(x: newSpeedX, y: newSpeedY);
            }
        }
    }
}
