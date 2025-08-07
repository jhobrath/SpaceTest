using Raylib_cs;
using GalagaFighter.Models;
using System.Numerics;
using System.Collections.Generic;
using System;
using System.IO;

namespace GalagaFighter
{
    public class Game
    {
        public const int ScreenWidth = 800;
        public const int ScreenHeight = 450;
        
        private List<GameObject> gameObjects;
        private Player player1;
        private Player player2;
        private Random random;

        // Sound effects
        private Sound shootSound;
        private Sound hitSound;
        private Sound powerUpSound;
        private Sound iceHitSound;
        private Sound wallStickSound;

        public Game()
        {
            // Get monitor dimensions for fullscreen
            int monitorWidth = Raylib.GetMonitorWidth(0);
            int monitorHeight = Raylib.GetMonitorHeight(0);
            
            Raylib.InitWindow(monitorWidth, monitorHeight, "Galaga Fighter");
            Raylib.ToggleFullscreen(); // Enable fullscreen mode
            Raylib.SetTargetFPS(60);
            
            // Initialize audio
            Raylib.InitAudioDevice();

            // Create simple placeholder sounds - the simplest approach
            // We'll just create empty sounds for now to avoid crashes
            // In a real game, you'd load actual sound files
            CreatePlaceholderSounds();

            gameObjects = new List<GameObject>();
            random = new Random();

            // Calculate positions and sizes based on actual screen size
            int actualWidth = Raylib.GetScreenWidth();
            int actualHeight = Raylib.GetScreenHeight();

            // Calculate scaling factors based on a reference resolution (1920x1080)
            float scaleX = actualWidth / 1920f;
            float scaleY = actualHeight / 1080f;
            float uniformScale = Math.Min(scaleX, scaleY); // Use smaller scale to maintain aspect ratio

            // Scale ship sizes based on screen resolution
            int shipWidth = (int)(60 * uniformScale);  // Increased from 40, scaled
            int shipHeight = (int)(120 * uniformScale); // Increased from 80, scaled
            
            // Scale player positions
            int playerMargin = (int)(60 * uniformScale); // Margin from screen edges

            player1 = new Player(
                new Rectangle(playerMargin, actualHeight / 2 - shipHeight / 2, shipWidth, shipHeight), 
                0.35f, KeyboardKey.W, KeyboardKey.S, KeyboardKey.D, true, uniformScale);
            
            player2 = new Player(
                new Rectangle(actualWidth - playerMargin - shipWidth, actualHeight / 2 - shipHeight / 2, shipWidth, shipHeight), 
                0.35f, KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, false, uniformScale);

            gameObjects.Add(player1);
            gameObjects.Add(player2);
        }

        private void CreatePlaceholderSounds()
        {
            // Load actual sound files instead of generating sounds
            try 
            {
                // Use the new sound files for specific actions
                shootSound = LoadSoundFile("projectile-default.wav");        // For all projectile shooting
                iceHitSound = LoadSoundFile("player_freezing.wav");          // For player getting frozen
                wallStickSound = LoadSoundFile("plank_landing.wav");         // For brown projectile wall stick
                
                // Keep generated sounds for other effects (or replace with your own files)
                hitSound = CreateBeepSound(200, 0.08f, 0.5f);               // Low pitched hit
                powerUpSound = CreateBeepSound(600, 0.2f, 0.4f);            // Medium pitched power-up
                
                // Set volume levels for sounds
                SetSoundVolumes();
                
                Console.WriteLine("Audio system initialized with mixed sound sources");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not initialize audio system: {ex.Message}");
                // Create fallbacks if sound loading fails
                CreateSoundFallbacks();
            }
        }

