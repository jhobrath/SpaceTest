using GalagaFighter.Core2.GameObjects.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Controllers
{
    public interface ICollisionController : IController<Collision>
    { }
    public class CollisionController : ICollisionController
    {
        public void Draw(Collision gameObject, float frameTime)
        {
            gameObject.Sprite.Draw(gameObject);
        }

        public void Update(Collision gameObject, float frameTime)
        {
            gameObject.Lifetime += frameTime;

            if(gameObject.Lifetime > gameObject.Duration)
            {
                gameObject.IsActive = false;
            }
        }
    }
}
