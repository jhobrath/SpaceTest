using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Projectiles
{
    public class DefaultShootEffect : ProjectileEffect
    {
        private readonly SpriteDecoration _moveDecoration = new SpriteDecoration(new StillImageSprite("Sprites/Ships/MainShip_Move.png")) { Key = "Move", MaintainRotation = true };
        protected override float Duration => 0f;

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Decorations.Create[this] = (g, m) => [_moveDecoration];
            modifiers.Guns.Create[this] = HandleGuns;
        }

        private List<Gun> HandleGuns(GameObject gameObject, PlayerModifiers modifiers)
        {
            var guns = new List<Gun> {new DefaultGun(gameObject, 0)
            {
                CollectedFrom = Id
            } };

            if (modifiers.WeaponCount >= 1)
                guns.Add(new BarGun(gameObject)
                {
                    CollectedFrom = Id
                });

            if(modifiers.WeaponCount >= 2)
                guns.Add(new HomingGun(gameObject) {
                    CollectedFrom = Id
                });

            if (modifiers.WeaponCount >= 3)
                guns.Add(new DefaultGun(gameObject, 1)
                {
                    CollectedFrom = Id
                });

            if (modifiers.WeaponCount >= 4)
                guns.Add(new ShotGun(gameObject)
                {
                    CollectedFrom = Id
                });

            return guns;
        }
    }
}
