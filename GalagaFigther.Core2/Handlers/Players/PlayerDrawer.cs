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
            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);

            var decorations = modifiers.Decorations.OrderBy(x => x.Depth);
            var hasDrawnPlayer = false;

            HandleColor(player, modifiers);

            foreach(var decoration in decorations)
            {
                if (decoration.Key == "Move" && player.Acceleration.Length() > 0f)
                    continue;

                if (!hasDrawnPlayer && decoration.Depth >= 0)
                {
                    player.Sprite.Draw(player);
                    hasDrawnPlayer = true;
                }

                decoration.Draw(player);
            }

            if (!hasDrawnPlayer)
                player.Sprite.Draw(player);
        }



        private void HandleColor(Player player, PlayerModifiers modifiers)
        {
            player.Color = Color.White;

            HandleColorChannel(modifiers.Display.RedAlpha, alpha => player.Color = player.Color.ApplyRed(alpha), amount => player.Color = player.Color.Darken(amount));
            HandleColorChannel(modifiers.Display.GreenAlpha, alpha => player.Color = player.Color.ApplyGreen(alpha), amount => player.Color = player.Color.Darken(amount));
            HandleColorChannel(modifiers.Display.BlueAlpha, alpha => player.Color = player.Color.ApplyBlue(alpha), amount => player.Color = player.Color.Darken(amount));

            if (modifiers.Display.Alpha != 1f)
                player.Color = player.Color.ApplyAlpha(modifiers.Display.Alpha);
        }

        private void HandleColorChannel(float channelValue, Action<float> applyTint, Action<float> applyDarken)
        {
            if (channelValue == 1f) return;

            if (channelValue > 1f)
                applyTint(channelValue - 1f);
            else
                applyDarken(1f - channelValue);
        }
    }
}
