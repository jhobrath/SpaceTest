using GalagaFigther.Core2.GameObjects.Projectiles;
using GalagaFigther.Core2.Models.Game;
using GalagaFigther.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Controllers
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
            projectile.Sprite.Draw(projectile.Rect, projectile.Rotation, projectile.Color);
        }

        public void Update(Projectile projectile, float frameTime)
        {
            projectile.Rotation = GetRotationFromSpeed(projectile);

            projectile.Move(projectile.Speed.X * frameTime, projectile.Speed.Y * frameTime);

            var screenData = _gameDataRegistry.Get<GameState>();
            if(projectile.X > screenData.ScreenSize.X)
            {
                projectile.IsActive = false;
            }
        }

        private float GetRotationFromSpeed(Projectile projectile)
        {
            return (float)(Math.Atan2(projectile.Speed.Y, projectile.Speed.X) * 180.0 / Math.PI);
        }
    }
}
