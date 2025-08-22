using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Collisions
{
    public class IceShotCollision : Collision
    {
        protected override bool FadeOut => false;

        public IceShotCollision(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialVelocity) 
            : base(owner, new SpriteWrapper(TextureService.Get("Sprites/Collisions/ice.png"), 5, .125f, repeat: false), initialPosition, initialSize, initialVelocity)
        {
            AudioService.PlayIceHitSound();
        }
    }
}
