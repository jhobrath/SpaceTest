using GalagaFighter.Core.Handlers.Players;
using GalagaFighter.Core.Handlers.Projectiles;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Linq;

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
        private readonly IPlayerDrawer _playerDrawer;
        private readonly IPlayerManagerFactory _playerManagerFactory;
        private readonly IPlayerSpender _playerSpender;
        private readonly IRepulsionProjectileService _repulsionProjectileService;
        private readonly IPlayerParticleManager _playerParticleManager;
        private readonly IParryProjectileService _parryProjectileService;

        // Per-player instance state (no more dictionaries!)
        private PlayerShootState _shootState = PlayerShootState.Idle;

        public PlayerController(IPlayerMover playerMover, IPlayerShooter playerShooter,
            IPlayerDrawer playerDrawer, IPlayerManagerFactory playerManagerFactory,
            IPlayerSpender playerSpender, IRepulsionProjectileService repulsionProjectileService,
            IPlayerParticleManager playerParticleManager, IParryProjectileService parryProjectileService)
        {
            _playerMover = playerMover;
            _playerShooter = playerShooter;
            _playerDrawer = playerDrawer;
            _playerManagerFactory = playerManagerFactory;
            _playerSpender = playerSpender;
            _repulsionProjectileService = repulsionProjectileService;
            _playerParticleManager = playerParticleManager;
            _parryProjectileService = parryProjectileService;
        }

        public void Update(Game game, Player player)
        {
            var frameTime = Raylib.GetFrameTime();
            var effectManager = _playerManagerFactory.GetEffectManager(player.Id);
            var modifiers = effectManager.GetModifiers();

            //TODO: THIS IS NOT THE PLACE FOR THIS
            UpdatePhantoms(player, modifiers);

            player.Rotation = player.IsPlayer1 ? 90f : -90f;
            _playerMover.Move(player, modifiers);
            var shootState = _playerShooter.Shoot(player, modifiers);
            _shootState = shootState;

            _playerShooter.ShootOneTime(player, modifiers);

            if (modifiers.IsRepulsive)
                _repulsionProjectileService.Repulse(player);

            if (modifiers.Parry)
                _parryProjectileService.Parry(player);

            var resourceManager = _playerManagerFactory.GetResourceManager(player);
            resourceManager.Update();



            _playerSpender.HandleDefensiveSpend(player, modifiers);
            _playerSpender.HandleOffensiveSpend(player, modifiers);

            _playerParticleManager.UpdateModifierEffects(player, modifiers);
        }

        private void UpdatePhantoms(Player player, EffectModifiers modifiers)
        {
            if(modifiers.PhantomCount != modifiers.Phantoms.Count)
            {
                modifiers.Phantoms.Clear();
                for (var i = 0; i < modifiers.PhantomCount; i++)
                    modifiers.Phantoms.Add(new Phantom(player, i));
            }
        }

        public void Draw(Player player)
        {
            var effectManager = _playerManagerFactory.GetEffectManager(player.Id);
            var modifiers = effectManager.GetModifiers();
            _playerDrawer.Draw(player, modifiers!, _shootState);
        }
    }
}