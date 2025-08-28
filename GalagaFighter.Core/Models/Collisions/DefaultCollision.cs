using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Collisions
{
    public class DefaultCollision : AnimatedCollision
    {
        public DefaultCollision(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialVelocity)
            : base(owner,
                   TextureService.Get("Sprites/Collisions/default.png"),
                   initialPosition,
                   GetSquare(initialSize),
                   initialVelocity,
                   38, .02f)
        {
        }

        private static Vector2 GetSquare(Vector2 initialSize)
        {
            var size = Math.Clamp(initialSize.Y, 50f, 200f);
            return new Vector2(size, size);
        }
    }
}
