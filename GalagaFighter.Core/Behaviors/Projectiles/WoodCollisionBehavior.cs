using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Projectiles
{
    public class WoodCollisionBehavior : ProjectileCollisionBehavior
    {
        private Rectangle? _plankedPosition;

        public WoodCollisionBehavior(IObjectService objectService) : base(objectService)
        {
        }

        public override void Apply(Projectile projectile, Player player)
        {
            var wood = (WoodProjectile)projectile;

            if(!wood.Planked)
            {
                if (projectile.Speed.X < 0)
                {
                    projectile.MoveTo(x: 0f);
                    projectile.HurryTo(0f, 0f);
                    projectile.SetMovementBehavior(new ProjectileMovementBehavior());
                    
                    wood.Planked = true;
                    _plankedPosition = player.Rect;
                }
                else if (projectile.Speed.X > 0)
                {
                    projectile.MoveTo(Game.Width - projectile.Rect.Width);
                    projectile.HurryTo(0f, 0f);
                    projectile.SetMovementBehavior(new ProjectileMovementBehavior());

                    wood.Planked = true;
                    _plankedPosition = player.Rect;
                }

                if(!wood.Planked)
                    base.Apply(projectile, player);
            }

            if (player.Id == projectile.Owner)
                return;

            if(_plankedPosition != null)
            {
                player.MoveTo(y: _plankedPosition.Value.Y);
            }
            else if (player.Speed.Y < 0)
            {
                if (player.Rect.Y < projectile.Rect.Y + player.Rect.Height)
                    player.MoveTo(y: projectile.Rect.Y + projectile.Rect.Height);
            }
            else if (player.Speed.Y > 0)
            {
                if (player.Rect.Y + player.Rect.Height > projectile.Rect.Y)
                    player.MoveTo(y: projectile.Rect.Y - player.Rect.Height);
            }
        }
    }
}
