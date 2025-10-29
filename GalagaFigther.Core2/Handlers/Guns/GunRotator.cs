using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Guns
{
    public interface IGunRotator
    {
        void Rotate(Gun gun, Player player);
    }
    public class GunRotator : IGunRotator
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public GunRotator(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Rotate(Gun gun, Player player)
        {
            var playerRotationData = _gameDataRegistry.Get<PlayerRotationData>(player);

            var minRotation = gun.MinRotation == null ? (float?)null : (playerRotationData.InitialRotation + gun.MinRotation.Value);
            var maxRotation = gun.MaxRotation == null ? (float?)null : (playerRotationData.InitialRotation + gun.MaxRotation.Value);

            if (minRotation.HasValue && gun.Rotation < minRotation.Value)
            {
                gun.Rotation = minRotation.Value;
                gun.AngularVelocity = Math.Abs(gun.AngularVelocity);
            }
            else if (maxRotation.HasValue && gun.Rotation > maxRotation.Value)
            {
                gun.Rotation = maxRotation.Value;
                gun.AngularVelocity = -Math.Abs(gun.AngularVelocity);
            }
        }
    }
}
