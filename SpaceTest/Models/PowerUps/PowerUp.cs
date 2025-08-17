using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.Effects;
using GalagaFighter;
using GalagaFighter.Models;
using System.Numerics;

namespace GalagaFighter.Models.PowerUps
{
    public abstract class PowerUp : GameObject
    {
        protected Vector2 _speed ;
        protected readonly Texture2D sprite;
        private Player? _owner = null;
        private float _collectSpeed = 0f;

        public bool Available => _owner == null;

        protected PowerUp(Rectangle rect, float speed, Texture2D sprite) : base(rect)
        {
            _speed = new Vector2(0f, speed);
            this.sprite = sprite;
            IsActive = true;
        }

        public override void Update(Game game)
        {
            var frameTime = Raylib.GetFrameTime();
            Rotation += frameTime * (_owner == null ? 100f : 570f) % 360; // Rotate at 100 degrees per second

            Rect.Y += _speed.Y * frameTime;
            Rect.X += _speed.X * frameTime;


            if (Rect.Y > Raylib.GetScreenHeight())
            {
                IsActive = false;
            }

            if(_owner!= null)
            {
                _speed = GetCollectedCurrentSpeed();

                Rect.Width *= (1 - 2*frameTime);
                Rect.Height *= (1 - 2*frameTime);

                if (Math.Abs(_owner.Rect.X - Rect.X) < 50f || Rect.Width < 50f)
                {
                    var effect = CreateEffect(_owner);
                    if (effect != null)
                        _owner.Stats.AddEffect(_owner, effect);
                    
                    IsActive = false;
                }
            }
        }

        public override void Draw()
        {
            if (sprite.Id > 0)
            {
                float destHeight = Rect.Height * 1.5f;
                float destWidth = 2.1f * Rect.Height * 1.5f;
                
                Raylib.DrawTexturePro(
                    sprite,
                    new Rectangle(0, 0, sprite.Width, sprite.Height),
                    new Rectangle(Center.X, Center.Y, destWidth, destHeight),
                    new Vector2(destWidth / 2f, destHeight / 2f),
                    Rotation,
                    Color.White
                );
            }
            else
            {
                Raylib.DrawRectangleRec(Rect, GetColor());
            }
        }

        public abstract PlayerEffect CreateEffect(Player player);
        protected abstract Color GetColor();

        public void Collect(Player owner)
        {
            _owner = owner;
            _collectSpeed = Math.Max(600f, Vector2.Distance(_owner.Center, Center));
            _speed = GetCollectedCurrentSpeed();
        }

        private Vector2 GetCollectedCurrentSpeed()
        {
            var distanceX = Math.Abs(_owner.Center.X - Center.X);
            var distanceY = Math.Abs(_owner.Center.Y - Center.Y);   

            var xPct = distanceX / (distanceX + distanceY);
            var yPct = distanceY / (distanceX + distanceY);

            return new Vector2(_collectSpeed * xPct * (_owner.Center.X < Center.X ? -1 : 1)*2, _collectSpeed*yPct*(_owner.Center.Y < Center.Y ? -1 : 1)*2);

        }
    }
}
