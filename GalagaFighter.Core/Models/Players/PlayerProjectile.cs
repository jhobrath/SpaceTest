using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Players
{
    public class PlayerProjectile
    {
        //Multipliers
        public float DamageMultiplier { get; set; } = 1f;
        public float SpeedMultiplier { get; set; } = 1f;
        public float SizeMultiplier { get; set; } = 1f;
        public float RedAlpha { get; set; } = 1f;
        public float BlueAlpha { get; set; } = 1f;
        public float GreenAlpha { get; set; } = 1f;
        public float OpacityAlpha { get; set; } = 1f;

        //Activators
        public bool DoubleShot { get; set; } = false;

        //Creation
        public List<Func<IProjectileUpdater, Player, Vector2, Projectile>> Projectiles = [];

        //Callbacks
        public Action<Projectile>? OnShoot { get; set; } = null;
        public Action<Projectile>? OnProjectileDestroyed { get; set; } = null;

    }
}
