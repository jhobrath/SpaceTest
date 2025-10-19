using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Projectiles;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Players;
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
            var screenWidth = Raylib.GetScreenWidth();
            var screenHeight = Raylib.GetScreenHeight();
            CreatePlayer(screenWidth, screenHeight);
        }

        private void CreatePlayer(int screenWidth, int screenHeight)
        {
           var player1 = CreatePlayer(0, 90, new(0,0), new(550f, screenHeight));
            var player2 = CreatePlayer(screenWidth - 168, -90, new(screenWidth-550f, 0), new(screenWidth,screenHeight));

            _objectService.Add(player1);
            _objectService.Add(player2);
        }

        private Player CreatePlayer(int x, float rotation, Vector2 min, Vector2 max)
        {
            var playerHeight = 168f;
            var pos = new Vector2(x, (max.Y - playerHeight) / 2);
            var size = Vector2.One * 168;
            var speed = Vector2.Zero;
            var player = new Player(pos, size, speed, new StillImageSprite("Sprites/Ships/MainShipBody.png"))
            {
                Rotation = rotation
            };

            var bounds = _gameDataRegistry.Get<PlayerBoundsData>(player);
            bounds.Min = min;
            bounds.Max = max;

            var effects = _gameDataRegistry.Get<PlayerEffects>(player);
            effects.Add(new DefaultShootEffect());

            return player;
        }
    }
}
