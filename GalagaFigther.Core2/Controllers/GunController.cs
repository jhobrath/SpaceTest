using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Guns;
using GalagaFighter.Core2.Models.Particles;
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
            { 
                _projectileShooter.Shoot(gun, barrel.Value, barrel.Key);
                Poof(gun, barrel.Key);
            }

            if (gun.Barrels.Count == 1)
                SetRecoil(gun);

            gun.ShotDue = false;
        }

        private void Poof(Gun gun, GunBarrel barrel)
        {
            var config = ParticleEffectTemplates.Get("SmokeTrail");
            config.Loop = false;
            config.Duration = .15f;
            config.Lifetime = .125f;
            config.Speed = new(0f, 0f);
            config.SpeedVariation = new(400f,400f);
            config.StartSize = 10f;
            config.EmissionRate = 200f;
            config.EndSize = 20f;
            config.StartColor = Color.Blue;//.ApplyAlpha(.5f);
            config.EndColor = Color.SkyBlue.ApplyAlpha(0);
            var barrelEnd = GetRotatedOffset(gun, barrel.End);
            var emitter = new ParticleEmitter(Game.Id, barrelEnd - new Vector2(5f,5f), 20f)
            {
                Config = config
            };

            _objectService.Add(emitter);
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

        private Vector2 GetRotatedOffset(GameObject objectShooting, Vector2 offset)
        {
            var offsetRotationRadians = (objectShooting.WorldRotation - 90) * MathF.PI / 180f;
            return objectShooting.Center + new Vector2(
                (offset.X * MathF.Cos(offsetRotationRadians) - offset.Y * MathF.Sin(offsetRotationRadians)),
                (offset.X * MathF.Sin(offsetRotationRadians) + offset.Y * MathF.Cos(offsetRotationRadians))
            );
        }
    }
}
