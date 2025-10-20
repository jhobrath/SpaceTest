using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Turrets
{
    public abstract class Turret : GameObject
    {
        public Turret(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite) 
            : base(owner, position, size, speed, sprite)
        {
            Owner = owner;
        }
    }

    public abstract class TurretGun : GameObject
    {
        protected TurretGun(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite) 
            : base(owner, position, size, speed, sprite)
        {
        }

        public abstract Func<Guid, List<GameObject>> OnShoot { get;  }

        //For ship facing right with 90 degrees rotation
        public abstract List<TurretGunOffset> GunOffsets { get; }
    }

    public class TurretGunOffset
    {
        public Vector2 Position { get; set; }
        public Vector2 SpeedMultiplier { get; set; }
    }
}
