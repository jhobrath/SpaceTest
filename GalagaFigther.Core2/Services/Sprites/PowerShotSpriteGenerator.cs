using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Services.Sprites
{
    public static class PowerShotSpriteGenerator
    {
        // Area: w*h = 320 (w=32, h=10)
        public static Texture2D GeneratePowerShotSprite(float drawWidth, float drawHeight, Color color, int frame)
        {
            var c = color;
            var key = $"PowerShotProjectile_{drawWidth}_{drawHeight}_{frame}_{c.R}_{c.G}_{c.B}";
            if (TextureCache.ContainsKey(key))
                return TextureCache.Get(key);

            int frameCount = 12;
            float frameProgress = frame / (float)frameCount;
            
            // Dynamic pulsing and energy effects based on frame
            float energyPulse = 1.2f + 0.4f * (float)Math.Sin((frame / (float)frameCount) * (float)Math.PI * 4);
            float corePulse = 1.0f + 0.3f * (float)Math.Sin((frame / (float)frameCount) * (float)Math.PI * 6);
            float sparklePhase = (frame / (float)frameCount) * (float)Math.PI * 8;
            float lightningPhase = (frame / (float)frameCount) * (float)Math.PI * 10;

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture((int)drawWidth, (int)drawHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            int texW = (int)drawWidth;
            int texH = (int)drawHeight;
            
            // 1. Draw animated flame trail behind projectile (left side)
            DrawFlameTrail(texW, texH, color, frame, frameCount);
            
            // 2. Draw sparkle dust particles trailing behind
            DrawSparkleTrail(texW, texH, frame, frameCount);
            
            // 3. Draw main projectile core with pulsing energy
            float coreW = drawWidth * 0.7f * corePulse;
            float coreH = drawHeight * 0.8f;
            DrawEnergyCore(texW, texH, coreW, coreH, color, energyPulse);
            
            // 4. Draw lightning arcs around the projectile
            DrawLightningArcs(texW, texH, frame, lightningPhase, color);
            
            // 5. Draw energy particles swirling around
            DrawEnergyParticles(texW, texH, frame, frameCount, color);
            
            // 6. Draw bright leading edge (right side - front face)
            DrawLeadingEdge(texW, texH, color, energyPulse);
            
            // 7. Draw fireworks burst effect (every few frames)
            if (frame % 4 == 0)
            {
                DrawFireworksBurst(texW, texH, frame, color);
            }

            Raylib.EndTextureMode();
            TextureCache.Set(key, renderTexture.Texture);
            return renderTexture.Texture;
        }

        // Draw animated flame trail behind the projectile
        private static void DrawFlameTrail(int texW, int texH, Color baseColor, int frame, int frameCount)
        {
            float flameLength = texW * 0.4f;
            float flameHeight = texH * 0.6f;
            
            // Create flame colors from orange to red
            Color[] flameColors = {
                new Color(255, 255, 100, 200),  // Bright yellow core
                new Color(255, 180, 50, 180),   // Orange
                new Color(255, 100, 30, 160),   // Red-orange
                new Color(200, 50, 20, 140),    // Dark red
            };
            
            for (int i = 0; i < flameColors.Length; i++)
            {
                float layerLength = flameLength * (1.0f - i * 0.2f);
                float layerHeight = flameHeight * (1.0f - i * 0.15f);
                float flicker = 1.0f + 0.3f * (float)Math.Sin((frame + i * 2) * (float)Math.PI / 3.0f);
                
                DrawFlameLayer(texW, texH, layerLength * flicker, layerHeight, flameColors[i]);
            }
        }
        
        private static void DrawFlameLayer(int texW, int texH, float length, float height, Color color)
        {
            int startX = (int)(texW * 0.1f);
            int centerY = texH / 2;
            
            for (int x = 0; x < (int)length; x++)
            {
                float progress = x / length;
                float flameWidth = height * (1.0f - progress * 0.8f);
                
                for (int y = -(int)(flameWidth/2); y <= (int)(flameWidth/2); y++)
                {
                    int drawX = startX + x;
                    int drawY = centerY + y;
                    
                    if (drawX >= 0 && drawX < texW && drawY >= 0 && drawY < texH)
                    {
                        float alpha = (1.0f - progress) * (1.0f - Math.Abs(y) / (flameWidth/2));
                        Color drawColor = new Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)(color.A * alpha));
                        Raylib.DrawPixel(drawX, drawY, drawColor);
                    }
                }
            }
        }

        // Draw sparkle dust particles trailing behind
        private static void DrawSparkleTrail(int texW, int texH, int frame, int frameCount)
        {
            Random rand = new Random(frame); // Seed with frame for consistent animation
            
            for (int i = 0; i < 15; i++)
            {
                float trailProgress = i / 15.0f;
                float x = texW * (0.05f + trailProgress * 0.25f);
                float y = texH * (0.2f + rand.NextSingle() * 0.6f);
                
                // Animate sparkles moving and fading
                float sparklePhase = (frame + i * 2) / (float)frameCount * (float)Math.PI * 2;
                float sparkleAlpha = 0.5f + 0.5f * (float)Math.Sin(sparklePhase);
                
                Color sparkleColor = new Color((byte)255, (byte)255, (byte)255, (byte)(255 * sparkleAlpha));
                DrawSparkle((int)x, (int)y, sparkleColor, 1 + (int)(2 * sparkleAlpha));
            }
        }
        
        private static void DrawSparkle(int x, int y, Color color, int size)
        {
            // Draw a cross-shaped sparkle
            for (int i = -size; i <= size; i++)
            {
                Raylib.DrawPixel(x + i, y, color);
                Raylib.DrawPixel(x, y + i, color);
            }
        }

        // Draw the main energy core of the projectile
        private static void DrawEnergyCore(int texW, int texH, float w, float h, Color baseColor, float pulse)
        {
            // Use the existing teardrop shape but with energy effects
            DrawTeardrop(texW, texH, w, h, baseColor, 1);
            
            // Add inner glow
            float innerW = w * 0.7f * pulse;
            float innerH = h * 0.7f;
            Color glowColor = new Color(
                (byte)Math.Min(255, baseColor.R + 100),
                (byte)Math.Min(255, baseColor.G + 100), 
                (byte)Math.Min(255, baseColor.B + 100),
                (byte)200
            );
            DrawTeardrop(texW, texH, innerW, innerH, glowColor, 1);
            
            // Add bright core
            float coreW = w * 0.4f;
            float coreH = h * 0.4f;
            Color coreColor = new Color((byte)255, (byte)255, (byte)255, (byte)180);
            DrawTeardrop(texW, texH, coreW, coreH, coreColor, 1);
        }

        // Draw lightning arcs around the projectile
        private static void DrawLightningArcs(int texW, int texH, int frame, float lightningPhase, Color baseColor)
        {
            Random rand = new Random(frame);
            Color lightningColor = new Color((byte)255, (byte)255, (byte)255, (byte)200);
            
            // Draw 3-4 lightning arcs
            for (int i = 0; i < 4; i++)
            {
                float arcPhase = lightningPhase + i * (float)Math.PI / 2;
                float arcRadius = texH * 0.6f * (0.8f + 0.4f * (float)Math.Sin(arcPhase));
                
                Vector2 center = new Vector2(texW * 0.6f, texH * 0.5f);
                Vector2 start = center + new Vector2(
                    arcRadius * (float)Math.Cos(arcPhase),
                    arcRadius * (float)Math.Sin(arcPhase)
                );
                Vector2 end = center + new Vector2(
                    arcRadius * (float)Math.Cos(arcPhase + (float)Math.PI),
                    arcRadius * (float)Math.Sin(arcPhase + (float)Math.PI)
                );
                
                DrawLightningBolt(start, end, lightningColor, rand);
            }
        }
        
        private static void DrawLightningBolt(Vector2 start, Vector2 end, Color color, Random rand)
        {
            Vector2 direction = end - start;
            float distance = direction.Length();
            Vector2 normalized = Vector2.Normalize(direction);
            
            Vector2 current = start;
            float traveled = 0;
            
            while (traveled < distance)
            {
                float segmentLength = Math.Min(5f, distance - traveled);
                Vector2 next = current + normalized * segmentLength;
                
                // Add random jaggedness
                Vector2 perpendicular = new Vector2(-normalized.Y, normalized.X);
                next += perpendicular * (rand.NextSingle() - 0.5f) * 8f;
                
                if (current.X >= 0 && current.X < 32 && current.Y >= 0 && current.Y < 10 &&
                    next.X >= 0 && next.X < 32 && next.Y >= 0 && next.Y < 10)
                {
                    Raylib.DrawLine((int)current.X, (int)current.Y, (int)next.X, (int)next.Y, color);
                }
                
                current = next;
                traveled += segmentLength;
            }
        }

        // Draw energy particles swirling around the projectile
        private static void DrawEnergyParticles(int texW, int texH, int frame, int frameCount, Color baseColor)
        {
            Random rand = new Random(42); // Fixed seed for consistent pattern
            
            for (int i = 0; i < 8; i++)
            {
                float angle = (frame + i * frameCount / 8) / (float)frameCount * (float)Math.PI * 2;
                float radius = texH * 0.4f;
                
                Vector2 center = new Vector2(texW * 0.6f, texH * 0.5f);
                Vector2 pos = center + new Vector2(
                    radius * (float)Math.Cos(angle),
                    radius * (float)Math.Sin(angle) * 0.5f // Flatten vertically
                );
                
                if (pos.X >= 0 && pos.X < texW && pos.Y >= 0 && pos.Y < texH)
                {
                    float alpha = 0.6f + 0.4f * (float)Math.Sin(angle * 2);
                    Color particleColor = new Color((byte)baseColor.R, (byte)baseColor.G, (byte)baseColor.B, (byte)(255 * alpha));
                    
                    // Draw particle with glow
                    Raylib.DrawCircle((int)pos.X, (int)pos.Y, 2, particleColor);
                    Raylib.DrawCircle((int)pos.X, (int)pos.Y, 1, Color.White);
                }
            }
        }

        // Draw bright leading edge at the front
        private static void DrawLeadingEdge(int texW, int texH, Color baseColor, float pulse)
        {
            int edgeX = (int)(texW * 0.9f);
            int centerY = texH / 2;
            
            // Draw bright edge glow
            for (int y = 0; y < texH; y++)
            {
                float distance = Math.Abs(y - centerY) / (float)(texH / 2);
                float intensity = (1.0f - distance) * pulse;
                
                if (intensity > 0)
                {
                    Color edgeColor = new Color((byte)255, (byte)255, (byte)255, (byte)(255 * intensity));
                    for (int x = edgeX; x < texW; x++)
                    {
                        float edgeIntensity = intensity * (1.0f - (x - edgeX) / (float)(texW - edgeX));
                        if (edgeIntensity > 0)
                        {
                            Color drawColor = new Color((byte)255, (byte)255, (byte)255, (byte)(255 * edgeIntensity));
                            Raylib.DrawPixel(x, y, drawColor);
                        }
                    }
                }
            }
        }

        // Draw fireworks burst effect
        private static void DrawFireworksBurst(int texW, int texH, int frame, Color baseColor)
        {
            Vector2 center = new Vector2(texW * 0.7f, texH * 0.5f);
            Random rand = new Random(frame);
            
            for (int i = 0; i < 12; i++)
            {
                float angle = i / 12.0f * (float)Math.PI * 2;
                float length = texH * 0.3f * rand.NextSingle();
                
                Vector2 end = center + new Vector2(
                    length * (float)Math.Cos(angle),
                    length * (float)Math.Sin(angle)
                );
                
                if (end.X >= 0 && end.X < texW && end.Y >= 0 && end.Y < texH)
                {
                    Color burstColor = new Color(
                        (byte)(baseColor.R + rand.Next(0, 100)),
                        (byte)(baseColor.G + rand.Next(0, 100)),
                        (byte)255,
                        (byte)150
                    );
                    Raylib.DrawLine((int)center.X, (int)center.Y, (int)end.X, (int)end.Y, burstColor);
                }
            }
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
