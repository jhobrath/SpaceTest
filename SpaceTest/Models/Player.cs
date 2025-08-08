using Raylib_cs;
using SpaceTest.Models.Projectiles;
using GalagaFighter.Models.Effects;
using GalagaFighter.Models.PowerUps;

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
        public readonly PlayerStats Stats;
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
            Stats = new PlayerStats(this);
            combat = new PlayerCombat(isPlayer1, projectileSpeed, scale, effectiveFireRate);

            UpHeldDuration = 0f;
            DownHeldDuration = 0f;
        }

        public override void Update(Game game)
        {
            if (Stats.Health <= 0)
            {
                IsActive = false;
                return;
            }

            float frameTime = Raylib.GetFrameTime();
            combat.UpdateTimer(frameTime);
            Stats.UpdateEffects(frameTime);

            // Use FrozenEffect for slow intensity
            float slowIntensity = 1.0f;
            var frozen = Stats.GetFirstEffect<FrozenEffect>();
            if (frozen != null)
                slowIntensity = frozen.GetSlowMultiplier();

            Rect = movement.HandleMovement(Rect, ref UpHeldDuration, ref DownHeldDuration, 
                slowIntensity, frameTime, game.GetGameObjects(), this);
            
            HandleShooting(game);
            HandleCollisions(game);
        }

        private void HandleShooting(Game game)
        {
            if (Raylib.IsKeyDown(shootKey))
            {
                foreach (var effect in Stats.GetActiveEffects())
                {
                    effect.OnShoot(game);
                }
            }
        }

        // Helper methods for effects to access internals
        public PlayerCombat GetCombat() => combat;
        public PlayerMovement GetMovement() => movement;
        public float GetScaleFactor() => scaleFactor;
        public KeyboardKey GetShootKey() => shootKey;
        public bool ToggleEngine() { _useLeftEngine = !_useLeftEngine; return _useLeftEngine; }

        public override void Draw()
        {
            bool isMoving = Raylib.IsKeyDown(upKey) || Raylib.IsKeyDown(downKey);
            var frozen = Stats.GetFirstEffect<FrozenEffect>();
            bool isSlowed = frozen != null;
            //float blueAlpha = isSlowed ? frozen.GetBlueAlpha() : 0f;
            renderer.DrawPlayer(Rect, isSlowed, isMoving);
        }

        // Interface methods for other classes
        public void TakeDamage(int damage) => Stats.TakeDamage(damage);
        // No more ApplySlowEffect on Stats; handled by FrozenEffect
        public void ApplySlowEffect(float duration)
        {
            Stats.AddEffect(this, new FrozenEffect(this, duration));
        }
        public void ApplySlowEffect() => ApplySlowEffect(5.0f);
        
        // Properties
        public int Health => Stats.Health;
        public int MaxBullets => Stats.MaxBullets;
        public int GetActiveBulletCount(List<GameObject> gameObjects)
        {
            return gameObjects
                .OfType<Projectile>()
                .Where(p => p.Owner == this && p is NormalProjectile)
                .Count();
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
                            var effect = powerUp.CreateEffect(this);
                            if (effect != null)
                            {
                                Stats.AddEffect(this, effect);
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
    }
}
