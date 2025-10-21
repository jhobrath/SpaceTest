using GalagaFighter.Core2.GameObjects.Guns;
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
        private readonly IObjectService _objectService;

        public GunController(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Update(Gun gun, float frameTime)
        {
            
        }

        public void Draw(Gun gun, float frameTime)
        {
            //var owner = _objectService.Get(gun.Owner);
            //var worldLocation = owner.TopLeft + gun.TopLeft;
            //var gunRect = new Rectangle((int)worldLocation.X, (int)worldLocation.Y,
            //    (int)gun.Width, (int)gun.Height);
            //
            //gun.Sprite.Draw(gunRect, owner.Rotation + gun.Rotation, gun.Color);
        }
    }
}
