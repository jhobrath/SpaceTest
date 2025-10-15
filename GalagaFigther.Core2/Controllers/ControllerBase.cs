using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Controllers
{

    public interface IController<T> where T : GameObject
    {
        void Update(T gameObject, float frameTime);
        void Draw(T gameObject, float frameTime);
    }

    public abstract class ControllerBase<T> where T : GameObject
    {
        protected ControllerBase()
        {
        }

        public abstract void Update(T gameObject, float frameTime);
        public abstract void Draw(T gameObject, float frameTime);
    }
}
