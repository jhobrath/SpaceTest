using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        private readonly IObjectService _objectService;

        public ProjectileController(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Draw(Projectile projectile, float frameTime)
        {
            projectile.Sprite.Draw(projectile);
        }

        public void Update(Projectile projectile, float frameTime)
        {
            Rotate(projectile);
            Home(projectile, frameTime);
            Deactivate(projectile, frameTime);
        }

        private void Home(Projectile projectile, float frameTime)
        {
            if (projectile.Homing == 0)
                return;

            var player = _objectService.Get<Player>(projectile.Owner);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
            var opponent = _objectService.GetOpponent(player);

            var finalHomingFactor = (projectile.Homing + modifiers.HomingFactor) * frameTime*3.0f;

            var currentSpeed = projectile.Speed.Length();
            var normalizedSpeed = Vector2.Normalize(projectile.Speed);
            var homingSpeed = Vector2.Normalize(-(projectile.WorldPosition - opponent.WorldPosition));

            var finalNormalizedSpeed = Vector2.Normalize(homingSpeed * finalHomingFactor + normalizedSpeed * (1-finalHomingFactor));
            var finalSpeed = finalNormalizedSpeed * currentSpeed;
            projectile.HurryTo(finalSpeed.X, finalSpeed.Y);
        }

        private void Rotate(Projectile projectile)
        {
            var theta = MathF.Atan2(projectile.Speed.Y, projectile.Speed.X);
            var rotation = theta * 180.0f / MathF.PI;
            projectile.Rotation = rotation;
        }

        private void Deactivate(Projectile projectile, float frameTime)
        {
            var screenData = _gameDataRegistry.Get<GameState>();
            if (projectile.WorldPosition.X < -50 || projectile.WorldPosition.X > screenData.ScreenSize.X + 50)
                projectile.IsActive = false;
            if (projectile.WorldPosition.Y < -50 || projectile.WorldPosition.Y > screenData.ScreenSize.Y + 50)
                projectile.IsActive = false;

            if(projectile.Lifetime != -1f)
            {
                projectile.Lifetime -= frameTime;
                if(projectile.Lifetime < 0f)
                {
                    projectile.IsActive = false;
                }    
            }
        }
    }
}
