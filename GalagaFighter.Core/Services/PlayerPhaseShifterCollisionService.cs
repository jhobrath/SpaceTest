using GalagaFighter.Core.Handlers.Collisions;
using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerPhaseShifterCollisionService
    {
        void HandleCollisions();
    }
    public class PlayerPhaseShifterCollisionService : IPlayerPhaseShifterCollisionService
    {
        private readonly IObjectService _objectService;
        private readonly IPlayerManagerFactory _playerManagerFactory;

        public PlayerPhaseShifterCollisionService(IObjectService objectService, IPlayerManagerFactory playerManagerFactory)
        {
            _objectService = objectService;
            _playerManagerFactory = playerManagerFactory;
        }

        public void HandleCollisions()
        {
            var shifters = _objectService.GetGameObjects<PhaseShifter>();
            var players = _objectService.GetGameObjects<Player>();

            foreach(var player in players)
            {
                var effectManager = _playerManagerFactory.GetEffectManager(player);
                if (effectManager.HasEffect<PhaseShiftedEffect>())
                    continue;

                foreach (var shifter in shifters)
                {
                    if (player.Id != shifter.Owner)
                        continue;

                    if(ContactCollisionDetector.HasCollision(shifter, player))
                    {
                        shifter.Health -= 10;
                        
                        effectManager.AddEffect(new PhaseShiftedEffect(player, shifter));
                    }
                }
            }
        }
    }
}
