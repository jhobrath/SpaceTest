using Raylib_cs;
using System;
using System.Numerics;
using System.Collections.Generic;
using GalagaFighter.Core.Services;

namespace GalagaFighter.Core.Static
{
    public static class SpriteGenerationService3
    {
        internal static SpriteWrapper CreatePhaseShifter()
        {
            int frameCount = 30;
            int frameWidth = 160;
            int frameHeight = 30;
            float frameDuration = 0.5f / frameCount;
            int textureWidth = frameCount * frameWidth;
            int textureHeight = frameHeight;
            string key = "PhaseShifter";

            if (TextureService.TryGetFromKey(key, out Texture2D cachedTexture))
               return new SpriteWrapper(cachedTexture, frameCount, frameDuration);

            // Gradient colors
            Color[] gradientColors = new[] {
                new Color(0, 120, 255, 255), // blue
                new Color(0, 255, 255, 255), // cyan
                new Color(255, 255, 255, 255) // white
            };

            int waveLines = 4;
            float[] animFrequencies = new float[] { 1f, 2f, 3f, 4f };
            float[] spatialFrequencies = new float[] { 0.08f, 0.10f, 0.13f, 0.16f };
            float[] phaseOffsets = new float[waveLines];
            float[] ampFrequencies = new float[] { 1f, 2f, 3f, 4f }; // Integer for perfect looping
            float[] ampPhaseOffsets = new float[waveLines];
            for (int i = 0; i < waveLines; i++)
            {
                phaseOffsets[i] = i * MathF.PI / 2f;
                ampPhaseOffsets[i] = i * MathF.PI / waveLines;
            }

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(textureWidth, textureHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);

            for (int frame = 0; frame < frameCount; frame++)
            {
                int xOffset = frame * frameWidth;
                float t = frame / (float)(frameCount - 1); // t goes from 0 to 1, inclusive
                float waveYCenter = frameHeight / 2f; // Always 15
                int wavePoints = 32;
                // Poles
                int anodeX = xOffset + 10;
                int anodeY = frameHeight / 2;
                int diodeX = xOffset + frameWidth - 10;
                int diodeY = frameHeight / 2;
                int poleWidth = 8;
                int poleRadius = 4;
                Color steelTop = new Color(210, 210, 230, 255);
                Color steelBottom = new Color(140, 140, 160, 255);
                // Linear growing/shrinking heights (swapped)
                int minPoleHeight = 12;
                int maxPoleHeight = 20;
                int startPoleHeight = 16; // starting height for both
                int anodePoleHeight = (int)(startPoleHeight + (maxPoleHeight - startPoleHeight) * t); // grow, always >= start
                int diodePoleHeight = (int)(startPoleHeight - (startPoleHeight - minPoleHeight) * t); // shrink, always <= start
                int anodePoleTop = anodeY - anodePoleHeight / 2;
                int diodePoleTop = diodeY - diodePoleHeight / 2;
                // Draw anode pole with vertical gradient and rounded ends
                for (int y = 0; y < anodePoleHeight; y++)
                {
                    float gradT = y / (float)anodePoleHeight;
                    Color gradColor = InterpolateGradient(new[] { steelTop, steelBottom }, gradT);
                    Raylib.DrawRectangle(anodeX - poleWidth / 2, anodePoleTop + y, poleWidth, 1, gradColor);
                }
                Raylib.DrawCircle(anodeX, anodePoleTop, poleRadius, steelTop);
                Raylib.DrawCircle(anodeX, anodePoleTop + anodePoleHeight, poleRadius, steelBottom);
                // Shimmer effect: animated white highlight
                int shimmerWidth = 2;
                int shimmerX = anodeX - poleWidth / 2 + (int)(MathF.Sin(t * 2 * MathF.PI) * 2);
                Raylib.DrawRectangle(shimmerX, anodePoleTop + 3, shimmerWidth, anodePoleHeight - 6, new Color(255,255,255,120));
                // Draw diode pole with vertical gradient and rounded ends
                for (int y = 0; y < diodePoleHeight; y++)
                {
                    float gradT = y / (float)diodePoleHeight;
                    Color gradColor = InterpolateGradient(new[] { steelTop, steelBottom }, gradT);
                    Raylib.DrawRectangle(diodeX - poleWidth / 2, diodePoleTop + y, poleWidth, 1, gradColor);
                }
                Raylib.DrawCircle(diodeX, diodePoleTop, poleRadius, steelTop);
                Raylib.DrawCircle(diodeX, diodePoleTop + diodePoleHeight, poleRadius, steelBottom);
                shimmerX = diodeX - poleWidth / 2 + (int)(MathF.Sin(t * 2 * MathF.PI + MathF.PI) * 2);
                Raylib.DrawRectangle(shimmerX, diodePoleTop + 3, shimmerWidth, diodePoleHeight - 6, new Color(255,255,255,120));
                // Extend waves to center of poles
                float waveStartX = anodeX;
                float waveEndX = diodeX;

                // Draw multiple oscillating wave lines with gradients
                for (int line = 0; line < waveLines; line++)
                {
                    float animFreq = animFrequencies[line];
                    float spatialFreq = spatialFrequencies[line];
                    float phaseOffset = phaseOffsets[line];
                    float ampFreq = ampFrequencies[line];
                    float ampPhase = ampPhaseOffsets[line];
                    float baseAmplitude = frameHeight * (0.3f - line * 0.05f);
                    float amplitude = baseAmplitude * (1f + 0.3f * MathF.Sin(t * 2 * MathF.PI * ampFreq + ampPhase));
                    // Remove verticalOffset so all waves are centered at y=15
                    for (int i = 0; i < wavePoints - 1; i++)
                    {
                        float x0 = waveStartX + (waveEndX - waveStartX) * (i / (float)(wavePoints - 1));
                        float x1 = waveStartX + (waveEndX - waveStartX) * ((i + 1) / (float)(wavePoints - 1));
                        float phase0 = t * 2 * MathF.PI * animFreq + x0 * spatialFreq + phaseOffset;
                        float phase1 = t * 2 * MathF.PI * animFreq + x1 * spatialFreq + phaseOffset;
                        float y0 = waveYCenter + MathF.Sin(phase0) * amplitude;
                        float y1 = waveYCenter + MathF.Sin(phase1) * amplitude;
                        // Gradient color
                        float gradT = i / (float)(wavePoints - 1);
                        Color gradColor = InterpolateGradient(gradientColors, gradT);
                        Raylib.DrawLine((int)(xOffset + x0), (int)(y0), (int)(xOffset + x1), (int)(y1), gradColor);
                    }
                }
            }

            Raylib.EndTextureMode();
            Texture2D texture = renderTexture.Texture;
            TextureService.Set(key, texture);
            return new SpriteWrapper(texture, frameCount, frameDuration);
        }

