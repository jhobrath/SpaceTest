using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

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
        public float RotationMultiplier { get; set; } = 1f;

        //Offsets
        public float RotationOffset { get; set; } = 1f;

        //Events
        public float WindUpDuration { get; set; } = 0f;
        public float WindUpSpeed { get; set; } = 0f;
        public float PlankDuration { get; set; } = 0f;
        public bool PlankStopsMovement { get; set; } = false;

        //State Management
        private Dictionary<string, object?> _data { get; set; } = [];
        public object this[string key] { get { return _data[key] ?? throw new ArgumentNullException(nameof(_data)); } set { _data[key] = value; } }

        //Deactivation
        public bool DeactivateOnCollision { get; set; } = true;

        //Activators
        public bool DoubleShot { get; set; } = false;

        //Creation
        public List<Func<IProjectileController, Player, Vector2, PlayerProjectile, Projectile>> Projectiles = [];


        //Callbacks
        public Action<Projectile>? OnShoot { get; set; } = null;
        public Action<Projectile>? OnWindUpReleased { get; set; } = null;
        public Action<Projectile>? OnProjectileDestroyed { get; set; } = null;
        public Action<Projectile>? OnSpriteUpdate { get; internal set; }

        public PlayerProjectile Clone()
        {
            return new PlayerProjectile
            {
                DamageMultiplier = DamageMultiplier,
                SpeedMultiplier = SpeedMultiplier,
                SizeMultiplier = SizeMultiplier,
                RotationMultiplier = RotationMultiplier,
                RotationOffset = RotationOffset,
                RedAlpha = RedAlpha,
                BlueAlpha = BlueAlpha,
                GreenAlpha = GreenAlpha,
                OpacityAlpha = OpacityAlpha,
                DoubleShot = DoubleShot,
                Projectiles = Projectiles,
                OnShoot = OnShoot,
                OnProjectileDestroyed = OnProjectileDestroyed,
                WindUpDuration = WindUpDuration,
                PlankDuration = PlankDuration,
                PlankStopsMovement = PlankStopsMovement,
                WindUpSpeed = WindUpSpeed,
                OnWindUpReleased = OnWindUpReleased,
                OnSpriteUpdate = OnSpriteUpdate,
                DeactivateOnCollision = DeactivateOnCollision
            };
        }
    }
}
