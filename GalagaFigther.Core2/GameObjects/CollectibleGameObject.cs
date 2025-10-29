using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects
{
    public interface ICollectible
    {
        Guid CollectedFrom { get; set; }
        bool IsActive { get; set; }
    }

    public class CollectibleGameObject : GameObject, ICollectible
    {
        public Guid CollectedFrom { get; set; }

        public CollectibleGameObject(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite) 
            : base(owner, position, size, speed, sprite)
        {
        }
    }
}
