using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services.Sprites
{
    public static class RepulseShieldSpriteGenerator
    {
        public static Texture2D CreateAnimatedMagnetShieldSprite(float drawWidth, float drawHeight, int frame)
        {
            var key = $"RepulseShield_{drawWidth}_{drawHeight}_{frame}";
             if (TextureCache.ContainsKey(key))
                 return TextureCache.Get(key);

            var frameCount = 24;
            float pulse = 1f + 0.2f * (float)Math.Sin((frame / (float)frameCount) * Math.PI * 2);
            int arcWidth = (int)drawWidth; // Arc length is constant
            int arcHeight = (int)drawHeight;
            int xOffset = 0;
            float centerX = drawWidth / 2;

            // Use the correct formula for a full-width arc
            float radius = (drawWidth * drawWidth) / (8f * drawHeight) * 3;
            float arcCenterY = -(radius - (drawHeight / 2f));

            // Pulse the distance between red and blue lines
            float baseSeparation = drawHeight * 0.35f;
            float separation = baseSeparation * pulse;
            float redLineOffset = -separation / 2f;
            float blueLineOffset = separation / 2f;

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture((int)drawWidth, (int)drawHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            for (int x = 0; x < arcWidth; x++)
            {
                float px = x + xOffset;
                float dx = ((float)x / (arcWidth - 1)) * (drawWidth - 1) - (drawWidth - 1) / 2f;
                float underSqrt = radius * radius - dx * dx;
                if (underSqrt < 0) continue;
                float yOnCircle = arcCenterY + (float)Math.Sqrt(underSqrt); // bottom arc, curves upward (concave)

                // Fade out edges
                float fadePixels = 50f;
                float edgeFade = 1f;
                if (x < fadePixels)
                    edgeFade = x / fadePixels;
                else if (x > arcWidth - 1 - fadePixels)
                    edgeFade = (arcWidth - 1 - x) / fadePixels;
                byte edgeAlpha = (byte)(255 * edgeFade);

                // Red line (inner)
                int redY = (int)(yOnCircle + redLineOffset);
                for (int t = 0; t < 4; t++)
                {
                    int y = redY + t;
                    if (y >= 0 && y < drawHeight)
                    {
                        Raylib.DrawPixel((int)px, y, new Color((int)220f, (int)40f, (int)40f, (int)edgeAlpha));
                    }
                }
                // Blue line (outer)
                int blueY = (int)(yOnCircle + blueLineOffset);
                for (int t = 0; t < 4; t++)
                {
                    int y = blueY + t;
                    if (y >= 0 && y < drawHeight)
                    {
                        Raylib.DrawPixel((int)px, y, new Color((int)120, (int)180, (int)255, (int)(230 * edgeFade)));
                    }
                }
                // Middle gradient (muted dark red)
                int midStart = redY + 4;
                int midEnd = blueY;
                if (midEnd < midStart) { var tmp = midStart; midStart = midEnd; midEnd = tmp; }
                for (int y = midStart; y < midEnd; y++)
                {
                    if (y >= 0 && y < drawHeight)
                    {
                        Raylib.DrawPixel((int)px, y, new Color((int)120, (int)30, (int)60, (int)(byte)(200 * edgeFade)));
                    }
                }
            }

            Raylib.EndTextureMode();
            TextureCache.Set(key, renderTexture.Texture);
            return renderTexture.Texture;
        }
    }
}
