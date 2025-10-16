using GalagaFighter.Core2.GameObjects;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services.Static
{
    public static class PolygonVerticesCompiler
    {
        public static Vector2[] GetVertices(Rectangle rect, Vector2 origin, float rotation)
        {
            // Get the four corners of the rectangle relative to center
            var halfWidth = rect.Width / 2f;
            var halfHeight = rect.Height / 2f;

            Vector2[] corners = new Vector2[4]
            {
                new Vector2(-halfWidth, -halfHeight), // Top-left
                new Vector2(halfWidth, -halfHeight),  // Top-right
                new Vector2(halfWidth, halfHeight),   // Bottom-right
                new Vector2(-halfWidth, halfHeight)   // Bottom-left
            };

            // Apply rotation if needed
            if (rotation != 0)
            {
                // Convert degrees to radians
                float rotationInRadians = rotation * MathF.PI / 180f;

                float cos = MathF.Cos(rotationInRadians);
                float sin = MathF.Sin(rotationInRadians);

                for (int i = 0; i < corners.Length; i++)
                {
                    float x = corners[i].X;
                    float y = corners[i].Y;
                    corners[i] = new Vector2(
                        x * cos - y * sin,
                        x * sin + y * cos
                    );
                }
            }

            // Translate to world position
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] += origin;
            }

            return corners;
        }
    }
}
