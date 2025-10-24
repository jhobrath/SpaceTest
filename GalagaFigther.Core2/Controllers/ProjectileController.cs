using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Controllers
{
    public interface IProjectileController : IController<Projectile>
    {

    }

    public class ProjectileController : IProjectileController 
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public ProjectileController(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Draw(Projectile projectile, float frameTime)
        {
            projectile.Sprite.Draw(projectile);
        }

        public void Update(Projectile projectile, float frameTime)
        {
            Rotate(projectile);
            Deactivate(projectile);
        }

        private void Rotate(Projectile projectile)
        {
            var theta = MathF.Atan2(projectile.Speed.Y, projectile.Speed.X);
            var rotation = theta * 180.0f / MathF.PI;
            projectile.Rotation = rotation;
        }

        private void Deactivate(Projectile projectile)
        {
            var screenData = _gameDataRegistry.Get<GameState>();
            if (projectile.X < -projectile.Width || projectile.X > screenData.ScreenSize.X)
                projectile.IsActive = false;
        }
    }
}
