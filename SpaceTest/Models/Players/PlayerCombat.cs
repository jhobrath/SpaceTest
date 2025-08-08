using System.Numerics;
using System.Linq;
using Raylib_cs;

namespace GalagaFighter.Models.Players
{
    public class PlayerCombat
    {
        private readonly bool isPlayer1;
        private readonly float projectileSpeed;
        private readonly float scaleFactor;
        private readonly float fireRate;
        public float FireTimer { get; private set; }

        public PlayerCombat(bool isPlayer1, float projectileSpeed, float scaleFactor, float fireRate)
        {
            this.isPlayer1 = isPlayer1;
            this.projectileSpeed = projectileSpeed;
            this.scaleFactor = scaleFactor;
            this.fireRate = fireRate;
            FireTimer = 0;
        }

        public void UpdateTimer(float frameTime)
        {
            FireTimer += frameTime;
        }

        public Vector2 GetProjectileSpawnPoint(Rectangle playerRect, float projectileWidth, float projectileHeight, bool isLeftEngine)
        {

            float shipCenterX = playerRect.X + playerRect.Width / 2f;
            float shipCenterY = playerRect.Y + playerRect.Height / 2f + (isLeftEngine ? -1 : 1)*playerRect.Height/4;
            float offset = (playerRect.Width > playerRect.Height ? playerRect.Width : playerRect.Height) * 0.5f;
            
            if (isPlayer1)
            {
                return new Vector2(
                    shipCenterX + offset,
                    shipCenterY - projectileHeight / 2f);
            }
            else
            {
                return new Vector2(
                    shipCenterX - offset - projectileWidth,
                    shipCenterY - projectileHeight / 2f);
            }
        }

        public bool CanFire(bool keyPressed, PlayerStats stats)
        {
            return keyPressed && FireTimer >= fireRate * stats.FireRateMultiplier;
        }

        public Rectangle GetProjectileRect(ProjectileType type, Vector2 spawnPoint)
        {
            int width, height;
            switch (type)
            {
                case ProjectileType.Wall:
                    width = (int)(150 * scaleFactor);
                    height = (int)(15 * scaleFactor);
                    break;
                case ProjectileType.Ice:
                    width = (int)(40 * scaleFactor);
                    height = (int)(20 * scaleFactor);
                    break;
                default: // Normal
                    width = (int)(30 * scaleFactor);
                    height = (int)(15 * scaleFactor);
                    break;
            }

            return new Rectangle(spawnPoint.X, spawnPoint.Y, width, height);
        }

        public float GetProjectileSpeed()
        {
            return isPlayer1 ? projectileSpeed : -projectileSpeed;
        }

        public void ResetFireTimer()
        {
            FireTimer = 0;
        }
    }
}