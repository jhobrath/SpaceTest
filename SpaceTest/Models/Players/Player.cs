using Raylib_cs;
using SpaceTest.Models.Projectiles;
using GalagaFighter.Models.Effects;
using GalagaFighter.Models.PowerUps;
using GalagaFigther.Models.Projectiles;
using GalagaFigther.Models;
using System.Numerics;
using GalagaFigther;

namespace GalagaFighter.Models.Players
{
    public class Player : GameObject
    {
        private readonly KeyboardKey upKey;
        private readonly KeyboardKey downKey;
        private readonly KeyboardKey shootKey;
        public readonly bool IsPlayer1;
        private readonly Texture2D shipSprite;
        private readonly float scaleFactor;

        private readonly PlayerMovement movement;
        private readonly PlayerRenderer renderer;
        public PlayerStats Stats;
        private readonly PlayerCombat combat;

        private const string _spritePath = "Sprites/Players/Player1.png";

        private float UpHeldDuration;
        private float DownHeldDuration;

        public bool _useLeftEngine;

        private readonly List<PlayerStats> _statsGroups = [];

        public Player(Rectangle rect, float fireRate, KeyboardKey up, KeyboardKey down, KeyboardKey shoot, bool isPlayer1, float scale) 
            : base(rect)
        {
            this.IsPlayer1 = isPlayer1;
            this.shootKey = shoot;
            this.scaleFactor = scale;
            this.upKey = up;
            this.downKey = down;

            // Load sprite
            shipSprite = TextureLibrary.Get(_spritePath);

            // Initialize components
            float baseSpeed = 20f * scale;
            float projectileSpeed = 17f * scale;
            float effectiveFireRate = fireRate * (float)Math.Pow(0.8f, 5);

            movement = new PlayerMovement(baseSpeed, up, down);
            renderer = new PlayerRenderer(shipSprite, isPlayer1, scale);
            combat = new PlayerCombat(isPlayer1, projectileSpeed, scale, effectiveFireRate);

            _statsGroups.Add(new PlayerStats(this));
            _statsGroups.Add(new PlayerStats(this));

            Stats = _statsGroups[0];


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

            // Aggregate all active effects' SpeedMultiplier
            float speedMultiplier = 1.0f;
            foreach (var effect in Stats.GetActiveEffects())
                speedMultiplier *= effect.SpeedMultiplier;

            Rect = movement.HandleMovement(Rect, ref UpHeldDuration, ref DownHeldDuration, 
                speedMultiplier, frameTime, game.GetGameObjects(), this);
            
            HandleShooting(game);
            HandleCollisions(game);
            HandleStatsSwitching(game);
        }

        private void HandleStatsSwitching(Game game)
        {
            if (Raylib.IsKeyPressed(GetStatsSwitchKey()))
            {
                // Switch to the next stats group
                int currentIndex = _statsGroups.IndexOf(Stats);
                int nextIndex = (currentIndex + 1) % _statsGroups.Count;
                Stats = _statsGroups[nextIndex];
                Game.PlayPowerUpSound();
            }
        }

        private KeyboardKey GetStatsSwitchKey()
        {
            if(IsPlayer1)
            {
                return KeyboardKey.A;
            }
            else
            {
                return KeyboardKey.Right;
            }
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

            var skew = 0f;
            if (UpHeldDuration > 0) skew = IsPlayer1 ? -.5f : .5f;
            else if (DownHeldDuration > 0) skew = IsPlayer1 ? .5f : -.5f;

            var playerRendering = new PlayerRendering
            {
                Scale = Rect.Width / shipSprite.Width,
                Color = Color.White,
                Rotation = IsPlayer1 ? 90f : -90f,
                Skew = skew,
                Texture = _spritePath
            };

            foreach (var effect in Stats.GetActiveEffects())
                effect.ModifyPlayerRendering(playerRendering);

            renderer.DrawPlayer(Rect, playerRendering, isMoving);
        }

        // Interface methods for other classes
        public void TakeDamage(int damage) => Stats.TakeDamage(damage);

        
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
            var collisions = new List<Collision>();
            foreach (var obj in gameObjects)
            {
                if(obj is WallProjectile wall)
                {
                    if (!wall.IsStuck && ((wall.Speed.X < 0 && wall.Rect.X <= Rect.Width/2) || (wall.Speed.X > 0 && wall.Rect.X + wall.Rect.Width >= Raylib.GetRenderWidth() - Rect.Width/2)))
                    {
                        wall.OnHit(this, game);
                        collisions.Add(new Collision(wall.Rect, 20, wall.Speed, useRight: !IsPlayer1));
                    }
                }
                else if (Raylib.CheckCollisionRecs(Rect, obj.Rect))
                {
                    if (obj is Projectile projectile && projectile.Owner != this)
                    {
                        projectile.OnHit(this, game);
                        collisions.Add(new Collision(projectile.Rect, 20, projectile.Speed, useRight: projectile.Owner.IsPlayer1));
                        if (projectile.DestroyOnHit)
                        {
                            projectile.IsActive = false;
                        }
                    }
                }
            }

            foreach(var collision in collisions)
            {
                game.AddGameObject(collision);
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

                            if(myProjectile.DestroyOnPowerUp)
                                myProjectile.IsActive = false;

                            Game.PlayPowerUpSound();
                            break;
                        }
                    }
                }
            }
        }
    }
}
