using GalagaFighter.Core.Behaviors;
using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core
{
    public class Game
    {
        public static float UniformScale => _uniformScale;
        public static float Margin => 35 * _uniformScale;
        public static float Height => _height;
        public static float Width => _width;

        public static Guid Id => Guid.NewGuid();

        public static Random Random => _random;

        private static float _uniformScale;
        private static float _height;
        private static float _width;

        private Player _player1;
        private Player _player2;
        private static Random _random = new Random();

        //private PlayerController _player1Controller;
        //private PlayerController _player2Controller;

        private readonly IPlayerProjectileCollisionService _playerProjectileCollisionService;
        private readonly IProjectilePowerUpCollisionService _projectilePowerUpCollisionService;
        private readonly IPlayerPowerUpCollisionService _playerPowerUpCollisionService;
        private readonly IPowerUpService _powerUpService;
        private readonly IObjectService _objectService;
        private readonly IInputService _inputService;
        private readonly IEventService _eventService;
        private readonly IPlayerEventService _playerEventService;
        private readonly IPlayerEffectManager _playerEffectManager;
        private readonly IPlayerUpdater _playerUpdater;
        private readonly IPlayerMover _playerMover;
        private readonly IPlayerShooter _playerShooter;
        private readonly IProjectileUpdater _projectileUpdater;

        public Game()
        {
            _objectService = new ObjectService();
            _inputService = new InputService(); 
            _playerEffectManager = new PlayerEffectManager();
            _playerProjectileCollisionService = new PlayerProjectileCollisionService(_objectService, _playerEffectManager);
            _projectilePowerUpCollisionService = new ProjectilePowerUpCollisionService(_objectService);
            _eventService = new EventService();
            _playerPowerUpCollisionService = new PlayerPowerUpCollisionService(_eventService, _objectService, _inputService);
            _powerUpService = new PowerUpService(_objectService);
            _playerEventService = new PlayerEventService(_eventService, _objectService, _inputService, _playerEffectManager);
            _playerMover = new PlayerMover(_inputService);
            _projectileUpdater = new ProjectileUpdater();
            _playerShooter = new PlayerShooter(_inputService, _objectService, _projectileUpdater);
            _playerUpdater = new PlayerUpdater(_playerMover, _playerShooter);
            _playerEventService.Initialize();
            UiService.Initialize(_playerEffectManager);
            InitializeWindow();
            InitializeScale();
            InitializePlayers();
        }

        private static void InitializeWindow()
        {
            int monitorWidth = Raylib.GetMonitorWidth(0);
            int monitorHeight = Raylib.GetMonitorHeight(0);

            Raylib.InitWindow(monitorWidth, monitorHeight, "Galaga Fighter");
            Raylib.SetTargetFPS(60);
        }

        private static void InitializeScale()
        {
            int actualWidth = Raylib.GetScreenWidth();
            int actualHeight = Raylib.GetScreenHeight();

            float scaleX = actualWidth / 1920f;
            float scaleY = actualHeight / 1080f;
            _uniformScale = Math.Min(scaleX, scaleY);
            _height = actualHeight;
            _width = actualWidth;
        }

        private void InitializePlayers()
        {
            int shipWidth = (int)(120 * _uniformScale);
            int shipHeight = (int)(120 * _uniformScale);
            int playerMargin = (int)(60 * _uniformScale);

            var shipSize = new Vector2(shipWidth, shipHeight);
            var position1 = new Vector2(playerMargin, _height / 2 - shipHeight / 2);
            var position2 = new Vector2(_width - playerMargin - shipWidth, _height / 2 - shipHeight / 2);

            var rect1 = new Rectangle(position1, shipSize);
            var rect2 = new Rectangle(position2, shipSize);

            var player1Mappings = new KeyMappings(KeyboardKey.W, KeyboardKey.S, KeyboardKey.D, KeyboardKey.A);
            var player2Mappings = new KeyMappings(KeyboardKey.Down, KeyboardKey.Up, KeyboardKey.Left, KeyboardKey.Right);

            // 1. Construct players
            _player1 = new Player(_playerUpdater, rect1, true);
            _player2 = new Player(_playerUpdater, rect2, false);

            // 2. Construct controllers
            //_player1Controller = new PlayerController(_playerEffectManager, _inputService, _eventService);
            //_player2Controller = new PlayerController(_playerEffectManager, _inputService, _eventService);

            // 3. Construct default effects and set collision behaviors using lambdas
            /*var defaultShootEffect1 = new DefaultShootEffect();
            defaultShootEffect1.SetMovementBehavior(new PlayerMovementBehavior());
            defaultShootEffect1.SetCollisionBehavior(new PlayerCollisionBehavior(
                _eventService, _objectService, (player, damage) => _player1Controller.TakeDamage(player, damage)));
            defaultShootEffect1.SetInputBehavior(new PlayerInputBehavior(_inputService));
            defaultShootEffect1.SetShootingBehavior(new PlayerShootingBehavior(_objectService));

            var defaultShootEffect2 = new DefaultShootEffect();
            defaultShootEffect2.SetMovementBehavior(new PlayerMovementBehavior());
            defaultShootEffect2.SetCollisionBehavior(new PlayerCollisionBehavior(
                _eventService, _objectService, (player, damage) => _player2Controller.TakeDamage(player, damage)));
            defaultShootEffect2.SetInputBehavior(new PlayerInputBehavior(_inputService));
            defaultShootEffect2.SetShootingBehavior(new PlayerShootingBehavior(_objectService));*/

            // 4. Add effects to players
            //_playerEffectManager.AddEffect(_player1, defaultShootEffect1);
            //_playerEffectManager.AddEffect(_player2, defaultShootEffect2);

            _objectService.AddGameObject(_player1);
            _objectService.AddGameObject(_player2);

            _inputService.AddPlayer(_player1.Id, player1Mappings);
            _inputService.AddPlayer(_player2.Id, player2Mappings);

            _player1.SetDrawPriority(0);
            _player2.SetDrawPriority(0);
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                Update();
                Draw();
            }

            Cleanup();
        }

        private void Update()
        {
            _playerEffectManager.UpdateEffects(Raylib.GetFrameTime());
            _playerProjectileCollisionService.HandleCollisions();
            _projectilePowerUpCollisionService.HandleCollisions();
            _playerPowerUpCollisionService.HandleCollisions();
            _inputService.Update();

            HandleInput();
            UpdateGameObjects();

            _powerUpService.Roll();
        }

        private void HandleInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.F11))
            {
                Raylib.ToggleFullscreen();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                Raylib.ClearWindowState(ConfigFlags.FullscreenMode);
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                _objectService.Reset();
                InitializePlayers();
            }
        }

        private void UpdateGameObjects()
        {
            var gameObjects = _objectService.GetGameObjects();
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {
                if (!gameObjects[i].IsActive)
                    _objectService.RemoveGameObject(gameObjects[i]);
                else
                    gameObjects[i].Update(this);
            }
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            DrawGameObjects();
            UiService.DrawUi(_player1, _player2);
            Raylib.EndDrawing();
        }

        private void DrawGameObjects()
        {
            var gameObjects = _objectService.GetGameObjects();
            foreach (var gameObject in gameObjects.OrderBy(x => x.DrawPriority))
                gameObject.Draw();
        }


        private void Cleanup()
        {
            //AudioService.Cleanup();
            Raylib.CloseWindow();
        }
    }

    public static class RaylibExtensions
    {
        public static Color ApplyBlue(this Color color, float blueAlpha)
        {
             var newColor = new Color(
                (byte)(color.R * (1 - blueAlpha)),
                (byte)(color.G * (1 - blueAlpha)),
                (byte)(color.B + (255 - color.B) * blueAlpha),
                color.A);

            return newColor;
        }

        public static Color ApplyGreen(this Color color, float greenAlpha)
        {
            var newColor = new Color(
               (byte)(color.R * (1 - greenAlpha)),
               (byte)(color.G + (255 - color.G) * greenAlpha),
               (byte)(color.B * (1 - greenAlpha)),
               color.A);
            return newColor;

        }

        public static Color ApplyRed(this Color color, float redAlpha)
        {
            var newColor = new Color(
               (byte)(color.R + (255 - color.R) * redAlpha),
               (byte)(color.G * (1 - redAlpha)),
               (byte)(color.B * (1 - redAlpha)),
               color.A);
            return newColor;
        }
    }
}
