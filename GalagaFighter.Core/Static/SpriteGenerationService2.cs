using Raylib_cs;
using System;
using System.Numerics;
using System.Collections.Generic;
using GalagaFighter.Core.Services;

namespace GalagaFighter.Core.Static
{
    public static class SpriteGenerationService2
    {
        public static SpriteWrapper CreateBeamSegmentSprite(int width = 20, int height = 50)
        {
            string key = $"BeamSegment_{width}_{height}";
            if (TextureService.TryGetFromKey(key, out Texture2D texture))
                return new SpriteWrapper(texture);

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            // Base vertical gradient only
            for (int y = 0; y < height; y++)
            {
                float t = Math.Abs(y - height / 2f) / (height / 2f);
                t = (float)Math.Pow(t, 0.7);
                int r = (int)(255 - 40 * t);
                int g = (int)(80 - 40 * t);
                int b = (int)(80 - 40 * t);
                Color glowColor = new Color(r, g, b, 255);
                Raylib.DrawLine(0, y, width - 1, y, glowColor);
            }

            Raylib.EndTextureMode();
            TextureService.Set(key, renderTexture.Texture);
            return new SpriteWrapper(renderTexture.Texture);
        }
    }
}