using GalagaFigther.Core2.Effects;
using GalagaFigther.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Models.Players
{
    public class PlayerModifiers
    {
        public List<Decoration> Decorations { get; set; } = [];

        public float RedAlpha { get; set; } = 1f;
        public float BlueAlpha { get; set; } = 1f;
        public float GreenAlpha { get; set; } = 1f;
        public float Alpha { get; set; } = 1f;

        public ProjectileModifiers Projectile { get; set; } = new();
    }

    public class ProjectileModifiers
    {
        public List<Decoration> Decorations { get; set; } = [];

        public Dictionary<string, Func<Guid, Vector2, List<GameObject>>> OnShoot { get; set; } = [];
    }
}
