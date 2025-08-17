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
        public Guid Owner => _owner;
        public Rectangle Rect => _rect;
        public float Rotation { get; set; } = 0f;
        public Vector2 Speed => _speed;
        public Color Color { get; set; } = Color.White;
        public SpriteWrapper Sprite { get; set; }
        public bool IsActive { get; set; }

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
            _speed.X *= x ?? 0f;
            _speed.Y *= y ?? 0f;
        }

        public void HurryTo(float? x = null, float? y = null)
        {
            _speed.X = x ?? _speed.X;
            _speed.Y = y ?? _speed.Y;
        }

        public OrientedBoundingBox GetBoundingBox()
        {
            // Step 1: Calculate the center of the rectangle
            Vector2 center = new Vector2(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height / 2);

            // Step 2: Calculate the four corners relative to the center, before rotation
            Vector2[] preRotationCorners = new Vector2[4];
            preRotationCorners[0] = new Vector2(-Rect.Width / 2, -Rect.Height / 2); // Top-left
            preRotationCorners[1] = new Vector2(Rect.Width / 2, -Rect.Height / 2);  // Top-right
            preRotationCorners[2] = new Vector2(Rect.Width / 2, Rect.Height / 2);   // Bottom-right
            preRotationCorners[3] = new Vector2(-Rect.Width / 2, Rect.Height / 2);  // Bottom-left

            // Step 3: Convert the rotation from degrees to radians for trigonometric functions
            float radians = Rotation * (MathF.PI / 180f);
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);

            // Step 4: Rotate each corner point and translate it to the final position
            Vector2[] finalCorners = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                Vector2 corner = preRotationCorners[i];

                // Rotate the point
                float xPrime = corner.X * cos - corner.Y * sin;
                float yPrime = corner.X * sin + corner.Y * cos;

                // Translate the point to its final position
                finalCorners[i] = new Vector2(xPrime, yPrime) + center;
            }

            return new OrientedBoundingBox(center, finalCorners);
        }
    }
}