        private void SetSoundVolumes()
        {
            // Set specific volume levels for each sound
            Raylib.SetSoundVolume(shootSound, .01f);      // 50% volume for projectile sounds
            Raylib.SetSoundVolume(iceHitSound, 1.0f);     // 100% volume for ice hit
            Raylib.SetSoundVolume(wallStickSound, 1.0f);  // 100% volume for wall stick
            Raylib.SetSoundVolume(hitSound, 1.0f);        // 100% volume for hit
            Raylib.SetSoundVolume(powerUpSound, 1.0f);    // 100% volume for power-up
        }

        private Sound LoadSoundFile(string filename)
        {
            // Try to load from multiple possible locations
            string[] possiblePaths = {
                filename,                           // Current directory
                $"sounds/{filename}",              // sounds subdirectory
                $"assets/{filename}",              // assets subdirectory
                $"audio/{filename}",               // audio subdirectory
                $"../../../{filename}",            // Project root (common in debug builds)
                $"../../../sounds/{filename}",     // Project root/sounds
                $"../../../assets/{filename}",     // Project root/assets
                $"../../../audio/{filename}"       // Project root/audio
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    Console.WriteLine($"Loading sound: {path}");
                    return Raylib.LoadSound(path);
                }
            }

            Console.WriteLine($"Warning: Could not find sound file '{filename}' in any expected location");
            throw new FileNotFoundException($"Sound file not found: {filename}");
        }

        private void CreateSoundFallbacks()
        {
            // Create fallback sounds if file loading fails
            try
            {
                shootSound = CreateBeepSound(800, 0.1f, 0.3f);      // High pitched shoot
                hitSound = CreateBeepSound(200, 0.08f, 0.5f);       // Low pitched hit
                powerUpSound = CreateBeepSound(600, 0.2f, 0.4f);    // Medium pitched power-up
                iceHitSound = CreateBeepSound(1200, 0.12f, 0.3f);   // Very high pitched ice
                wallStickSound = CreateBeepSound(150, 0.15f, 0.4f); // Very low pitched wall
                
                // Set volume levels for fallback sounds
                SetSoundVolumes();
                
                Console.WriteLine("Audio system initialized with generated fallback sounds");
            }
            catch
            {
                // Create silent sounds as last resort
                CreateSilentFallbacks();
            }
        }

        private void CreateSilentFallbacks()
        {
            // Create minimal silent sounds as fallback
            Wave silentWave = new Wave
            {
                SampleCount = 100,
                SampleRate = 22050,
                SampleSize = 16,
                Channels = 1,
                Data = default
            };

            //shootSound = Raylib.LoadSoundFromWave(silentWave);
            //hitSound = Raylib.LoadSoundFromWave(silentWave);
            //powerUpSound = Raylib.LoadSoundFromWave(silentWave);
            //iceHitSound = Raylib.LoadSoundFromWave(silentWave);
            //wallStickSound = Raylib.LoadSoundFromWave(silentWave);
        }

        private Sound CreateBeepSound(float frequency, float duration, float volume)
        {
            // Use a much simpler approach with Raylib's built-in wave generation
            // Create a basic sine wave using Raylib's LoadWave functions
            
            int sampleRate = 22050;
            int frameCount = (int)(sampleRate * duration);
            
            // Generate the wave using a simple approach
            var waveData = new float[frameCount];
            
            for (int i = 0; i < frameCount; i++)
            {
                float time = (float)i / sampleRate;
                float envelope = 1.0f - (time / duration); // Fade out envelope
                waveData[i] = volume * envelope * MathF.Sin(2 * MathF.PI * frequency * time);
            }
            
            // Create a proper WAV file in memory
            byte[] wavFile = CreateWAVFile(waveData, sampleRate, 1);
            
            // Load using LoadWaveFromMemory with proper WAV format
            Wave wave = Raylib.LoadWaveFromMemory(".wav", wavFile);
            return Raylib.LoadSoundFromWave(wave);
        }

