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

        // Base speeds that will be scaled by screen resolution
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
            
            // Scale movement and projectile speeds based on screen resolution
            baseSpeed = 250f * scaleFactor;        // Increased base speed, scaled by resolution
            projectileSpeed = 400f * scaleFactor;  // Increased projectile speed, scaled by resolution
            
            // Generate the ship sprite at actual rectangle size for better fullscreen appearance
            shipSprite = SpriteGenerator.CreatePlayerShip(isPlayer1, (int)rect.Width, (int)rect.Height);
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

            HandleMovement(frameTime, game);
            HandleShooting(game);
            HandleCollisions(game);
        }

        private void HandleMovement(float frameTime, Game game)
        {
            Rectangle newRect = Rect;

            if (Raylib.IsKeyDown(upKey))
            {
                UpHeldDuration += frameTime;
                float currentSpeed = (baseSpeed / (1 + UpHeldDuration * slowdownFactor)) * (SlowTimer > 0 ? 0.5f : 1.0f);
                newRect.Y -= currentSpeed;
            }
            else
            {
                UpHeldDuration = 0f;
            }

            if (Raylib.IsKeyDown(downKey))
            {
                DownHeldDuration += frameTime;
                float currentSpeed = (baseSpeed / (1 + DownHeldDuration * slowdownFactor)) * (SlowTimer > 0 ? 0.5f : 1.0f);
                newRect.Y += currentSpeed;
            }
            else
            {
                DownHeldDuration = 0f;
            }

            // Check if the new position would collide with any walls
            bool canMove = true;
            foreach (var obj in game.GetGameObjects())
            {
                if (obj is Projectile projectile && projectile.Owner != this && projectile.BlocksMovement)
                {
                    if (Raylib.CheckCollisionRecs(newRect, projectile.Rect))
                    {
                        canMove = false;
                        break;
                    }
                }
            }

            // Only update position if movement is allowed
            if (canMove)
            {
                Rect = newRect;
            }
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
                    int wallWidth = (int)(150 * scaleFactor);  // Scaled wall width
                    int wallHeight = (int)(15 * scaleFactor);  // Scaled wall height
                    int wallOffset = (int)(Rect.Width * 0.6f); // Position relative to ship size
                    
                    rect = new Rectangle(
                        shipCenterX + (isPlayer1 ? wallOffset : -wallOffset - wallWidth), 
                        shipCenterY - wallHeight / 2, 
                        wallWidth, wallHeight);
                    HasWall = false;
                }
                else if (IceShotTimer > 0)
                {
                    type = ProjectileType.Ice;
                    int iceSize = (int)(30 * scaleFactor);     // Scaled ice projectile
                    int iceOffset = (int)(Rect.Width * 0.6f);
                    
                    rect = new Rectangle(
                        shipCenterX + (isPlayer1 ? iceOffset : -iceOffset - iceSize), 
                        shipCenterY - iceSize / 2, 
                        iceSize, iceSize);
                }
                else
                {
                    type = ProjectileType.Normal;
                    int bulletWidth = (int)(30 * scaleFactor);  // Scaled normal bullet (doubled)
                    int bulletHeight = (int)(15 * scaleFactor); // Scaled normal bullet (doubled)
                    int bulletOffset = (int)(Rect.Width * 0.6f);
                    
                    rect = new Rectangle(
                        shipCenterX + (isPlayer1 ? bulletOffset : -bulletOffset - bulletWidth), 
                        shipCenterY - bulletHeight / 2, 
                        bulletWidth, bulletHeight);
                }
                
                game.AddGameObject(ProjectileFactory.Create(type, rect, speed, this));
                
                // Play shoot sound
                game.PlayShootSound();
            }
        }

        private void HandleCollisions(Game game)
        {
            var gameObjects = game.GetGameObjects().ToList();
            
            // Check for collisions with this player's body
            foreach (var obj in gameObjects)
            {
                if (Raylib.CheckCollisionRecs(Rect, obj.Rect))
                {
                    // Did we get hit by a projectile that isn't ours?
                    if (obj is Projectile projectile && projectile.Owner != this)
                    {
                        // Let the projectile handle its own hit behavior
                        projectile.OnHit(this, game);
                        
                        if (projectile.DestroyOnHit)
                        {
                            projectile.IsActive = false;
                        }
                    }
                }
            }

            // Check for collisions between this player's projectiles and other objects
            var myProjectiles = gameObjects.OfType<Projectile>().Where(p => p.Owner == this).ToList();
            foreach (var myProjectile in myProjectiles)
            {
                foreach (var obj in gameObjects)
                {
                    if (Raylib.CheckCollisionRecs(myProjectile.Rect, obj.Rect))
                    {
                        if (obj is PowerUp powerUp)
                        {
                            if (powerUp.Type == PowerUpType.FireRate)
                            {
                                FireRate *= 0.8f;
                            }
                            else if (powerUp.Type == PowerUpType.IceShot)
                            {
                                IceShotTimer = 10.0f;
                            }
                            else if (powerUp.Type == PowerUpType.Wall)
                            {
                                HasWall = true;
                            }
                            powerUp.IsActive = false;
                            myProjectile.IsActive = false;
                            game.PlayPowerUpSound();
                            break;
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            // Calculate rotation and position for proper ship orientation
            float rotation = isPlayer1 ? 90f : -90f; // Player 1 faces right, Player 2 faces left
            Vector2 origin = new Vector2(shipSprite.Width / 2, shipSprite.Height / 2);
            Vector2 position = new Vector2(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height / 2);
            
            // Draw the ship sprite with rotation
            Raylib.DrawTextureEx(shipSprite, position, rotation, 1.0f, Color.White);
            
            // Add a tint overlay if slowed (also needs rotation)
            if (SlowTimer > 0)
            {
                Raylib.DrawTextureEx(shipSprite, position, rotation, 1.0f, new Color(0, 0, 255, 100)); // Blue tint
            }
            
            // Add engine trail effect when moving
            if (Raylib.IsKeyDown(upKey) || Raylib.IsKeyDown(downKey))
            {
                DrawEngineTrail();
            }
        }
        
        private void DrawEngineTrail()
        {
            // Engine trail positions need to account for ship rotation and larger size
            // Player 1 faces right (90°), so engine trail comes from the back (left side)
            // Player 2 faces left (-90°), so engine trail comes from the back (right side)
            
            int trailX, trailY;
            
            if (isPlayer1)
            {
                // Player 1 faces right, engine trail from left side
                trailX = (int)Rect.X - 10; // Adjusted for larger ship
                trailY = (int)(Rect.Y + Rect.Height / 2);
            }
            else
            {
                // Player 2 faces left, engine trail from right side
                trailX = (int)(Rect.X + Rect.Width + 5); // Adjusted for larger ship
                trailY = (int)(Rect.Y + Rect.Height / 2);
            }
            
            Color trailColor = new Color(255, 150, 0, 150); // Orange with transparency
            
            // Make engine trails larger for bigger ships
            for (int i = 0; i < 5; i++) // More trail particles
            {
                int offset = i * (isPlayer1 ? -3 : 3); // Larger spacing
                int radius = 4 - i; // Larger trail particles
                if (radius > 0)
                {
                    Raylib.DrawCircle(trailX + offset, trailY, radius, trailColor);
                }
            }
        }
    }
}
