using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Handlers.Players;
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
    public interface IPlayerController : IController<Player>
    {

    }
    public class PlayerController : IPlayerController 
    {
        private readonly IPlayerAccelerator _playerAccelerator;
        private readonly IPlayerRotator _playerRotator;
        private readonly IPlayerAffector _playerAffector;
        private readonly IPlayerDrawer _playerDrawer;
        private readonly IPlayerShooter _playerShooter;
        private readonly IPlayerBounder _playerBounder;
        private readonly IPlayerTurretDeployer _playerTurretDeployer;

        public PlayerController(IPlayerAccelerator playerAccelerator, IPlayerRotator playerRotator,
            IPlayerAffector playerAffector, IPlayerDrawer playerDrawer, IPlayerShooter playerShooter, 
            IPlayerBounder playerBounder, IPlayerTurretDeployer playerTurretDeployer)
            : base()
        {
            _playerAccelerator = playerAccelerator;
            _playerRotator = playerRotator;
            _playerAffector = playerAffector;
            _playerDrawer = playerDrawer;
            _playerShooter = playerShooter;
            _playerBounder = playerBounder;
            _playerTurretDeployer = playerTurretDeployer;
        }

        public void Update(Player player, float frameTime)
        {
            _playerAffector.Affect(player, frameTime);
            _playerAccelerator.Accelerate(player, frameTime);
            _playerRotator.Rotate(player, frameTime);
            _playerShooter.Shoot(player, frameTime);
            _playerTurretDeployer.Deploy(player, frameTime);
            _playerBounder.Bound(player);
        }

        public void Draw(Player player, float frameTime)
        {
            _playerDrawer.Draw(player, frameTime);
        }
    }
}
