using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services.Sprites
{
    public static class DefaultProjectileSpriteGenerator
    {
        public static Texture2D CreateProjectileSprite(int width = 10, int height = 5, Color? color = null)
        {
            string colorKey = color.HasValue ? $"_{color.Value.R},{color.Value.G},{color.Value.B},{color.Value.A}" : "";
            string key = $"Projectile_Default_{width}_{height}{colorKey}";
            if (TextureCache.TryGetValue(key, out var texture))
                return texture;

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            Draw(width, height, color);
            
            Raylib.EndTextureMode();
            TextureCache.Set(key, renderTexture.Texture);
            return renderTexture.Texture;
        }

        private static void Draw(int width, int height, Color? color)
        {
            float scaleX = width / 10f;
            float scaleY = height / 5f;
            int bulletHeight = (int)(2 * scaleY);
            int tipRadius = (int)(1 * Math.Min(scaleX, scaleY));
            Raylib.DrawRectangle(0, height / 2 - bulletHeight / 2, width, bulletHeight, color ?? Color.Yellow);
            Raylib.DrawCircle(width - tipRadius, height / 2, tipRadius, Color.White);
        }
    }
}
