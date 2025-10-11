using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Controllers
{
    public class PlayerController : ControllerBase<Player>
    {
        public PlayerController(IGameDataRegistry gameDataRegistry)
            : base(gameDataRegistry)
        {
        }

        public override void Update(Player player, float frameTime)
        {
            var data = _gameDataRegistry.Get<PlayerGameData>(player);
            player.Move(player.Speed.X * frameTime, player.Speed.Y * frameTime);
            player.Rotation += 120 * frameTime;
        }

        public override void Draw(Player player, float frameTime)
        {
            player.Sprite.Draw(player);
        }
    }
}
