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
        // Area: w*h = 320 (w=32, h=10)
        public static Texture2D CreateAnimatedShotGunShellProjectile(float drawWidth, float drawHeight, Color color, int frame)
        {
            if (drawWidth <= 0) drawWidth = 600;
            if (drawHeight <= 0) drawHeight = 100;
            int frameCount = 5;
            frame = Math.Clamp(frame, 0, frameCount - 1);
            var c = color;
            var key = $"ShotGunShell_{drawWidth}_{drawHeight}_{c.R}_{c.G}_{c.B}_{frame}";
            if (TextureCache.ContainsKey(key))
                return TextureCache.Get(key);

            // Opacity by frame
            float[] opacities = { 1.0f, 1.0f, 0.75f, 0.5f, 0.25f };
            float opacity = opacities[Math.Clamp(frame, 0, opacities.Length - 1)];

            // Pellet parameters
            int pelletCount = (int)(drawWidth * drawHeight / 120); // Plentiful
            float minRadius = 1.2f;
            float maxRadius = 3.0f;
            Random rand = new(frame + (int)drawWidth + (int)drawHeight + c.R + c.G + c.B); // Deterministic per frame

            // Triangle vertices: point at left center, base at right top/bottom
            float px = 0;
            float py = drawHeight / 2f;
            float bx1 = drawWidth - 1;
            float by1 = 0;
            float bx2 = drawWidth - 1;
            float by2 = drawHeight - 1;

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture((int)drawWidth, (int)drawHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            for (int i = 0; i < pelletCount; i++)
            {
                // Barycentric coordinates for random point in triangle
                float u = (float)rand.NextDouble();
                float v = (float)rand.NextDouble();
                if (u + v > 1) { u = 1 - u; v = 1 - v; }
                float x = px * (1 - u - v) + bx1 * u + bx2 * v;
                float y = py * (1 - u - v) + by1 * u + by2 * v;
                float pelletSize = (frame == 0)
                    ? (float)(minRadius + (maxRadius - minRadius) * rand.NextDouble() * 0.5)
                    : (float)(minRadius + (maxRadius - minRadius) * rand.NextDouble());
                pelletSize = Math.Min(pelletSize, maxRadius);
                float pelletAlpha = (frame == 0) ? 1.0f : opacity;
                Color pelletColor = (frame == 0)
                    ? new Color((byte)255, (byte)255, (byte)200, (byte)(255 * pelletAlpha))
                    : new Color((byte)c.R, (byte)c.G, (byte)c.B, (byte)(255 * pelletAlpha));
                Raylib.DrawCircle((int)x, (int)y, pelletSize, pelletColor);
                // Add a white highlight for extra brightness
                if (frame == 0)
                {
                    Raylib.DrawCircle((int)x, (int)y, pelletSize * 0.5f, new Color((byte)255, (byte)255, (byte)255, (byte)180));
                }
            }

            Raylib.EndTextureMode();
            TextureCache.Set(key, renderTexture.Texture);
            return renderTexture.Texture;
        }
    }
}
