using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Handlers.Players
{
    public interface IPlayerDrawer
    {
        void Draw(Player player, float frameTime);
    }

    public class PlayerDrawer : IPlayerDrawer
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public PlayerDrawer(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Draw(Player player, float frameTime)
        {
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);

            var decorations = modifiers.Decorations.OrderBy(x => x.Depth);
            var hasDrawnPlayer = false;

            foreach(var decoration in decorations)
            {
                if (!hasDrawnPlayer && decoration.Depth >= 0)
                {
                    player.Sprite.Draw(player.Rect, player.Rotation, player.Color);
                    hasDrawnPlayer = true;
                }

                decoration.Draw(player);

            }
        }
    }
}
