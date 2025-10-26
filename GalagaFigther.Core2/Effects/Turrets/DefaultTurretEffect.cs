using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Turrets
{
    public class DefaultTurretEffect : PlayerEffect
    {
        protected override float Duration => 0f;

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.CreateTurrets.Add(this, HandleOnDeploy);
        }

        private List<Turret> HandleOnDeploy(GameObject owner)
        {
            var turret = new DefaultTurret(owner);
            return [turret];
        }
    }
}
