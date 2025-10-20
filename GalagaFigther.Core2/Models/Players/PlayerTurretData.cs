using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerTurretData : IGameObjectData<Player>
    {
        public float DeployCountdown { get; set; } = 0;
    }
}
