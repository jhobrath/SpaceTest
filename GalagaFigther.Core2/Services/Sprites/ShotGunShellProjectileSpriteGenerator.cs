using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services.Sprites
{
    public class ShotGunShellProjectileSpriteGenerator
    {
        private static Random _random = new Random();

        // Area: w*h = 320 (w=32, h=10)
        public static Texture2D CreateAnimatedShotGunShellProjectile(float drawWidth, float drawHeight, Color color, int frame)
        {
            if (drawWidth <= 0) drawWidth = 600;
            if (drawHeight <= 0) drawHeight = 100;
            int frameCount = 7;
            frame = Math.Clamp(frame, 0, frameCount - 1);
            var c = color;
            var key = $"ShotGunShell_{drawWidth}_{drawHeight}_{c.R}_{c.G}_{c.B}_{frame}";
            if (TextureCache.ContainsKey(key))
                return TextureCache.Get(key);

            // Opacity by frame (now 7 frames)
            float[] opacities = { 0.65f, 0.85f, 1.0f, 1.0f, 0.65f, 0.5f, 0.4f };
            float opacity = opacities[Math.Clamp(frame, 0, opacities.Length - 1)];

            // Animated triangle base position (still expands, then stays full)
            float[] basePercents = { 0.25f, 0.6f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
            float basePercent = basePercents[Math.Clamp(frame, 0, basePercents.Length - 1)];
            float baseX = drawWidth * basePercent;

            // As frame increases, minX moves further right for thinning
            float[] minXFracs = { 0.0f, 0.15f, 0.3f, 0.4f, 0.5f, 0.6f, 0.65f };
            float minXFrac = minXFracs[Math.Clamp(frame, 0, minXFracs.Length - 1)];
            float minX = baseX * minXFrac;

            // Jitter for base only
            float jitterAmount = drawWidth * 0.03f;
            float px = 0; // left point, fixed
            float py = drawHeight / 2f; // fixed
            float baseYSpread = (drawHeight / 2f) * basePercent;
            float by1 = py - baseYSpread + (float)(_random.NextDouble() - 0.5) * jitterAmount; // top
            float by2 = py + baseYSpread + (float)(_random.NextDouble() - 0.5) * jitterAmount; // bottom
            float bx1 = baseX + (float)(_random.NextDouble() - 0.5) * jitterAmount;
            float bx2 = baseX + (float)(_random.NextDouble() - 0.5) * jitterAmount;

            // Reduce pellet count for last two frames
            int basePelletCount = (int)(drawWidth * drawHeight / 120);
            int pelletCount = frame < 5 ? basePelletCount : (int)(basePelletCount * (frame == 5 ? 0.5f : 0.35f));
            float minRadius = 0.6f;
            float maxRadius = 1.5f;
            Random rand = new(frame + (int)drawWidth + (int)drawHeight + c.R + c.G + c.B);

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture((int)drawWidth, (int)drawHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            int placed = 0;
            int maxTries = pelletCount * 5;
            int tries = 0;
            while (placed < pelletCount && tries < maxTries)
            {
                tries++;
                // Barycentric coordinates for random point in triangle
                float u = (float)rand.NextDouble();
                float v = (float)rand.NextDouble();
                if (u + v > 1) { u = 1 - u; v = 1 - v; }
                float x = px * (1 - u - v) + bx1 * u + bx2 * v;
                float y = py * (1 - u - v) + by1 * u + by2 * v;
                // Add random offset for extra naturalness
                x += (float)(rand.NextDouble() - 0.5) * jitterAmount * 0.7f;
                y += (float)(rand.NextDouble() - 0.5) * jitterAmount * 0.7f;
                if (x < minX) continue; // skip if too far left for this frame
                float pelletSize = (frame == 0)
                    ? (float)(minRadius + (maxRadius - minRadius) * rand.NextDouble() * 0.5)
                    : (float)(minRadius + (maxRadius - minRadius) * rand.NextDouble());
                pelletSize = Math.Min(pelletSize, maxRadius);
                float pelletAlpha = opacity;
                Color pelletColor = new Color((byte)c.R, (byte)c.G, (byte)c.B, (byte)(255 * pelletAlpha));
                Raylib.DrawCircle((int)x, (int)y, pelletSize, pelletColor);
                placed++;
            }

            Raylib.EndTextureMode();
            TextureCache.Set(key, renderTexture.Texture);
            return renderTexture.Texture;
        }
    }
}
