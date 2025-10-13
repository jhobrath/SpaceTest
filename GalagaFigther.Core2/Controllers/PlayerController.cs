using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Handlers.Players;
using GalagaFigther.Core2.Models.Players;
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
        private readonly IPlayerMover _playerMover;
        private readonly IPlayerRotator _playerRotator;
        private readonly IPlayerAffector _playerAffector;
        private readonly IPlayerDrawer _playerDrawer;

        public PlayerController(IPlayerMover playerMover, IPlayerRotator playerRotator,
            IPlayerAffector playerAffector, IPlayerDrawer playerDrawer)
            : base()
        {
            _playerMover = playerMover;
            _playerRotator = playerRotator;
            _playerAffector = playerAffector;
            _playerDrawer = playerDrawer;
        }

        public void Update(Player player, float frameTime)
        {
            _playerMover.Move(player, frameTime);
            _playerRotator.Rotate(player, frameTime);
            _playerAffector.Affect(player, frameTime);
        }

        public void Draw(Player player, float frameTime)
        {
            _playerDrawer.Draw(player, frameTime);
        }
    }
}
