using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Collisions
{
    public class IceShotCollision : AnimatedCollision
    {
        protected override bool FadeOut => false;

        public IceShotCollision(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialVelocity)
            : base(owner,
                   TextureService.Get("Sprites/Collisions/ice.png"),
                   initialPosition,
                   initialSize,
                   initialVelocity,
                   5, .125f)
        {
            AudioService.PlayIceHitSound();
        }
    }
}
