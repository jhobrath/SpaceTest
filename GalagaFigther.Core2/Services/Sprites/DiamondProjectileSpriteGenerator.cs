using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Services.Sprites
{
    public static class DiamondProjectileSpriteGenerator
    {
        // Area: (w*h)/2 = 200 (w=20, h=20)
        public static Texture2D CreateAnimatedDiamondProjectile(float drawWidth = 20, float drawHeight = 20, Color? color = null, int frame = 0)
        {
            var c = color ?? Color.Orange;
            var key = $"DiamondProjectile_{drawWidth}_{drawHeight}_{frame}_{c.R}_{c.G}_{c.B}";
            if (TextureCache.ContainsKey(key))
                return TextureCache.Get(key);

            int frameCount = 8;
            float pulse = 1f + 0.10f * (float)Math.Sin((frame / (float)frameCount) * Math.PI * 2);
            float w = drawWidth * pulse;
            float h = drawHeight * (1f - 0.05f * (float)Math.Cos((frame / (float)frameCount) * Math.PI * 2));

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture((int)drawWidth, (int)drawHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            Vector2 center = new Vector2(drawWidth/2, drawHeight/2);
            Vector2[] points = new Vector2[] {
                new Vector2(center.X, center.Y-h/2),
                new Vector2(center.X+w/2, center.Y),
                new Vector2(center.X, center.Y+h/2),
                new Vector2(center.X-w/2, center.Y)
            };
            Raylib.DrawTriangle(points[0], points[1], points[2], c);
            Raylib.DrawTriangle(points[2], points[3], points[0], c);
            // Inner highlight
            Raylib.DrawTriangle(
                Vector2.Lerp(points[0], center, 0.5f),
                Vector2.Lerp(points[1], center, 0.5f),
                center,
                new Color(255,255,255,100));
            Raylib.EndTextureMode();
            TextureCache.Set(key, renderTexture.Texture);
            return renderTexture.Texture;
        }
    }
}
