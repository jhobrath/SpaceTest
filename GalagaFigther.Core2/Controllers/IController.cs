using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Controllers
{
    public interface IController<T> where T : GameObject
    {
        void Update(T gameObject, float frameTime);
        void Draw(T gameObject, float frameTime);
    }
}
