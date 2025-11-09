using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerBounder
    {
        void Bound(Player player);
    }
    public class PlayerBounder : IPlayerBounder
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        private const int _maxOverstep = 500;

        public PlayerBounder(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Bound(Player player)
        {
            var bounds = _gameDataRegistry.Get<PlayerBoundsData>(player);

            var playerPosition = MathExtensions.Clamp(player.WorldPosition,
                bounds.Min, bounds.Max - player.Rect.Size);

            player.MoveTo(y: playerPosition.Y);


            if (player.Id == Game.Player1Id)
                UpdatePlayer1Position(player, bounds);
            else
                UpdatePlayer2Position(player, bounds);

            UpdatePlayerSpeed(player, bounds);

            bounds.LastLegalPosition = player.Rect.Position;
            bounds.LastLegalWorldPosition = player.WorldPosition;
        }

        private static void UpdatePlayerSpeed(Player player, PlayerBoundsData bounds)
        {
            if (player.Speed.X > bounds.MaxSpeed.X)
                player.HurryTo(bounds.MaxSpeed.X);
            if (player.Speed.X < -bounds.MaxSpeed.X)
                player.HurryTo(-bounds.MaxSpeed.X);
            if (player.Speed.Y > bounds.MaxSpeed.Y)
                player.HurryTo(y: bounds.MaxSpeed.Y);
            if (player.Speed.Y < -bounds.MaxSpeed.Y)
                player.HurryTo(y: -bounds.MaxSpeed.X);
        }

        private bool UpdatePlayer2Position(Player player, PlayerBoundsData bounds)
        {
            var xIsNaN = !(player.Y <= 0) && !(player.Y > 0);
            if (player.X > bounds.Max.X || xIsNaN)
            {
                player.MoveTo(bounds.LastLegalPosition.X);
                player.WorldPosition = bounds.LastLegalWorldPosition;
                return false;
            }

            var yIsNaN = !(player.Y <= 0) && !(player.Y > 0);
            if (player.Y < bounds.Min.Y || yIsNaN)
            {
                player.MoveTo(y: bounds.LastLegalPosition.Y);
                player.WorldPosition = bounds.LastLegalWorldPosition;
                return false;
            }

            var playerOverstep = bounds.Min.X - player.WorldPosition.X;
            if (playerOverstep < 0)
                return false;

            var baseStats = _gameDataRegistry.Get<PlayerBaseStats>(player);
            var overstepPct = playerOverstep / _maxOverstep;
            player.Hurry(x: 200f * overstepPct);
            return true;
        }

        private bool UpdatePlayer1Position(Player player, PlayerBoundsData bounds)
        {
            var xIsNaN = !(player.X <= 0) && !(player.X > 0);
            if (player.X < bounds.Min.X || xIsNaN)
            {
                player.MoveTo(bounds.LastLegalPosition.X);
                player.WorldPosition = bounds.LastLegalWorldPosition;
                return false;
            }

            var yIsNaN = !(player.Y <= 0) && !(player.Y > 0);
            if (player.Y < bounds.Min.Y || yIsNaN)
            {
                player.MoveTo(y: bounds.LastLegalPosition.Y);
                player.WorldPosition = bounds.LastLegalWorldPosition;
                return false;
            }

            var playerOverstep = player.WorldPosition.X - bounds.Max.X;
            if (playerOverstep < 0)
                return false;

            var baseStats = _gameDataRegistry.Get<PlayerBaseStats>(player);
            var overstepPct = playerOverstep / _maxOverstep;
            player.Hurry(x: -200f * overstepPct);
            return true;
        }
    }
}
