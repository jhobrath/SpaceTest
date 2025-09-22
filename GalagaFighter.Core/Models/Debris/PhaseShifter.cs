using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Debris
{
    public class PhaseShifter : GameObject
    {
        public float Health { get; set; } = 100f;

        public PhaseShifter(Player player)
            : base(player.Id, SpriteGenerationService3.CreatePhaseShifter(),
                  GetInitialPosition(player),
                  new Vector2(160, 30),
                  player.Speed)
        {
        }

        private static Vector2 GetInitialPosition(Player player)
        {
            var positionX = player.Position.X;
            var positionY = player.Center.Y;

            return new Vector2(positionX, positionY);
        }

        public override void Update(Game game)
        {
        }

        public override void Draw()
        {
            Sprite.Draw(Position, Rotation, 160, 30, Raylib_cs.Color.White);

            var lifePct = Health / 100f;
            var lifebarColor = Color.White.ApplyRed(1 - lifePct).ApplyGreen(lifePct);
            Raylib.DrawRectangle((int)Position.X + 5, (int)(Position.Y + Rect.Height), (int)(Rect.Width - 10), 1, Color.Gray);
            Raylib.DrawRectangle((int)Position.X + 5, (int)(Position.Y + Rect.Height), (int)((Rect.Width-10)*lifePct), 1, lifebarColor);
        }
    }
}
