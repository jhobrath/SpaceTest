using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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
        private readonly IObjectService _objectService;

        public GunRotator(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Rotate(Gun gun, Player player)
        {
            Constrain(gun, player);
            Home(gun, player);
        }

        private void Home(Gun gun, Player player)
        {
            if (gun.RotationHoming == 0)
                return;

            var opponent = _objectService.GetOpponent(player);

            var vector = Vector2.Normalize(opponent.WorldPosition - gun.WorldPosition);
            var angleRadians = MathF.Atan2(-vector.Y, vector.X);
            var angleRaylib = (-angleRadians * 180F / MathF.PI) + 90;
            var rotationDifference = (angleRaylib - gun.WorldRotation) % 360;
            if (rotationDifference < -180)
                rotationDifference += 360f;
            else if (rotationDifference > 180)
                rotationDifference -= 360f;

            if (rotationDifference < 0)
                gun.AngularVelocity = -gun.RotationHoming;
            else
                gun.AngularVelocity = gun.RotationHoming;
        }

        private void Constrain(Gun gun, Player player)
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
