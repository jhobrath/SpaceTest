using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Services.Sprites
{
    public static class ShieldPixelSpriteGenerator
    {
        // Area: w*h = 320 (w=32, h=10)
        public static Texture2D CreateAnimatedShieldPixel(float drawWidth, float drawHeight, Color color, int frame)
        {
            var c = color;
            var key = $"ShieldPixel_{drawWidth}_{drawHeight}_{frame}_{c.R}_{c.G}_{c.B}";
            if (TextureCache.ContainsKey(key))
                return TextureCache.Get(key);

            int frameCount = 12;
            float pulse = 1.15f + 0.18f * (float)Math.Sin((frame / (float)frameCount) * Math.PI * 2);
            float w = drawWidth * pulse;
            float h = drawHeight * (1f - 0.10f * (float)Math.Cos((frame / (float)frameCount) * Math.PI * 2));

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture((int)drawWidth, (int)drawHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            Raylib.DrawRectangle(0, 0, (int)drawWidth, (int)drawHeight, color);

            Raylib.EndTextureMode();
            TextureCache.Set(key, renderTexture.Texture);
            return renderTexture.Texture;
        }
    }
}
