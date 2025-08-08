using Raylib_cs;
using System.Numerics;
using System.Linq;
using GalagaFighter.Models.Players;

namespace GalagaFighter.Models.Players
{
    public class Player : GameObject
    {
        private readonly KeyboardKey upKey;
        private readonly KeyboardKey downKey;
        private readonly KeyboardKey shootKey;
        private readonly bool isPlayer1;
        private readonly Texture2D shipSprite;
        private readonly float scaleFactor;

        private readonly PlayerMovement movement;
        private readonly PlayerRenderer renderer;
        private readonly PlayerStats stats;
        private readonly PlayerCombat combat;

        private float UpHeldDuration;
        private float DownHeldDuration;

        public bool _useLeftEngine;

        public Player(Rectangle rect, float fireRate, KeyboardKey up, KeyboardKey down, KeyboardKey shoot, bool isPlayer1, float scale) 
            : base(rect)
        {
            this.isPlayer1 = isPlayer1;
            this.shootKey = shoot;
            this.scaleFactor = scale;
            this.upKey = up;
            this.downKey = down;
            
            // Load sprite
            string spritePath = isPlayer1 ? "Sprites/Player1.png" : "Sprites/Player2.png";
            shipSprite = Raylib.LoadTexture(spritePath);

            // Initialize components
            float baseSpeed = 20f * scale;
            float projectileSpeed = 17f * scale;
            float effectiveFireRate = fireRate * (float)Math.Pow(0.8f, 5);

            movement = new PlayerMovement(baseSpeed, up, down);
            renderer = new PlayerRenderer(shipSprite, isPlayer1, scale);
            stats = new PlayerStats();
            combat = new PlayerCombat(isPlayer1, projectileSpeed, scale, effectiveFireRate);

            UpHeldDuration = 0f;
            DownHeldDuration = 0f;
        }

        public override void Update(Game game)
        {
            if (stats.Health <= 0)
            {
                IsActive = false;
                return;
            }

            float frameTime = Raylib.GetFrameTime();
            combat.UpdateTimer(frameTime);
            stats.UpdateEffects(frameTime);

            // Update position
            float slowIntensity = stats.CalculateSlowIntensity();
            Rect = movement.HandleMovement(Rect, ref UpHeldDuration, ref DownHeldDuration, 
                slowIntensity, frameTime, game.GetGameObjects(), this);
            
            HandleShooting(game);
            HandleCollisions(game);
        }

        private void HandleShooting(Game game)
        {
            var activeBulletCount = GetActiveBulletCount(game.GetGameObjects());
            if (!combat.CanFire(Raylib.IsKeyDown(shootKey), stats, activeBulletCount))
                return;

            ProjectileType type;
            Rectangle rect;
            Vector2 speed = new Vector2(combat.GetProjectileSpeed(), Math.Min(3, Math.Max(-3, this.movement.Speed*.3333f)));
            _useLeftEngine = !_useLeftEngine; // Alternate between left and right engines

            if (stats.HasWall)
            {
                combat.ResetFireTimer();
                type = ProjectileType.Wall;
                Vector2 spawn = combat.GetProjectileSpawnPoint(Rect, 150 * scaleFactor, 15 * scaleFactor, _useLeftEngine);
                rect = combat.GetProjectileRect(type, spawn);
                stats.ConsumeWall();
            }
            else if (stats.IceShotTimer > 0)
            {
                combat.ResetFireTimer();
                type = ProjectileType.Ice;
                Vector2 spawn = combat.GetProjectileSpawnPoint(Rect, 40 * scaleFactor, 20 * scaleFactor, _useLeftEngine);
                rect = combat.GetProjectileRect(type, spawn);
            }
            else
            {
                int activeBullets = GetActiveBulletCount(game.GetGameObjects());
                if (activeBullets >= stats.MaxBullets)
                    return;

                combat.ResetFireTimer();
                type = ProjectileType.Normal;
                Vector2 spawn = combat.GetProjectileSpawnPoint(Rect, 30 * scaleFactor, 15 * scaleFactor, _useLeftEngine);
                rect = combat.GetProjectileRect(type, spawn);
            }

            game.AddGameObject(ProjectileFactory.Create(type, rect, speed, this));
            game.PlayShootSound();
        }

        private void HandleCollisions(Game game)
        {
            var gameObjects = game.GetGameObjects();
            
            foreach (var obj in gameObjects)
            {
                if(obj is WallProjectile wall)
                {
                    if (!wall.IsStuck && (wall.Rect.X <= 0 || wall.Rect.X + wall.Rect.Width >= Raylib.GetRenderWidth()))
                    {
                        wall.OnHit(this, game);
                    }
                }

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
                            stats.ApplyPowerUpEffect(powerUp.Type);
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
            bool isMoving = Raylib.IsKeyDown(upKey) || Raylib.IsKeyDown(downKey);
            bool isSlowed = stats.IceEffectCount > 0;
            renderer.DrawPlayer(Rect, isSlowed, isMoving);
            //renderer.DrawDebugBounds(Rect);
        }

        // Interface methods for other classes
        public void TakeDamage(int damage) => stats.TakeDamage(damage);
        public void ApplySlowEffect(float duration) => stats.ApplySlowEffect(duration);
        public void ApplySlowEffect() => stats.ApplySlowEffect(5.0f);
        
        // Properties
        public int Health => stats.Health;
        public int MaxBullets => stats.MaxBullets;
        public float IceShotTimer
        {
            get => stats.IceShotTimer;
            set => stats.ResetIceShotTimer();
        }
        public int IceEffectCount => stats.IceEffectCount;
        public float CurrentSlowIntensity => stats.CurrentSlowIntensity;
        public int GetActiveBulletCount(List<GameObject> gameObjects)
        {
            return gameObjects
                .OfType<Projectile>()
                .Where(p => p.Owner == this && p is NormalProjectile)
                .Count();
        }
    }
}
