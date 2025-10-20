using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
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
        public abstract List<TurretGun> Guns { get; }

        public Turret(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite) 
            : base(owner, position, size, speed, sprite)
        {
            Owner = owner;
        }
    }

    public abstract class TurretGun : GameObject
    {
        public abstract List<Gun> Guns { get; }

        protected TurretGun(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite) 
            : base(owner, position, size, speed, sprite)
        {
        }
    }
}
