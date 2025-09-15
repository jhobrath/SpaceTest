using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class PoisonProjectile : Projectile
    {
        public static readonly Vector2 _baseSpeed = new Vector2(100f, 0f);
        public static readonly Vector2 _baseSize = new Vector2(100f, 100f);
        private Vector2 _originalPosition;

        public override Vector2 BaseSpeed => _baseSpeed;
        public override Vector2 BaseSize => _baseSize;
        public override int BaseDamage => 0;
        public override Vector2 SpawnOffset => Vector2.Zero;
        public override bool DamageOverTime => true;

        public PoisonProjectile(IProjectileController controller, Player owner, SpriteWrapper sprite, Vector2 initialPosition, PlayerProjectile modifiers) 
            : base(controller, owner, sprite, initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            _originalPosition = initialPosition;
        }

        private float _lifeTime = 0f;
        public override void Update(Game game)
        {
            _lifeTime += Raylib.GetFrameTime();
            base.Update(game);
        }

        public override void Draw()
        {
            //Poison projectile is a bubble filled with poison gas
            //The space ship for this power up contains two prongs at its tip that dispense 
            //  poison soap to form the bubbles
            //The two prong tips are at _originalPosition + (-31, -24) and _originalPosition + (-31, 24). 
            //We need an animation that draws the frames of the bubble forming from the prong tips
            //Once the bubble is formed, I will fade in an actual png sprite to replace it
            //The bubble should take .35 seconds to form over about 8-12 frames.
        }
    }
}
