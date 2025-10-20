using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using System.Numerics;

namespace GalagaFighter.Core2.Models
{
    public class Gun : List<GunBarrel>
    {
        private readonly Func<Guid, List<GameObject>> _onShoot;

        public Gun(List<GunBarrel> barrels, Func<Guid, List<GameObject>> onShoot) : base(barrels)
        {
            _onShoot = onShoot;
        }

        public List<GameObject> Shoot(Guid guid) => _onShoot?.Invoke(guid) ?? [];
    }

    public struct GunBarrel
    {
        public GunBarrel(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
    }
}
