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

        public static SpriteWrapper CreateAbsorptionEffectSprite(int frameCount = 20, float frameDuration = 0.06f, int width = 200, int height = 20)
        {
            return new SpriteWrapper(
                (position, rotation, drawWidth, drawHeight, scale, frame) =>
                {
                    // Create a pulsing/expanding wave effect that represents the parry force
                    float waveProgress = (frame / (float)frameCount) % 1f;
                    float pulse = 1f + 0.4f * (float)Math.Sin(waveProgress * (float)Math.PI * 2);
                    float expansion = 1f + 0.3f * waveProgress;
                    
                    int effectHeight = (int)(height * scale * pulse);
                    int effectWidth = (int)(width * scale * expansion);
                    int yOffset = (int)(drawHeight / 2 - effectHeight / 2);
                    int xOffset = (int)(drawWidth / 2 - effectWidth / 2);
                    
                    float radians = rotation * (float)Math.PI / 180f;
                    float centerX = position.X;
                    float centerY = position.Y;
                    
                    // Draw multiple wave layers to create a "force field" effect
                    for (int layer = 0; layer < 3; layer++)
                    {
                        float layerOffset = layer * effectHeight / 6f;
                        float layerIntensity = 1f - (layer * 0.3f);
                        
                        for (int x = 0; x < effectWidth; x++)
                        {
                            float progress = (float)x / effectWidth;
                            
                            // Create a wave that pushes outward from center
                            float wavePhase = progress * (float)Math.PI * 4 + waveProgress * (float)Math.PI * 6;
                            float waveHeight = (float)Math.Sin(wavePhase) * 0.6f;  // Increased from 0.3f to 0.6f for 2x amplitude
                            
                            // Create a curved base arc that curves downward at the center and upward at edges (opposite of magnet shield)
                            // To wrap around the ship, we want the edges high and center low
                            float baseArc = (progress - 0.5f) * (progress - 0.5f) * 16f;  // Increased from 4f to 16f for much more curve
                            baseArc *= 0.8f; // Scale it down a bit
                            
                            float combinedWave = baseArc + waveHeight;
                            
                            int baseY = (int)(combinedWave * (effectHeight - 8) + yOffset + layerOffset);
                            
                            // Draw force field lines with varying thickness
                            int thickness = layer == 0 ? 8 : (layer == 1 ? 6 : 4);
                            for (int t = 0; t < thickness; t++)
                            {
                                int y = baseY + t;
                                int drawX = x + xOffset;
                                
                                // Apply rotation transformation
                                float localX = drawX - drawWidth / 2f;
                                float localY = y - drawHeight / 2f;
                                float rotatedX = localX * (float)Math.Cos(radians) - localY * (float)Math.Sin(radians);
                                float rotatedY = localX * (float)Math.Sin(radians) + localY * (float)Math.Cos(radians);
                                
                                int finalX = (int)(centerX + rotatedX);
                                int finalY = (int)(centerY + rotatedY);
                                
                                if (finalX >= 0 && finalX < Raylib.GetScreenWidth() && finalY >= 0 && finalY < Raylib.GetScreenHeight())
                                {
                                    Color parryColor;
                                    float alpha = layerIntensity * (1f - (float)t / thickness);
                                    
                                    if (layer == 0)
                                    {
                                        // Core energy - bright white/yellow
                                        if (t <= 2)
                                            parryColor = new Color(255, 255, 255, (int)(255 * alpha));
                                        else
                                            parryColor = new Color(255, 255, 150, (int)(200 * alpha));
                                    }
                                    else if (layer == 1)
                                    {
                                        // Mid layer - energy blue
                                        parryColor = new Color(100, 150, 255, (int)(180 * alpha));
                                    }
                                    else
                                    {
                                        // Outer layer - force field purple
                                        parryColor = new Color(150, 100, 255, (int)(120 * alpha));
                                    }
                                    
                                    Raylib.DrawPixel(finalX, finalY, parryColor);
                                }
                            }
                        }
                    }
                    
                    // Draw energy burst points along the curved arc to show force direction
                    for (int burst = 0; burst < 5; burst++)
                    {
                        float burstPos = (burst + 1) / 6f;
                        int burstX = (int)(burstPos * effectWidth + xOffset);
                        
                        // Calculate burst position using the same curved arc formula
                        float baseArc = (burstPos - 0.5f) * (burstPos - 0.5f) * 16f;  // Increased curve intensity to match
                        baseArc *= 0.8f;
                        
                        int burstY = (int)(baseArc * (effectHeight - 8) + yOffset);
                        
                        // Create small energy bursts
                        for (int i = 0; i < 3; i++)
                        {
                            int energyY = burstY - (i * 2) - 2;
                            
                            float localX = burstX - drawWidth / 2f;
                            float localY = energyY - drawHeight / 2f;
                            float rotatedX = localX * (float)Math.Cos(radians) - localY * (float)Math.Sin(radians);
                            float rotatedY = localX * (float)Math.Sin(radians) + localY * (float)Math.Cos(radians);
                            
                            int finalX = (int)(centerX + rotatedX);
                            int finalY = (int)(centerY + rotatedY);
                            
                            if (finalX >= 0 && finalX < Raylib.GetScreenWidth() && finalY >= 0 && finalY < Raylib.GetScreenHeight())
                            {
                                Color burstColor = new Color(255, 255, 255, 255 - (i * 80));
                                Raylib.DrawPixel(finalX, finalY, burstColor);
                            }
                        }
                    }
                },
                frameCount,
                frameDuration
            );
        }

        public static SpriteWrapper CreateAnimatedParrySprite(int frameCount = 24, float frameDuration = 0.05f, int width = 200, int height = 20)
        {
            return new SpriteWrapper(
                (position, rotation, drawWidth, drawHeight, scale, frame) =>
                {
                    float pulse = 1f + 0.2f * (float)Math.Sin((frame / (float)frameCount) * (float)Math.PI * 2);
                    float pushEffect = 1f + 0.3f * (float)Math.Sin((frame / (float)frameCount) * (float)Math.PI * 8); // Faster animation for pushing effect
                    
                    int arcHeight = (int)(height * scale * pulse);
                    int arcWidth = (int)(width * scale);
                    int yOffset = (int)(drawHeight / 2 - arcHeight / 2);
                    int xOffset = (int)(drawWidth / 2 - arcWidth / 2);
                    float radians = rotation * (float)Math.PI / 180f;
                    float centerX = position.X;
                    float centerY = position.Y;
                    
                    // --- MAIN SHIELD ARC --- (same as GenerateMagnetShieldSprite)
                    for (int x = 0; x < arcWidth; x++)
                    {
                        float progress = (float)x / arcWidth;
                        float curve = 1f - (progress - 0.5f) * (progress - 0.5f) * 4f;
                        int baseY = (int)(curve * (arcHeight - 6)) + yOffset;
                        
                        for (int thickness = 0; thickness < 6; thickness++)
                        {
                            int y = baseY + thickness;
                            int drawX = x + xOffset;
                            
                            // Apply rotation transformation
                            float localX = drawX - drawWidth / 2f;
                            float localY = y - drawHeight / 2f;
                            float rotatedX = localX * (float)Math.Cos(radians) - localY * (float)Math.Sin(radians);
                            float rotatedY = localX * (float)Math.Sin(radians) + localY * (float)Math.Cos(radians);
                            int finalX = (int)(centerX + rotatedX);
                            int finalY = (int)(centerY + rotatedY);
                            
                            if (finalX >= 0 && finalX < Raylib.GetScreenWidth() && finalY >= 0 && finalY < Raylib.GetScreenHeight())
                            {
                                Color shieldColor;
                                if (thickness <= 1)
                                    shieldColor = new Color(255, 255, 255, 255); // White core
                                else if (thickness <= 3)
                                    shieldColor = new Color(255, 100, 200, 255); // Magenta/pink
                                else
                                    shieldColor = new Color(200, 50, 150, 180); // Purple
                                
                                Raylib.DrawPixel(finalX, finalY, shieldColor);
                            }
                        }
                    }
                    
                    // --- EDGE PILLARS --- (same as GenerateMagnetShieldSprite but different colors)
                    for (int edge = 0; edge < 4; edge++)
                    {
                        for (int y = 0; y < arcHeight; y++)
                        {
                            float edgeIntensity = 1f - (float)y / arcHeight;
                            int alpha = (int)(200 * edgeIntensity);
                            Color leftEdge = new Color(255, 150, 200, alpha); // Pink edges
                            
                            // Left edge
                            int leftX = edge + xOffset;
                            int rightX = arcWidth - 1 - edge + xOffset;
                            int edgeY = y + yOffset;
                            
                            // Apply rotation for left edge
                            float localXLeft = leftX - drawWidth / 2f;
                            float localYLeft = edgeY - drawHeight / 2f;
                            float rotatedXLeft = localXLeft * (float)Math.Cos(radians) - localYLeft * (float)Math.Sin(radians);
                            float rotatedYLeft = localXLeft * (float)Math.Sin(radians) + localYLeft * (float)Math.Cos(radians);
                            int finalXLeft = (int)(centerX + rotatedXLeft);
                            int finalYLeft = (int)(centerY + rotatedYLeft);
                            
                            // Apply rotation for right edge
                            float localXRight = rightX - drawWidth / 2f;
                            float localYRight = edgeY - drawHeight / 2f;
                            float rotatedXRight = localXRight * (float)Math.Cos(radians) - localYRight * (float)Math.Sin(radians);
                            float rotatedYRight = localXRight * (float)Math.Sin(radians) + localYRight * (float)Math.Cos(radians);
                            int finalXRight = (int)(centerX + rotatedXRight);
                            int finalYRight = (int)(centerY + rotatedYRight);
                            
                            if (finalXLeft >= 0 && finalXLeft < Raylib.GetScreenWidth() && finalYLeft >= 0 && finalYLeft < Raylib.GetScreenHeight())
                                Raylib.DrawPixel(finalXLeft, finalYLeft, leftEdge);
                            if (finalXRight >= 0 && finalXRight < Raylib.GetScreenWidth() && finalYRight >= 0 && finalYRight < Raylib.GetScreenHeight())
                                Raylib.DrawPixel(finalXRight, finalYRight, leftEdge);
                        }
                    }
                    
                    // --- ENERGY NODES --- (same as GenerateMagnetShieldSprite but different colors)
                    for (int node = 1; node <= 3; node++)
                    {
                        int nodeX = (arcWidth / 4) * node + xOffset;
                        float nodeProgress = (float)(nodeX - xOffset) / arcWidth;
                        float nodeCurve = 1f - (nodeProgress - 0.5f) * (nodeProgress - 0.5f) * 4f;
                        int nodeBaseY = (int)(nodeCurve * (arcHeight - 6)) + yOffset;
                        if (nodeBaseY + 1 < arcHeight + yOffset)
                        {
                            // Apply rotation
                            float localX = nodeX - drawWidth / 2f;
                            float localY = (nodeBaseY + 1) - drawHeight / 2f;
                            float rotatedX = localX * (float)Math.Cos(radians) - localY * (float)Math.Sin(radians);
                            float rotatedY = localX * (float)Math.Sin(radians) + localY * (float)Math.Cos(radians);
                            int finalX = (int)(centerX + rotatedX);
                            int finalY = (int)(centerY + rotatedY);
                        
                            if (finalX >= 0 && finalX < Raylib.GetScreenWidth() && finalY >= 0 && finalY < Raylib.GetScreenHeight())
                                Raylib.DrawPixel(finalX, finalY, new Color(255, 255, 255, 255));
                        }
                    }
                    
                    // --- OUTWARD PUSH EFFECT --- (unique to parry shield)
                    for (int burst = 0; burst < 8; burst++)
                    {
                        float burstAngle = (burst / 8f) * (float)Math.PI * 2;
                        float burstIntensity = (float)Math.Sin((frame / (float)frameCount) * (float)Math.PI * 6 + burst) * 0.5f + 0.5f;
                        
                        if (burstIntensity > 0.6f) // Only show strong bursts
                        {
                            int burstLength = (int)(25 * burstIntensity * pushEffect);
                            for (int i = 5; i < burstLength; i++) // Start further out for outward push effect
                            {
                                float burstX = centerX + (float)Math.Cos(burstAngle) * i * 1.5f;
                                float burstY = centerY + (float)Math.Sin(burstAngle) * i * 1.5f;
                                
                                if (burstX >= 0 && burstX < Raylib.GetScreenWidth() && burstY >= 0 && burstY < Raylib.GetScreenHeight())
                                {
                                    int alpha = (int)(255 * burstIntensity * (1f - (float)(i - 5) / (burstLength - 5)));
                                    Color burstColor = new Color(255, 200, 255, alpha); // Bright magenta bursts
                                    Raylib.DrawPixel((int)burstX, (int)burstY, burstColor);
                                }
                            }
                        }
                    }
                },
                frameCount,
                frameDuration
            );
        }
    }
}