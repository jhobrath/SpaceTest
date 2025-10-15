using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.GameObjects.Projectiles;
using GalagaFigther.Core2.Models.Players;
using GalagaFigther.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Handlers.Players
{
    public interface IPlayerShooter
    {
        void Shoot(Player player, float frameTime);
    }
    public class PlayerShooter : IPlayerShooter
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IInputService _inputService;
        private readonly IObjectService _objectService;

        public PlayerShooter(IInputService inputService, IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _inputService = inputService;
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Shoot(Player player, float frameTime)
        {
            var shootData = _gameDataRegistry.Get<PlayerShootData>();
                var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);

            if(shootData.ShotCountdown > 0)
            {
                shootData.ShotCountdown -= frameTime;
                return;
            }

            if(_inputService.Shoot.IsDown)
            {
                var posX = player.X + player.Width;
                var posY = player.Center.Y + (shootData.LastShotLeft ? 1 : -1) * 47f;
                var position = new Vector2(posX, posY);

                foreach(var item in modifiers.Projectile.OnShoot)
                {
                    var projectiles = item.Value(player.Id, position);
                    foreach(var projectile in projectiles)
                    { 
                        projectile.Move(y: -projectile.Height / 2);
                        _objectService.Add(projectile);
                    }
                }

                shootData.LastShotLeft = !shootData.LastShotLeft;
                shootData.ShotCountdown = .15f;

            }
        }
    }
}
