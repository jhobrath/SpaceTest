using GalagaFighter;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core
{
    public enum SpriteMode { Drawn, StillImage, Animation }

    public class SpriteWrapper
    {
        public SpriteMode Mode { get; }
        public Texture2D Texture { get; }
        public Color Color { get; set; }

        private int _frameCount { get; }
        private float _frameDuration { get; }
        private bool _framesComplete { get; set; } = false;
        private int _currentFrame { get; set; } = 0;
        private float _animationTimer;
        private bool _frameRepeat;
        private readonly Action<Vector2, float, float, float, float>? _drawAction = null;

        // For drawn mode
        public SpriteWrapper(Action<Vector2, float, float, float, float> drawAction)
        {
            Mode = SpriteMode.Drawn;
            _drawAction = drawAction;
        }

        // For still image
        public SpriteWrapper(string texturePath)
        {
            Mode = SpriteMode.StillImage;
            Texture = TextureService.Get(texturePath);
        }

        // For still image with palette swap
        public SpriteWrapper(string texturePath, Color paletteSwapColor)
        {
            Mode = SpriteMode.StillImage;
            var originalTexture = TextureService.Get(texturePath);
            Texture = PaletteSwapService.CreatePaletteSwappedTexture(originalTexture, paletteSwapColor);
        }

        // For still image
        public SpriteWrapper(Texture2D texture)
        {
            Mode = SpriteMode.StillImage;
            Texture = texture;
        }

        // For animation
        public SpriteWrapper(Texture2D texture, int frameCount, float frameDuration, bool repeat = true)
        {
            Mode = SpriteMode.Animation;
            Texture = texture;
            _frameCount = frameCount;
            _frameDuration = frameDuration;
            _frameRepeat = repeat;
            _animationTimer = 0f;
        }

        // For rendering a specific frame of a texture (static frame from a spritesheet)
        public SpriteWrapper(Texture2D texture, int frameIndex, int frameCount)
        {
            Mode = SpriteMode.StillImage;
            Texture = texture;
            _frameCount = frameCount;
            _currentFrame = frameIndex;
        }

        public void Update(float frameTime)
        {
            if (Mode == SpriteMode.Animation)
            {
                if (_framesComplete)
                    return;

                _animationTimer += frameTime;
                if (_animationTimer >= _frameDuration)
                {
                    _animationTimer -= _frameDuration;
                    _currentFrame++;
                    if (!_frameRepeat && _currentFrame >= _frameCount)
                    {
                        _framesComplete = true;
                        _currentFrame = _frameCount - 1;
                        return;
                    }
                    _currentFrame %= _frameCount;
                }
            }
        }

        public void DrawFromTopLeft(Vector2 position, float rotation, float width, float height, Color? color = null)
        {
            var center = new Vector2(position.X + width / 2f, position.Y + height / 2f);
            Draw(center, rotation, width, height, color);
        }

        public void Draw(Vector2 position, float rotation, float width, float height, Color? color = null)
        {
            switch (Mode)
            {
                case SpriteMode.Drawn:
                    _drawAction?.Invoke(position, rotation, width, height, 1f);
                    break;
                case SpriteMode.StillImage:
                    // If _frameCount > 1, draw only the specified frame
                    if (_frameCount > 1)
                    {
                        float frameWidth = Texture.Width / (float)_frameCount;
                        Rectangle source = new Rectangle(frameWidth * _currentFrame, 0, frameWidth, Texture.Height);
                        Raylib.DrawTexturePro(
                            Texture,
                            source,
                            new Rectangle(position.X, position.Y, width, height),
                            new Vector2(width / 2, height / 2),
                            rotation,
                            color ?? Color.White);
                    }
                    else
                    {
                        Raylib.DrawTexturePro(
                            Texture,
                            new Rectangle(0, 0, Texture.Width, Texture.Height),
                            new Rectangle(position.X, position.Y, width, height),
                            new Vector2(width / 2, height / 2),
                            rotation,
                            color ?? Color.White);
                    }
                    break;
                case SpriteMode.Animation:
                    DrawAnimated(position, rotation, width, height, color);
                    break;
            }
        }

        private void DrawAnimated(Vector2 position, float rotation, float width, float height, Color? color = null)
        {
            float frameWidth = Texture.Width / (float)_frameCount;
            Rectangle source = new Rectangle(frameWidth * _currentFrame, 0, frameWidth, Texture.Height);
            Raylib.DrawTexturePro(
                Texture,
                source,
                new Rectangle(position.X, position.Y, width, height),
                new Vector2(width / 2f, height / 2f),
                rotation,
                color ?? Color.White);
        }
    }
}
