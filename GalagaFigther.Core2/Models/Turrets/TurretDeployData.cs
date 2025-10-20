using GalagaFighter.Core2.GameObjects.Turrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Turrets
{
    public class TurretDeployData : IGameObjectData<Turret>
    {
        public float Lifetime { get; set; }
        public float ShotCountdown { get; set; }
    }
}
