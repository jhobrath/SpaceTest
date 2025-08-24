using GalagaFighter.Core.Handlers.Players;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;

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
        private readonly IPlayerSwitcher _playerSwitcher;
        private readonly IPlayerProjectileCollisionService _playerProjectileCollisionService;
        private readonly IPlayerDrawer _playerDrawer;

        // Per-player instance state (no more dictionaries!)
        private EffectModifiers? _modifiers;
        private PlayerShootState _shootState = PlayerShootState.Idle;

        public PlayerController(IPlayerMover playerMover, IPlayerShooter playerShooter, IPlayerSwitcher playerSwitcher, 
            IPlayerProjectileCollisionService playerProjectileCollisionService, IPlayerDrawer playerDrawer)
        {
            _playerMover = playerMover;
            _playerShooter = playerShooter;
            _playerSwitcher = playerSwitcher;
            _playerProjectileCollisionService = playerProjectileCollisionService;
            _playerDrawer = playerDrawer;
        }

        public void Update(Game game, Player player)
        {
            var frameTime = Raylib.GetFrameTime();

            var modifiers = GetModifiers(player, frameTime);

            player.Rotation = player.IsPlayer1 ? 90f : -90f;
            player.Sprite = modifiers.Sprite;

            _playerMover.Move(player, modifiers);
            var shootState = _playerShooter.Shoot(player, modifiers);
            _shootState = shootState;

            _playerSwitcher.Switch(player, modifiers);
            _playerProjectileCollisionService.HandleCollisions(player, modifiers);

            player.Sprite.Update(frameTime);
        }

        private EffectModifiers GetModifiers(Player player, float frameTime)
        { 
            var effects = new EffectModifiers(player.Sprite)
            {
                Stats = new PlayerStats(),
                Display = new PlayerDisplay() { Rotation = player.Rotation },
                Projectile = new PlayerProjectile()
            };

            foreach (var effect in player.Effects)
                if (!effect.IsProjectile || effect == player.SelectedProjectile)
                    effect.OnUpdate(frameTime);

            foreach (var effect in player.Effects)
                if (!effect.IsProjectile || effect == player.SelectedProjectile)
                    effect.Apply(effects);

            _modifiers = effects;

            return effects;
        }

        public void Draw(Player player)
        {
            _playerDrawer.Draw(player, _modifiers!, _shootState);
        }
    }
}