using GalagaFigther.Core2.GameObjects;
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
        private readonly Dictionary<Type, object> _controllerRegistry = [];
        private readonly IController<Player> _playerController; 

        public ControllerFactory(IController<Player> playerController)
        {
            _playerController = playerController;
        }

        public void Update<T>(T gameObject, float frameTime) where T : GameObject
        {
            switch (gameObject)
            {
                case Player player:
                    _playerController.Update(player, frameTime);
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
            }
            
            throw new ArgumentException(null, nameof(gameObject));
        }
    }
}
