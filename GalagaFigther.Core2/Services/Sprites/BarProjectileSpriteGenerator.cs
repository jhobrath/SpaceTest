using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Services.Sprites
{
    public static class BarProjectileSpriteGenerator
    {
        // Area: w*h = 320 (w=32, h=10)
        public static Texture2D CreateAnimatedBarProjectile(float drawWidth, float drawHeight, Color color, int frame)
        {
            var c = color;
            var key = $"BarProjectile_{drawWidth}_{drawHeight}_{frame}_{c.R}_{c.G}_{c.B}";
            if (TextureCache.ContainsKey(key))
                return TextureCache.Get(key);

            int frameCount = 12;
            float pulse = 1.15f + 0.18f * (float)Math.Sin((frame / (float)frameCount) * Math.PI * 2);
            float w = drawWidth * pulse;
            float h = drawHeight * (1f - 0.10f * (float)Math.Cos((frame / (float)frameCount) * Math.PI * 2));

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture((int)drawWidth, (int)drawHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            // Dramatic animated aura (pulsing outer glow)
            float auraPulse = 2.0f + 0.5f * (float)Math.Sin((frame / (float)frameCount) * Math.PI * 2 + Math.PI/2);
            for (int i = 8; i >= 1; i--)
            {
                float alpha = 16 * i;
                float auraW = w + i * auraPulse * 2;
                float auraH = h + i * auraPulse * 2;
                // Draw aura as a rounded+pointed shape
                DrawTeardrop((int)drawWidth, (int)drawHeight, auraW, auraH, new Color((byte)c.R, (byte)c.G, (byte)c.B, (byte)alpha), 0);
            }

            // Silver border (outer)
            int border = 2;
            Color silver = new Color(220, 220, 235, 255);
            DrawTeardrop((int)drawWidth, (int)drawHeight, w+border*2, h+border*2, silver, border);

            // Bar with metallic gradient (darker at edges, lighter in center, more contrast)
            for (int x = 0; x < (int)w; x++)
            {
                float t = (float)x / (w-1);
                float edgeFade = 0.5f + 0.5f * (float)Math.Cos((t-0.5f)*Math.PI); // Center is much lighter
                Color gradColor = new Color(
                    Math.Clamp((int)(c.R * edgeFade + 40 * (1-edgeFade)), 0, 255),
                    Math.Clamp((int)(c.G * edgeFade + 40 * (1-edgeFade)), 0, 255),
                    Math.Clamp((int)(c.B * edgeFade + 60 * (1-edgeFade)), 0, 255),
                    255);
                // Draw vertical line for each x in the teardrop shape
                DrawTeardropColumn((int)drawWidth, (int)drawHeight, w, h, x, gradColor, 0);
            }

            // Animated diagonal silver stripes
            int stripeWidth = 5;
            int stripeGap = 7;
            int stripeOffset = (frame * 3) % (stripeWidth + stripeGap);
            for (int x = -((int)h); x < (int)w + (int)h; x += stripeWidth + stripeGap)
            {
                for (int dx = 0; dx < stripeWidth; dx++)
                {
                    int sx = x + dx + stripeOffset;
                    for (int y = 0; y < (int)h; y++)
                    {
                        if (IsInTeardrop((int)drawWidth, (int)drawHeight, w, h, sx+y, y, 0))
                        {
                            Raylib.DrawPixel((int)((drawWidth-w)/2)+sx+y, (int)((drawHeight-h)/2)+y, new Color(230, 230, 255, 60));
                        }
                    }
                }
            }

            // Dramatic shimmer highlight (moves across the bar, much brighter)
            float shimmerPos = (w + 12) * ((float)frame / frameCount);
            for (int y = 0; y < (int)h; y++)
            {
                int sx = (int)((drawWidth-w)/2 + shimmerPos - 6);
                for (int dx = 0; dx < 12; dx++)
                {
                    int px = sx + dx;
                    if (IsInTeardrop((int)drawWidth, (int)drawHeight, w, h, px-(int)((drawWidth-w)/2), y, 0))
                    {
                        float highlight = 1f - Math.Abs(dx-6)/6f;
                        Raylib.DrawPixel(px, (int)((drawHeight-h)/2)+y, new Color(255,255,255,(120*highlight)));
                    }
                }
            }

            // Afterimage trail (faint, behind the bar, offset by frame)
            int trailFrames = 4;
            for (int t = 1; t <= trailFrames; t++)
            {
                float trailAlpha = 24f / t;
                float trailOffset = t * 3f + (float)Math.Sin((frame-t)/(float)frameCount * Math.PI*2) * 2f;
                for (int x = 0; x < (int)w; x++)
                {
                    for (int y = 0; y < (int)h; y++)
                    {
                        int px = (int)((drawWidth-w)/2)+x - (int)trailOffset;
                        int py = (int)((drawHeight-h)/2)+y;
                        if (IsInTeardrop((int)drawWidth, (int)drawHeight, w, h, x, y, 0) && px >= 0 && px < drawWidth)
                        {
                            Raylib.DrawPixel(px, py, new Color((byte)c.R, (byte)c.G, (byte)c.B, (byte)trailAlpha));
                        }
                    }
                }
            }

            Raylib.EndTextureMode();
            TextureCache.Set(key, renderTexture.Texture);
            return renderTexture.Texture;
        }

        // Draws a teardrop (rounded left, pointed right) filled shape
        private static void DrawTeardrop(int texW, int texH, float w, float h, Color color, int border)
        {
            int left = (int)((texW-w)/2);
            int top = (int)((texH-h)/2);
            int radius = (int)(h/2);
            int bodyW = (int)(w * 0.62f); // Rectangle body width: 62% of total width
            int right = left + (int)w;
            int bottom = top + (int)h;
            // Draw left semicircle
            Raylib.DrawCircle(left+radius, top+radius, radius-border, color);
            // Draw main rectangle body (stop well before the point)
            Raylib.DrawRectangle(left+radius, top+border, bodyW-radius, (int)h-2*border, color);
            // Draw right triangle (point), fills the rest of the width
            Vector2 p1 = new Vector2(left+bodyW, top+border);
            Vector2 p2 = new Vector2(left+bodyW, bottom-border);
            Vector2 p3 = new Vector2(right-1, top+h/2);
            Raylib.DrawTriangle(p1, p2, p3, color);
        }

        // Draws a vertical column in the teardrop shape
        private static void DrawTeardropColumn(int texW, int texH, float w, float h, int x, Color color, int border)
        {
            int left = (int)((texW-w)/2);
            int top = (int)((texH-h)/2);
            int radius = (int)(h/2);
            int bodyW = (int)(w * 0.62f);
            int xx = left + x;
            for (int y = 0; y < (int)h; y++)
            {
                int yy = top + y;
                // Check if in left semicircle
                if (x < radius)
                {
                    float dx = x - radius;
                    float dy = y - radius;
                    if (dx*dx + dy*dy > (radius-border)*(radius-border))
                        continue;
                }
                // Check if in right triangle (point)
                else if (x >= bodyW)
                {
                    float relX = x - bodyW;
                    float relY = y - h/2;
                    float triWidth = w - bodyW;
                    float slope = h/(2.0f*triWidth);
                    if (Math.Abs(relY) > (triWidth-relX)*slope)
                        continue;
                }
                // Rectangle body
                else if (x >= radius && x < bodyW) { /* always in shape */ }
                else continue;
                Raylib.DrawPixel(xx, yy, color);
            }
        }

        // Checks if a pixel is inside the teardrop shape
        private static bool IsInTeardrop(int texW, int texH, float w, float h, int x, int y, int border)
        {
            int left = (int)((texW-w)/2);
            int top = (int)((texH-h)/2);
            int radius = (int)(h/2);
            int bodyW = (int)(w * 0.62f);
            // Left semicircle
            if (x < radius)
            {
                float dx = x - radius;
                float dy = y - radius;
                if (dx*dx + dy*dy > (radius-border)*(radius-border))
                    return false;
            }
            // Right triangle (point)
            else if (x >= bodyW)
            {
                float relX = x - bodyW;
                float relY = y - h/2;
                float triWidth = w - bodyW;
                float slope = h/(2.0f*triWidth);
                if (Math.Abs(relY) > (triWidth-relX)*slope)
                    return false;
            }
            // Rectangle body
            else if (x >= radius && x < bodyW) { /* always in shape */ }
            else return false;
            return true;
        }
    }
}
