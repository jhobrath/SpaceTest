using GalagaFighter.Core.Behaviors;
using GalagaFighter.Core.Models;
using GalagaFighter;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using System.Net.Http.Headers;

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

        private readonly IPlayerProjectileCollisionService _playerProjectileCollisionService;
        private readonly IProjectilePowerUpCollisionService _projectilePowerUpCollisionService;
        private readonly IPlayerPowerUpCollisionService _playerPowerUpCollisionService;
        private readonly IPowerUpService _powerUpService;
        private readonly IObjectService _objectService;
        private readonly IInputService _inputService;

        public Game()
        {
            _objectService = new ObjectService();
            _inputService = new InputService(); 
            _playerProjectileCollisionService = new PlayerProjectileCollisionService(_objectService);
            _projectilePowerUpCollisionService = new ProjectilePowerUpCollisionService(_objectService);
            _playerPowerUpCollisionService = new PlayerPowerUpCollisionService(_objectService, _inputService);
            _powerUpService = new PowerUpService(_objectService);

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

            var sprite1 = new SpriteWrapper(TextureService.Get("Sprites/Players/Player1.png"));
            var sprite2 = new SpriteWrapper(TextureService.Get("Sprites/Players/Player1.png"));

            var position1 = new Vector2(playerMargin, _height / 2 - shipHeight / 2);
            var position2 = new Vector2(_width - playerMargin - shipWidth, _height / 2 - shipHeight / 2);

            var display1 = new PlayerDisplay(sprite1, new Rectangle(position1.X, position1.Y, shipSize.X, shipSize.Y), 90f);
            var display2 = new PlayerDisplay(sprite2, new Rectangle(position2.X, position2.Y, shipSize.X, shipSize.Y), -90f);

            var player1Mappings = new KeyMappings(KeyboardKey.W, KeyboardKey.S, KeyboardKey.D);
            var player2Mappings = new KeyMappings(KeyboardKey.Down, KeyboardKey.Up, KeyboardKey.Left);

            _player1 = new Player(Id, display1, true);
            _player2 = new Player(Id, display2, false);

            _player1.SetMovementBehavior(new PlayerMovementBehavior());
            _player1.SetCollisionBehavior(new PlayerCollisionBehavior(_objectService));
            _player1.SetInputBehavior(new PlayerInputBehavior(_inputService));
            _player1.SetShootingBehavior(new PlayerShootingBehavior(_objectService));
            _objectService.AddGameObject(_player1);

            _player2.SetMovementBehavior(new PlayerMovementBehavior());
            _player2.SetCollisionBehavior(new PlayerCollisionBehavior(_objectService));
            _player2.SetInputBehavior(new PlayerInputBehavior(_inputService));
            _player2.SetShootingBehavior(new PlayerShootingBehavior(_objectService));
            _objectService.AddGameObject(_player2);

            _inputService.AddPlayer(_player1.Id, player1Mappings);
            _inputService.AddPlayer(_player2.Id, player2Mappings);

            _player1.SetDrawPriority(0);
            _player1.SetDrawPriority(0);
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
