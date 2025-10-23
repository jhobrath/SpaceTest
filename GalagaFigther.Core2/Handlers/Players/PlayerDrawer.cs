using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerDrawer
    {
        void Draw(Player player, float frameTime);
    }

    public class PlayerDrawer : IPlayerDrawer
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;

        public PlayerDrawer(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Draw(Player player, float frameTime)
        {
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);

            var decorations = modifiers.Decorations.OrderBy(x => x.Depth);
            var hasDrawnPlayer = false;

            HandleColor(player, modifiers);

            foreach(var decoration in decorations)
            {
                if (!hasDrawnPlayer && decoration.Depth >= 0)
                {
                    player.Sprite.Draw(player.Rect, player.Rotation, player.Color);
                    hasDrawnPlayer = true;
                }

                decoration.Draw(player);
            }

            if (!hasDrawnPlayer)
                player.Sprite.Draw(player.Rect, player.Rotation, player.Color);

            var guns = _objectService.GetChildren<Gun>(player);
            foreach(var gun in guns)
            {
                gun.Sprite.Draw(player.Rect, player.Rotation, player.Color);
            }
        }



        private void HandleColor(Player player, PlayerModifiers modifiers)
        {
            player.Color = Color.White;

            if (modifiers.RedAlpha < 1f)
                player.Color = player.Color.ApplyRed(modifiers.RedAlpha);
            
            if (modifiers.GreenAlpha < 1f)
                player.Color = player.Color.ApplyGreen(modifiers.GreenAlpha);
            
            if (modifiers.BlueAlpha < 1f)
                player.Color = player.Color.ApplyBlue(modifiers.BlueAlpha);

            if (modifiers.Alpha > 0f)
                player.Color = player.Color.ApplyAlpha(modifiers.Alpha);
        }
    }
}
