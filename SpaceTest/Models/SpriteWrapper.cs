using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Models
{
    public enum SpriteMode { Drawn, StillImage, Animation }

    public class SpriteWrapper
    {
        public SpriteMode Mode { get; }
        public Texture2D Texture { get; }
        public int FrameCount { get; }
        public float FrameDuration { get; }
        private float _animationTimer;
        private int _currentFrame;
        private readonly Action<Vector2, float, float, float, float> _drawAction;

        // For drawn mode
        public SpriteWrapper(Action<Vector2, float, float, float, float> drawAction)
        {
            Mode = SpriteMode.Drawn;
            _drawAction = drawAction;
        }

        // For still image
        public SpriteWrapper(Texture2D texture)
        {
            Mode = SpriteMode.StillImage;
            Texture = texture;
        }

        // For animation
        public SpriteWrapper(Texture2D texture, int frameCount, float frameDuration)
        {
            Mode = SpriteMode.Animation;
            Texture = texture;
            FrameCount = frameCount;
            FrameDuration = frameDuration;
            _animationTimer = 0f;
            _currentFrame = 0;
        }

        public void Update(float frameTime)
        {
            if (Mode == SpriteMode.Animation)
            {
                _animationTimer += frameTime;
                if (_animationTimer >= FrameDuration)
                {
                    _animationTimer -= FrameDuration;
                    _currentFrame = (_currentFrame + 1) % FrameCount;
                }
            }
        }

        public void Draw(Vector2 position, float rotation, float width, float height)
        {
            switch (Mode)
            {
                case SpriteMode.Drawn:
                    _drawAction?.Invoke(position, rotation, width, height, 1f);
                    break;
                case SpriteMode.StillImage:
                    Raylib.DrawTexturePro(
                        Texture,
                        new Rectangle(0, 0, Texture.Width, Texture.Height),
                        new Rectangle(position.X, position.Y, width, height),
                        new Vector2(width / 2f, height / 2f),
                        rotation,
                        Color.White);
                    break;
                case SpriteMode.Animation:
                    float frameWidth = Texture.Width / (float)FrameCount;
                    Rectangle source = new Rectangle(frameWidth * _currentFrame, 0, frameWidth, Texture.Height);
                    Raylib.DrawTexturePro(
                        Texture,
                        source,
                        new Rectangle(position.X, position.Y, width, height),
                        new Vector2(width / 2f, height / 2f),
                        rotation,
                        Color.White);
                    break;
            }
        }
    }
}
