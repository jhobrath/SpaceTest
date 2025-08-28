using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects.Offensive
{
    public class RicochetEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/ricochet.png";
        protected override float Duration => 10f;
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
