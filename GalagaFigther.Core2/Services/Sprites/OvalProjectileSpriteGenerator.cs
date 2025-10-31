using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Services.Sprites
{
    public static class OvalProjectileSpriteGenerator
    {
        // Area: pi * r1 * r2 = ~282 (w=20, h=18)
        public static Texture2D CreateAnimatedOvalProjectile(float drawWidth = 20, float drawHeight = 18, Color? color = null, int frame = 0)
        {
            var c = color ?? Color.Yellow;
            var key = $"OvalProjectile_{drawWidth}_{drawHeight}_{frame}_{c.R}_{c.G}_{c.B}";
            if (TextureCache.ContainsKey(key))
                return TextureCache.Get(key);

            int frameCount = 8;
            float pulse = 1f + 0.08f * (float)Math.Sin((frame / (float)frameCount) * Math.PI * 2);
            float w = drawWidth * pulse;
            float h = drawHeight * (1f - 0.05f * (float)Math.Cos((frame / (float)frameCount) * Math.PI * 2));

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture((int)drawWidth, (int)drawHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            // Outer glow
            Raylib.DrawEllipse((int)(drawWidth/2), (int)(drawHeight/2), w/2+2, h/2+2, new Color(255,255,255,60));
            // Main oval
            Raylib.DrawEllipse((int)(drawWidth/2), (int)(drawHeight/2), w/2, h/2, c);
            // Inner highlight
            Raylib.DrawEllipse((int)(drawWidth/2), (int)(drawHeight/2.5), w/4, h/6, new Color(255,255,255,120));

            Raylib.EndTextureMode();
            TextureCache.Set(key, renderTexture.Texture);
            return renderTexture.Texture;
        }
    }
}
