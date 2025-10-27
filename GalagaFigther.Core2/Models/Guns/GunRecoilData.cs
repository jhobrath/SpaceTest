using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalagaFighter.Core2.GameObjects.Guns;

namespace GalagaFighter.Core2.Models.Guns
{
    public class GunRecoilData : IGameObjectData<Gun>
    {
        public float RecoilPeriod { get; set; } = 0f;
        public float RecoilLifetime { get; set; } = 0f;
        public float RecoilDistance { get; set; } = 0f;
    }
}
