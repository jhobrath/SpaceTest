using GalagaFighter.Core.Behaviors;
using GalagaFighter.Core.Models;
using GalagaFighter;
using GalagaFighter.Core;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core
{
    public class Game
    {
        public static float UniformScale => _uniformScale;
        public static float Margin => 35 * _uniformScale;
        public static float Height => _height;
        public static float Width => _width;

        public static Random Random => _random;

        private static float _uniformScale;
        private static float _height;
        private static float _width;

        private Player _player1;
        private Player _player2;
        private List<GameObject> _gameObjects;
        private static Random _random = new Random();
        private readonly ICollisionService _collisionService;

        public Game()
        {
            _collisionService = new CollisionService();

            InitializeWindow();
            InitializeScale();
            InitializePlayers();
            InitializeGameObjects();
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

            var inputBehavior1 = new PlayerInputBehavior(new KeyMappings(KeyboardKey.W, KeyboardKey.S, KeyboardKey.D));
            var inputBehavior2 = new PlayerInputBehavior(new KeyMappings(KeyboardKey.Down, KeyboardKey.Up, KeyboardKey.Left));

            var shipSize = new Vector2(shipWidth, shipHeight);

            var sprite1 = new SpriteWrapper(TextureLibrary.Get("Sprites/Players/Player1.png"));
            var sprite2 = new SpriteWrapper(TextureLibrary.Get("Sprites/Players/Player1.png"));

            var position1 = new Vector2(playerMargin, _height / 2 - shipHeight / 2);
            var position2 = new Vector2(_width - playerMargin - shipWidth, _height / 2 - shipHeight / 2);

            var display1 = new PlayerDisplay(sprite1, new Rectangle(position1.X, position1.Y, shipSize.X, shipSize.Y), 90f);
            var display2 = new PlayerDisplay(sprite2, new Rectangle(position2.X, position2.Y, shipSize.X, shipSize.Y), -90f);

            _player1 = new Player(inputBehavior1, display1, true);
            _player2 = new Player(inputBehavior2, display2, false);
        }

        private void InitializeGameObjects()
        {
            _gameObjects = new List<GameObject>();
            _gameObjects.Add(_player1);
            _gameObjects.Add(_player2);
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
            HandleInput();
            UpdateGameObjects();
            //SpawnPowerUps();
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
                InitializePlayers();
                InitializeGameObjects();
            }
        }

        private void UpdateGameObjects()
        {
            var player1Collisions = _collisionService.GetProjectileCollisions(_player1, _player2.Projectiles);
            var player2Collisions = _collisionService.GetProjectileCollisions(_player2, _player1.Projectiles);

            _player1.SetCollisions(player1Collisions);
            _player2.SetCollisions(player2Collisions);

            for (int i = _gameObjects.Count - 1; i >= 0; i--)
            {
                if (!_gameObjects[i].IsActive)
                    _gameObjects.RemoveAt(i);
                else
                    _gameObjects[i].Update(this);
            }
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            DrawGameObjects();
            DrawUI();

            Raylib.EndDrawing();
        }


        private void DrawUI()
        {
            int healthTextSize = (int)(24 * _uniformScale);
            int controlTextSize = (int)(20 * _uniformScale);
            int winnerTextSize = (int)(50 * _uniformScale);
            int statusTextSize = (int)(16 * _uniformScale);
            int margin = (int)(15 * _uniformScale);

            var player1Health = _player1.Health;
            var player2Health = _player2.Health;    

            // Health and bullet capacity display
            Raylib.DrawText($"P1 Health: {player1Health}", margin, margin, healthTextSize, Color.White);
            Raylib.DrawText($"P2 Health: {player2Health}", (int)_width - (int)(250 * _uniformScale), margin, healthTextSize, Color.White);

            // Bullet capacity display
            int bulletStatusY = margin + (int)(30 * _uniformScale);

            // Winner display
            if (_player1.Health <= 0 || _player2.Health <= 0)
            {
                string winner = _player1.Health > 0 ? "Player 1 Wins!" : "Player 2 Wins!";
                Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), winner, winnerTextSize, 1);
                Raylib.DrawText(winner, (int)(_width / 2 - textSize.X / 2), (int)(_width / 2 - textSize.Y / 2), winnerTextSize, Color.Gold);
            }
        }

        public void AddGameObject(GameObject gameObject)
        {
            _gameObjects.Add(gameObject);
        }

        private void DrawGameObjects()
        {
            foreach (var obj in _gameObjects)
            {
                obj.Draw();
            }
        }


        private void Cleanup()
        {
            //AudioService.Cleanup();
            Raylib.CloseWindow();
        }

    }
}
