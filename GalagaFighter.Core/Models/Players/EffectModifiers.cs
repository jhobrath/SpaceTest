using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Players
{
    public class EffectModifiers
    {
        public SpriteDecorations? Decorations { get; set; }

        public PlayerStats Stats { get; set; } = new();
        public PlayerDisplay Display { get; set; } = new();
        public PlayerProjectile Projectile { get; set; } = new();
        
        //To be recategorized later
        public bool Magnetic { get; set; }
        public bool Untouchable { get; set; }
        public bool IsRepulsive { get; set; }
        public List<Phantom> Phantoms { get; set; } = [];
        public int PhantomCount { get; internal set; }

        public EffectModifiers()
        {
        }
    }
}
