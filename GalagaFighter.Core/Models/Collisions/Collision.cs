using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Collisions
{
    public abstract class Collision : GameObject
    {
        protected virtual float SpeedDecreaseFactor => 15f;
        protected virtual bool FadeOut => true;

        private float _lifetimeThusFar = 0f;
        private float _speedDecreaseFactor = 0f;

        private Player? _gluedTo;
        private float? _gluedOffsetY;

        public Collision(Guid owner, SpriteWrapper sprite, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(owner, sprite, initialPosition, initialSize, new Vector2(Math.Clamp(initialSpeed.X, -50f, 50f),0f))
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

            if (Sprite.FramesComplete)
                IsActive = false;

            _lifetimeThusFar += frameTime;
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

            var lifetimeDuration = (Sprite.FrameCount * Sprite.FrameDuration);
            var pctThru = _lifetimeThusFar / lifetimeDuration;

            if (pctThru < .75f)
                return;

            var pctToFade = Math.Clamp(1 - ((pctThru - .75f) / .25f), 0, 1);
            Color = new Color(255, 255, 255, Convert.ToInt32(100f * pctToFade));
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
