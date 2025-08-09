using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Models.Effects
{
    public abstract class ProjectileEffect : PlayerEffect
    {
        public ProjectileEffect(Player player) : base(player)
        {
        }

        protected virtual int ProjectileWidth { get; } = 30;
        protected virtual int ProjectileHeight { get; } = 15;
        protected virtual bool OneTimeUse { get; } = false;

        public override void OnShoot(Game game)
        {
            var combat = Player.GetCombat();
            if (!combat.CanFire(Raylib.IsKeyDown(Player.GetShootKey()), Player.Stats))
                return;
            combat.ResetFireTimer();
            var scaleFactor = Player.GetScaleFactor();
            var _useLeftEngine = Player.ToggleEngine();

            var spawnPoint = combat.GetProjectileSpawnPoint(Player.Rect, ProjectileWidth * scaleFactor, ProjectileHeight * scaleFactor, _useLeftEngine);
            var rect = new Rectangle(spawnPoint.X, spawnPoint.Y, ProjectileWidth * scaleFactor, ProjectileHeight * scaleFactor);
            var speed = new Vector2(combat.GetProjectileSpeed(), Math.Min(3, Math.Max(-3, Player.GetMovement().Speed * .3333f)));
            var projectile = ProjectileFactory.Create(this, rect, speed, Player);
            game.AddGameObject(projectile);

            game.PlayShootSound();
            if (OneTimeUse) 
                IsActive = false;
        }
    }
}
