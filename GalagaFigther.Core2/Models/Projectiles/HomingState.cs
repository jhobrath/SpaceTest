using GalagaFighter.Core2.GameObjects.Projectiles;

namespace GalagaFighter.Core2.Models.Projectiles
{
    public class HomingState : IGameObjectData<Projectile>
    {
        public float Homing { get; set; }
    }
}
