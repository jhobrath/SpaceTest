using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core
{
    public class SpriteDecorations : Dictionary<string, SpriteDecoration?>
    {
        public SpriteDecoration? ShootLeft { get => this.GetValueOrDefault(nameof(ShootLeft)); set => this[nameof(ShootLeft)] = value; }
        public SpriteDecoration? ShootRight { get => this.GetValueOrDefault(nameof(ShootRight)); set => this[nameof(ShootRight)] = value; }
        public SpriteDecoration? ShootBoth { get => this.GetValueOrDefault(nameof(ShootBoth)); set => this[nameof(ShootBoth)] = value; }
        public SpriteDecoration? WindUpLeft { get => this.GetValueOrDefault(nameof(WindUpLeft)); set => this[nameof(WindUpLeft)] = value; }
        public SpriteDecoration? WindUpRight { get => this.GetValueOrDefault(nameof(WindUpRight)); set => this[nameof(WindUpRight)] = value; }
        public SpriteDecoration? WindUpBoth { get => this.GetValueOrDefault(nameof(WindUpBoth)); set => this[nameof(WindUpBoth)] = value; }
        public SpriteDecoration? Move { get => this.GetValueOrDefault(nameof(Move)); set => this[nameof(Move)] = value; }
    }

    public class SpriteDecoration
    {
        public SpriteWrapper Sprite { get; set; }
        public Vector2 Offset = new Vector2(0, 0);
        public Vector2? Size = new Vector2(0, 0);

        public SpriteDecoration(SpriteWrapper sprite) 
            : this(sprite, new Vector2(0, 0)) { }

        public SpriteDecoration(SpriteWrapper sprite, Vector2 offset, Vector2? size = null)
        {
            Sprite = sprite;
            Offset = offset;
            Size = size;
        }

        public void Draw(Vector2 center, Vector2 size, float rotation, Color color)
        {
            Sprite.Draw(center, rotation, size.X, size.Y, color);
        }
    }
}
