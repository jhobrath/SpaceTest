using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerRotator
    {
        public void Rotate(Player player, float frameTime);
    }

    public class PlayerRotator : IPlayerRotator
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public PlayerRotator(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Rotate(Player player, float frameTime)
        {
            var rotationData = _gameDataRegistry.Get<PlayerRotationData>(player);
            if(rotationData.InitialRotation == float.MinValue)
                rotationData.InitialRotation = player.Rotation;

            var offsetY = rotationData.MaxRotationDueToMovement * Math.Clamp(player.Speed.Y / 1000f, -1, 1);
            var offsetX = rotationData.MaxRotationDueToMovement * Math.Clamp(player.Speed.X / 1000f, -1, 1);

            player.Rotation = rotationData.InitialRotation 
                + offsetY*rotationData.DirectionalVector.Y
                + offsetX*rotationData.DirectionalVector.X;
        }
    }
}
