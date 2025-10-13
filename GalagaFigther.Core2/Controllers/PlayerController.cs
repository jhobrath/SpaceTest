using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Handlers.Players;
using GalagaFigther.Core2.Models.Player;
using GalagaFigther.Core2.Services;
using GalagaFigther.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Controllers
{
    public interface IController<T> where T : GameObject
    {
        void Update(T gameObject, float frameTime);
        void Draw(T gameObject, float frameTime);
    }

    public class PlayerController : IController<Player>
    {
        public readonly IPlayerMover _playerMover;
        public readonly IPlayerRotator _playerRotator;

        public PlayerController(IPlayerMover playerMover, IPlayerRotator playerRotator)
            : base()
        {
            _playerMover = playerMover;
            _playerRotator = playerRotator;
        }

        public void Update(Player player, float frameTime)
        {
            _playerMover.Move(player, frameTime);
            _playerRotator.Rotate(player, frameTime);
        }

        public void Draw(Player player, float frameTime)
        {
            player.Sprite.Draw(player);
        }
    }
}
