using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using Raylib_cs;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerUpdater
    {
        void Update(Game game, Player player);
    }

    public class PlayerUpdater : IPlayerUpdater
    {
        private IPlayerMover _playerMover;
        private IPlayerShooter _playerShooter;
        private IPlayerSwitcher _playerSwitcher;

        public PlayerUpdater(IPlayerMover playerMover, IPlayerShooter playerShooter, IPlayerSwitcher playerSwitcher)
        {
            _playerMover = playerMover;
            _playerShooter = playerShooter;
            _playerSwitcher = playerSwitcher;
         }

        public void Update(Game game, Player player)
        {
            var frameTime = Raylib.GetFrameTime();

            EffectModifiers effects = GetModifiers(player, frameTime);

            player.Rotation = player.IsPlayer1 ? 90f : -90f;
            player.CurrentFrameSprite = effects.Sprite;

            _playerMover.Move(player, effects);
            _playerShooter.Shoot(player, effects);
            _playerSwitcher.Switch(player, effects);
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
