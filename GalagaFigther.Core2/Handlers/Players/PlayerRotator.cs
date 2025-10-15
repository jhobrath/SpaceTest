using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Models.Game;
using GalagaFigther.Core2.Models.Players;
using GalagaFigther.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Handlers.Players
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
            var ranges = _gameDataRegistry.Get<PlayerRanges>();
            var rotationData = _gameDataRegistry.Get<PlayerRotationData>(player);
            if(rotationData.InitialRotation == float.MinValue)
                rotationData.InitialRotation = player.Rotation;

            var offset = ranges.MaxRotation * player.Speed.Y / ranges.MaxSpeedY;
            player.Rotation = rotationData.InitialRotation + offset;
        }
    }
}
