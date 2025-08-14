using Raylib_cs;
using System;
using System.IO;

namespace GalagaFighter.Services
{
    public interface IAudioService
    {
        void PlayShootSound();
        void PlayHitSound();
        void PlayPowerUpSound();
        void PlayIceHitSound();
        void PlayWallStickSound();
        void Initialize();
        void Cleanup();
        void PlayExplosionConversionSound();
        void PlayBurningSound();
        void PlayMagnetSound();
        void PlayMudSplat();
        void PlayMagnetReleaseSound();
    }

    public class AudioService : IAudioService
    {
        private Sound shootSound;
        private Sound hitSound;
        private Sound powerUpSound;
        private Sound iceHitSound;
        private Sound wallStickSound;
        private Sound explosionConversionSound;
        private Sound burningSound;
        private Sound magnetSound;
        private Sound mudsplat;
        private Sound magnetReleaseSound;

        public void Initialize()
        {
            Raylib.InitAudioDevice();
            CreateSounds();
        }

        public void Cleanup()
        {
            Raylib.UnloadSound(shootSound);
            Raylib.UnloadSound(hitSound);
            Raylib.UnloadSound(iceHitSound);
            Raylib.UnloadSound(wallStickSound);
            Raylib.UnloadSound(explosionConversionSound);
            Raylib.UnloadSound(powerUpSound);
            Raylib.UnloadSound(burningSound);
            Raylib.UnloadSound(magnetSound);
            Raylib.UnloadSound(mudsplat);
            Raylib.UnloadSound(magnetReleaseSound);
            Raylib.CloseAudioDevice();
        }

        public void PlayShootSound() => Raylib.PlaySound(shootSound);
        public void PlayHitSound() => Raylib.PlaySound(hitSound);
        public void PlayIceHitSound() => Raylib.PlaySound(iceHitSound);
        public void PlayWallStickSound() => Raylib.PlaySound(wallStickSound);
        public void PlayExplosionConversionSound() => Raylib.PlaySound(explosionConversionSound);
        public void PlayPowerUpSound() => Raylib.PlaySound(powerUpSound);
        public void PlayBurningSound() => Raylib.PlaySound(burningSound);
        public void PlayMagnetSound() => Raylib.PlaySound(magnetSound);
        public void PlayMudSplat() => Raylib.PlaySound(mudsplat);
        public void PlayMagnetReleaseSound() => Raylib.PlaySound(magnetReleaseSound);

        private void CreateSounds()
        {
            try 
            {
                // Try to load custom sound files first
                shootSound = LoadSoundFile("projectile-default.wav");
                iceHitSound = LoadSoundFile("player_freezing.wav");
                wallStickSound = LoadSoundFile("plank_landing.wav");
                explosionConversionSound = LoadSoundFile("explosion_conversion.ogg");
                hitSound = LoadSoundFile("collision-default.wav");
                powerUpSound = LoadSoundFile("powerup-default.wav");
                burningSound = LoadSoundFile("burning.wav");
                magnetSound = LoadSoundFile("magnet.wav");
                mudsplat = LoadSoundFile("mudsplat.wav");
                magnetReleaseSound = LoadSoundFile("magnet-release.wav");

                SetSoundVolumes();
                Console.WriteLine("Audio system initialized with mixed sound sources");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not initialize audio system: {ex.Message}");
                CreateFallbackSounds();
            }
        }

        private Sound LoadSoundFile(string filename)
        {
            string[] possiblePaths = {
                filename,
                $"sounds/{filename}",
                $"assets/{filename}",
                $"audio/{filename}",
                $"../../../{filename}",
                $"../../../sounds/{filename}",
                $"../../../assets/{filename}",
                $"../../../audio/{filename}"
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

        private void CreateFallbackSounds()
        {
            try
            {
                shootSound = CreateBeepSound(800, 0.1f, 0.3f);
                hitSound = CreateBeepSound(200, 0.08f, 0.5f);
                powerUpSound = CreateBeepSound(600, 0.2f, 0.4f);
                iceHitSound = CreateBeepSound(1200, 0.12f, 0.3f);
                wallStickSound = CreateBeepSound(150, 0.15f, 0.4f);

                SetSoundVolumes();
                Console.WriteLine("Audio system initialized with generated fallback sounds");
            }
            catch
            {
                // Create silent sounds as last resort - implementation would go here
                Console.WriteLine("Audio system failed - running silently");
            }
        }

        private void SetSoundVolumes()
        {
            Raylib.SetSoundVolume(shootSound, 0.01f);  // Very quiet for rapid fire
            Raylib.SetSoundVolume(iceHitSound, 1.0f);
            Raylib.SetSoundVolume(wallStickSound, 1.0f);
            Raylib.SetSoundVolume(hitSound, 1.0f);
            Raylib.SetSoundVolume(powerUpSound, .005f);
            Raylib.SetSoundVolume(explosionConversionSound, .05f);
            Raylib.SetSoundVolume(burningSound, 1.5f);
            Raylib.SetSoundVolume(magnetSound, .05f);
            Raylib.SetSoundVolume(mudsplat, .1f);
            Raylib.SetSoundVolume(magnetReleaseSound, .1f);
        }

        private Sound CreateBeepSound(float frequency, float duration, float volume)
        {
            int sampleRate = 22050;
            int frameCount = (int)(sampleRate * duration);
            var waveData = new float[frameCount];
            
            for (int i = 0; i < frameCount; i++)
            {
                float time = (float)i / sampleRate;
                float envelope = 1.0f - (time / duration);
                waveData[i] = volume * envelope * MathF.Sin(2 * MathF.PI * frequency * time);
            }
            
            byte[] wavFile = CreateWAVFile(waveData, sampleRate, 1);
            Wave wave = Raylib.LoadWaveFromMemory(".wav", wavFile);
            return Raylib.LoadSoundFromWave(wave);
        }

        private byte[] CreateWAVFile(float[] samples, int sampleRate, int channels)
        {
            int bitsPerSample = 16;
            int bytesPerSample = bitsPerSample / 8;
            int dataSize = samples.Length * bytesPerSample;
            int fileSize = 44 + dataSize;

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                // WAV Header
                writer.Write("RIFF".ToCharArray());
                writer.Write(fileSize - 8);
                writer.Write("WAVE".ToCharArray());

                // Format Subchunk
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * channels * bytesPerSample);
                writer.Write((short)(channels * bytesPerSample));
                writer.Write((short)bitsPerSample);

                // Data Subchunk
                writer.Write("data".ToCharArray());
                writer.Write(dataSize);

                foreach (float sample in samples)
                {
                    short pcmSample = (short)(sample * 32767);
                    writer.Write(pcmSample);
                }

                return ms.ToArray();
            }
        }
    }
}