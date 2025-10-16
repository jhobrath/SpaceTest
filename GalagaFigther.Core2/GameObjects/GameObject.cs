using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Helpers;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects
{
    public abstract class GameObject
    {
        //Primary State
        public Guid Id { get; set; }
        public Rectangle Rect { get; set; }
        public Vector2 Speed { get; set; }
        public Vector2 Acceleration { get; set; }
        public virtual Vector2 Origin => TopLeft;
        public SpriteBase Sprite { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; } = Color.White;

        //Destruction
        public bool IsActive { get; set; } = true;

        //Helpers
        public float X { get { return Rect.Position.X; } set { MoveTo(x: value); } }
        public float Y { get { return Rect.Position.Y; } set { MoveTo(y: value); } }
        public float Width => Rect.Width;
        public float Height => Rect.Height;
        public Vector2 TopLeft => Rect.Position;
        public Vector2 Center => Rect.Position + Rect.Size / 2;

        public GameObject(Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite)
        {
            Id = Guid.NewGuid();
            Rect = new(position, size);
            Speed = speed;
            Sprite = sprite;
            Sprite = sprite;
        }

        public void Move(float? x = null, float? y = null) => 
            Rect = new(new Vector2(X + (x ?? 0), Y + (y ?? 0)), new Vector2(Width, Height));

        public void MoveTo(float? x = null, float? y = null) =>
            Rect = new(new Vector2(x ?? X, y ?? Y), new Vector2(Width, Height));

        public void MoveBy(float? x = null, float? y = null) =>
            Rect = new(new Vector2(X * (x ?? 0), Y * (y ?? 0)), new Vector2(Width, Height));

        public void Hurry(float? x = null, float? y = null) =>
            Speed += new Vector2(x ?? 0, y ?? 0);

        public void HurryTo(float? x = null, float? y = null) =>
            Speed = new(x ?? Speed.X, y ?? Speed.Y);

        public void HurryBy(float? x = null, float? y = null) =>
            Speed *= new Vector2(x ?? 1, y ?? 1);

        public void Accel(float? x = null, float? y = null) =>
            Acceleration += new Vector2(x ?? 0, y ?? 0);

        public void AccelTo(float? x = null, float? y = null) =>
            Acceleration = new(x ?? Acceleration.X, y ?? Acceleration.Y);

        public void AccelBy(float? x = null, float? y = null) =>
            Acceleration *= new Vector2(x ?? 1, y ?? 1);

        public virtual void Deactivate()
        {
            IsActive = false;
        }
    }
}
