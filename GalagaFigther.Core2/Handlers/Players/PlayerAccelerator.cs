using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services.Static;
using System.Numerics;
using GalagaFighter.Core2.Models.Players;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerAccelerator
    {
        void Accelerate(Player player, float frameTime);
    }
    public class PlayerMover : IPlayerAccelerator
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IInputService _inputService;

        public PlayerMover(IGameDataRegistry gameDataRegistry, IInputService inputService)
        {
            _gameDataRegistry = gameDataRegistry;
            _inputService = inputService;
        }

        public void Accelerate(Player player, float frameTime)
        {
            var accelX = 0f;
            var accelY = 0f;

            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
                    
            if (inputData.Left && !inputData.Right)
                accelX = -500f/.2f;
            if (inputData.Right && !inputData.Left)
                accelX = 500f/.2f;

            if (inputData.Forward.IsDown && !inputData.Back.IsDown)
                accelY = -300f/.18f;

            if (inputData.Back.IsDown && !inputData.Forward.IsDown)
                accelY = 300f/.18f;

            player.AccelTo(accelX, accelY);

            if(accelX > 0)
                player.HurryTo(Math.Max(player.Speed.X, modifiers.Stats.SpeedMultiplier * 200f));
            else if(accelX < 0)
                player.HurryTo(Math.Min(player.Speed.X, modifiers.Stats.SpeedMultiplier * -200f));

            if (accelY > 0)
                player.HurryTo(y: Math.Max(player.Speed.Y, modifiers.Stats.SpeedMultiplier * 700f));
            else if (accelY < 0)
                player.HurryTo(y: Math.Min(player.Speed.Y, modifiers.Stats.SpeedMultiplier * -700f));

        }
    }
}
