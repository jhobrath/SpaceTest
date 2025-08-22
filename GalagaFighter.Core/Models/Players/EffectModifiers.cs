namespace GalagaFighter.Core.Models.Players
{
    public class EffectModifiers
    {
        public SpriteWrapper Sprite { get; set; }
        public PlayerStats Stats { get; set; } = new();
        public PlayerDisplay Display { get; set; } = new();
        public PlayerProjectile Projectile { get; set; } = new();
        public bool Magnetic { get; set; }

        public EffectModifiers(SpriteWrapper sprite)
        {
            Sprite = sprite;
        }
    }
}
