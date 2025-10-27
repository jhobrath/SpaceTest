using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Models.Guns;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        private readonly IProjectileShooter _projectileShooter;

        public GunController(IObjectService objectService, IGameDataRegistry gameDataRegistry, 
            IProjectileShooter projectileShooter)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
            _projectileShooter = projectileShooter;
        }

        public void Update(Gun gun, float frameTime)
        {
            var player = _objectService.GetPlayer(gun);
            Rotate(gun, player);
            Shoot(gun);
            Recoil(gun, frameTime);
        }

        private void Shoot(Gun gun)
        {
            if (!gun.ShotDue)
                return;

            var player = _objectService.GetPlayer(gun);
            foreach (var barrel in gun.Shoot(player))
                _projectileShooter.Shoot(gun, barrel.Value, barrel.Key);

            if (gun.Barrels.Count == 1)
                SetRecoil(gun);

            gun.ShotDue = false;
        }

        private void Recoil(Gun gun, float frameTime)
        {
            var recoilData = _gameDataRegistry.Get<GunRecoilData>(gun);
            if (recoilData.RecoilPeriod == 0f)
                return;

            recoilData.RecoilLifetime += frameTime;

            var pct = (recoilData.RecoilPeriod - recoilData.RecoilLifetime) / recoilData.RecoilPeriod;
            var angle = ((90 - gun.Rotation) * MathF.PI / 180f);
            var coords = new Vector2(-MathF.Cos(angle) * recoilData.RecoilDistance * pct, MathF.Sin(angle) * recoilData.RecoilDistance * pct);
            gun.MoveTo(gun.Width/2 + coords.X, gun.Height/2 + coords.Y);

            if(pct <= 0)
            {
                gun.MoveTo(0f, 0f);
                recoilData.RecoilPeriod = 0f;
            }
        }

        private void SetRecoil(Gun gun)
        {
            var recoilData = _gameDataRegistry.Get<GunRecoilData>(gun);
            recoilData.RecoilLifetime = 0f;
            recoilData.RecoilPeriod = .5f;
            recoilData.RecoilDistance = 10f;
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
