using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;

namespace GalagaFighter.Core.Models.Effects
{
    public class DefaultShootEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/firerate1.png";
        public override bool IsProjectile => true;
        private SpriteWrapper _sprite;
        private SpriteDecorations _decorations;
        private Color? _palleteSwap = null;

        public DefaultShootEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip.png"));
            _decorations = SetDecorations();
        }

        public DefaultShootEffect(Color palleteSwap)
        {
            _sprite = new SpriteWrapper("Sprites/Ships/MainShip.png", palleteSwap);
            _decorations = SetDecorations();
            _palleteSwap = palleteSwap;
        }

        private SpriteDecorations SetDecorations()
        {
            return new SpriteDecorations()
            {
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_ShootBoth.png"))),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_ShootLeft.png"))),
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_ShootRight.png"))),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_Move.png")))
            };
        }



        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Decorations = _decorations; 
            modifiers.Projectile.OnShootProjectiles.Add((updater, owner, position, modifiers) => new DefaultProjectile(updater, owner, position, modifiers, _palleteSwap));
        }
    }
}
