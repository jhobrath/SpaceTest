using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using System.Numerics;

namespace GalagaFighter.Core2.GameObjects.Guns
{
    public abstract class Gun : GameObject 
    {
        public Gun(GameObject owner, SpriteBase sprite) 
            : base(owner.Id, owner.Rect.Position, owner.Rect.Size, owner.Speed, sprite)
        {
        }

        public virtual List<GunBarrel> Barrels { get; set; } = [];
        public abstract Dictionary<GunBarrel, GameObject> Shoot(Guid guid);
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
