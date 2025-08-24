using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects
{
    public class RicochetEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/firerate1.png";
        public override bool IsProjectile => false;

        public RicochetEffect()
        {
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.CanRicochet = true;
        }
    }
}
