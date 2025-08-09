using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using System.Collections.Generic;

namespace GalagaFighter.Models.Players
{
    public class PlayerMovement
    {
        private readonly float baseSpeed;
        private readonly KeyboardKey upKey;
        private readonly KeyboardKey downKey;
        private const float slowdownFactor = 2.0f;
        public float Speed { get; set; }

        public PlayerMovement(float baseSpeed, KeyboardKey upKey, KeyboardKey downKey)
        {
            this.baseSpeed = baseSpeed;
            this.upKey = upKey;
            this.downKey = downKey;
        }

        public Rectangle HandleMovement(Rectangle currentRect, ref float upHeldDuration, ref float downHeldDuration, 
            float slowIntensity, float frameTime, List<GameObject> gameObjects, Player player)
        {
            Rectangle newRect = currentRect;

            if (Raylib.IsKeyDown(upKey))
            {
                upHeldDuration += frameTime;
                Speed = -(baseSpeed / (1 + upHeldDuration * slowdownFactor)) * slowIntensity;
                newRect.Y += Speed;
            }
            else
            {
                upHeldDuration = 0f;
            }

            if (Raylib.IsKeyDown(downKey))
            {
                downHeldDuration += frameTime;
                Speed = (baseSpeed / (1 + downHeldDuration * slowdownFactor)) * slowIntensity;
                newRect.Y += Speed;
            }
            else
            {
                downHeldDuration = 0f;
            }

            if(!Raylib.IsKeyDown(downKey) && !Raylib.IsKeyDown(upKey))
            {
                Speed = 0f;
            }

            // Check for wall collisions
            foreach (var obj in gameObjects)
            {
                if (obj is Projectile projectile && projectile.Owner != player && projectile.BlocksMovement)
                {
                    if (Raylib.CheckCollisionRecs(newRect, projectile.Rect))
                    {
                        return currentRect; // Block movement
                    }
                }
            }

            return newRect;
        }
    }
}