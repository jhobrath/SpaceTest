using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GalagaFighter.Core.Models.Players;
using Raylib_cs;

namespace GalagaFighter.Core.Models.Debris
{
    public class Tether : GameObject
    {
        public float Health { get; set; } = 100f;

        public Tether(Player player) 
            : base(player.Id, new SpriteWrapper(@"Sprites\Debris\TetherShield.png"), 
                  GetInitialPosition(player), 
                  new Vector2(50, 160), 
                  player.Speed)
        {
        }

        private static Vector2 GetInitialPosition(Player player)
        {
            var x = player.IsPlayer1 ? player.Rect.X + player.Rect.Width : player.Rect.X - 50f;
            var y = player.Rect.Y;

            return new Vector2(x, y);
        }

        public override void Draw()
        {
            Sprite.Draw(Rect.Position, 0f, 50, 160, Color.White);
        }

        public override void Update(Game game)
        {
            var frameTime = Raylib.GetFrameTime();
            Move(Speed.X * frameTime, Speed.Y * frameTime);
        }
    }
}
