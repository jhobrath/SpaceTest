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
    public interface IPlayerController : IController<Player>
    {

    }
    public class PlayerController : IPlayerController 
    {
        private readonly IPlayerMover _playerMover;
        private readonly IPlayerRotator _playerRotator;
        private readonly IPlayerAffector _playerAffector;
        private readonly IPlayerDrawer _playerDrawer;
        private readonly IPlayerShooter _playerShooter;

        public PlayerController(IPlayerMover playerMover, IPlayerRotator playerRotator,
            IPlayerAffector playerAffector, IPlayerDrawer playerDrawer, IPlayerShooter playerShooter)
            : base()
        {
            _playerMover = playerMover;
            _playerRotator = playerRotator;
            _playerAffector = playerAffector;
            _playerDrawer = playerDrawer;
            _playerShooter = playerShooter;
        }

        public void Update(Player player, float frameTime)
        {
            _playerMover.Move(player, frameTime);
            _playerRotator.Rotate(player, frameTime);
            _playerAffector.Affect(player, frameTime);
            _playerShooter.Shoot(player, frameTime);
        }

        public void Draw(Player player, float frameTime)
        {
            _playerDrawer.Draw(player, frameTime);
        }
    }
}
