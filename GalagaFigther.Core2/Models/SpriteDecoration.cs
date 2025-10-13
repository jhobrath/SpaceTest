using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Helpers;
using Raylib_cs;
using System.Numerics;

namespace GalagaFigther.Core2.Models
{
    public abstract class Decoration
    {
        public Vector2 Offset { get; set; }
        public bool MaintainRotation { get; set; }
        public bool MaintainColor { get; set; }
        public float InitialRotation { get; set; } = 0f;
        public int Depth { get; set; } = 0;

        public abstract void Update(GameObject gameObject, float frameTime);
        public abstract void Draw(GameObject gameObject);

    }

    public class SpriteDecoration : Decoration
    {
        public SpriteBase Sprite { get; set; }

        public SpriteDecoration(SpriteBase sprite, Vector2? offset = null, bool followRotation = false)
        {
            Offset = offset ?? Vector2.Zero;
            Sprite = sprite;
            MaintainRotation = followRotation;
        }

        public override void Update(GameObject gameObject, float frameTime)
        {
            Sprite.Update(frameTime);
        }

        public override void Draw(GameObject gameObject)
        {
            var rect = new Rectangle(gameObject.Rect.Position + Offset, gameObject.Rect.Size);
            var rotation = MaintainRotation ? gameObject.Rotation : 0;
            var color = MaintainColor ? gameObject.Color : Color.White;

            Sprite.Draw(rect, InitialRotation + rotation, color);
        }
    }
}
