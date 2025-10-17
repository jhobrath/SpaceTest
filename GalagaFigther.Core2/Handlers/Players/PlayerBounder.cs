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

            var playerPosition = MathExtensions.Clamp(player.Rect.Position,
                bounds.Min, bounds.Max - player.Rect.Size);

            player.MoveTo(playerPosition.X, playerPosition.Y);
        }
    }
}
