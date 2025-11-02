using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Sprites;
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

            var rotationData = _gameDataRegistry.Get<PlayerRotationData>(player);

            foreach (var decoration in decorations)
            {
                if (decoration.Key == "Move" && player.Acceleration.Length() < 1f)
                    continue;

                if (!hasDrawnPlayer && decoration.Depth >= 0)
                {
                    DrawPlayer(player, frameTime);
                    hasDrawnPlayer = true;
                }

                decoration.Update(player, frameTime);
                decoration.Draw(player);
            }

            if (!hasDrawnPlayer)
                DrawPlayer(player, frameTime);

        }

        private void DrawPlayer(Player player, float frameTime)
        {
            var existingEffects = _gameDataRegistry.Get<PlayerRenderEffects>(player);
            if (existingEffects.Count == 0)
            { 
                player.Sprite.Draw(player);
                return;
            }
            var renderDetails = new RenderDetails
            {
                X = player.WorldPosition.X,
                Y = player.WorldPosition.Y,
                Width = player.Width,
                Height = player.Height,
                Red = player.Color.R,
                Green = player.Color.G,
                Blue = player.Color.B,
                Alpha = player.Color.A,
                Rotation = player.WorldRotation
            };

            foreach (var effect in existingEffects)
                effect.Apply(renderDetails, frameTime);

            existingEffects.RemoveAll(x => !x.IsActive);

            player.Sprite.Draw(new Rectangle(renderDetails.X, renderDetails.Y, renderDetails.Width, renderDetails.Height), renderDetails.Rotation, new Color((byte)renderDetails.Red, (byte)renderDetails.Green, (byte)renderDetails.Blue, (byte)renderDetails.Alpha));
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
