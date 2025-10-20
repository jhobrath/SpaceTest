using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
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
        public int EffectCount { get; set; }
        public List<Decoration> Decorations { get; set; } = [];

        public float RedAlpha { get; set; } = 1f;
        public float BlueAlpha { get; set; } = 1f;
        public float GreenAlpha { get; set; } = 1f;
        public float Alpha { get; set; } = 1f;

        public ProjectileModifiers Projectile { get; set; } = new();
        public TurretModifiers Turret { get; set; } = new();
        public Vector2 GunOffset { get; internal set; }
        public float FireRate { get; set; } = 1f;
        public float TurretDeployRate { get; set; } = 1f;
    }

    public class ProjectileModifiers
    {
        public List<Decoration> Decorations { get; set; } = [];

        public Dictionary<string, Func<Guid, List<GameObject>>> OnShoot { get; set; } = [];
    }

    public class TurretModifiers
    {
        public List<Decoration> Decorations { get; set; } = [];
        public Dictionary<string, Func<Guid, Vector2, List<GameObject>>> OnDeploy { get; set; } = [];
    }
}
