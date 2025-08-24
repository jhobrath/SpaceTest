using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Static;
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

        public static Guid _gameId = Guid.NewGuid();
        public static Guid Id => _gameId;

        public static Random Random => _random;

        private static float _uniformScale;
        private static float _height;
        private static float _width;

        private Player _player1;
        private Player _player2;
        private static Random _random = new Random();

        // Services resolved from Registry
        private readonly IPlayerProjectileCollisionService _playerProjectileCollisionService;
        private readonly IProjectilePowerUpCollisionService _projectilePowerUpCollisionService;
        private readonly IPlayerPowerUpCollisionService _playerPowerUpCollisionService;
        private readonly IPowerUpService _powerUpService;
        private readonly IObjectService _objectService;
        private readonly IInputService _inputService;

        // Player-specific controllers
        private readonly IPlayerController _playerController1;
        private readonly IPlayerController _playerController2;

        public Game()
        {
            Registry.Configure();
            
            _objectService = Registry.Get<IObjectService>();
            _inputService = Registry.Get<IInputService>();
            _powerUpService = Registry.Get<IPowerUpService>();
            _playerProjectileCollisionService = Registry.Get<IPlayerProjectileCollisionService>();
            _projectilePowerUpCollisionService = Registry.Get<IProjectilePowerUpCollisionService>();
            _playerPowerUpCollisionService = Registry.Get<IPlayerPowerUpCollisionService>();
            
            // Create separate controller instances for each player
            _playerController1 = Registry.Get<IPlayerController>();
            _playerController2 = Registry.Get<IPlayerController>();
            
            UiService.Initialize();
            InitializeWindow();
            InitializeScale();
            InitializePlayers();
            AudioService.Initialize();
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
            int shipWidth = (int)(160 * _uniformScale);
            int shipHeight = (int)(160 * _uniformScale);
            int playerMargin = (int)(0 * _uniformScale);

            var shipSize = new Vector2(shipWidth, shipHeight);
            var position1 = new Vector2(playerMargin, _height / 2 - shipHeight / 2);
            var position2 = new Vector2(_width - playerMargin - shipWidth, _height / 2 - shipHeight / 2);

            var rect1 = new Rectangle(position1, shipSize);
            var rect2 = new Rectangle(position2, shipSize);

            var player1Mappings = new KeyMappings(KeyboardKey.W, KeyboardKey.S, KeyboardKey.D, KeyboardKey.A);
            var player2Mappings = new KeyMappings(KeyboardKey.Down, KeyboardKey.Up, KeyboardKey.Left, KeyboardKey.Right);

            // 1. Construct players with their own controllers
            _player1 = new Player(_playerController1, rect1, true);
            _player2 = new Player(_playerController2, rect2, false);

            _objectService.AddGameObject(_player1);
            _objectService.AddGameObject(_player2);

            _inputService.AddPlayer(_player1.Id, player1Mappings);
            _inputService.AddPlayer(_player2.Id, player2Mappings);

            _player1.SetDrawPriority(0);
            _player2.SetDrawPriority(0);

#if DEBUG
            _player1.Effects.Add(new IceShotEffect());
            _player1.Effects.Add(new ExplosiveShotEffect());
            _player1.Effects.Add(new WoodShotEffect());
            _player1.Effects.Add(new NinjaShotEffect());
            _player1.Effects.Add(new MagnetEffect());
            _player1.Effects.Add(new MudShotEffect());
            _player2.Effects.Add(new IceShotEffect());
            _player2.Effects.Add(new ExplosiveShotEffect());
            _player2.Effects.Add(new WoodShotEffect());
            _player2.Effects.Add(new NinjaShotEffect());
            _player2.Effects.Add(new MagnetEffect());
            _player2.Effects.Add(new MudShotEffect());
#endif
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
            {
#if DEBUG
         //       Raylib.DrawRectangleLinesEx(gameObject.Rect, 2 * _uniformScale, Color.Red);
#endif
                gameObject.Draw();
            }
        }


        private void Cleanup()
        {
            AudioService.Cleanup();
            Raylib.CloseWindow();
        }
    }
}
