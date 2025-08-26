using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Collisions
{
    public class PlayerCollisionDetector
    {
        public bool HasCollision(Player player, Projectile projectile)
        {
            // First do a quick rectangle check for performance
            if (!Raylib.CheckCollisionRecs(player.Rect, projectile.Rect))
                return false;
                
            // Define triangle points within the player's rectangle based on current rotation
            var triangle = GetPlayerTriangle(player);
            
            // Check multiple points of the projectile for better accuracy
            var projectilePoints = new Vector2[]
            {
                projectile.Center,
                new Vector2(projectile.Rect.X, projectile.Rect.Y), // Top-left
                new Vector2(projectile.Rect.X + projectile.Rect.Width, projectile.Rect.Y), // Top-right
                new Vector2(projectile.Rect.X, projectile.Rect.Y + projectile.Rect.Height), // Bottom-left
                new Vector2(projectile.Rect.X + projectile.Rect.Width, projectile.Rect.Y + projectile.Rect.Height) // Bottom-right
            };
            
            foreach (var point in projectilePoints)
            {
                if (IsPointInTriangle(point, triangle[0], triangle[1], triangle[2]))
                    return true;
            }
            
            return false;
        }
        
        private Vector2[] GetPlayerTriangle(Player player)
        {
            var rect = player.Rect;
            var center = new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
            
            // Base triangle coordinates for 0-degree rotation (vertical ship pointing up)
            // Corrected coordinates as provided by the user
            Vector2[] baseTriangle = new Vector2[]
            {
                // Left wing: x: 4.5% in, y: 68.5% down  
                new Vector2(rect.Width * 0.045f - rect.Width / 2f, rect.Height * 0.685f - rect.Height / 2f),
                // Right wing: x: 95.5% in, y: 68.5% down
                new Vector2(rect.Width * 0.955f - rect.Width / 2f, rect.Height * 0.685f - rect.Height / 2f),
                // Ship tip: x: 50% in, y: 8% down
                new Vector2(rect.Width * 0.5f - rect.Width / 2f, rect.Height * 0.08f - rect.Height / 2f)
            };
            
            // Apply rotation transformation based on player's current rotation
            float radians = player.Rotation * (MathF.PI / 180f);
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);
            
            Vector2[] rotatedTriangle = new Vector2[3];
            for (int i = 0; i < 3; i++)
            {
                Vector2 point = baseTriangle[i];
                
                // Rotate the point around the origin
                float xPrime = point.X * cos - point.Y * sin;
                float yPrime = point.X * sin + point.Y * cos;
                
                // Translate to final position relative to the player's center
                rotatedTriangle[i] = new Vector2(xPrime, yPrime) + center;
            }
            
            return rotatedTriangle;
        }
        
        private bool IsPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            // Using barycentric coordinates for point-in-triangle test
            var v0 = c - a;
            var v1 = b - a;
            var v2 = point - a;
            
            var dot00 = Vector2.Dot(v0, v0);
            var dot01 = Vector2.Dot(v0, v1);
            var dot02 = Vector2.Dot(v0, v2);
            var dot11 = Vector2.Dot(v1, v1);
            var dot12 = Vector2.Dot(v1, v2);
            
            var invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
            var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            var v = (dot00 * dot12 - dot01 * dot02) * invDenom;
            
            return (u >= 0) && (v >= 0) && (u + v <= 1);
        }
    }
}