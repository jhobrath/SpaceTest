using GalagaFighter.Core.Models.Collisions;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core
{
    public abstract class GameObject
    {
        public Vector2 Center => new Vector2(Rect.X + Rect.Width / 2f, Rect.Y + Rect.Height / 2f);
        public Vector2 Position => Rect.Position;

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid Owner { get { return _owner; } set { _owner = value; } }
        public Rectangle Rect => _rect;
        public float Rotation { get; set; } = 0f;
        public Vector2 Speed => _speed;
        public Color Color { get; set; } = Color.White;
        public SpriteWrapper Sprite { get; set; }
        public bool IsActive { get; set; }
        public virtual double DrawPriority => _drawPriority;

        public Hitbox? Hitbox { get; set; }

        private double _drawPriority = 1;

        private Vector2 _speed;
        private Rectangle _rect;
        private Guid _owner;

        public GameObject(Guid owner, SpriteWrapper sprite, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            _owner = owner;
            _rect = new Rectangle(initialPosition, initialSize);
            _speed = initialSpeed;
            Sprite = sprite;
            IsActive = true;
        }

        public abstract void Update(Game game);
        public abstract void Draw();

        public void Move(float? x = null, float? y = null)
        {
            _rect.X += x ?? 0f;
            _rect.Y += y ?? 0f;
        }

        public void MoveTo(float? x = null, float? y = null)
        {
            _rect.X = x ?? _rect.X;
            _rect.Y = y ?? _rect.Y;
        }

        public void Hurry(float? x = null, float? y = null)
        {
            _speed.X *= x ?? 1f;
            _speed.Y *= y ?? 1f;
        }

        public void HurryTo(float? x = null, float? y = null)
        {
            _speed.X = x ?? _speed.X;
            _speed.Y = y ?? _speed.Y;
        }

        public void Scale(float? x = null, float? y = null)
        {
            _rect.Width *= x ?? 1f;
            _rect.Height *= y ?? 1f;
        }

        public void ScaleTo(float? x = null, float? y = null)
        {
            _rect.Width = x ?? _rect.Width;
            _rect.Height = y ?? _rect.Height;
        }

        public void SetOwner(Guid id) => _owner = id;

        public void SetDrawPriority(double drawPriority) => _drawPriority = drawPriority;
    }
}
