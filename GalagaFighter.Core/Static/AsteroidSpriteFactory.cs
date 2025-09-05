using System;
using GalagaFighter.Core.Models.Debris;
using Raylib_cs;
using System.Numerics;
using GalagaFighter.Core.Services;

namespace GalagaFighter.Core.Static
{
    public static class AsteroidSpriteFactory
    {
        private static readonly string[] AsteroidTextures = new[]
        {
            "Sprites/Debris/asteroid_1.png",
            "Sprites/Debris/asteroid_2.png",
            "Sprites/Debris/asteroid_3.png",
            "Sprites/Debris/asteroid_4.png",
            "Sprites/Debris/asteroid_5.png"
        };

        // Creates a procedurally masked asteroid sprite from a random PNG
        public static SpriteWrapper CreateProceduralAsteroidSprite(int minVertices = 8, int maxVertices = 16, float irregularity = 0.5f)
        {
            var randomIndex = Game.Random.Next(AsteroidTextures.Length);
            var texturePath = AsteroidTextures[randomIndex];
            Image baseImage = Raylib.LoadImage(texturePath);
            int width = baseImage.Width;
            int height = baseImage.Height;

            // Generate random polygon mask with more irregularity
            Vector2 center = new Vector2(width / 2f, height / 2f);
            float radius = Math.Min(width, height) * 0.45f;
            int vertexCount = Game.Random.Next(minVertices, maxVertices + 1);
            Vector2[] vertices = new Vector2[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                float angle = (float)(i * 2 * Math.PI / vertexCount);
                float r = radius * (1f + (float)(Game.Random.NextDouble() * irregularity - irregularity / 2));
                vertices[i] = new Vector2(
                    center.X + r * (float)Math.Cos(angle),
                    center.Y + r * (float)Math.Sin(angle)
                );
            }

            // Prepare mask array
            bool[] mask = new bool[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = y * width + x;
                    mask[idx] = PointInPolygon(x + 0.5f, y + 0.5f, vertices); // Use pixel center for accuracy
                }
            }

