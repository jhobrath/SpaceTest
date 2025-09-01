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
        public bool Repeat { get; set; }

        private int _frameCount;
        private float _frameDuration;
        private int _currentFrame;
        private float _animationTimer;
        private readonly Action<Vector2, float, float, float, float>? _drawAction = null;
        private readonly Action<Vector2, float, float, float, float, int>? _drawAnimatedAction = null;

        private bool _reachedLastFrame = false;

        // For static drawn mode
        public SpriteWrapper(Action<Vector2, float, float, float, float> drawAction)
        {
            Mode = SpriteMode.Drawn;
            _drawAction = drawAction;
            _frameCount = 1;
            _frameDuration = 0f;
            _currentFrame = 0;
        }

        // For animated drawn mode
        public SpriteWrapper(Action<Vector2, float, float, float, float, int> drawAnimatedAction, int frameCount, float frameDuration)
        {
            Mode = SpriteMode.Drawn;
            _drawAnimatedAction = drawAnimatedAction;
            _frameCount = frameCount;
            _frameDuration = frameDuration;
            _currentFrame = 0;
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
        public SpriteWrapper(Texture2D texture, int frameCount, float frameDuration, bool repeat = false)
        {
            Mode = SpriteMode.Animation;
            Texture = texture;
            _frameCount = frameCount;
            _frameDuration = frameDuration;
            _currentFrame = 0;
            Repeat = repeat;
        }

        public void Update(float frameTime)
        {
            if ((Mode == SpriteMode.Animation || (Mode == SpriteMode.Drawn && _frameCount > 1)) && _frameCount > 1)
            {
                _animationTimer += frameTime;
                if (_animationTimer >= _frameDuration)
                {
                    _animationTimer -= _frameDuration;

                    if (!Repeat && _currentFrame == _frameCount - 1)
                        _reachedLastFrame = true;
                    else if (!Repeat && _reachedLastFrame && _currentFrame != _frameCount - 1)
                        _currentFrame = _frameCount - 1;
                    else 
                        _currentFrame = (_currentFrame + 1) % _frameCount;
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
                    if (_drawAnimatedAction != null)
                        _drawAnimatedAction.Invoke(position, rotation, width, height, 1f, _currentFrame);
                    else
                        _drawAction?.Invoke(position, rotation, width, height, 1f);
                    break;
                case SpriteMode.StillImage:
                    Raylib.DrawTexturePro(
                        Texture,
                        new Rectangle(0, 0, Texture.Width, Texture.Height),
                        new Rectangle(position.X, position.Y, width, height),
                        new Vector2(width / 2, height / 2),
                        rotation,
                        color ?? Color.White);
                    break;
                case SpriteMode.Animation:
                    DrawAnimated(position, rotation, width, height, color);
                    break;
            }
        }

        private void DrawAnimated(Vector2 position, float rotation, float width, float height, Color? color = null)
        {
            float frameWidth = Texture.Width / (float)_frameCount;
            Rectangle source = new(frameWidth * _currentFrame, 0, frameWidth, Texture.Height);
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
