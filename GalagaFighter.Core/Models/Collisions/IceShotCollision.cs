using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Collisions
{
    public class IceShotCollision : DefaultCollision
    {
        protected override int FrameSkip => 0;

        public IceShotCollision(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialVelocity) 
            : base(owner, initialPosition, initialSize, initialVelocity)
        {
            Sprite = new SpriteWrapper(TextureService.Get("Sprites/Collisions/iceshot.png"), 5, .125f);
        }
    }
}
