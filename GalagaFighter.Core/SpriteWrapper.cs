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

        public int FrameCount { get; }
        public float FrameDuration { get; }
        public bool FramesComplete { get; private set; } = false;
        public Color Color { get; set; }

        private float _animationTimer;
        private bool _frameRepeat;
        public int CurrentFrame;
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
            FrameCount = frameCount;
            FrameDuration = frameDuration;
            _frameRepeat = repeat;
            _animationTimer = 0f;
            CurrentFrame = 0;
        }

        public void Update(float frameTime)
        {
            if (Mode == SpriteMode.Animation)
            {
                if (FramesComplete)
                    return;

                _animationTimer += frameTime;
                
                if (_animationTimer >= FrameDuration)
                {
                    _animationTimer -= FrameDuration;
                    CurrentFrame++;
                    
                    if(!_frameRepeat && CurrentFrame >= FrameCount)
                    { 
                        FramesComplete = true;
                        CurrentFrame--;
                        return;
                    }

                    CurrentFrame %= FrameCount;
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
                    Raylib.DrawTexturePro(
                        Texture,
                        new Rectangle(0, 0, Texture.Width, Texture.Height),
                        new Rectangle(position.X, position.Y, width, height),
                        new Vector2(width/2, height/2),
                        rotation,
                        color ?? Color.White);
                    break;
                case SpriteMode.Animation:
                    DrawAnimated(position, rotation, width, height, color: color);
                    break;
            }
        }

        public void DrawAnimated(Vector2 position, float rotation, float width, float height, int? frame = null, Color? color = null)
        {
            if(FrameCount == 8)
            {
                var s = "";
            }

            frame = frame ?? CurrentFrame;
            float frameWidth = Texture.Width / (float)FrameCount;
            Rectangle source = new Rectangle(frameWidth * frame.Value, 0, frameWidth, Texture.Height);
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
