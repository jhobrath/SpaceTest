using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Helpers;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Models
{
    public interface IDecoration
    {
        void Update(GameObject gameObject, float frameTime);
        void Draw(GameObject gameObject);
    }

    public class SpriteDecoration : IDecoration
    {
        public Vector2 Offset { get; set; }
        public SpriteBase Sprite { get; set; }
        public bool MaintainRotation { get; set; }
        public bool MaintainColor { get; set; }

        public SpriteDecoration(SpriteBase sprite, Vector2? offset = null, bool followRotation = false)
        {
            Offset = offset ?? Vector2.Zero;
            Sprite = sprite;
            MaintainRotation = followRotation;
        }

        public void Update(GameObject gameObject, float frameTime)
        {
            Sprite.Update(frameTime);
        }

        public void Draw(GameObject gameObject)
        {
            var rect = new Rectangle(gameObject.Rect.Position + Offset, gameObject.Rect.Size);
            var rotation = MaintainRotation ? gameObject.Rotation : 0;
            var color = MaintainColor ? gameObject.Color : Color.White;

            Sprite.Draw(rect, rotation, color);
        }
    }
}
