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
using GalagaFighter.Core.Handlers.Players;

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
        private readonly IPlayerEffectManagerFactory _effectManagerFactory;

        // Player-specific controllers
        private readonly IPlayerController _playerController1;
        private readonly IPlayerController _playerController2;

        private string[] _args = [];

        public Game()
        {
            Registry.Configure();
            
            _objectService = Registry.Get<IObjectService>();
            _inputService = Registry.Get<IInputService>();
            _powerUpService = Registry.Get<IPowerUpService>();
            _playerProjectileCollisionService = Registry.Get<IPlayerProjectileCollisionService>();
            _projectilePowerUpCollisionService = Registry.Get<IProjectilePowerUpCollisionService>();
            _playerPowerUpCollisionService = Registry.Get<IPlayerPowerUpCollisionService>();
            _effectManagerFactory = Registry.Get<IPlayerEffectManagerFactory>();

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
            GiveAllEffects(_player1);
            GiveAllEffects(_player2);
#endif
        }

        private void GiveAllEffects(Player player)
        {
            var effectManager = _effectManagerFactory.GetEffectManager(player);
            //effectManager.AddEffect(new RicochetEffect());
            //effectManager.AddEffect(new IceShotEffect());
            //effectManager.AddEffect(new ExplosiveShotEffect());
            //effectManager.AddEffect(new WoodShotEffect());
            //effectManager.AddEffect(new NinjaShotEffect());
            //effectManager.AddEffect(new MagnetEffect());
            //effectManager.AddEffect(new MudShotEffect());
        }

        public void Run(string[] args)
        {
            InitializePlayersFromArgs(args);

            while (!Raylib.WindowShouldClose())
            {
                Update();
                Draw();
            }

            Cleanup();
        }

        private void InitializePlayersFromArgs(string[] args)
        {
            _args = args;

            string player1Args = null;
            string player2Args = null;

            // Parse command line arguments looking for --player1 and --player2
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--player1" && i + 1 < args.Length)
                {
                    player1Args = args[i + 1];
                }
                else if (args[i] == "--player2" && i + 1 < args.Length)
                {
                    player2Args = args[i + 1];
                }
            }

            // Parse and apply player configurations
            if (!string.IsNullOrEmpty(player1Args))
            {
                InitializePlayer(_player1, player1Args);
            }
            else
            {
                var effectManager = _effectManagerFactory.GetEffectManager(_player1);
                effectManager.AddEffect(new DefaultShootEffect());
            }

            if (!string.IsNullOrEmpty(player2Args))
            {
                InitializePlayer(_player2, player2Args);
            }
            else
            {
                var effectManager = _effectManagerFactory.GetEffectManager(_player2);
                effectManager.AddEffect(new DefaultShootEffect());

            }
        }

        private void InitializePlayer(Player player, string configString)
        {
            var parts = configString.Split(',');
            if (parts.Length == 7)
            {
                var health = float.Parse(parts[2]);
                var stats = new PlayerStats
                {
                    SpeedMultiplier = float.Parse(parts[3]),
                    FireRateMultiplier = 1/float.Parse(parts[4]),
                    Damage = float.Parse(parts[5]),
                    Shield = float.Parse(parts[6])
                };
                var color = parts[0] switch
                {
                    "SkyBlue" => Color.SkyBlue,
                    "Red" => Color.Red,
                    "Lime" => Color.Lime,
                    "Purple" => Color.Purple,
                    "Orange" => Color.Orange,
                    "Cyan" => new Color(0, 255, 255, 255),
                    "Yellow" => Color.Yellow,
                    "White" => Color.White,
                    "Pink" => Color.Pink,
                    "DarkGray" => Color.DarkGray,
                    _ => Color.White
                };
                player.Initialize(health, stats, color);

                var effectManager = _effectManagerFactory.GetEffectManager(player);
                effectManager.AddEffect(new DefaultShootEffect(color));

                var effect = parts[1];
                if (effect == "SurpriseShot")
                    effectManager.AddEffect(new SurpriseShotEffect());
                else if (effect == "Splitter")
                    effectManager.AddEffect(new SplitterEffect());
                else if (effect == "Ricochet")
                    effectManager.AddEffect(new RicochetEffect());
                else if(effect == "TimedBarrage")
                    effectManager.AddEffect(new TimedBarrageEffect());
            }
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
                if (_args.Length > 0)
                    InitializePlayersFromArgs(_args);
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
