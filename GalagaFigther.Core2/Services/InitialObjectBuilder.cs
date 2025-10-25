using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Projectiles;
using GalagaFighter.Core2.Effects.Turrets;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
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
        private IInputService _inputService;
        private IGameObjectPositionService _gameObjectPositionService;

        public InitialObjectBuilder(IObjectService objectService, IPersistentValueHandler persistentValueHandler, 
            IGameDataRegistry gameDataRegistry, IInputService inputService, IGameObjectPositionService gameObjectPositionService)
        {
            _objectService = objectService;
            _persistentValueHandler = persistentValueHandler;
            _gameDataRegistry = gameDataRegistry;
            _inputService = inputService;
            _gameObjectPositionService = gameObjectPositionService;
        }

        public void Build()
        {
            var screenWidth = Raylib.GetScreenWidth();
            var screenHeight = Raylib.GetScreenHeight();
            CreatePlayers(screenWidth, screenHeight);
        }

        private void CreatePlayers(int screenWidth, int screenHeight)
        {
           var player1 = CreatePlayer(0, 90, new(0,0), new(550f, screenHeight), ShipPalettes.AzureWing);
            var player2 = CreatePlayer(screenWidth - 168, -90, new(screenWidth-550f, 0), new(screenWidth,screenHeight), ShipPalettes.VoidHunter);

            _objectService.Add(player1);
            _objectService.Add(player2);

            RegisterInputMappings(player1, player2);
        }

        private void RegisterInputMappings(Player player1, Player player2)
        {
            var player1Mappings = new KeyMappings(
                KeyboardKey.W,    // Forward
                KeyboardKey.S,    // Back
                KeyboardKey.A,    // Left
                KeyboardKey.D,    // Right
                KeyboardKey.K,    // Shoot
                KeyboardKey.J,    // Defend
                KeyboardKey.U     // Deploy Turret
            );

            var player2Mappings = new KeyMappings(
                KeyboardKey.W,    // Forward (TODO: Update with different keys)
                KeyboardKey.S,    // Back (TODO: Update with different keys)
                KeyboardKey.A,    // Left (TODO: Update with different keys)
                KeyboardKey.D,    // Right (TODO: Update with different keys)
                KeyboardKey.K,    // Shoot (TODO: Update with different keys)
                KeyboardKey.J,    // Defend (TODO: Update with different keys)
                KeyboardKey.U     // Deploy Turret (TODO: Update with different keys)
            );

            _inputService.AddPlayer(player1.Id, player1Mappings);
            _inputService.AddPlayer(player2.Id, player2Mappings);
        }

        private Player CreatePlayer(int x, float rotation, Vector2 min, Vector2 max, Color palette)
        {
            var playerHeight = 168f;
            var pos = new Vector2(x, (max.Y - playerHeight) / 2);
            var size = Vector2.One * 168;
            var speed = Vector2.Zero;

            // Create sprite without palette swap initially
            var shipSprite = new StillImageSprite("Sprites/Ships/MainShipBody.png");

            var player = new Player(pos, size, speed, shipSprite)
            {
                Rotation = rotation,
                Palette = palette // Set specific palette color
            };

            var bounds = _gameDataRegistry.Get<PlayerBoundsData>(player);
            bounds.Min = min;
            bounds.Max = max;

            var effects = _gameDataRegistry.Get<PlayerEffects>(player);
            effects.Add(new DefaultShootEffect());
            effects.Add(new DefaultTurretEffect());

            // Create engine trail emitter
            var engineConfig = ParticleEffectTemplates.Get("EngineTrail");
            var engineTrail = new ParticleEmitter(
                owner: player.Id,
                position: new Vector2(80, 140),
                size: Vector2.One,
                offset: Vector2.Zero,
                sprite: new StillImageSprite("Sprites/Particles/default.png")
            );
            
            // Store config in GameDataRegistry following the established pattern
            _gameDataRegistry.Set(engineTrail, engineConfig);
            
            _objectService.Add(engineTrail);
            _gameObjectPositionService.RegisterParent(player, engineTrail);

            return player;
        }
    }
}
