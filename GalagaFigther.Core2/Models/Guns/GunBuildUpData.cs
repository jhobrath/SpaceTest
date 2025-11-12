using GalagaFighter.Core2.GameObjects.Guns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Guns
{
    public class GunBuildUpData : IGameObjectData<Gun>
    {
        public float BuildUpTimer { get; set; } = 0f;
    }
}
