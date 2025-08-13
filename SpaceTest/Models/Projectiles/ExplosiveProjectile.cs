using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Effects;
using GalagaFighter.Models.Players;
using GalagaFigther;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using System;
using System.Numerics;

namespace SpaceTest.Models.Projectiles
{
    public class ExplosiveProjectile : Projectile
    {
        private const int ExplosionRadius = 200;

        private float _width = 0f;
        private float _height = 0f;
        private float _originalWidth = 0f;
        private float _originalHeight = 0f;
        private float _growthPercentage = 6f;

        public override bool DestroyOnPowerUp =>  false;

        public ExplosiveProjectile(Rectangle rect, Vector2 speed, Player owner, ProjectileEffect ownerEffect)
            : base(rect, speed, owner, ownerEffect)
        {
            sprite = TextureLibrary.Get("Sprites/Projectiles/explosion.png");
            sprite.Width = Convert.ToInt32(rect.Width);
            sprite.Height = Convert.ToInt32(rect.Height);
            _width = rect.Width;
            _height = rect.Height;
            _originalWidth = _width;
            _originalHeight = _height;
        }

        public override int Damage => 20; // Increased damage for explosive projectile

        public static int ExplosionRadius1 => ExplosionRadius;

        public override void OnHit(Player target, Game game)
        {
            // Deal damage to the target
            target.TakeDamage(Damage);
            
            game.PlayHitSound(); // Could add a special explosion sound here
        }

        public override void Update(Game game)
        {
            var centerX = Rect.X + Rect.Width / 2;
            var centerY = Rect.Y + Rect.Height / 2;
            var percentageAcross = centerX / Raylib.GetScreenWidth();
            var comparableSize = Owner.IsPlayer1 ? _growthPercentage * percentageAcross : _growthPercentage * (1 - percentageAcross);
            var newWidth = Math.Max(_originalWidth, comparableSize * _originalWidth);
            var newHeight = Math.Max(_originalHeight, comparableSize * _originalHeight);

            Rect.X = centerX - newWidth / 2;
            Rect.Y = centerY - newHeight / 2;
            Rect.Width = newWidth;
            Rect.Height = newHeight;

            sprite.Width = Convert.ToInt32(newWidth);
            sprite.Height = Convert.ToInt32(newHeight);

            base.Update(game);

            //Rect.X *= 1 + 2 * Raylib.GetFrameTime();
            //Rect.Y *= 1 + 2 * Raylib.GetFrameTime();
        }

        public override Color GetColor()
        {
            return Color.Orange; // Distinctive explosive color
        }

        public override void Draw()
        {
            Raylib.DrawTexture(sprite, (int)Rect.X, (int)Rect.Y, Color.White);
        }
    }
}