using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core
{
    public abstract class GameObject
    {
        public Rectangle Rect;
        public float Rotation { get; set; } = 0f;
        public Vector2 Center => new Vector2(Rect.X + Rect.Width / 2f, Rect.Y + Rect.Height / 2f);

        public bool IsActive;

        public GameObject(Rectangle rect)
        {
            Rect = rect;
            IsActive = true;
        }

        public abstract void Update(Game game);
        public abstract void Draw();

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
