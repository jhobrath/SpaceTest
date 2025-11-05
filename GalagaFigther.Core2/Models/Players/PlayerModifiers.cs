using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerModifiers : IGameObjectData<Player>
    {
        // Player-specific modifier groups
        public PlayerStats Stats { get; set; } = new();
        public PlayerDisplay Display { get; set; } = new();

        public float TurretDeployRate { get; set; } = 1f;
        public float Polarity { get; set; } = 0f; //Additive, Positive = Magnetic, Negative = Repulsive
        public float HomingFactor { get; set; } = 0f;


        // Properties that get cloned to created objects
        public ProjectileModifiers Projectile { get; set; } = new();
        public TurretModifiers Turret { get; set; } = new();

        //Child objects
        public PlayerModifiersChildren<Decoration> Decorations { get; set; } = [];
        public PlayerModifiersChildren<ParticleEmitter> ParticleEmitters { get; set; } = [];
        public List<Action<Player>> PlayerActions { get; set; } = [];
    }

    public class PlayerStats
    {
        public float FireRateMultiplier { get; set; } = 1f;
        public float SpeedMultiplier { get; set; } = 1f;
        public float HealthMultiplier { get; set; } = 1f;
        public float DamageMultiplier { get; set; } = 1f;
        public float ShieldMultiplier { get; set; } = 1f;
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
    }

    public class TurretModifiers
    {
        public List<Decoration> Decorations { get; set; } = [];
    }
}

public class PlayerModifiersChildren<T> : List<T> where T : class, ICollectible
{
    public PlayerModifiersChildren()
    {
    }

    public PlayerModifiersChildren(IEnumerable<T> collection) : base(collection)
    {
    }

    public Dictionary<PlayerEffect, Func<GameObject, PlayerModifiers, List<T>>> Create { get; set; } = [];
}
