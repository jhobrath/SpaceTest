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

            var maxOverstep = 500;
            if (player.Id == Game.Player1Id)
            {
                if (player.X < bounds.Min.X)
                { 
                    player.MoveTo(x: bounds.Min.X);
                    return;
                }

                var playerOverstep = player.WorldPosition.X - bounds.Max.X;
                if (playerOverstep < 0)
                    return;

                var baseStats = _gameDataRegistry.Get<PlayerBaseStats>(player);
                var overstepPct = playerOverstep / maxOverstep;
                player.Hurry(x: -200f * overstepPct);
            }
            else
            {
                if (player.X > bounds.Max.X)
                {
                    player.MoveTo(x: bounds.Max.X);
                    return;
                }

                var playerOverstep = bounds.Min.X - player.WorldPosition.X;
                if (playerOverstep < 0)
                    return;

                var baseStats = _gameDataRegistry.Get<PlayerBaseStats>(player);
                var overstepPct = playerOverstep / maxOverstep;
                player.Hurry(x: 200f * overstepPct);
            }
        }
    }
}
