using Raylib_cs;
using GalagaFighter.Models;
using GalagaFighter.Models.Players;
using GalagaFighter.Services;
using System.Numerics;
using System.Collections.Generic;
using System;
using GalagaFighter.Models.PowerUps;

namespace GalagaFighter
{
    public class Game
    {
        private List<GameObject> gameObjects;
        private Player player1;
        private Player player2;
        private Random random;
        private IAudioService audioService;

        public Game()
        {
            InitializeWindow();
            InitializeAudio();
            InitializePlayers();
            InitializeGameObjects();
        }

        private void InitializeWindow()
        {
            int monitorWidth = Raylib.GetMonitorWidth(0);
            int monitorHeight = Raylib.GetMonitorHeight(0);
            
            Raylib.InitWindow(monitorWidth, monitorHeight, "Galaga Fighter");
            //Raylib.ToggleFullscreen();
            Raylib.SetTargetFPS(60);
        }

        private void InitializeAudio()
        {
            audioService = new AudioService();
            audioService.Initialize();
        }

        private void InitializePlayers()
        {
            // Calculate positions and sizes based on actual screen size
            int actualWidth = Raylib.GetScreenWidth();
            int actualHeight = Raylib.GetScreenHeight();

            // Calculate scaling factors based on reference resolution (1920x1080)
            float scaleX = actualWidth / 1920f;
            float scaleY = actualHeight / 1080f;
            float uniformScale = Math.Min(scaleX, scaleY);

            // Scale ship sizes and positions
            int shipWidth = (int)(120 * uniformScale);
            int shipHeight = (int)(120 * uniformScale);
            int playerMargin = (int)(60 * uniformScale);

            player1 = new Player(
                new Rectangle(playerMargin, actualHeight / 2 - shipHeight / 2, shipWidth, shipHeight), 
                1.2f, KeyboardKey.W, KeyboardKey.S, KeyboardKey.D, true, uniformScale);
            
            player2 = new Player(
                new Rectangle(actualWidth - playerMargin - shipWidth, actualHeight / 2 - shipHeight / 2, shipWidth, shipHeight), 
                1.2f, KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, false, uniformScale);
        }

        private void InitializeGameObjects()
        {
            gameObjects = new List<GameObject>();
            random = new Random();
            
            gameObjects.Add(player1);
            gameObjects.Add(player2);
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
            SpawnPowerUps();
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

            if(Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                InitializePlayers();
                InitializeGameObjects();
            }
        }

        private void UpdateGameObjects()
        {
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {
                if (!gameObjects[i].IsActive)
                {
                    gameObjects.RemoveAt(i);
                }
                else
                {
                    gameObjects[i].Update(this);
                }
            }
        }

        private void SpawnPowerUps()
        {
            if (random.Next(0, 20 * 5) == 1)
            {
                int powerUpTypeIndex = random.Next(0, 3);
                PowerUpType type = (PowerUpType)powerUpTypeIndex;
                int screenWidth = Raylib.GetScreenWidth();
                int screenHeight = Raylib.GetScreenHeight();
                float uniformScale = Math.Min(screenWidth / 1920f, screenHeight / 1080f);
                int powerUpSize = (int)(30 * uniformScale);
                Rectangle rect = new Rectangle(
                    random.Next(100, screenWidth - 100),
                    -powerUpSize,
                    powerUpSize,
                    powerUpSize);
                PowerUp powerUp = type switch
                {
                    PowerUpType.FireRate => new GalagaFighter.Models.PowerUps.FireRatePowerUp(rect, 2f * uniformScale),
                    PowerUpType.IceShot => new GalagaFighter.Models.PowerUps.IceShotPowerUp(rect, 2f * uniformScale),
                    PowerUpType.Wall => new GalagaFighter.Models.PowerUps.WallPowerUp(rect, 2f * uniformScale),
                    _ => null
                };
                if (powerUp != null)
                    gameObjects.Add(powerUp);
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

        private void DrawGameObjects()
        {
            foreach (var obj in gameObjects)
            {
                obj.Draw();
            }
        }

        private void DrawUI()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            
            float uniformScale = Math.Min(screenWidth / 1920f, screenHeight / 1080f);
            int healthTextSize = (int)(24 * uniformScale);
            int controlTextSize = (int)(20 * uniformScale);
            int winnerTextSize = (int)(50 * uniformScale);
            int statusTextSize = (int)(16 * uniformScale);
            int margin = (int)(15 * uniformScale);
            
            // Health and bullet capacity display
            Raylib.DrawText($"P1 Health: {player1.Health}", margin, margin, healthTextSize, Color.White);
            Raylib.DrawText($"P2 Health: {player2.Health}", screenWidth - (int)(250 * uniformScale), margin, healthTextSize, Color.White);

            // Bullet capacity display
            int bulletStatusY = margin + (int)(30 * uniformScale);
            int p1ActiveBullets = player1.GetActiveBulletCount(gameObjects);
            int p2ActiveBullets = player2.GetActiveBulletCount(gameObjects);
            
            string p1BulletStatus = $"P1 Bullets: {p1ActiveBullets}/{player1.MaxBullets}";
            string p2BulletStatus = $"P2 Bullets: {p2ActiveBullets}/{player2.MaxBullets}";
            
            Color p1BulletColor = p1ActiveBullets >= player1.MaxBullets ? Color.Red : Color.Yellow;
            Color p2BulletColor = p2ActiveBullets >= player2.MaxBullets ? Color.Red : Color.Yellow;
            
            Raylib.DrawText(p1BulletStatus, margin, bulletStatusY, statusTextSize, p1BulletColor);
            Vector2 p2BulletTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), p2BulletStatus, statusTextSize, 1);
            Raylib.DrawText(p2BulletStatus, screenWidth - (int)(250 * uniformScale), bulletStatusY, statusTextSize, p2BulletColor);

            // Ice effect status display
            int iceStatusY = bulletStatusY + (int)(25 * uniformScale);
            // Remove display of IceEffectCount and CurrentSlowIntensity
            // If you want to show frozen status, query effects directly from player.Stats

            // Controls
            Raylib.DrawText("F11 - Toggle Fullscreen | ESC - Exit", margin, screenHeight - (int)(40 * uniformScale), controlTextSize, Color.LightGray);

            // Winner display
            if (player1.Health <= 0 || player2.Health <= 0)
            {
                string winner = player1.Health > 0 ? "Player 1 Wins!" : "Player 2 Wins!";
                Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), winner, winnerTextSize, 1);
                Raylib.DrawText(winner, (int)(screenWidth / 2 - textSize.X / 2), (int)(screenHeight / 2 - textSize.Y / 2), winnerTextSize, Color.Gold);
            }
        }

        private void Cleanup()
        {
            audioService.Cleanup();
            Raylib.CloseWindow();
        }

        // Public interface for game objects
        public void AddGameObject(GameObject obj)
        {
            gameObjects.Add(obj);
        }

        public Player GetOpponent(Player player)
        {
            return player == player1 ? player2 : player1;
        }

        public List<GameObject> GetGameObjects()
        {
            return gameObjects;
        }

        // Audio interface - delegates to audio service
        public void PlayShootSound() => audioService.PlayShootSound();
        public void PlayHitSound() => audioService.PlayHitSound();
        public void PlayPowerUpSound() => audioService.PlayPowerUpSound();
        public void PlayIceHitSound() => audioService.PlayIceHitSound();
        public void PlayWallStickSound() => audioService.PlayWallStickSound();
    }
}
