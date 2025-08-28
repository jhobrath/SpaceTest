using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Collisions
{
    public abstract class Collision : GameObject
    {
        protected virtual float SpeedDecreaseFactor => 5f;
        protected virtual bool FadeOut => true;
        protected virtual float Duration => 1f; // Default duration for fade-out

        private float _lifetimeThusFar = 0f;
        private float _speedDecreaseFactor = 0f;
        private Player? _gluedTo;
        private float? _gluedOffsetY;

        public Collision(Guid owner, SpriteWrapper sprite, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(owner, sprite, initialPosition, initialSize, new Vector2(Math.Clamp(initialSpeed.X, -500f, 500f),0f))
        {
            SetDrawPriority(5);
            Rotation = 360f * (float)Game.Random.NextDouble();
            var randomScaleFactor = (float)Game.Random.NextDouble() * .5f + .75f;
            Scale(randomScaleFactor, randomScaleFactor);
            _speedDecreaseFactor = SpeedDecreaseFactor * (float)(Game.Random.NextDouble() * .5 + .75f);
        }

        public override void Update(Game game)
        {
            var frameTime = Raylib.GetFrameTime();
            UpdatePosition(frameTime);
            UpdateColor(frameTime);
            UpdateGluedVerticalPosition();
            Sprite.Update(frameTime);
            _lifetimeThusFar += frameTime;
            if (_lifetimeThusFar >= Duration)
                IsActive = false;
        }

        private void UpdatePosition(float frameTime)
        {
            var speedChange = 1 - SpeedDecreaseFactor * frameTime;
            Hurry(x: speedChange);
            Move(x: Speed.X * frameTime);
        }

        private void UpdateColor(float frameTime)
        {
            if (!FadeOut)
                return;

            var pctThru = _lifetimeThusFar / Duration;
            if (pctThru < 0.5f)
                return;
            var pctToFade = Math.Clamp(1 - ((pctThru - 0.5f) / 0.5f), 0, 1);
            int alpha = Convert.ToInt32(255f * pctToFade);
            Color = new Color(Color.R, Color.G, Color.B, alpha);
        }

        public override void Draw()
        {
            Sprite.Draw(Center, 0f, Rect.Width, Rect.Height, Color);
        }

        public void GlueVerticallyTo(Player target)
        {
            _gluedTo = target;
            _gluedOffsetY = Rect.Y - _gluedTo.Rect.Y;
        }

        protected void UpdateGluedVerticalPosition()
        {
            if (_gluedTo == null || !_gluedOffsetY.HasValue)
                return;
            var newCenter = _gluedTo.Rect.Y + _gluedOffsetY.Value;
            MoveTo(y: newCenter);
        }
    }
}
