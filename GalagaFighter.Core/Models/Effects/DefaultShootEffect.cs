using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;

namespace GalagaFighter.Core.Models.Effects
{
    public class DefaultShootEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/firerate1.png";
        public override bool IsProjectile => true;
        public SpriteWrapper _sprite;

        public DefaultShootEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/Player1.png"));
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Projectile.Projectiles.Add((updater, owner, position, modifiers) => new DefaultProjectile(updater, owner, position, modifiers));
        }
    }
}
