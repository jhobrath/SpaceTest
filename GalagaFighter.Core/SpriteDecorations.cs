using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
        public SpriteDecoration? Guns { get => this.GetValueOrDefault(nameof(Guns)); set => this[nameof(Guns)] = value; }
        public SpriteDecoration? Glow { get => this.GetValueOrDefault(nameof(Glow)); set => this[nameof(Glow)] = value; } 

        public List<SpriteDecoration> Other => this.Where(kv => 
            kv.Key != nameof(ShootLeft) && kv.Key != nameof(ShootRight) && kv.Key != nameof(ShootBoth) &&
            kv.Key != nameof(WindUpLeft) && kv.Key != nameof(WindUpRight) && kv.Key != nameof(WindUpBoth) &&
            kv.Key != nameof(Move) && kv.Key != nameof(Guns) && kv.Key != nameof(Glow))
            .Select(kv => kv.Value)
            .Where(v => v != null)
            .ToList()!;

        public void Apply(SpriteDecorations decorations)
        {
            foreach (var decoration in decorations)
                this[decoration.Key] = decoration.Value;
        }
    }

    public class SpriteDecoration
    {
        public SpriteWrapper Sprite { get; set; }
        public bool FollowRotation { get; internal set; }

        public Vector2 Offset = new(0, 0);
        public Vector2? Size = new Vector2(0, 0);

        public SpriteDecoration(SpriteWrapper sprite) 
            : this(sprite, new Vector2(0, 0)) { }

        public SpriteDecoration(SpriteWrapper sprite, Vector2 offset, Vector2? size = null)
        {
            Sprite = sprite;
            Offset = offset;
            Size = size;

            if(sprite.Color.R == 0 && sprite.Color.G == 0 && sprite.Color.B == 0 && sprite.Color.A == 0)
            {
                sprite.Color = Color.White;
            }
        }

        public void Draw(Vector2 center, Vector2 size, float rotation, Color color)
        {
            Sprite.Draw(center, rotation, size.X, size.Y, color);
        }
    }
}
