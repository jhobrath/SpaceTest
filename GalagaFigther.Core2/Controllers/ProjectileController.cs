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
            Rotate(projectile);
            Move(projectile, frameTime);
            Deactivate(projectile);
        }

        private static void Move(Projectile projectile, float frameTime)
        {
            projectile.Move(projectile.Speed.X * frameTime, projectile.Speed.Y * frameTime);
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
