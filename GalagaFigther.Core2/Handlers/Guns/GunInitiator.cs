using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models.Guns;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Guns
{
    public interface IGunInitiator
    {
        void Initiate(Gun gun, Player player, float frameTime);
    }
    public class GunInitiator : IGunInitiator
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;

        public GunInitiator(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Initiate(Gun gun, Player player, float frameTime)
        {
            if (!gun.IsBuildUp)
                return;

            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
            var shootData = _gameDataRegistry.Get<GunBuildUpData>(gun);

            if (!gun.ShotInitiated)
            {
                if(!inputData.PowerShot.IsPressed)
                    return;

                shootData.BuildUpTimer += frameTime;
                gun.ShotInitiated = true;
                return;
            }

            if(inputData.PowerShot.IsDown)
            {
                shootData.BuildUpTimer += frameTime;
                DebugWriter.Write("      " + shootData.BuildUpTimer);
                return;
            }

            gun.ShotInitiated = false;
            gun.ShotRequested = true;
        }
    }
}
