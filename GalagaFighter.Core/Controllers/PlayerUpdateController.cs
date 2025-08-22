using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;

namespace GalagaFighter.Core.Controllers
{
    public interface IPlayerController
    {
        void Update(Game game, Player player);
    }

    public class PlayerUpdateController : IPlayerController
    {
        private readonly IPlayerMover _playerMover;
        private readonly IPlayerShooter _playerShooter;
        private readonly IPlayerSwitcher _playerSwitcher;
        private readonly IPlayerProjectileCollisionService _playerProjectileCollisionService;

        public PlayerUpdateController(IPlayerMover playerMover, IPlayerShooter playerShooter, IPlayerSwitcher playerSwitcher, IPlayerProjectileCollisionService playerProjectileCollisionService)
        {
            _playerMover = playerMover;
            _playerShooter = playerShooter;
            _playerSwitcher = playerSwitcher;
            _playerProjectileCollisionService = playerProjectileCollisionService;
        }

        public void Update(Game game, Player player)
        {
            var frameTime = Raylib.GetFrameTime();

            var effects = GetModifiers(player, frameTime);

            player.Rotation = player.IsPlayer1 ? 90f : -90f;
            player.CurrentFrameSprite = effects.Sprite;

            UpdateColors(player, effects);

            _playerMover.Move(player, effects);
            _playerShooter.Shoot(player, effects);
            _playerSwitcher.Switch(player, effects);
            _playerProjectileCollisionService.HandleCollisions(player, effects);

            player.CurrentFrameSprite.Update(frameTime);
        }

        private void UpdateColors(Player player, EffectModifiers effects)
        {
            player.CurrentFrameColor = Color.White;
            
            if(effects.Display.RedAlpha != 1f)
                player.CurrentFrameColor = player.CurrentFrameColor.ApplyRed(1 - effects.Display.RedAlpha);
            
            if(effects.Display.GreenAlpha != 1f)
                player.CurrentFrameColor = player.CurrentFrameColor.ApplyGreen(1 - effects.Display.GreenAlpha);

            if(effects.Display.BlueAlpha != 1f)
                player.CurrentFrameColor = player.CurrentFrameColor.ApplyBlue(1 - effects.Display.BlueAlpha);

            if(effects.Display.Opacity != 1f)
                player.CurrentFrameColor = player.CurrentFrameColor.ApplyAlpha(1 - effects.Display.Opacity);
        }

        private static EffectModifiers GetModifiers(Player player, float frameTime)
        { 

            var effects = new EffectModifiers(player.Sprite)
            {
                Stats = new PlayerStats(),
                Display = new PlayerDisplay(),
                Projectile = new PlayerProjectile()
            };

            foreach (var effect in player.Effects)
                if (!effect.IsProjectile || effect == player.SelectedProjectile)
                    effect.OnUpdate(frameTime);

            foreach (var effect in player.Effects)
                if (!effect.IsProjectile || effect == player.SelectedProjectile)
                    effect.Apply(effects);

            return effects;
        }
    }
}