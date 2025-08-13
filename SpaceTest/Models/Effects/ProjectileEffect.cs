using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Players;
using GalagaFigther.Models.Projectiles;
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
        protected virtual Vector2 SpawnOffset => new Vector2(0,0);
        protected virtual float? OnHitMaxRemainingTime => null;
        protected virtual string Texture => null;

        protected abstract Projectile Spawn(Rectangle rect, Vector2 speed);

        public override void OnShoot(Game game)
        {
            var combat = Player.GetCombat();
            if (!combat.CanFire(Raylib.IsKeyDown(Player.GetShootKey()), Player.Stats))
                return;
            combat.ResetFireTimer();
            var scaleFactor = Player.GetScaleFactor();
            var _useLeftEngine = Player.ToggleEngine();

            var spawnPoint = combat.GetProjectileSpawnPoint(Player.Rect, ProjectileWidth * scaleFactor, ProjectileHeight * scaleFactor, _useLeftEngine);

            spawnPoint.X += SpawnOffset.X * scaleFactor*(Player.IsPlayer1 ? 1 : -1);
            spawnPoint.Y += SpawnOffset.Y * scaleFactor*(_useLeftEngine ? 1 : -1);

            var rect = new Rectangle(spawnPoint.X, spawnPoint.Y, ProjectileWidth * scaleFactor, ProjectileHeight * scaleFactor);
            var speed = new Vector2(combat.GetProjectileSpeed(), Math.Min(3, Math.Max(-3, Player.GetMovement().Speed * .3333f)));
            var projectile = Spawn(rect, speed);
            game.AddGameObject(projectile);

            game.PlayShootSound();
            if (OneTimeUse) 
                IsActive = false;
        }


        public override void ModifyPlayerRendering(PlayerRendering playerRendering)
        {
            if (Texture != null)
                playerRendering.Texture = Texture;
        }

        public virtual void OnHit()
        {
            if(OnHitMaxRemainingTime != null)
                SetMaxRemainingTime(OnHitMaxRemainingTime.Value);
        }
    }
}
