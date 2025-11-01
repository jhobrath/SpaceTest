using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core2.Models
{
    public abstract class Decoration : ICollectible
    {
        public Guid CollectedFrom { get; set; }

        public string? Key { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 Size { get; set; }
        public bool MaintainRotation { get; set; } = true;
        public bool MaintainColor { get; set; } = true;
        public float InitialRotation { get; set; } = 0f;
        public bool MaintainAlpha { get; set; } = true;
        public int Depth { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public abstract void Update(GameObject gameObject, float frameTime);
        public abstract void Draw(GameObject gameObject);
    }

    public class SpriteDecoration : Decoration
    {
        public SpriteBase Sprite { get; set; }

        public SpriteDecoration(SpriteBase sprite, Vector2? offset = null, Vector2? size = null)
        {
            Offset = offset ?? Vector2.Zero;
            Sprite = sprite;
            Size = size ?? Vector2.Zero;
        }

        public override void Update(GameObject gameObject, float frameTime)
        {
            Sprite.Update(frameTime);
        }

        public override void Draw(GameObject gameObject)
        {
            // Use the center of the gameObject as the anchor
            Vector2 gameObjectCenter = gameObject.Center;

            var rotationDegrees = -(gameObject.WorldRotation - 90f);

            // Rotate the offset by the parent's rotation
            float rotationRadians = (MaintainRotation ? rotationDegrees : 0) * (float)Math.PI / 180f;
            Vector2 rotatedOffset = new Vector2(
                Offset.X * (float)Math.Cos(rotationRadians) - Offset.Y * (float)Math.Sin(rotationRadians),
                Offset.X * (float)Math.Sin(rotationRadians) + Offset.Y * (float)Math.Cos(rotationRadians)
            );

            // The center of the decoration
            Vector2 decorationCenter = gameObjectCenter + rotatedOffset;

            // Rectangle should be centered at decorationCenter
            Vector2 decorationSize = Size == Vector2.Zero ? gameObject.Rect.Size : Size;
            Vector2 decorationTopLeft = decorationCenter - 0.5f * decorationSize;

            var rect = new Rectangle(decorationTopLeft, decorationSize);
            var rotation = MaintainRotation ? gameObject.WorldRotation : 0;
            var color = MaintainColor ? gameObject.Color : Color.White;

            if (!MaintainColor && MaintainAlpha)
                color = new Color(1f, 1f, 1f, gameObject.Color.A/255f);

            var center = rect.Position + rect.Size/2; // (0,0)
            var size = rect.Size;
            var finalRect = new Rectangle(center, size); // (0,0), (168,168)
            Sprite.Draw(finalRect, InitialRotation + rotation, color);
        }
    }
}
