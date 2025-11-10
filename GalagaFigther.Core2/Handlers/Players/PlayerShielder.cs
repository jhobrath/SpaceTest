using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Shields;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerShielder
    {
        void Shield(Player player, float frameTime);
    }

    public class PlayerShielder : IPlayerShielder
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;

        public PlayerShielder(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Shield(Player player, float frameTime)
        {
            var shieldCount = _objectService.GetAll<ShieldPixel>().Count(x => x.Owner == player.Id);
            if (shieldCount >= 10)
                return;

            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
            var shieldData = _gameDataRegistry.Get<PlayerShieldState>(player);

            if(!inputData.Shield.IsDown)
            {
                CenterPanels(shieldData.JustDrawn);
                shieldData.JustDrawn = [];
                shieldData.Start = null;
                return;
            }

            var playerTip = GetPlayerTip(player);

            if (!shieldData.Start.HasValue)
            {
                shieldData.Start = playerTip;
                return;
            }

            if(Vector2.Distance(playerTip, shieldData.Start.Value) > 30)
            {
                var shieldPixel = CreateShieldPixel(player, shieldData.Start.Value);
                shieldData.JustDrawn.Add(shieldPixel);
                shieldData.Start = playerTip;
            }
        }

        private void CenterPanels(List<ShieldPixel> justDrawn)
        {
            for(var i = 0;i < justDrawn.Count;i++)
            {

            }
        }

        private ShieldPixel CreateShieldPixel(Player player, Vector2 start)
        {
            var centerPoint = (player.WorldPosition + start) / 2f;
            var direction = GetPlayerTip(player) - start;
            var angleRadians = MathF.Atan2(direction.Y, direction.X);
            var angleDegrees = angleRadians * 180f / MathF.PI; // Negate for Raylib, +90 to align



            var shieldPixel = new ShieldPixel(player.Id, centerPoint);

            var actualDistance = direction.Length();
            shieldPixel.ScaleTo(x: actualDistance);
            shieldPixel.WorldPosition = centerPoint;
            shieldPixel.Rotation = angleDegrees;
            shieldPixel.Palette = player.Palette;

            _objectService.Add(shieldPixel);
            return shieldPixel;
        }

        private Vector2 GetPlayerTip(Player player)
        {
            var offsetAtZeroDegrees = new Vector2(0, -200); // Tip is 50 units "up" in local space
            var point = PolygonVerticesCompiler.RotatePoint(player.WorldPosition + offsetAtZeroDegrees, player.WorldPosition, player.WorldRotation);
            return point;
        }
    }
}
