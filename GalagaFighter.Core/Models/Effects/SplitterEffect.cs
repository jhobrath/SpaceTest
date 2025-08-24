using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects
{
    public class SplitterEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/firerate1.png";
        public override bool IsProjectile => false;

        public SplitterEffect()
        {
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.CanSplit = true;
        }
    }
}
