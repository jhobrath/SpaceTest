using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects
{
    public class MudSplatEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/mudsplaticon.png";
        protected override float Duration => 5f;

        private readonly Player _player;
        private readonly MudSplat _splat;

        public MudSplatEffect(Player player, MudSplat mudSplat)
        {
            _player = player;
            _splat = mudSplat;
        }

        public override void Apply(EffectModifiers modifiers)
        {
            var distance = Math.Abs(_player.Center.Y - _splat.Center.Y);
            if (distance > 150f)
                return;
         
            modifiers.Stats.SpeedMultiplier = 0.3f; 
            modifiers.Display.GreenAlpha = 0.7f; 
        }

        public override void Deactivate()
        {
            _splat.IsActive = false;
            base.Deactivate();
        }
    }
}