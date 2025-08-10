using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Players;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using System.Numerics;

namespace SpaceTest.Models.Projectiles
{
    public class NinjaProjectile : Projectile
    {
        private readonly SpriteWrapper spriteWrapper;
        private float _trueRectX;
        private float _trueRectY;
        private float _theta;
        private float _radius = 100;
        private bool _isPlayer1 = false;
        private static Random _random = new Random();
        private float _randomTilt = 1;
        private int _frame = 0;
        private bool _isOffFrame = false;

        public NinjaProjectile(Rectangle rect, Vector2 speed, Player owner) 
            : base(rect, speed, owner) 
        {
            // Use ninja.png as an animated sprite (3 frames, 0.12s per frame)
            var texture = Raylib.LoadTexture("Sprites/Projectiles/ninja.png");
            spriteWrapper = new SpriteWrapper(texture, 3, 0.12f);

            _trueRectX = rect.X;
            _trueRectY = rect.Y;
            _theta = 0;
            _isPlayer1 = owner.IsPlayer1;
        }

        public override int Damage => 20; // Ninja do double damage

        public override Color GetColor()
        {
            return Color.White;
        }

        public override void Update(Game game)
        {
            _trueRectX += Speed.X;
            _trueRectY += Speed.Y;
            var frameTime = Raylib.GetFrameTime();
            _theta += Raylib.GetFrameTime() * (_isPlayer1 ? -1 : 1) * 360f;

            float offsetX = (float)Math.Cos(_theta) * _randomTilt;
            float offsetY = (float)Math.Sin(_theta) * _randomTilt;

            var sw = Raylib.GetScreenWidth();
            var sh = Raylib.GetScreenWidth();

            if (_isPlayer1)
            { 
                offsetX = offsetX * (_trueRectX / sw);
                offsetY = offsetY * (_trueRectX / sw);
            }
            else
            { 
                offsetX = offsetX * (sw - _trueRectX) / sw;
                offsetY = offsetY * (sw - _trueRectX) / sw;
            }

            Rect.X = _trueRectX + offsetX * _radius;
            Rect.Y = _trueRectY + offsetY * _radius;


            if (Rect.X < -Rect.Width || Rect.X > Raylib.GetScreenWidth())
                IsActive = false;

            spriteWrapper.Update(Raylib.GetFrameTime());
        }

        public override void Draw()
        {
            if (_isOffFrame)
            {
                _frame++;
                _frame = _frame % 3;
                _isOffFrame = false;
            }
            else
            {
                _isOffFrame = true;
            }
            
            //Rotate the opposite way for player 2
            _frame = _isPlayer1 ? _frame : (3 - _frame);

            // Draw animated ninja sprite
            spriteWrapper.DrawAnimated(
                new Vector2(Rect.X + Rect.Width / 2f, Rect.Y + Rect.Height / 2f),
                0f,
                Rect.Width,
                Rect.Height,
                _frame
            );
        }
    }
}