        internal static SpriteWrapper CreatePhaseShifterOverlay()
        {
            int texWidth = 160;
            int texHeight = 160;
            int frameCount = 30;
            float frameDuration = 0.5f / frameCount;
            var waveColor = Color.White;//Colored by tint
            string key = $"PhaseShifterOverlay";

            if (TextureService.TryGetFromKey(key, out Texture2D existingTexture))
                return new SpriteWrapper(existingTexture, frameCount, frameDuration);

            // Use actual Player hitbox vertices as percentages, but flip Y coordinates
            Vector2 vTip = new Vector2(0.5f * texWidth, (1.0f - 0.08f) * texHeight);      // Ship tip: 50% in, 92% down (flipped)
            Vector2 vLeft = new Vector2(0.045f * texWidth, (1.0f - 0.685f) * texHeight);  // Left wing: 4.5% in, 31.5% down (flipped)
            Vector2 vRight = new Vector2(0.955f * texWidth, (1.0f - 0.685f) * texHeight); // Right wing: 95.5% in, 31.5% down (flipped)


            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(texWidth * frameCount, texHeight);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            Raylib.BeginBlendMode(BlendMode.AlphaPremultiply);
            for (int frame = 0; frame < frameCount; frame++)
            {
                int xOffset = frame * texWidth;
                float t = frame / (float)frameCount;
                // Wavy effect parameters
                int lines = 48;
                float waveAmplitude = 8f + 6f * MathF.Sin(t * 2 * MathF.PI);
                float waveFrequency = 3.5f;
                float phase = t * 2 * MathF.PI;
                
                for (int i = 0; i < lines; i++)
                {
                    float pct = i / (float)(lines - 1);
                    // Interpolate from tip to base (left wing to right wing)
                    Vector2 left = Vector2.Lerp(vTip, vLeft, pct);
                    Vector2 right = Vector2.Lerp(vTip, vRight, pct);
                    
                    // Draw a wavy line between left and right
                    int segments = 32;
                    for (int s = 0; s < segments - 1; s++)
                    {
                        float segPct0 = s / (float)(segments - 1);
                        float segPct1 = (s + 1) / (float)(segments - 1);
                        Vector2 p0 = Vector2.Lerp(left, right, segPct0);
                        Vector2 p1 = Vector2.Lerp(left, right, segPct1);
                        // Modulate y with sine wave
                        float wave0 = MathF.Sin(phase + segPct0 * waveFrequency * 2 * MathF.PI + pct * MathF.PI) * waveAmplitude * pct;
                        float wave1 = MathF.Sin(phase + segPct1 * waveFrequency * 2 * MathF.PI + pct * MathF.PI) * waveAmplitude * pct;
                        p0.Y += wave0;
                        p1.Y += wave1;
                        Raylib.DrawLine((int)(xOffset + p0.X), (int)p0.Y, (int)(xOffset + p1.X), (int)p1.Y, waveColor);
                    }
                }
            }
            Raylib.EndBlendMode();
            Raylib.EndTextureMode();
            Texture2D texture = renderTexture.Texture;
            TextureService.Set(key, texture);
            return new SpriteWrapper(texture, frameCount, frameDuration);
        }

        // Helper for color gradient interpolation
        private static Color InterpolateGradient(Color[] colors, float t)
        {
            if (colors.Length == 1) return colors[0];
            float scaled = t * (colors.Length - 1);
            int idx = (int)scaled;
            float localT = scaled - idx;
            if (idx >= colors.Length - 1) return colors[colors.Length - 1];
            Color c0 = colors[idx];
            Color c1 = colors[idx + 1];
            return new Color(
                (byte)(c0.R + (c1.R - c0.R) * localT),
                (byte)(c0.G + (c1.G - c0.G) * localT),
                (byte)(c0.B + (c1.B - c0.B) * localT),
                (byte)(c0.A + (c1.A - c0.A) * localT)
            );
        }
    }
}