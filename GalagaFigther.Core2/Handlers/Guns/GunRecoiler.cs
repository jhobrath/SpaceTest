using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models.Guns;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Guns
{
    public interface IGunRecoiler
    {
        void Recoil(Gun gun, float frameTime);
    }
    public class GunRecoiler : IGunRecoiler
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public GunRecoiler(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Recoil(Gun gun, float frameTime)
        {
            var recoilData = _gameDataRegistry.Get<GunRecoilData>(gun);
            if (recoilData.RecoilPeriod == 0f)
                return;

            recoilData.RecoilLifetime += frameTime;

            var pct = (recoilData.RecoilPeriod - recoilData.RecoilLifetime) / recoilData.RecoilPeriod;
            var angle = ((90 - gun.Rotation) * MathF.PI / 180f);
            var coords = new Vector2(-MathF.Cos(angle) * recoilData.RecoilDistance * pct, MathF.Sin(angle) * recoilData.RecoilDistance * pct);
            gun.MoveTo(gun.Width / 2 + coords.X, gun.Height / 2 + coords.Y);

            if (pct <= 0)
            {
                gun.MoveTo(0f, 0f);
                recoilData.RecoilPeriod = 0f;
            }
        }
    }
}
