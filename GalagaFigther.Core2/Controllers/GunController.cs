using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Controllers
{
    public interface IGunController : IController<Gun>
    {

    }

    public class GunController : IGunController
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;

        public GunController(IObjectService objectService, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
        }

        public void Update(Gun gun, float frameTime)
        {
            var player = _objectService.GetPlayer(gun);
            Rotate(gun, player);
        }

        private void Rotate(Gun gun, Player player)
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

        public void Draw(Gun gun, float frameTime)
        {
            gun.Sprite.Draw(gun);
        }
    }
}
