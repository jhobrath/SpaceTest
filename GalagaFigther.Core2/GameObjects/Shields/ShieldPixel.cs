using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Services.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace GalagaFighter.Core2.GameObjects.Shields
{
    public class ShieldPixel : GameObject
    {
        public ShieldPixel(Guid owner, Vector2 position) 
            : base(owner, position, new(30, 10), new(0,0), new StillImageSprite("Sprites/Shield/ShieldPixel.png"))
        {
        }

        public float Health { get; set; } = 10f;
    }
}
