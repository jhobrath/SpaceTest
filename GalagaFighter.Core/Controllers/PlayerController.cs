using GalagaFighter.Core.Handlers.Players;
using GalagaFighter.Core.Handlers.Projectiles;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;

namespace GalagaFighter.Core.Controllers
{
    public interface IPlayerController
    {
        void Draw(Player player);
        void Update(Game game, Player player);
    }

    public class PlayerController : IPlayerController
    {
        private readonly IPlayerMover _playerMover;
        private readonly IPlayerShooter _playerShooter;
        private readonly IInputService _inputService;
        private readonly IPlayerProjectileCollisionService _playerProjectileCollisionService;
        private readonly IPlayerDrawer _playerDrawer;
        private readonly IPlayerManagerFactory _playerManagerFactory;
        private readonly IPlayerSpender _playerSpender;
        private readonly IRepulsionProjectileService _repulsionProjectileService;

        // Per-player instance state (no more dictionaries!)
        private PlayerShootState _shootState = PlayerShootState.Idle;

        public PlayerController(IPlayerMover playerMover, IPlayerShooter playerShooter, IInputService inputService,
            IPlayerProjectileCollisionService playerProjectileCollisionService, IPlayerDrawer playerDrawer,
            IPlayerManagerFactory playerManagerFactory, IPlayerSpender playerSpender, IRepulsionProjectileService repulsionProjectileService)
        {
            _playerMover = playerMover;
            _playerShooter = playerShooter;
            _inputService = inputService;
            _playerProjectileCollisionService = playerProjectileCollisionService;
            _playerDrawer = playerDrawer;
            _playerManagerFactory = playerManagerFactory;
            _playerSpender = playerSpender;
            _repulsionProjectileService = repulsionProjectileService;
        }

        public void Update(Game game, Player player)
        {
            var frameTime = Raylib.GetFrameTime();
            var effectManager = _playerManagerFactory.GetEffectManager(player.Id);
            var modifiers = effectManager.GetModifiers();

            player.Rotation = player.IsPlayer1 ? 90f : -90f;
            player.Sprite = modifiers.Sprite;

            _playerMover.Move(player, modifiers);
            var shootState = _playerShooter.Shoot(player, modifiers);
            _shootState = shootState;

            _playerShooter.ShootOneTime(player, modifiers);

            if (modifiers.IsRepulsive)
                _repulsionProjectileService.Repulse(player);
            //var switchButton = _inputService.GetSwitch(player.Id);
            //if (switchButton.IsPressed)
            //    effectManager.SwitchEffect();

            _playerProjectileCollisionService.HandleCollisions(player, modifiers);

            var resourceManager = _playerManagerFactory.GetResourceManager(player);
            resourceManager.Update();

            _playerSpender.HandleDefensiveSpend(player, modifiers);
            _playerSpender.HandleOffensiveSpend(player, modifiers);
        }

        public void Draw(Player player)
        {
            var effectManager = _playerManagerFactory.GetEffectManager(player.Id);
            var modifiers = effectManager.GetModifiers();
            _playerDrawer.Draw(player, modifiers!, _shootState);
        }
    }
}