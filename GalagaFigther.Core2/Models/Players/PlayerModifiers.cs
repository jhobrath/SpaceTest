using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Models.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerModifiers : IGameObjectData<Player>
    {
        // System/Framework properties
        public int EffectCount { get; set; }

        // Player-specific modifier groups
        public PlayerStats Stats { get; set; } = new();
        public PlayerDisplay Display { get; set; } = new();

        // Properties that get cloned to created objects
        public ProjectileModifiers Projectile { get; set; } = new();
        public TurretModifiers Turret { get; set; } = new();
      
        //Child objects
        public Dictionary<PlayerEffect, Func<GameObject, List<Decoration>>> CreateDecorations { get; set; } = [];
        public List<Decoration> Decorations { get; set; } = [];

        public Dictionary<PlayerEffect, Func<GameObject, List<Gun>>> CreateGuns { get; set; } = [];
        public List<Gun> Guns { get; set; } = [];

        public Dictionary<PlayerEffect, Func<GameObject, List<Turret>>> CreateTurrets { get; set; } = [];
        public List<Turret> Turrets { get; set; } = [];

        public Dictionary<PlayerEffect, Func<GameObject, List<ParticleEmitter>>> CreateParticleEmitters { get; set; } = [];
        public List<ParticleEmitter> ParticleEmitters { get; set; } = [];
    }

    public class PlayerStats
    {
        // Player gameplay statistics (movement, combat, etc.)
        public float FireRate { get; set; } = 1f;
        public float TurretDeployRate { get; set; } = 1f;
        public float SpeedMultiplier { get; set; } = 1f;
    }

    public class PlayerDisplay
    {
        // Player visual appearance modifiers
        public float RedAlpha { get; set; } = 1f;
        public float BlueAlpha { get; set; } = 1f;
        public float GreenAlpha { get; set; } = 1f;
        public float Alpha { get; set; } = 1f;
    }

    public class ProjectileModifiers
    {
        public List<Decoration> Decorations { get; set; } = [];
        public Dictionary<string, Func<Guid, List<GameObject>>> OnShoot { get; set; } = [];
    }

    public class TurretModifiers
    {
        public List<Decoration> Decorations { get; set; } = [];
    }
}
