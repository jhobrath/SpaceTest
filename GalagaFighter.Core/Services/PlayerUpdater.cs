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

        public PlayerUpdater(IPlayerMover playerMover, IPlayerShooter playerShooter)
        {
            _playerMover = playerMover;
            _playerShooter = playerShooter;
        }

        public void Update(Game game, Player player)
        {
            var frameTime = Raylib.GetFrameTime();

            var effects = new EffectModifiers(player.Sprite)
            {
                Stats = new PlayerStats(),
                Display = new PlayerDisplay(),
                Projectile = new PlayerProjectile()
            };

            foreach(var effect in player.Effects)
                effect.OnUpdate(frameTime);

            foreach(var playerEffect in player.Effects)
                playerEffect.Apply(effects);

            player.Rotation = player.IsPlayer1 ? 90f : -90f;

            _playerMover.Move(player, effects);
            _playerShooter.Shoot(player, effects);
        }
    }
}
