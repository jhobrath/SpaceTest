using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.GameObjects.Projectiles;
using GalagaFigther.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Controllers
{
    public interface IControllerFactory
    {
        void Update<T>(T gameObject, float frameTime) where T : GameObject;
        void Draw<T>(T gameObject, float frameTime) where T : GameObject;
    }

    public class ControllerFactory : IControllerFactory
    {
        private readonly IController<Player> _playerController;
        private readonly IController<DefaultProjectile> _defaultProjectileController;

        public ControllerFactory(IController<Player> playerController,
            IController<DefaultProjectile> defaultProjectileController)
        {
            _playerController = playerController;
            _defaultProjectileController = defaultProjectileController;
        }

        public void Update<T>(T gameObject, float frameTime) where T : GameObject
        {
            switch (gameObject)
            {
                case Player player:
                    _playerController.Update(player, frameTime);
                    return;
                case DefaultProjectile projectile:
                    _defaultProjectileController.Update(projectile, frameTime);
                    return;
            }

            throw new ArgumentException(null, nameof(gameObject));
        }

        public void Draw<T>(T gameObject, float frameTime) where T : GameObject
        {
            switch (gameObject)
            {
                case Player player:
                    _playerController.Draw(player, frameTime);
                    return;
                case DefaultProjectile projectile:
                    _defaultProjectileController.Draw(projectile, frameTime);
                    return;
            }
            
            throw new ArgumentException(null, nameof(gameObject));
        }
    }
}
