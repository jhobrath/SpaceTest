using Raylib_cs;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Models
{
    public class Player : GameObject
    {
        public int Health;
        public float UpHeldDuration;
        public float DownHeldDuration;
        public float FireRate;
        public float FireTimer;
        public float IceShotTimer;
        public float SlowTimer;
        public bool HasWall;
        private readonly KeyboardKey upKey;
        private readonly KeyboardKey downKey;
        private readonly KeyboardKey shootKey;
        private readonly bool isPlayer1;
        private readonly Texture2D shipSprite;
        private readonly float scaleFactor;

        // Extracted helper - first step in modular refactor
        private readonly PlayerMovementHelper movementHelper;
        private readonly PlayerRenderingHelper renderingHelper;

        private readonly float baseSpeed;
        private readonly float projectileSpeed;
        private const float slowdownFactor = 3.0f;

        public Player(Rectangle rect, float fireRate, KeyboardKey up, KeyboardKey down, KeyboardKey shoot, bool isPlayer1, float scale) : base(rect)
        {
            Health = 100;
            UpHeldDuration = 0f;
            DownHeldDuration = 0f;
            FireRate = fireRate;
            FireTimer = 0;
            IceShotTimer = 0;
            SlowTimer = 0;
            HasWall = false;
            upKey = up;
            downKey = down;
            shootKey = shoot;
            this.isPlayer1 = isPlayer1;
            scaleFactor = scale;
            
            baseSpeed = 16f * scaleFactor;
            projectileSpeed = 17f * scaleFactor;
            
            shipSprite = SpriteGenerator.CreatePlayerShip(isPlayer1, (int)rect.Width, (int)rect.Height);

            // Initialize helper classes
            movementHelper = new PlayerMovementHelper(baseSpeed, upKey, downKey);
            renderingHelper = new PlayerRenderingHelper(shipSprite, isPlayer1, scaleFactor);
        }

        public override void Update(Game game)
        {
            if (Health <= 0)
            {
                IsActive = false;
                return;
            }

            float frameTime = Raylib.GetFrameTime();
            FireTimer += frameTime;
            if (SlowTimer > 0) SlowTimer -= frameTime;
            if (IceShotTimer > 0) IceShotTimer -= frameTime;

            // Use movement helper
            Rect = movementHelper.HandleMovement(Rect, ref UpHeldDuration, ref DownHeldDuration, 
                SlowTimer > 0, frameTime, game.GetGameObjects(), this);
            
            HandleShooting(game);
            HandleCollisions(game);
        }

        private void HandleShooting(Game game)
        {
            if (Raylib.IsKeyDown(shootKey) && FireTimer >= FireRate)
            {
                FireTimer = 0;

                ProjectileType type;
                Rectangle rect;
                float speed = isPlayer1 ? projectileSpeed : -projectileSpeed;

                // Scale projectile sizes and positions based on ship scale
                int shipCenterX = (int)(Rect.X + Rect.Width / 2);
                int shipCenterY = (int)(Rect.Y + Rect.Height / 2);

                if (HasWall)
                {
                    type = ProjectileType.Wall;
                    int wallWidth = (int)(150 * scaleFactor);
                    int wallHeight = (int)(15 * scaleFactor);
                    int wallOffset = (int)(Rect.Width * 0.6f);
                    
                    rect = new Rectangle(
                        shipCenterX + (isPlayer1 ? wallOffset : -wallOffset - wallWidth), 
                        shipCenterY - wallHeight / 2, 
                        wallWidth, wallHeight);
                    HasWall = false;
                }
                else if (IceShotTimer > 0)
                {
                    type = ProjectileType.Ice;
                    int iceSize = (int)(30 * scaleFactor);
                    int iceOffset = (int)(Rect.Width * 0.6f);
                    
                    rect = new Rectangle(
                        shipCenterX + (isPlayer1 ? iceOffset : -iceOffset - iceSize), 
                        shipCenterY - iceSize / 2, 
                        iceSize, iceSize);
                }
                else
                {
                    type = ProjectileType.Normal;
                    int bulletWidth = (int)(30 * scaleFactor);
                    int bulletHeight = (int)(15 * scaleFactor);
                    int bulletOffset = (int)(Rect.Width * 0.6f);
                    
                    rect = new Rectangle(
                        shipCenterX + (isPlayer1 ? bulletOffset : -bulletOffset - bulletWidth), 
                        shipCenterY - bulletHeight / 2, 
                        bulletWidth, bulletHeight);
                }
                
                game.AddGameObject(ProjectileFactory.Create(type, rect, speed, this));
                game.PlayShootSound();
            }
        }

        private void HandleCollisions(Game game)
        {
            var gameObjects = game.GetGameObjects().ToList();
            
            foreach (var obj in gameObjects)
            {
                if (Raylib.CheckCollisionRecs(Rect, obj.Rect))
                {
                    if (obj is Projectile projectile && projectile.Owner != this)
                    {
                        projectile.OnHit(this, game);
                        
                        if (projectile.DestroyOnHit)
                        {
                            projectile.IsActive = false;
                        }
                    }
                }
            }

            var myProjectiles = gameObjects.OfType<Projectile>().Where(p => p.Owner == this).ToList();
            foreach (var myProjectile in myProjectiles)
            {
                foreach (var obj in gameObjects)
                {
                    if (Raylib.CheckCollisionRecs(myProjectile.Rect, obj.Rect))
                    {
                        if (obj is PowerUp powerUp)
                        {
                            ApplyPowerUpEffect(powerUp.Type);
                            powerUp.IsActive = false;
                            myProjectile.IsActive = false;
                            game.PlayPowerUpSound();
                            break;
                        }
                    }
                }
            }
        }

        private void ApplyPowerUpEffect(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.FireRate:
                    FireRate *= 0.8f;
                    break;
                case PowerUpType.IceShot:
                    IceShotTimer = 10.0f;
                    break;
                case PowerUpType.Wall:
                    HasWall = true;
                    break;
            }
        }

        public override void Draw()
        {
            bool isMoving = Raylib.IsKeyDown(upKey) || Raylib.IsKeyDown(downKey);
            renderingHelper.DrawPlayer(Rect, SlowTimer > 0, isMoving);
        }

        // Clean interface methods for projectiles
        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public void ApplySlowEffect(float duration)
        {
            SlowTimer = duration;
        }
    }

    // Helper class for movement logic
    public class PlayerMovementHelper
    {
        private readonly float baseSpeed;
        private readonly KeyboardKey upKey;
        private readonly KeyboardKey downKey;
        private const float slowdownFactor = 3.0f;

        public PlayerMovementHelper(float baseSpeed, KeyboardKey upKey, KeyboardKey downKey)
        {
            this.baseSpeed = baseSpeed;
            this.upKey = upKey;
            this.downKey = downKey;
        }

        public Rectangle HandleMovement(Rectangle currentRect, ref float upHeldDuration, ref float downHeldDuration, 
            bool isSlowed, float frameTime, List<GameObject> gameObjects, Player player)
        {
            Rectangle newRect = currentRect;

            if (Raylib.IsKeyDown(upKey))
            {
                upHeldDuration += frameTime;
                float currentSpeed = (baseSpeed / (1 + upHeldDuration * slowdownFactor)) * (isSlowed ? 0.5f : 1.0f);
                newRect.Y -= currentSpeed;
            }
            else
            {
                upHeldDuration = 0f;
            }

            if (Raylib.IsKeyDown(downKey))
            {
                downHeldDuration += frameTime;
                float currentSpeed = (baseSpeed / (1 + downHeldDuration * slowdownFactor)) * (isSlowed ? 0.5f : 1.0f);
                newRect.Y += currentSpeed;
            }
            else
            {
                downHeldDuration = 0f;
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

    // Helper class for rendering
    public class PlayerRenderingHelper
    {
        private readonly Texture2D shipSprite;
        private readonly bool isPlayer1;
        private readonly float scaleFactor;

        public PlayerRenderingHelper(Texture2D shipSprite, bool isPlayer1, float scaleFactor)
        {
            this.shipSprite = shipSprite;
            this.isPlayer1 = isPlayer1;
            this.scaleFactor = scaleFactor;
        }

        public void DrawPlayer(Rectangle playerRect, bool isSlowed, bool isMoving)
        {
            DrawShip(playerRect, isSlowed);
            
            if (isMoving)
            {
                DrawEngineTrail(playerRect);
            }
        }

        private void DrawShip(Rectangle playerRect, bool isSlowed)
        {
            float rotation = isPlayer1 ? 90f : -90f;
            Vector2 position = new Vector2(playerRect.X + playerRect.Width / 2, playerRect.Y + playerRect.Height / 2);
            
            Raylib.DrawTextureEx(shipSprite, position, rotation, 1.0f, Color.White);
            
            if (isSlowed)
            {
                Raylib.DrawTextureEx(shipSprite, position, rotation, 1.0f, new Color(0, 0, 255, 100));
            }
        }

        private void DrawEngineTrail(Rectangle playerRect)
        {
            int trailX, trailY;
            int trailOffset = (int)(15 * scaleFactor);
            
            if (isPlayer1)
            {
                trailX = (int)playerRect.X - trailOffset;
                trailY = (int)(playerRect.Y + playerRect.Height / 2);
            }
            else
            {
                trailX = (int)(playerRect.X + playerRect.Width + trailOffset / 2);
                trailY = (int)(playerRect.Y + playerRect.Height / 2);
            }
            
            Color trailColor = new Color(255, 150, 0, 150);
            
            int particleCount = (int)(7 * scaleFactor);
            int maxRadius = (int)(6 * scaleFactor);
            int spacing = (int)(4 * scaleFactor);
            
            for (int i = 0; i < particleCount; i++)
            {
                int offset = i * (isPlayer1 ? -spacing : spacing);
                int radius = Math.Max(1, maxRadius - i);
                
                Raylib.DrawCircle(trailX + offset, trailY, radius, trailColor);
            }
        }
    }
}
