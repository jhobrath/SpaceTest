using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Projectiles
{
    public interface IMagnetProjectileService
    {
        void DeMagnetize(Player player);
        void Magnetize(Player player);
    }
    public class MagnetProjectileService : IMagnetProjectileService
    {
        private IObjectService _objectService;
        private IInputService _inputService;

        public MagnetProjectileService(IObjectService objectService, IInputService inputService)
        {
            _objectService = objectService;
            _inputService = inputService;
        }

        public void DeMagnetize(Player player)
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();
            AudioService.StopMagnetSound();
            AudioService.PlayMagnetReleaseSound();

            foreach (var projectile in projectiles.Where(x => x.IsMagnetic))
            {
                var offset = (projectile.Center.X - player.Center.X);
                var distance = Math.Abs(offset);

                if (distance < 500f)
                {
                    projectile.Modifiers.RedAlpha = 1f;
                    projectile.Modifiers.BlueAlpha = 1f;
                    projectile.SetOwner(player.Id);
                    projectile.HurryTo(1000f * (player.IsPlayer1 ? 1 : -1), 0f);
                }
            }
        }

        public void Magnetize(Player player)
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();

            foreach(var projectile in projectiles.Where(x => x.IsMagnetic))
            {
                var magnetPoint = new Vector2(
                    player.IsPlayer1 ? (player.Rect.X + player.Rect.Width + 100f) : (player.Rect.X - 100f),
                    player.Center.Y);

                var distanceX = magnetPoint.X - projectile.Center.X;
                var distanceY = magnetPoint.Y - projectile.Center.Y;
                var totalDistance = distanceX + distanceY;

                var pctX = distanceX / totalDistance;
                var pctY = distanceY / totalDistance;

                var totalAmountToMove = (totalDistance / 30f);// * Raylib.GetFrameTime();

                var amountToMoveX = pctX * totalAmountToMove;
                var amountToMoveY = pctY * totalAmountToMove;

                var frameTime = Raylib.GetFrameTime();

                projectile.Modifiers.VerticalPositionIncrement = 0f;
                projectile.Modifiers.VerticalPositionMultiplier = 1f;
                projectile.HurryTo(amountToMoveX/frameTime, amountToMoveY / frameTime);


                var offset = (projectile.Center.X - player.Center.X);
                var distance = Math.Abs(offset);
                if(distance < 500f)
                {
                    projectile.Modifiers.RedAlpha = .7f;
                    projectile.Modifiers.BlueAlpha = .9f;
                }

                projectile.Move(amountToMoveX - projectile.Speed.X * frameTime, amountToMoveY - projectile.Speed.Y * Raylib.GetFrameTime());

                if(player.IsPlayer1 && projectile.Rect.X < magnetPoint.X)
                    projectile.MoveTo(x: magnetPoint.X);

                if (!player.IsPlayer1 && projectile.Rect.X + projectile.Rect.Width > magnetPoint.X)
                    projectile.MoveTo(x: magnetPoint.X - projectile.Rect.Width);
            }
        }
    }
}
