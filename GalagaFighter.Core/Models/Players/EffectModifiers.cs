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
        public int PhantomCount { get;  set; }
        public List<ParticleEffect> ParticleEffects { get; set; } = [];
        public bool Jiggle { get; set; }
        public bool Parry { get; set; }
        public bool Warp { get; set; }
        public bool BulletShield { get; set; }


        //This flag informs services that the modifiers were reset
        //  in case they were updating modifiers on the fly.
        public bool WereReset { get; set; } = true;
        public bool AffectedByShootMeter { get; set; }

        public EffectModifiers()
        {
        }
    }
}
