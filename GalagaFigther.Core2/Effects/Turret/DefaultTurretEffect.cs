using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Turret
{
    public class DefaultTurretEffect : PlayerEffect
    {
        protected override float Duration => 0f;

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Turret.OnDeploy.Add(nameof(DefaultTurretEffect), HandleOnDeploy);
        }

        private List<GameObject> HandleOnDeploy(Guid guid, Vector2 position)
        {
            var turret = new DefaultTurret(guid, position);
            var turretGun = new DefaultTurretGun(turret);

            return [turret, turretGun];
        }
    }
}
