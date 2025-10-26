using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core2.Models
{
    public abstract class Decoration
    {
        public string? Key { get; set; }
        public Vector2 Offset { get; set; }
        public bool MaintainRotation { get; set; } = true;
        public bool MaintainColor { get; set; } = true;
        public float InitialRotation { get; set; } = 0f;
        public bool MaintainAlpha { get; set; } = true;
        public int Depth { get; set; } = 0;

        public abstract void Update(GameObject gameObject, float frameTime);
        public abstract void Draw(GameObject gameObject);

    }

    public class SpriteDecoration : Decoration
    {
        public SpriteBase Sprite { get; set; }

        public SpriteDecoration(SpriteBase sprite, Vector2? offset = null)
        {
            Offset = offset ?? Vector2.Zero;
            Sprite = sprite;
        }

        public override void Update(GameObject gameObject, float frameTime)
        {
            Sprite.Update(frameTime);
        }

        public override void Draw(GameObject gameObject)
        {
            var rect = new Rectangle(gameObject.WorldPosition + Offset, gameObject.Rect.Size);
            var rotation = MaintainRotation ? gameObject.WorldRotation : 0;
            var color = MaintainColor ? gameObject.Color : Color.White;

            if (!MaintainColor && MaintainAlpha)
                color = new Color(1f, 1f, 1f, gameObject.Color.A/255f);


            Sprite.Draw(rect, InitialRotation + rotation, color);
        }
    }
}
