using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class MagnetEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/Magnetshot.png";
        protected override float Duration => 5f;
        public override bool IsProjectile => true;

        private readonly SpriteDecorations _decorations;


        public MagnetEffect()
        {
            _decorations = new SpriteDecorations
            {
                Guns = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMagnetGuns.png"))),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_Move.png"))),
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMagnetGuns_ShootBoth.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Magnetic = true;
            modifiers.Decorations = _decorations;
        }
    }
}
