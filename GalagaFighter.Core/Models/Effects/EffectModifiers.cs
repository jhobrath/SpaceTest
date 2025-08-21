using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects
{
    public class EffectModifiers
    {
        public SpriteWrapper Sprite { get; set; }
        public PlayerStats Stats { get; set; } = new();
        public PlayerDisplay Display { get; set; } = new();
        public PlayerProjectile Projectile { get; set; } = new();

        public EffectModifiers(SpriteWrapper sprite)
        {
            Sprite = sprite;
        }
    }
}
