using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Game;
using System.Numerics;

namespace GalagaFighter.Core2.Models.Projectiles
{
    public class ProjectileVeerState : IGameObjectData<Projectile>
    {
        public Vector2 OriginalSpeed { get; set; }
        public Vector2 CurrentVeer { get; set; }
        public bool Initialized { get; set; }
    }
}
