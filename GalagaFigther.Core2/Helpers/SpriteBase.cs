using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core2.Helpers
{
    public abstract class SpriteBase
    {
        protected Lazy<Texture2D> _texture;
        protected Func<Texture2D> _textureFactory;
        protected Rectangle? _source = null;
        
        private PaletteSwap? _paletteSwap;
        
        /// <summary>
        /// Palette swap configuration for this sprite. Setting this will invalidate the lazy texture.
        /// </summary>
        public PaletteSwap? PaletteSwap 
        { 
            get => _paletteSwap;
            set 
            {
                if (_paletteSwap?.Equals(value) == true) return;
                
                _paletteSwap = value;
                InvalidateTexture();
            }
        }

        /// <summary>
        /// Gets the current texture, creating it lazily if needed
        /// </summary>
        protected Texture2D CurrentTexture => _texture.Value;

        /// <summary>
        /// Sets the palette swap using a simple from-to color approach
        /// </summary>
        public void SetPaletteSwap(Color fromColor, Color toColor)
        {
            PaletteSwap = Models.PaletteSwap.CreateSwap(fromColor, toColor);
        }

        /// <summary>
        /// Clears the palette swap, reverting to the original texture
        /// </summary>
        public void ClearPaletteSwap()
        {
            PaletteSwap = null;
        }

        /// <summary>
        /// Invalidates the current texture, forcing regeneration on next access
        /// </summary>
        protected virtual void InvalidateTexture()
        {
            _texture = new Lazy<Texture2D>(_textureFactory);
        }

        protected void SetTextureFactory(Func<Texture2D> textureFactory)
        {
            _textureFactory = textureFactory;
            InvalidateTexture();
        }

        public virtual void Update(float frameTime)
        {
        }

        public virtual void Draw(GameObject gameObject)
        {
            var center = gameObject.WorldPosition; // (0,0)
            var size = gameObject.Rect.Size;
            var rect = new Rectangle(center, size); // (0,0), (168,168)
            Draw(rect, gameObject.WorldRotation, gameObject.Color);
        }

        public virtual void Draw(Rectangle rect, float rotation = 0f, Color? color = null)
        {
            var texture = CurrentTexture;
            var source = _source ?? new Rectangle(0, 0, texture.Width, texture.Height);
            Raylib.DrawTexturePro(texture, source, rect, rect.Size / 2, rotation, color ?? Color.White);
        }

        public SpriteBase()
        {
        }
    }

    public class DrawnSprite : SpriteBase
    {
        public DrawnSprite(Texture2D texture)
        {
            SetTextureFactory(() => texture);
        }

        public DrawnSprite(Func<Color?, Texture2D> colorAwareTextureFactory)
        {
            SetTextureFactory(() => colorAwareTextureFactory(PaletteSwap?.TargetColor));
        }
    }

    public class AnimatedDrawnSprite : SpriteBase
    {
        private readonly Func<float, float, Color, int, Texture2D> _drawFunction;
        private readonly Vector2 _size;
        private readonly int _frameCount;
        private readonly float _frameLength = 0;

        protected int _frameIndex = 0;
        protected float _thisFrameLength = 0;

        public AnimatedDrawnSprite(Vector2 size, int frameCount, float frameLength, Func<float, float, Color, int, Texture2D> drawFunction)
        {
            _drawFunction = drawFunction;
            _size = size;
            _frameCount = frameCount;
            _frameLength = frameLength;

            _texture = new Lazy<Texture2D>(() => _drawFunction(_size.X, _size.Y, PaletteSwap?.TargetColor ?? Color.Red, _frameIndex));
            _source = new Rectangle(0, 0, _size);

            SetTextureFactory(() => 
                _drawFunction(_size.X, _size.Y, PaletteSwap?.TargetColor ?? Color.Red, _frameIndex)
            );
        }

        public override void Update(float frameTime)
        {
            _thisFrameLength += frameTime;
            if (_thisFrameLength < _frameLength)
                return;

            _frameIndex = (_frameIndex + 1) % _frameCount;
            _thisFrameLength = _thisFrameLength - _frameLength;

            var texture = _drawFunction(_size.X, _size.Y, PaletteSwap?.TargetColor ?? Color.Red, _frameIndex);
            _texture = new Lazy<Texture2D>(() => texture);
            _source = new Rectangle(0, 0, _size);
        }
    }

    public class StillImageSprite : SpriteBase
    {
        private readonly string _texturePath;

        public StillImageSprite(string texturePath)
        {
            _texturePath = texturePath;
            SetTextureFactory(() => 
            {
                var originalTexture = TextureCache.Get(_texturePath);
                return PaletteSwap == null 
                    ? originalTexture 
                    : PaletteSwapService.CreatePaletteSwappedTexture(originalTexture, PaletteSwap);
            });
        }
    }

    public class AnimatedImageSprite : SpriteBase
    {
        private readonly string _texturePath;
        private readonly int _frameCount;
        private readonly int _frameWidth = 0;
        private readonly int _frameHeight = 0;
        private readonly float _frameLength = 0;
        private int _framesPerRow = 1;

        protected int _frameIndex = 0;
        protected float _thisFrameLength = 0;

        public AnimatedImageSprite(string texturePath, int frameCount, int frameWidth, int frameHeight, float frameLength) 
        {
            _frameCount = frameCount;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _frameLength = frameLength;
            _texturePath = texturePath;

            SetTextureFactory(() => 
            {
                var originalTexture = TextureCache.Get(_texturePath);
                var texture = PaletteSwap == null 
                    ? originalTexture 
                    : PaletteSwapService.CreatePaletteSwappedTexture(originalTexture, PaletteSwap);

                _framesPerRow = texture.Width / _frameWidth;
                return texture;
            });

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
            var row = _frameIndex / _framesPerRow;
            var col = _frameIndex % _framesPerRow;

            _source = new(col * _frameWidth, row * _frameHeight, _frameWidth, _frameHeight);
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

    public class SpriteCanvas
    {
        public SpriteCanvas()
        {
        }
    }

    public class NonRepeatingAnimatedDrawnSprite : AnimatedDrawnSprite
    {
        private readonly int _frameCount;

        private bool _hasStarted = false;
        private bool _hasCompleted = true;

        public NonRepeatingAnimatedDrawnSprite(Vector2 size, int frameCount, float frameLength, Func<float, float, Color, int, Texture2D> drawFunction)
            : base(size, frameCount, frameLength, drawFunction)
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

            if (_hasStarted)
            {
                _hasCompleted = true;
                _frameIndex = _frameCount - 1;
            }
        }

        public bool IsComplete()
        {
            return _hasCompleted;
        }
    }
}
