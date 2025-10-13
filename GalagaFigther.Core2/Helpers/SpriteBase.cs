using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Services.Static;
using Raylib_cs;
using System.Numerics;

namespace GalagaFigther.Core2.Helpers
{
    public abstract class SpriteBase
    {
        protected Texture2D _texture;
        protected Rectangle? _source = null;

        public virtual void Update(float frameTime)
        {
        }

        public virtual void Draw(GameObject gameObject)
        {
            Draw(gameObject.Rect, gameObject.Rotation, gameObject.Color);
        }

        public virtual void Draw(Rectangle rect, float rotation = 0f, Color? color = null)
        {
            var source = _source ?? new Rectangle(0, 0, _texture.Width, _texture.Height);
            var dest = new Rectangle(rect.Position + rect.Size/2, rect.Size);
            Raylib.DrawTexturePro(_texture, source, dest, dest.Position - rect.Position, rotation, color ?? Color.White);
        }

        public SpriteBase()
        {

        }
    }

    public class StillImageSprite : SpriteBase
    {
        public StillImageSprite(string texturePath)
        {
            _texture = TextureCache.Get(texturePath);
        }
    }

    public class AnimatedImageSprite : SpriteBase
    {
        private readonly int _frameCount;
        private readonly int _frameWidth = 0;
        private readonly int _frameHeight = 0;
        private readonly float _frameLength = 0;
        private readonly int _framesPerRow;

        protected int _frameIndex = 0;
        protected float _thisFrameLength = 0;

        public AnimatedImageSprite(string texturePath, int frameCount, int frameWidth, int frameHeight, float frameLength) 
        {
            _frameCount = frameCount;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _frameLength = frameLength;

            _texture = TextureCache.Get(texturePath);

            _framesPerRow = _texture.Width / _frameWidth;
            _source = new(Vector2.Zero, new Vector2(_frameWidth, _frameHeight));
        }

        public override void Update(float frameTime)
        {
            _thisFrameLength += frameTime;
            if (_thisFrameLength < _frameLength)
                return;

            _frameIndex = (_frameIndex + 1) % _frameCount;
            _thisFrameLength = _thisFrameLength - _frameLength;

            SetSource();
        }

        protected void SetSource()
        {
            var row = _framesPerRow / _frameWidth;
            var col = _framesPerRow % _frameWidth;

            _source = new(row * _frameWidth, col * _frameHeight, _frameWidth, _frameHeight);
        }
    }

    public class NonRepeatingAnimatedImageSprite : AnimatedImageSprite
    {
        private readonly int _frameCount;

        private bool _hasStarted = false;
        private bool _hasCompleted = true;

        public NonRepeatingAnimatedImageSprite(string texturePath, int frameCount, int frameWidth, int frameHeight, float frameLength) 
            : base(texturePath, frameCount, frameWidth, frameHeight, frameLength)
        {
            _frameCount = frameCount;
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            if (_frameIndex > 0)
            { 
                _hasStarted = true;
                return;
            }
            
            //_frameIndex must be zero here
            if (_hasStarted)
            {
                _hasCompleted = true;
                _frameIndex = _frameCount - 1;
                SetSource();
            }
        }

        public bool IsComplete()
        {
            return _hasCompleted;
        }
    }
}
