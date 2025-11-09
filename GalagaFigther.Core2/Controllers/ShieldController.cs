using GalagaFighter.Core2.GameObjects.Shields;
using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Controllers
{
    public interface IShieldController : IController<ShieldPixel>
    {

    }
    public class ShieldController : IShieldController
    {
        public void Draw(ShieldPixel shield, float frameTime)
        {
            shield.Sprite.Draw(shield);
        }

        public void Update(ShieldPixel shieldPixel, float frameTime)
        {
        }
    }
}
