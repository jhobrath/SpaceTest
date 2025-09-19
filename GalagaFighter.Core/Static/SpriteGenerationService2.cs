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

        public static SpriteWrapper CreateBulletShieldEffect(int frameCount = 20, float frameDuration = 0.06f, int width = 200, int height = 20)
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

        private static Color GetAverageBubbleColor()
        {
            return new Color(180, 200, 255, 24); // average values
        }

        private static Color GetAverageSoapColor()
        {
            return new Color(160, 180, 255, 120); // average values
        }

        private static Color GetAverageHighlightColor()
        {
            return new Color(220, 230, 255, 80); // average values
        }


        // Bubble wobble animation for realistic floating motion
        private static float _wobbleFrequency1 = .8f;  // average value
        private static float _wobbleFrequency2 = 1.6f;  // average value 
        private static float _wobbleAmplitude = 1.3f;   // average value
        private static float _finalBubbleRadius = 35f; // average value

        public static SpriteWrapper CreatePoisonFormationSprite(int frameCount = 20, float frameDuration = 0.017f)
        {
            string key = $"PoisonFormation_{frameCount}";
            if (TextureService.TryGetFromKey(key, out Texture2D texture))
                return new SpriteWrapper(texture, frameCount, frameDuration);

            int frameWidth = 100;
            int frameHeight = 100;
            int textureWidth = frameWidth * frameCount;
            int textureHeight = frameHeight;

            var directionMultiplier = 1f; // Mirror for Player 2

            // Use average bubble colors
            var bubbleColor = GetAverageBubbleColor();
            var soapColor = GetAverageSoapColor();
            var highlightColor = GetAverageHighlightColor();

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(textureWidth, textureHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            Raylib.BeginBlendMode(BlendMode.AlphaPremultiply); 

            for (int frame = 0; frame < frameCount; frame++)
            {
                int xOffset = frame * frameWidth;
                //Raylib.DrawCircle(xOffset + 50, 50, 20, Color.White);
                DrawFormation(xOffset, frame, bubbleColor, soapColor, highlightColor, directionMultiplier);
                // TODO: Call PoisonProjectile.DrawFormationFrame(frame, frameCount, xOffset, frameHeight)
                // This method should draw the formation animation for the given frame at (xOffset, 0)
            }

            Raylib.EndBlendMode();
            Raylib.EndTextureMode();
            TextureService.Set(key, renderTexture.Texture);
            return new SpriteWrapper(renderTexture.Texture, frameCount, frameDuration);
        }


        private static void DrawFormation(float xOffset, int frame, Color bubbleColor, Color soapColor, Color highlightColor, float directionMultiplier)
        {
            var progress = frame / 20f;

            // Stage 1 (0.0 - 0.3): Semi-circle formation connected to prongs
            if (progress <= 0.3f)
            {
                float stageProgress = progress / 0.3f;
                DrawFormationSemiCircleStage(xOffset, stageProgress, soapColor, highlightColor, directionMultiplier);
            }
            // Stage 2 (0.3 - 0.7): Semi-ellipse growth (vertical grows 2x faster than horizontal)
            else if (progress <= 0.7f)
            {
                float stageProgress = (progress - 0.3f) / 0.4f;
                DrawFormationSemiEllipseStage(xOffset, stageProgress, soapColor, bubbleColor, highlightColor, directionMultiplier);
            }
            // Stage 3 (0.7 - 1.0): Detachment and sphere formation
            else
            {
                float stageProgress = (progress - 0.7f) / 0.3f;
                DrawFormationSphereDetachmentStage(xOffset, stageProgress, bubbleColor, soapColor, highlightColor, directionMultiplier);
            }
        }

        private static void DrawFormationSemiCircleStage(float xOffset, float progress, Color soapColor, Color highlightColor, float directionMultiplier)
        {
            var centerPoint = new Vector2(12.78f + xOffset, 50f);
            var topProng = new Vector2(12.78f + xOffset, 50f - 9.89f);
            var bottomProng = new Vector2(12.78f + xOffset, 50f + 9.89f);

            float prongDistance = Vector2.Distance(topProng, bottomProng);


            // Start with a small semi-circle that grows
            float initialRadius = prongDistance / 3f; // Start small relative to prong distance
            float currentRadius = initialRadius * progress;

            // Draw the semi-circle connected to prongs (mirrored for Player 2)
            Vector2 bubbleCenter = centerPoint + new Vector2(currentRadius * directionMultiplier, 0f);

            // Calculate subtle wobble for realistic bubble motion with randomization
            float wobbleIntensity = progress * (0.25f + (float)Game.Random.NextDouble() * 0.1f); // 0.25-0.35 variation

            // Randomize segment count for organic variety
            int segments = 14 + Game.Random.Next(4); // 14-17 segments

            // Draw the curved part of the semi-circle with subtle wobble
            for (int i = 0; i < segments; i++)
            {
                // Draw the appropriate half based on player direction
                float startAngle = directionMultiplier > 0 ? -0.5f : 0.5f; // Right half for P1, left half for P2
                float angle1 = (startAngle + (i / (float)segments) * directionMultiplier) * (float)Math.PI;
                float angle2 = (startAngle + ((i + 1) / (float)segments) * directionMultiplier) * (float)Math.PI;

                // Apply subtle wobble with randomized phase shifts
                float randomPhase = (float)(Game.Random.NextDouble() * Math.PI * 2);
                float wobble1 = (float)Math.Sin(progress * _wobbleFrequency1 * Math.PI * 2 + angle1 * 3 + randomPhase) * _wobbleAmplitude * wobbleIntensity;
                float wobble2 = (float)Math.Sin(progress * _wobbleFrequency2 * Math.PI * 2 + angle2 * 3 + randomPhase) * _wobbleAmplitude * wobbleIntensity;

                Vector2 point1 = bubbleCenter + new Vector2(
                    (float)Math.Cos(angle1) * (currentRadius + wobble1),
                    (float)Math.Sin(angle1) * (currentRadius + wobble1)
                );
                Vector2 point2 = bubbleCenter + new Vector2(
                    (float)Math.Cos(angle2) * (currentRadius + wobble2),
                    (float)Math.Sin(angle2) * (currentRadius + wobble2)
                );

                Raylib.DrawLineEx(point1, point2, 2f, soapColor);
            }

            // Draw transparent soap film connections
            Vector2 topConnection = bubbleCenter + new Vector2(0, -currentRadius);
            Vector2 bottomConnection = bubbleCenter + new Vector2(0, currentRadius);

            Raylib.DrawLineEx(topProng, topConnection, 2f, soapColor);
            Raylib.DrawLineEx(bottomProng, bottomConnection, 2f, soapColor);

            // Add subtle soap film highlights with randomized timing
            float highlightThreshold = 0.4f + (float)Game.Random.NextDouble() * 0.2f; // 0.4-0.6 random threshold
            if (progress > highlightThreshold)
            {
                float dropletSize = 1.2f + (float)Game.Random.NextDouble() * 0.6f; // 1.2-1.8 size variation
                Raylib.DrawCircle((int)topConnection.X, (int)topConnection.Y, dropletSize, highlightColor);
                Raylib.DrawCircle((int)bottomConnection.X, (int)bottomConnection.Y, dropletSize, highlightColor);
            }
        }

        private static void DrawFormationSemiEllipseStage(float xOffset, float progress, Color soapColor, Color bubbleColor, Color highlightColor, float directionMultiplier)
        {
            var prongDistance = 9.89f * 2f; //Each prong is 12.78 pixels from center
            var centerPoint = new Vector2(12.78f + xOffset, 50f);
            var topProng = new Vector2(12.78f + xOffset, 50f - 9.89f);
            var bottomProng = new Vector2(12.78f + xOffset, 50f + 9.89f);

            float initialRadius = prongDistance / 3f;

            // Make the ellipse grow with slight randomization
            float horizontalGrowthRate = 0.55f + (float)Game.Random.NextDouble() * 0.1f; // 0.55-0.65 variation
            float verticalGrowthRate = 0.85f + (float)Game.Random.NextDouble() * 0.1f; // 0.85-0.95 variation

            float horizontalRadius = initialRadius + ((_finalBubbleRadius - initialRadius) * horizontalGrowthRate * progress);
            float maxVerticalGrowth = _finalBubbleRadius * verticalGrowthRate - initialRadius;
            float verticalRadius = initialRadius + (maxVerticalGrowth * progress);

            // Ellipse center follows player direction
            Vector2 ellipseCenter = centerPoint + new Vector2(horizontalRadius * directionMultiplier, 0f);

            // Calculate wobble intensity with randomization
            float wobbleIntensity = progress * (0.45f + (float)Game.Random.NextDouble() * 0.1f); // 0.45-0.55 variation

            // Randomize segment count for organic ellipse variation
            int segments = 18 + Game.Random.Next(4); // 18-21 segments

            // Draw the transparent semi-ellipse with randomized wobble
            for (int i = 0; i < segments; i++)
            {
                // Draw the appropriate half based on player direction
                float startAngle = directionMultiplier > 0 ? -0.5f : 0.5f; // Right half for P1, left half for P2
                float angle1 = (startAngle + (i / (float)segments) * directionMultiplier) * (float)Math.PI;
                float angle2 = (startAngle + ((i + 1) / (float)segments) * directionMultiplier) * (float)Math.PI;

                // Apply different wobble patterns with randomized complexity
                float randomPhase1 = (float)(Game.Random.NextDouble() * Math.PI);
                float randomPhase2 = (float)(Game.Random.NextDouble() * Math.PI);

                float wobbleH1 = (float)Math.Sin(progress * _wobbleFrequency1 * Math.PI * 2 + angle1 * 2 + randomPhase1) * _wobbleAmplitude * wobbleIntensity;
                float wobbleV1 = (float)Math.Sin(progress * _wobbleFrequency2 * Math.PI * 2 + angle1 * 4 + randomPhase2) * _wobbleAmplitude * wobbleIntensity * 0.7f;
                float wobbleH2 = (float)Math.Sin(progress * _wobbleFrequency1 * Math.PI * 2 + angle2 * 2 + randomPhase1) * _wobbleAmplitude * wobbleIntensity;
                float wobbleV2 = (float)Math.Sin(progress * _wobbleFrequency2 * Math.PI * 2 + angle2 * 4 + randomPhase2) * _wobbleAmplitude * wobbleIntensity * 0.7f;

                Vector2 point1 = ellipseCenter + new Vector2(
                    (float)Math.Cos(angle1) * (horizontalRadius + wobbleH1),
                    (float)Math.Sin(angle1) * (verticalRadius + wobbleV1)
                );
                Vector2 point2 = ellipseCenter + new Vector2(
                    (float)Math.Cos(angle2) * (horizontalRadius + wobbleH2),
                    (float)Math.Sin(angle2) * (verticalRadius + wobbleV2)
                );

                Raylib.DrawLineEx(point1, point2, 2f, soapColor);
            }

            // Draw transparent soap film connections
            Vector2 topConnection = ellipseCenter + new Vector2(0, -verticalRadius);
            Vector2 bottomConnection = ellipseCenter + new Vector2(0, verticalRadius);

            Raylib.DrawLineEx(topProng, topConnection, 2f, soapColor);
            Raylib.DrawLineEx(bottomProng, bottomConnection, 2f, soapColor);

            // Fill with randomized transparency timing
            float fillThreshold = 0.25f + (float)Game.Random.NextDouble() * 0.1f; // 0.25-0.35 random start
            if (progress > fillThreshold)
            {
                float fillProgress = (progress - fillThreshold) / (1f - fillThreshold);
                float transparencyVariation = 0.5f + (float)Game.Random.NextDouble() * 0.2f; // 0.5-0.7 variation
                Color fillColor = new(
                    bubbleColor.R,
                    bubbleColor.G,
                    bubbleColor.B,
                    (byte)(bubbleColor.A * fillProgress * transparencyVariation)
                );

                // Draw filled semi-ellipse with randomized density, considering direction
                int stepSize = 2 + Game.Random.Next(2); // 2-3 pixel steps for variation
                float fillStart = directionMultiplier > 0 ? 0 : -horizontalRadius;
                float fillEnd = directionMultiplier > 0 ? horizontalRadius : 0;

                for (float x = fillStart; (directionMultiplier > 0 ? x <= fillEnd : x >= fillEnd); x += stepSize * directionMultiplier)
                {
                    float normalizedX = Math.Abs(x) / horizontalRadius;
                    if (normalizedX <= 1f)
                    {
                        float y = verticalRadius * (float)Math.Sin(Math.Acos(normalizedX));
                        Vector2 fillPoint = ellipseCenter + new Vector2(x, 0);
                        Raylib.DrawRectangle((int)fillPoint.X, (int)(fillPoint.Y - y), stepSize, (int)(y * 2), fillColor);
                    }
                }

                // Add randomized highlights during growth
                float highlightThreshold = 0.55f + (float)Game.Random.NextDouble() * 0.1f; // 0.55-0.65
                if (progress > highlightThreshold)
                {
                    float highlightScale = 0.25f + (float)Game.Random.NextDouble() * 0.1f; // 0.25-0.35 variation
                    float highlightRadius = Math.Min(horizontalRadius, verticalRadius) * highlightScale;
                    float offsetX = (-6f + (float)Game.Random.NextDouble() * 2f) * directionMultiplier; // Mirror offset for Player 2
                    float offsetY = -4f + (float)Game.Random.NextDouble() * 2f; // -4 to -2
                    Raylib.DrawCircleLines((int)(ellipseCenter.X + offsetX), (int)(ellipseCenter.Y + offsetY), highlightRadius, highlightColor);
                }
            }
        }

        private static void DrawFormationSphereDetachmentStage(float xOffset, float progress, Color bubbleColor, Color soapColor, Color highlightColor, float directionMultiplier)
        {
            var centerPoint = new Vector2(12.78f + xOffset, 50f);
            var topProng = new Vector2(12.78f + xOffset, 50f - 9.89f);
            var bottomProng = new Vector2(12.78f + xOffset, 50f + 9.89f);


            // Calculate the final bubble position (detached from prongs) with player direction
            Vector2 finalBubbleCenter = centerPoint + new Vector2(_finalBubbleRadius * directionMultiplier, 0f);

            // Calculate the starting values from the end of stage 2
            float prongDistance = Vector2.Distance(topProng, bottomProng);
            float initialRadius = prongDistance / 3f;

            // The ellipse now grows to 90% of final size with randomization
            float maxVerticalGrowth = _finalBubbleRadius * (0.85f + (float)Game.Random.NextDouble() * 0.1f) - initialRadius; // 0.85-0.95 variation
            float maxVerticalRadius = initialRadius + maxVerticalGrowth;
            float maxHorizontalRadius = initialRadius + ((_finalBubbleRadius - initialRadius) * (0.55f + (float)Game.Random.NextDouble() * 0.1f)); // 0.55-0.65 variation

            Vector2 startCenter = centerPoint + new Vector2(maxHorizontalRadius * directionMultiplier, 0f);
            Vector2 currentCenter = Vector2.Lerp(startCenter, finalBubbleCenter, progress);
            float currentRadius = maxVerticalRadius + (_finalBubbleRadius - maxVerticalRadius) * progress;

            // Maximum wobble with randomization during final formation
            float wobbleIntensity = (0.75f + (float)Game.Random.NextDouble() * 0.25f) + progress * 0.2f; // 0.75-1.0 base + growth

            // Randomized wobble complexity for unique sphere ripples
            int wobbleSegments = 30 + Game.Random.Next(6); // 30-35 segments for variation
            List<Vector2> wobbledPoints = [];

            for (int i = 0; i < wobbleSegments; i++)
            {
                float angle = (i / (float)wobbleSegments) * (float)Math.PI * 2f;

                // Multiple sine waves with randomized phases for complex, unique bubble surface ripples
                float randomPhase1 = (float)(Game.Random.NextDouble() * Math.PI * 2);
                float randomPhase2 = (float)(Game.Random.NextDouble() * Math.PI * 2);

                float wobble1 = (float)Math.Sin(progress * _wobbleFrequency1 * Math.PI * 2 + angle * 4 + randomPhase1) * _wobbleAmplitude * wobbleIntensity;
                float wobble2 = (float)Math.Sin(progress * _wobbleFrequency2 * Math.PI * 2 + angle * 6 + randomPhase2) * _wobbleAmplitude * wobbleIntensity * 0.6f;
                float totalWobble = wobble1 + wobble2;

                Vector2 wobbledPoint = currentCenter + new Vector2(
                    (float)Math.Cos(angle) * (currentRadius + totalWobble),
                    (float)Math.Sin(angle) * (currentRadius + totalWobble)
                );
                wobbledPoints.Add(wobbledPoint);
            }

            // Draw the filled circle with randomized transparency
            float fillAlpha = 1f + (float)Game.Random.NextDouble() * 0.1f; // 1.0-1.1 slight variation
            Color randomizedBubbleColor = new(bubbleColor.R, bubbleColor.G, bubbleColor.B, (byte)(bubbleColor.A * fillAlpha));
            Raylib.DrawCircle((int)currentCenter.X, (int)currentCenter.Y, currentRadius, randomizedBubbleColor);

            // Draw the wobbled outline
            for (int i = 0; i < wobbledPoints.Count; i++)
            {
                Vector2 point1 = wobbledPoints[i];
                Vector2 point2 = wobbledPoints[(i + 1) % wobbledPoints.Count];
                Raylib.DrawLineEx(point1, point2, 2f, soapColor);
            }

            // Draw diminishing transparent connections with randomized strength
            float connectionStrength = (1f - progress) * (0.9f + (float)Game.Random.NextDouble() * 0.2f); // 0.9-1.1 variation
            if (connectionStrength > 0)
            {
                Color fadingSoapColor = new(
                    soapColor.R,
                    soapColor.G,
                    soapColor.B,
                    (byte)(soapColor.A * connectionStrength)
                );

                // Connection points with slight randomization
                Vector2 topDirection = Vector2.Normalize(topProng - currentCenter);
                Vector2 bottomDirection = Vector2.Normalize(bottomProng - currentCenter);

                float connectionVariation = 0.9f + (float)Game.Random.NextDouble() * 0.2f; // 0.9-1.1 variation
                Vector2 topConnectionPoint = currentCenter + topDirection * currentRadius * connectionVariation;
                Vector2 bottomConnectionPoint = currentCenter + bottomDirection * currentRadius * connectionVariation;

                float lineThickness = 2f * connectionStrength * (0.8f + (float)Game.Random.NextDouble() * 0.4f); // 0.8-1.2 variation
                Raylib.DrawLineEx(topProng, topConnectionPoint, lineThickness, fadingSoapColor);
                Raylib.DrawLineEx(bottomProng, bottomConnectionPoint, lineThickness, fadingSoapColor);
            }

            // Add realistic soap film highlights with randomization and player direction
            if (progress > 0.3f)
            {
                float highlightProgress = (progress - 0.3f) / 0.7f;
                float highlightRadius = currentRadius * (0.65f + (float)Game.Random.NextDouble() * 0.1f); // 0.65-0.75 variation

                // Main highlight with randomized wobble and position (mirrored for Player 2)
                float highlightOffsetX = (-8f + (float)Game.Random.NextDouble() * 3f) * directionMultiplier; // Mirror offset for Player 2
                float highlightOffsetY = -8f + (float)Game.Random.NextDouble() * 3f; // -8 to -5
                float highlightWobble = (float)Math.Sin(progress * _wobbleFrequency1 * Math.PI) * (1.5f + (float)Game.Random.NextDouble() * 1f); // 1.5-2.5 wobble
                Raylib.DrawCircleLines((int)(currentCenter.X + highlightOffsetX + highlightWobble * directionMultiplier), (int)(currentCenter.Y + highlightOffsetY), highlightRadius * 0.3f,
                    new Color(highlightColor.R, highlightColor.G, highlightColor.B, (byte)(highlightColor.A * highlightProgress)));

                // Secondary smaller highlight with different randomization
                if (progress > 0.6f)
                {
                    float secOffsetX = (-12f + (float)Game.Random.NextDouble() * 2f) * directionMultiplier; // Mirror offset for Player 2
                    float secOffsetY = -5f + (float)Game.Random.NextDouble() * 2f; // -5 to -3
                    float secondaryWobble = (float)Math.Sin(progress * _wobbleFrequency2 * Math.PI) * (1f + (float)Game.Random.NextDouble() * 1f); // 1-2 wobble
                    float secHighlightRadius = highlightRadius * (0.1f + (float)Game.Random.NextDouble() * 0.1f); // 0.1-0.2 size variation
                    Raylib.DrawCircleLines((int)(currentCenter.X + secOffsetX + secondaryWobble * directionMultiplier), (int)(currentCenter.Y + secOffsetY), secHighlightRadius,
                        new Color((byte)255, (byte)255, (byte)255, (byte)(30 + Game.Random.Next(20) * highlightProgress))); // 30-49 alpha variation
                }
            }
        }

    }
}