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
            var isNearSplat = false;
            foreach (var effect in Stats.GetActiveEffects())
            { 
                speedMultiplier *= effect.SpeedMultiplier;

                if (!isNearSplat && effect is MudSplatEffect splat)
                    if (splat.IsNear(Rect))
                        isNearSplat = true;
            }

            if(isNearSplat)
            {
                speedMultiplier *= .3f;
            }

            Rect = movement.HandleMovement(Rect, ref UpHeldDuration, ref DownHeldDuration, 
                speedMultiplier, frameTime, game.GetGameObjects(), this);
            
            HandleShooting(game);
            HandleCollisions(game);
            HandleStatsSwitching(game);

            PrintStats();
        }

        private void PrintStats()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            float uniformScale = Math.Min(screenWidth / 1920f, screenHeight / 1080f);
            int margin = (int)(15 * uniformScale);

            var xPos = IsPlayer1 ? margin : screenWidth - (int)(250 * uniformScale);
            var yPos = margin + (int)(30 * uniformScale);

            var verticalOffset = 0;
            foreach (var stats in _statsGroups)
            {
                var row = 0;
                var col = 0;

                var effects = stats.GetActiveEffects();
                var icons = effects.Where(e => !(e is DefaultShootEffect) && !(e is FireRateEffect))
                    .Select(x => x.IconPath).ToList();

                var fireRateCount = Math.Max(1, Math.Min(8, GetFireRateIndex(stats.FireRateMultiplier)));
                icons.Insert(0, $"Sprites/Effects/firerate{fireRateCount}.png");

                var rows = (int)Math.Ceiling(icons.Count / 6f);
                if (stats == Stats)
                    Raylib.DrawRectangle(xPos - 2, yPos + verticalOffset - 2, 6 * 30 + 4, rows * 30 + 4, Color.DarkGray);

                foreach (var icon in icons)
                {
                    var thisX = xPos + col * 30f * uniformScale;
                    var thisY = yPos + row * 30f * uniformScale + verticalOffset;
                    var texture = TextureLibrary.Get(icon);

                    Raylib.DrawTextureEx(
                        texture,
                        new Vector2(thisX, thisY),
                        0f,
                        30f / texture.Width,
                        Color.White);

                    col++;
                    col = col % 6;
                    if (col == 0)
                        row++;
                }

                verticalOffset += row * (int)(30 * uniformScale) + (int)(30 * uniformScale);
            }
        }

        public static int GetFireRateIndex(float inputValue)
        {
            var baseValue = .72f;
            // Handle invalid inputs.
            if (inputValue > baseValue)
            {
                return 1;
            }

            int n = 1;
            double currentPower = baseValue;
            double previousPower;

            // Loop through powers of BaseValue to find the correct range.
            while (true)
            {
                previousPower = currentPower;
                currentPower = Math.Pow(baseValue, n + 1);

                // Check if the inputValue falls within the current range.
                // The range for 'n' is (BaseValue^n, BaseValue^(n-1)]
                if (inputValue <= previousPower && inputValue > currentPower)
                {
                    return n + 1;
                }

                // Handle the special case where the input value is exactly BaseValue.
                if (Math.Abs(inputValue - baseValue) < float.Epsilon)
                {
                    return 1;
                }

                n++;
            }
        }

        private void HandleStatsSwitching(Game game)
        {
            if (Raylib.IsKeyPressed(GetStatsSwitchKey()))
            {
                foreach(var effect in Stats.GetActiveEffects())
                {
                    effect.OnStatsSwitch();
                }

                var health = Stats.Health;
                // Switch to the next stats group
                int currentIndex = _statsGroups.IndexOf(Stats);
                int nextIndex = (currentIndex + 1) % _statsGroups.Count;
                Stats = _statsGroups[nextIndex];
                Stats.Health = health;
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
                Texture = new SpriteWrapper(_spritePath)
            };

            foreach (var effect in Stats.GetActiveEffects())
            {
                effect.ModifyPlayerRendering(playerRendering);
            }

            renderer.DrawPlayer(Rect, playerRendering, isMoving);

            foreach (var effect in Stats.GetActiveEffects())
                effect.OnDraw();

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
                if (obj is WallProjectile wall)
                {
                    if (!wall.IsStuck && ((wall.Speed.X < 0 && wall.Rect.X <= Rect.Width / 2) || (wall.Speed.X > 0 && wall.Rect.X + wall.Rect.Width >= Raylib.GetRenderWidth() - Rect.Width / 2)))
                    {
                        wall.OnHit(this, game);
                        collisions.Add(new Collision(wall.Rect, 20, wall.Speed, useRight: !IsPlayer1));
                    }
                }
                else if (obj is MudProjectile mud && mud.Owner != this)
                {
                    if((mud.Speed.X < 0 && mud.Rect.X <= Rect.Width / 2) || (mud.Speed.X > 0 && mud.Rect.X + mud.Rect.Width >= Raylib.GetRenderWidth() - Rect.Width / 2))
                    {
                        mud.OnHit(this, game);
                        mud.IsActive = false;
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
