using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using System.Numerics;

namespace GalagaFighter.Core2.GameObjects.Guns
{
    public abstract class Gun : CollectibleGameObject 
    {
        public Gun(GameObject owner, SpriteBase sprite) 
            : base(owner.Id, Vector2.Zero, owner.Rect.Size, Vector2.Zero, sprite)
        {
            CountDown = FireRate;
        }

        public virtual List<GunBarrel> Barrels { get; set; } = [];
        public abstract Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter);

        public float? MinRotation = null;
        public float? MaxRotation = null;
        public float? CountDown { get; set; }
        public virtual float FireRate => .15f;
        public virtual bool ShotRequested { get; set; }
        public float RotationHoming { get; set; } = 0;
        public float RecoveryTime { get; set; } = 0f;
        public bool IsPlayerGun { get; set; } = true;
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
        public float Recoil { get; set; } = 0f;

        public bool Shoot { get; set; }
    }
}
