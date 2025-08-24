using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;

namespace GalagaFighter.Core.Controllers
{
    public interface IPlayerController
    {
        void Draw(Player player);
        void Update(Game game, Player player);
    }

    public class PlayerUpdateController : IPlayerController
    {
        private readonly IPlayerMover _playerMover;
        private readonly IPlayerShooter _playerShooter;
        private readonly IPlayerSwitcher _playerSwitcher;
        private readonly IPlayerProjectileCollisionService _playerProjectileCollisionService;
        private readonly IPlayerDrawer _playerDrawer;

        private readonly Dictionary<Player, EffectModifiers> _modifiers = [];
        private readonly Dictionary<Player, PlayerShootState> _shootStates = [];

        public PlayerUpdateController(IPlayerMover playerMover, IPlayerShooter playerShooter, IPlayerSwitcher playerSwitcher, 
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
            _shootStates[player] = shootState;

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

            _modifiers[player] = effects;

            return effects;
        }

        public void Draw(Player player)
        {
            _playerDrawer.Draw(player, _modifiers[player], _shootStates.GetValueOrDefault(player, PlayerShootState.Idle));
        }
    }
}