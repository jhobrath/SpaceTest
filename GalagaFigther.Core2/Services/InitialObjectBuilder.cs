using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Projectiles;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core2.Services
{
    public interface IInitialObjectBuilder
    {
        void Build();
    }
    public class InitialObjectBuilder : IInitialObjectBuilder
    {
        private IObjectService _objectService;
        private IPersistentValueHandler _persistentValueHandler;
        private IGameDataRegistry _gameDataRegistry;

        public InitialObjectBuilder(IObjectService objectService, IPersistentValueHandler persistentValueHandler, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            _persistentValueHandler = persistentValueHandler;
            _gameDataRegistry = gameDataRegistry;
        }

        public void Build()
        {
            var screenHeight = Raylib.GetScreenHeight();
            var screenWidth = Raylib.GetScreenHeight();
            CreatePlayer(screenWidth, screenHeight);
        }

        private void CreatePlayer(int screenWidth, int screenHeight)
        {
            var playerHeight = 168f;
            var pos = new Vector2(0f, (screenHeight - playerHeight) / 2);
            var size = Vector2.One * 168;
            var speed = Vector2.Zero;
            var player = new Player(pos, size, speed, new StillImageSprite("Sprites/Ships/MainShipBody.png"))
            {
                Rotation = 90
            };

            _persistentValueHandler.RegisterRange(player, p => p.X, 0, 400f);
            _persistentValueHandler.RegisterRange(player, p => p.Y, 0, screenHeight - player.Height);

            var effects = _gameDataRegistry.Get<List<PlayerEffect>>(player);
            effects.Add(new DefaultShootEffect());

            _objectService.Add(player);
        }
    }
}
