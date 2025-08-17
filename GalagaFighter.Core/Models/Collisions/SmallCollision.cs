using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Collisions
{
    public class SmallCollision : DefaultCollision
    {
        protected virtual int FrameSkip => 6;

        public SmallCollision(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialVelocity) 
            : base(owner, initialPosition, initialSize, initialVelocity)
        {
        }
    }
}
