using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Players
{
    public class PlayerDisplay
    {
        private Rectangle _rect;
        public Rectangle Rect => _rect;

        public SpriteWrapper Sprite { get; set; }
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Rotation { get; set; } = 0f;
        public float Size { get; set; } = 1.0f;
        public Color Color { get; set; } = Color.White;

        public float Alpha { get; set;} = 1f;

        private readonly Vector2 _size;
        public PlayerDisplay(SpriteWrapper sprite, Rectangle rect, float rotation) 
        {
            Sprite = sprite;
            Position = rect.Position;
            Rotation = rotation;
            _size = rect.Size;
            _rect = rect;
        }
    }
}
