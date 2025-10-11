using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Services
{
    public interface IControllerFactory
    {
        void Update<T>(T gameObject, float frameTime) where T : GameObject;
        void Draw<T>(T gameObject, float frameTime) where T : GameObject;
    }

    public class ControllerFactory : IControllerFactory
    {
        private readonly Dictionary<Type, object> _controllerRegistry = [];
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly ControllerBase<Player> _playerController; 

        public ControllerFactory(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
            _playerController = new PlayerController(_gameDataRegistry);
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
