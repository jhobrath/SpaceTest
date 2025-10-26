using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
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
        public Guid Owner { get; set; }

        public Rectangle Rect { get; set; }
        public Vector2 Speed { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; } = Color.White;
        
        public Vector2 Acceleration { get; set; }
        public float AngularVelocity { get; set; }
        public virtual Vector2 Drag => Vector2.Zero;
        
        public virtual Vector2 Origin => TopLeft;
        public SpriteBase Sprite { get; set; }
        public Vector2[]? Bounds { get; set;  }

        private Color _palette = Color.White; // Default to white (no change)

        private List<SpriteDecoration> Decorations { get; set; } = [];

        /// <summary>
        /// The palette color for this game object (Raylib Color).
        /// Setting this will automatically update the sprite's color using red-to-color palette swapping.
        /// Set to Color.White to disable palette swapping.
        /// </summary>
        public Color Palette 
        { 
            get => _palette;
            set 
            {
                if (_palette.Equals(value)) return; // No change needed
                
                _palette = value;
                
                // Automatically update the sprite's palette swap
                // Only apply if not white (white = no palette swap)
                if (value.Equals(Color.White))
                {
                    Sprite?.ClearPaletteSwap();
                }
                else
                {
                    // Default assumption: swap red to the new color (common for ships/objects)
                    Sprite?.SetPaletteSwap(Color.Red, value);
                }
            }
        }

        //World Position
        public Vector2 WorldPosition { get; set; }
        public float WorldRotation { get; set; }

        //Destruction
        public bool IsActive { get; set; } = true;

        //Helpers
        public float X { get { return Rect.Position.X; } set { MoveTo(x: value); } }
        public float Y { get { return Rect.Position.Y; } set { MoveTo(y: value); } }
        public float Width => Rect.Width;
        public float Height => Rect.Height;
        public Vector2 TopLeft => WorldPosition;
        public Vector2 Center => WorldPosition + Rect.Size / 2;

        public GameObject(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite)
        {
            Id = Guid.NewGuid();
            Owner = owner;
            Rect = new(position, size);
            Speed = speed;
            Sprite = sprite;
            Sprite = sprite;

            WorldPosition = Rect.Position;
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

        public void Scale(float? x = null, float? y = null) =>
            ScaleTo(Width + (x ?? 0), Height + (y ?? 0));

        public void ScaleTo(float? x = null, float? y = null)
        {
            var offset = new Vector2(x ?? Width, y ?? Height) - Rect.Size;
            Rect = new(Rect.Position - offset/2, Rect.Size + offset);
        }

        public void ScaleBy(float? x = null, float? y = null) =>
            ScaleTo(Width * (x ?? 1), Height * (x ?? 1));

        public virtual void Deactivate()
        {
            IsActive = false;
        }
    }
}
