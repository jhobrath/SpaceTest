using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Handlers.Guns;
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
        public Type Type => typeof(Gun);
        
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        private readonly IProjectileShooter _projectileShooter;
        private readonly IGunRotator _gunRotator;
        private readonly IGunShooter _gunShooter;
        private readonly IGunRecoiler _gunRecoiler;
        private readonly IGunInitiator _gunInitiator;

        public GunController(IObjectService objectService, IGameDataRegistry gameDataRegistry,
            IProjectileShooter projectileShooter, IGunRotator gunRotator, IGunShooter gunShooter,
            IGunRecoiler gunRecoiler, IGunInitiator gunInitiator)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
            _projectileShooter = projectileShooter;
            _gunRotator = gunRotator;
            _gunShooter = gunShooter;
            _gunRecoiler = gunRecoiler;
            _gunInitiator = gunInitiator;
        }

        public void Update(Gun gun, float frameTime)
        {
            gun.CountDown += frameTime;

            var player = _objectService.GetPlayer(gun);
            _gunRotator.Rotate(gun, player);
            _gunInitiator.Initiate(gun, player, frameTime);
            _gunShooter.Shoot(gun);
            _gunRecoiler.Recoil(gun, frameTime);

            gun.ShotRequested = false;
        }

        public void Draw(Gun gun, float frameTime)
        {
            var hasDrawnGun = false;
                    
            foreach(var decoration in gun.Decorations.OrderBy(x => x.Depth))
            {
                if(decoration.Depth > 0 && !hasDrawnGun)
                {
                    DrawGun(gun);
                    hasDrawnGun = true;
                }

                decoration.Update(gun, frameTime);
                decoration.Sprite.Update(frameTime);
                decoration.Sprite.Draw(new(gun.WorldPosition, gun.Rect.Size), gun.WorldRotation + decoration.Rotation, Color.White);
            }

            if (!hasDrawnGun)
                DrawGun(gun);
        }

        private void DrawGun(Gun gun)
        {
            gun.Sprite.Draw(gun);
        }
    }
}
