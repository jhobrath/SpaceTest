using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Effects;
using GalagaFighter.Models.Players;
using GalagaFigther;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using System.Numerics;

namespace SpaceTest.Models.Projectiles
{
    public class ExplosiveProjectile : Projectile
    {
        private readonly float _originalWidth = 0f;
        private readonly float _originalHeight = 0f;
        private float _rotationAmount;
        private float _rotation;
        private const float _growthPercentage = 6f;
        private int _frame = 0;
        private static Random _random = new Random();

        public override bool DestroyOnPowerUp =>  false;

        private readonly SpriteWrapper _spriteWrapper;

        public ExplosiveProjectile(Rectangle rect, Vector2 speed, Player owner, ProjectileEffect ownerEffect)
            : base(rect, speed, owner, ownerEffect)
        {
            var texture = TextureLibrary.Get("Sprites/Projectiles/explosion.png");
            _spriteWrapper = new SpriteWrapper(texture, 2, 0.12f);

            sprite.Width = Convert.ToInt32(rect.Width);
            sprite.Height = Convert.ToInt32(rect.Height);
            _originalWidth = rect.Width;
            _originalHeight = rect.Height;

            _rotationAmount = (float)_random.NextDouble() * 20f - 10f;
            _rotation = 0f;
        }

        public override List<PlayerEffect> GetEffects(Player target) => new List<PlayerEffect>
        {
            new BurningEffect(target)
        };

        public override int Damage => 20; // Increased damage for explosive projectile

        public override void Update(Game game)
        {
            var centerX = Rect.X + Rect.Width / 2;
            var centerY = Rect.Y + Rect.Height / 2;
            var percentageAcross = centerX / Raylib.GetScreenWidth();
            var comparableSize = Owner.IsPlayer1 ? _growthPercentage * percentageAcross : _growthPercentage * (1 - percentageAcross);
            var newWidth = Math.Max(_originalWidth, comparableSize * _originalWidth);
            var newHeight = Math.Max(_originalHeight, comparableSize * _originalHeight);


            percentageAcross = Owner.IsPlayer1 ? percentageAcross : (1 - percentageAcross);
            if (percentageAcross > .5 && _frame == 0)
            {
                Game.PlayExplosionConversionSound();
                _frame = 1;
                _rotationAmount *= .25f;
            }
            
            _rotation = (_rotation + _rotationAmount) % 360;

            Rect.X = centerX - newWidth / 2;
            Rect.Y = centerY - newHeight / 2;
            Rect.Width = newWidth;
            Rect.Height = newHeight;

            sprite.Width = Convert.ToInt32(newWidth);
            sprite.Height = Convert.ToInt32(newHeight);

            base.Update(game);
        }

        public override Color GetColor()
        {
            return Color.Orange; // Distinctive explosive color
        }

        public override void Draw()
        {
            // Draw animated ninja sprite
            _spriteWrapper.DrawAnimated(
                new Vector2(Rect.X + Rect.Width / 2f, Rect.Y + Rect.Height / 2f),
                _rotation,
                Rect.Width,
                Rect.Height,
                _frame
            );
        }
    }
}