        private byte[] CreateWAVFile(float[] samples, int sampleRate, int channels)
        {
            // Create a proper WAV file with headers
            int bitsPerSample = 16;
            int bytesPerSample = bitsPerSample / 8;
            int dataSize = samples.Length * bytesPerSample;
            int fileSize = 44 + dataSize; // 44 bytes for WAV header

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                // WAV Header
                writer.Write("RIFF".ToCharArray()); // ChunkID
                writer.Write(fileSize - 8); // ChunkSize
                writer.Write("WAVE".ToCharArray()); // Format

                // Format Subchunk
                writer.Write("fmt ".ToCharArray()); // Subchunk1ID
                writer.Write(16); // Subchunk1Size (PCM)
                writer.Write((short)1); // AudioFormat (PCM)
                writer.Write((short)channels); // NumChannels
                writer.Write(sampleRate); // SampleRate
                writer.Write(sampleRate * channels * bytesPerSample); // ByteRate
                writer.Write((short)(channels * bytesPerSample)); // BlockAlign
                writer.Write((short)bitsPerSample); // BitsPerSample

                // Data Subchunk
                writer.Write("data".ToCharArray()); // Subchunk2ID
                writer.Write(dataSize); // Subchunk2Size

                // Audio Data (convert float samples to 16-bit PCM)
                foreach (float sample in samples)
                {
                    short pcmSample = (short)(sample * 32767);
                    writer.Write(pcmSample);
                }

                return ms.ToArray();
            }
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                Update();
                Draw();
            }

            // Clean up audio resources
            Raylib.UnloadSound(shootSound);
            Raylib.UnloadSound(hitSound);
            Raylib.UnloadSound(powerUpSound);
            Raylib.UnloadSound(iceHitSound);
            Raylib.UnloadSound(wallStickSound);
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }

        // Public methods for playing sounds
        public void PlayShootSound() => Raylib.PlaySound(shootSound);
        public void PlayHitSound() => Raylib.PlaySound(hitSound);
        public void PlayPowerUpSound() => Raylib.PlaySound(powerUpSound);
        public void PlayIceHitSound() => Raylib.PlaySound(iceHitSound);
        public void PlayWallStickSound() => Raylib.PlaySound(wallStickSound);

        private void Update()
        {
            // Handle fullscreen toggle and exit
            if (Raylib.IsKeyPressed(KeyboardKey.F11))
            {
                Raylib.ToggleFullscreen();
            }
            
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                Environment.Exit(0); // Exit the game
            }

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
            
            // Simplified power-up spawning
            if (random.Next(0, 60 * 5) == 1)
            {
                SpawnPowerUp();
            }
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            foreach (var obj in gameObjects)
            {
                obj.Draw();
            }
            
            // Draw UI
            DrawUI();

            Raylib.EndDrawing();
        }

        private void SpawnPowerUp()
        {
            int powerUpTypeIndex = random.Next(0, 3);
            PowerUpType type = (PowerUpType)powerUpTypeIndex;
            int screenWidth = Raylib.GetScreenWidth();
            Rectangle rect = new Rectangle(random.Next(100, screenWidth - 100), -20, 20, 20);
            gameObjects.Add(new PowerUp(rect, type, 2.0f));
        }

        private void DrawUI()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            
            Raylib.DrawText($"P1 Health: {player1.Health}", 10, 10, 20, Color.White);
            Raylib.DrawText($"P2 Health: {player2.Health}", screenWidth - 200, 10, 20, Color.White);

            // Draw controls info
            Raylib.DrawText("F11 - Toggle Fullscreen | ESC - Exit", 10, screenHeight - 30, 16, Color.LightGray);

            if (player1.Health <= 0 || player2.Health <= 0)
            {
                string winner = player1.Health > 0 ? "Player 1 Wins!" : "Player 2 Wins!";
                Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), winner, 40, 1);
                Raylib.DrawText(winner, (int)(screenWidth / 2 - textSize.X / 2), (int)(screenHeight / 2 - textSize.Y / 2), 40, Color.Gold);
            }
        }
        
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
    }
}
