using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Particles
{
    public class Particle : GameObject
    {
        public float Lifetime { get; private set; }
        public float MaxLifetime { get; private set; }
        public Vector2 Acceleration { get; set; }
        public float StartSize { get; private set; }
        public float EndSize { get; private set; }
        public Color StartColor { get; private set; }
        public Color EndColor { get; private set; }
        public float FadeOutTime { get; set; } = 0.2f; // Time before particle fades out
        public bool UseGravity { get; set; } = false;
        public float GravityStrength { get; set; } = 98f; // Pixels per second squared
        public float Drag { get; set; } = 0f; // Air resistance (0 = no drag, 1 = full drag)

        private float _currentSize;
        private Color _currentColor;

        public Particle(Guid owner, SpriteWrapper sprite, Vector2 initialPosition, Vector2 initialSize, 
                       Vector2 initialSpeed, float lifetime, Color startColor, Color endColor, 
                       float startSize, float endSize) 
            : base(owner, sprite, initialPosition, initialSize, initialSpeed)
        {
            MaxLifetime = lifetime;
            Lifetime = 0f;
            StartColor = startColor;
            EndColor = endColor;
            StartSize = startSize;
            EndSize = endSize;
            _currentSize = startSize;
            _currentColor = startColor;
            SetDrawPriority(0.5); // Draw particles behind most objects but in front of background
        }

        public override void Update(Game game)
        {
            float frameTime = Raylib.GetFrameTime();
            Lifetime += frameTime;

            // Check if particle should be deactivated
            if (Lifetime >= MaxLifetime)
            {
                IsActive = false;
                return;
            }

            // Apply gravity if enabled
            if (UseGravity)
            {
                Acceleration = new Vector2(Acceleration.X, Acceleration.Y + GravityStrength * frameTime);
            }

            // Apply acceleration to speed
            HurryTo(Speed.X + Acceleration.X * frameTime, Speed.Y + Acceleration.Y * frameTime);

            // Apply drag
            if (Drag > 0)
            {
                float dragForce = 1f - (Drag * frameTime);
                dragForce = Math.Max(0f, dragForce); // Prevent negative drag
                HurryTo(Speed.X * dragForce, Speed.Y * dragForce);
            }

            // Move particle
            Move(Speed.X * frameTime, Speed.Y * frameTime);

            // Update visual properties based on lifetime
            UpdateVisualProperties();

            // Update sprite animation if applicable
            Sprite?.Update(frameTime);
        }

        private void UpdateVisualProperties()
        {
            float progress = Lifetime / MaxLifetime;
            
            // Interpolate size
            _currentSize = StartSize + (EndSize - StartSize) * progress;
            ScaleTo(_currentSize, _currentSize);

            // Interpolate color
            _currentColor = new Color(
                (int)(StartColor.R + (EndColor.R - StartColor.R) * progress),
                (int)(StartColor.G + (EndColor.G - StartColor.G) * progress),
                (int)(StartColor.B + (EndColor.B - StartColor.B) * progress),
                (int)(StartColor.A + (EndColor.A - StartColor.A) * progress)
            );

            // Apply fade out effect near end of life
            if (Lifetime >= MaxLifetime - FadeOutTime)
            {
                float fadeProgress = (MaxLifetime - Lifetime) / FadeOutTime;
                _currentColor = new Color(_currentColor.R, _currentColor.G, _currentColor.B, 
                                        (int)(_currentColor.A * fadeProgress));
            }

            Color = _currentColor;
        }

        public override void Draw()
        {
            if (Sprite != null)
            {
                Sprite.Draw(Center, Rotation, Rect.Width, Rect.Height, _currentColor);
            }
            else
            {
                // Fallback: draw as a simple colored rectangle
                Raylib.DrawRectanglePro(
                    new Rectangle(Center.X, Center.Y, Rect.Width, Rect.Height),
                    new Vector2(Rect.Width / 2f, Rect.Height / 2f),
                    Rotation,
                    _currentColor
                );
            }
        }

        // Helper method to get progress (0.0 to 1.0)
        public float GetLifetimeProgress() => Lifetime / MaxLifetime;

        // Helper method to check if particle is fading out
        public bool IsFadingOut() => Lifetime >= MaxLifetime - FadeOutTime;
    }
}