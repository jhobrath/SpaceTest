using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
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
        public Vector2 SizeMultiplier { get; set; } = new Vector2(1f,1f);
        public float RedAlpha { get; set; } = 1f;
        public float BlueAlpha { get; set; } = 1f;
        public float GreenAlpha { get; set; } = 1f;
        public float Opacity { get; set; } = 1f;

        //Offsets
        public float RotationOffset { get; set; } = 0f; //Always leave at zero in effects
        public float RotationOffsetIncrement { get; set; } = 0f; //Amount of rotation each second
        public float RotationOffsetMultiplier { get; set; } = 1f; //Amount to increase increment each second

        public float VerticalPositionOffset { get; set; } = 0f; //Always leave at zero in effects
        public float VerticalPositionIncrement { get; set; } = 0f; //Amount of rotation each second
        public float VerticalPositionMultiplier { get; set; } = 1f; //Amount to increase vertical movement each second

        //Movement
        public float WindUpDuration { get; set; } = 0f;
        public float WindUpSpeed { get; set; } = 0f;
        public float PlankDuration { get; set; } = 0f;
        public bool PlankStopsMovement { get; set; } = false;
        public bool IgnoreShipMovement { get; set; } = false;

        //Edge Collision
        public float CollideDistanceFromEdge { get; set; } = 0f;

        public Dictionary<PlayerEffect, List<float>> Phases { get; set; } = [];

        //Deactivation
        public bool DeactivateOnCollision { get; set; } = true;

        //Activators
        public bool DoubleShot { get; set; } = false;

        //Creation
        public List<Func<IProjectileController, Player, Vector2, PlayerProjectile, Projectile>> OnShootProjectiles = [];
        public List<Func<IProjectileController, Player, Vector2, PlayerProjectile, Projectile>> OneTimeProjectiles = [];

        //Events
        public Action<Projectile>? OnShoot { get; set; } = null;
        public Action<Projectile>? OnWindUpReleased { get; set; } = null;
        public Action<Projectile>? OnProjectileDestroyed { get; set; } = null;
        public Action<Projectile>? OnSpriteUpdate { get; internal set; }
        public Action<Projectile, PlayerEffect, int>? OnPhaseChange { get; set; } = null;
        public Func<Player, Projectile, List<GameObject>>? OnCollide { get; set; } = null;

        public SpriteWrapper? Sprite { get; set; } = null;
        public bool CanSplit { get; set; }
        public bool CanRicochet { get; set; }
        public Action? OnClone { get; set; }
        public Action<Projectile, Projectile, Player, Player>? OnNearProjectile { get; set; }
        public float Homing { get; internal set; }

        public PlayerProjectile Clone()
        {
            return new PlayerProjectile
            {
                DamageMultiplier = DamageMultiplier,
                SpeedMultiplier = SpeedMultiplier,
                SizeMultiplier = SizeMultiplier,
                RedAlpha = RedAlpha,
                BlueAlpha = BlueAlpha,
                GreenAlpha = GreenAlpha,
                Opacity = Opacity,
                DoubleShot = DoubleShot,
                OnShootProjectiles = OnShootProjectiles,
                OnShoot = OnShoot,
                OnProjectileDestroyed = OnProjectileDestroyed,
                WindUpSpeed = WindUpSpeed,
                WindUpDuration = WindUpDuration,
                PlankDuration = PlankDuration,
                PlankStopsMovement = PlankStopsMovement,
                CollideDistanceFromEdge = CollideDistanceFromEdge,
                OnWindUpReleased = OnWindUpReleased,
                OnSpriteUpdate = OnSpriteUpdate,
                OnCollide = OnCollide,
                DeactivateOnCollision = DeactivateOnCollision,
                Phases = Phases,
                OnPhaseChange = OnPhaseChange,
                Sprite = Sprite,
                RotationOffset = RotationOffset,
                RotationOffsetIncrement = RotationOffsetIncrement,
                RotationOffsetMultiplier = RotationOffsetMultiplier,
                VerticalPositionIncrement = VerticalPositionIncrement,
                VerticalPositionOffset = VerticalPositionOffset,
                VerticalPositionMultiplier = VerticalPositionMultiplier,
                IgnoreShipMovement = IgnoreShipMovement,
                CanSplit = CanSplit,
                CanRicochet = CanRicochet,
                OnClone = OnClone,
                OnNearProjectile = OnNearProjectile,
                Homing = Homing,


                //Don't include
                OneTimeProjectiles = []
            };
        }
    }
}