            // Roughen the mask edges
            Random rand = Game.Random;
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int idx = y * width + x;
                    if (mask[idx] && IsEdgePixel(mask, x, y, width, height))
                    {
                        // Randomly shrink or grow the mask at the edge
                        if (rand.NextDouble() < 0.25) // 25% chance to shrink
                        {
                            mask[idx] = false;
                        }
                        else if (rand.NextDouble() < 0.15) // 15% chance to grow
                        {
                            // Try to grow to a neighbor
                            int dx = rand.Next(-1, 2);
                            int dy = rand.Next(-1, 2);
                            int nidx = (y + dy) * width + (x + dx);
                            if (!mask[nidx]) mask[nidx] = true;
                        }
                    }
                }
            }

            // Create a fully transparent image to draw on
            Image asteroidImage = Raylib.GenImageColor(width, height, Color.Blank);

            // Copy pixels from base image if inside mask, apply edge roughness and dithering
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = y * width + x;
                    if (mask[idx])
                    {
                        Color pixel = Raylib.GetImageColor(baseImage, x, y);
                        if (IsEdgePixel(mask, x, y, width, height))
                        {
                            // Dither: randomly set some edge pixels to partial alpha
                            if (rand.NextDouble() < 0.2)
                                pixel.A = (byte)(pixel.A * 0.5);
                            pixel = Darken(pixel, 0.7f);
                        }
                        Raylib.ImageDrawPixel(ref asteroidImage, x, y, pixel);
                    }
                    // else: leave transparent
                }
            }

            Texture2D asteroidTexture = Raylib.LoadTextureFromImage(asteroidImage);
            Raylib.UnloadImage(baseImage);
            Raylib.UnloadImage(asteroidImage);

            return new SpriteWrapper(asteroidTexture);
        }

        // Returns both the procedurally masked asteroid sprite and its vertices
        public static (SpriteWrapper sprite, Vector2[] vertices) CreateProceduralAsteroidSpriteWithVertices(int minVertices = 8, int maxVertices = 16, float irregularity = 0.5f)
        {
            var randomIndex = Game.Random.Next(AsteroidTextures.Length);
            var texturePath = AsteroidTextures[randomIndex];
            Image baseImage = Raylib.LoadImage(texturePath);
            int width = baseImage.Width;
            int height = baseImage.Height;

            // Generate random polygon mask with more irregularity
            Vector2 center = new Vector2(width / 2f, height / 2f);
            float radius = Math.Min(width, height) * 0.45f;
            int vertexCount = Game.Random.Next(minVertices, maxVertices + 1);
            Vector2[] vertices = new Vector2[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                float angle = (float)(i * 2 * Math.PI / vertexCount);
                float r = radius * (1f + (float)(Game.Random.NextDouble() * irregularity - irregularity / 2));
                vertices[i] = new Vector2(
                    center.X + r * (float)Math.Cos(angle),
                    center.Y + r * (float)Math.Sin(angle)
                );
            }

            // Prepare mask array
            bool[] mask = new bool[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = y * width + x;
                    mask[idx] = PointInPolygon(x + 0.5f, y + 0.5f, vertices); // Use pixel center for accuracy
                }
            }

            // Roughen the mask edges (managed array)
            Random rand = Game.Random;
            for (int pass = 0; pass < 2; pass++) // Multiple passes for stronger effect
            {
                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        int idx = y * width + x;
                        if (mask[idx] && IsEdgePixel(mask, x, y, width, height))
                        {
                            if (rand.NextDouble() < 0.4) // 40% chance to shrink
                                mask[idx] = false;
                            else if (rand.NextDouble() < 0.25) // 25% chance to grow
                            {
                                int dx = rand.Next(-1, 2);
                                int dy = rand.Next(-1, 2);
                                int nidx = (y + dy) * width + (x + dx);
                                if (!mask[nidx]) mask[nidx] = true;
                            }
                        }
                    }
                }
            }


            unsafe 
            { 
                var basePixels = Raylib.LoadImageColors(baseImage);

                var asteroidPixels = new Color[width * height];
                for (int i = 0; i < asteroidPixels.Length; i++)
                    asteroidPixels[i] = Color.Blank; // Fully transparent

                // Copy pixels from base image if inside mask, apply edge roughness and dithering
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = y * width + x;
                        if (mask[idx])
                        {
                            Color pixel = basePixels[idx];
                            if (IsEdgePixel(mask, x, y, width, height))
                            {
                                if (rand.NextDouble() < 0.3)
                                    pixel.A = (byte)(pixel.A * 0.5);
                                pixel = Darken(pixel, 0.7f);
                            }
                            asteroidPixels[idx] = pixel;
                        }
                    }
                }


                // Create image from pixel array
                Image asteroidImage = Raylib.GenImageColor(width, height, Color.Blank);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = y * width + x;
                        Raylib.ImageDrawPixel(ref asteroidImage, x, y, asteroidPixels[idx]);
                    }
                }

                Texture2D asteroidTexture = Raylib.LoadTextureFromImage(asteroidImage);
                Raylib.UnloadImage(baseImage);
                Raylib.UnloadImage(asteroidImage);

                return (new SpriteWrapper(asteroidTexture), vertices);
            }
        }

        // Point-in-polygon test (ray casting)
        private static bool PointInPolygon(float px, float py, Vector2[] poly)
        {
            bool inside = false;
            for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
            {
                if (((poly[i].Y > py) != (poly[j].Y > py)) &&
                    (px < (poly[j].X - poly[i].X) * (py - poly[i].Y) / ((poly[j].Y - poly[i].Y) + 0.0001f) + poly[i].X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        // Helper: checks if pixel is on edge of mask
        private static bool IsEdgePixel(bool[] mask, int x, int y, int width, int height)
        {
            if (!mask[y * width + x]) return false;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx < 0 || nx >= width || ny < 0 || ny >= height) continue;
                    if (!mask[ny * width + nx]) return true;
                }
            }
            return false;
        }

        // Helper: darken color
        private static Color Darken(Color color, float factor)
        {
            return new Color(
                (byte)(color.R * factor),
                (byte)(color.G * factor),
                (byte)(color.B * factor),
                color.A
            );
        }
    }
}
