using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class MudSplat : GameObject
    {
        private readonly float _rotation = 0f;
        private int _frame;
        private float _lifetime = 0f;

        public MudSplat(Guid owner, Vector2 initialPosition, Vector2 initialSize)
            : base(owner, GetSprite(), initialPosition, initialSize, new Vector2(0f, 0f))
        {
            AudioService.PlayMudSplat();
            _rotation = (float)Game.Random.NextDouble() * 360f;
            _frame = Game.Random.Next(0, 3); 
        }

        private static SpriteWrapper GetSprite()
        {
            return new SpriteWrapper(TextureService.Get("Sprites/Projectiles/mud_splat.png"), 3, 1000f, repeat: false);
        }

        public override void Update(Game game)
        {
            _lifetime += Raylib.GetFrameTime();
            if (_lifetime > 5f)
                IsActive = false;
        }

        public override void Draw()
        {
            Sprite.DrawAnimated(Center, _rotation, 300f, 300f, _frame);
        }
    }
}
