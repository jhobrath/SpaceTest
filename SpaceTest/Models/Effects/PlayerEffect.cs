using GalagaFighter.Models.Players;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public abstract class PlayerEffect
    {
        protected readonly Player Player;
        public bool IsActive { get; protected set; } = true;

        protected float _remainingTime = 0f;
        private bool _lastFrameWasZero = false;
        private float _decorationWidth = 0f;
        private float _decorationHeight = 0f;
        private float _decorationRotation = 0f;
        private Vector2 _decorationPosition = Vector2.Zero;

        public virtual float FireRateMultiplier => 1f;

        private static Random _random = new Random();

        // Speed multiplier for stacking movement effects
        public virtual float SpeedMultiplier => 1.0f;

        protected virtual float Duration => 0f;

        public virtual bool AllowSelfStacking => true;
        public virtual bool DisableShooting => false;

        public abstract string IconPath { get; }
        public virtual int? TextureFrame => null;

        protected virtual SpriteWrapper? Decoration => null;

        public virtual void OnStatsSwitch()
        {

        }

        protected PlayerEffect(Player player)
        {
            Player = player;
            _remainingTime = Duration;
        }

        public virtual void OnActivate() { }
        public virtual void OnUpdate(float frameTime) 
        {
            if (Duration != 0f)
            {
                _remainingTime -= frameTime;
                if (_remainingTime <= 0)
                    IsActive = false;
            }

            if (Decoration != null)
                Decoration.Update(frameTime);
        }
        public virtual void OnDeactivate() { }
        public virtual void OnShoot(Game game) { }

        public virtual void OnDraw()
        {
            if (Decoration != null )
            {
                if(Decoration.CurrentFrame == 0 && !_lastFrameWasZero)
                {
                    _lastFrameWasZero = true;
                    _decorationWidth = Math.Min(Math.Max(Player.Rect.Width/10, Player.Rect.Width * (float)_random.NextDouble()), Player.Rect.Width/5);
                    _decorationHeight = Math.Min(Math.Max(Player.Rect.Height * (float)_random.NextDouble(), Player.Rect.Height/10), Player.Rect.Width/5);
                    _decorationHeight = _decorationWidth;
                    _decorationPosition = new Vector2(
                        Player.Rect.Position.X + (Player.Rect.Width - _decorationWidth) * (float)_random.NextDouble(), 
                        Player.Rect.Position.Y + (Player.Rect.Height - _decorationHeight) * (float)_random.NextDouble()
                    );
                    _decorationRotation = 360f * (float)_random.NextDouble();
                }

                if(Decoration.CurrentFrame != 0)
                {
                    _lastFrameWasZero = false;
                }
                
                Decoration.DrawFromTopLeft(_decorationPosition, 0f, _decorationWidth, _decorationHeight);
            }
        }

        public bool ShouldDeactivate() => !IsActive;


        public virtual void ModifyPlayerRendering(PlayerRendering playerRendering)
        {
        }

        public void SetMaxRemainingTime(float duration)
        {
            _remainingTime = Math.Min(_remainingTime, duration);
        }
    }
}