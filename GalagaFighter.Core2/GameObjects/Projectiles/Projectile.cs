File: GalagaFighter.Core2\GameObjects\Projectiles\Projectile.cs
````````csharp
        public float Homing { get; set; } = 0f;
        public float Veer { get; set; } = 0f;

        public override void Update(Projectile projectile, float frameTime)
        {
            // Get homing value from state model
            var homingState = _gameDataRegistry.Get<GalagaFighter.Core2.Models.Projectiles.HomingState>(projectile);
            if (homingState.Homing == 0)
                return;
            var player = _objectService.Get<Player>(projectile.Owner);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
            var opponent = _objectService.GetOpponent(player);
            var finalHomingFactor = (homingState.Homing + modifiers.HomingFactor) * frameTime * 3.0f;
            var currentSpeed = projectile.Speed.Length();
            var normalizedSpeed = Vector2.Normalize(projectile.Speed);
            var homingSpeed = Vector2.Normalize(-(projectile.WorldPosition - opponent.WorldPosition));
            var finalNormalizedSpeed = Vector2.Normalize(homingSpeed * finalHomingFactor + normalizedSpeed * (1 - finalHomingFactor));
            var finalSpeed = finalNormalizedSpeed * currentSpeed;
            projectile.HurryTo(finalSpeed.X, finalSpeed.Y);
        }

        public override void Update(Projectile projectile, float frameTime)
        {
            // Get veer value from state model
            var veerState = _gameDataRegistry.Get<GalagaFighter.Core2.Models.Projectiles.VeerState>(projectile);
            if (veerState.Veer == 0f)
                return;
            var veerData = _gameDataRegistry.Get<GalagaFighter.Core2.Models.Projectiles.ProjectileVeerState>(projectile);
            if (!veerData.Initialized)
            {
                veerData.OriginalSpeed = projectile.Speed;
                var random = new Random(Guid.NewGuid().GetHashCode());
                float curveStrength = (float)(random.NextDouble() * 2.0 - 1.0) * veerState.Veer;
                veerData.CurrentVeer = new System.Numerics.Vector2(curveStrength, 0);
                veerData.Initialized = true;
            }
            float speedMag = projectile.Speed.Length();
            if (speedMag == 0f) return;
            float curveStrengthUsed = veerData.CurrentVeer.X;
            float angle = (curveStrengthUsed / speedMag) * frameTime;
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            var v = projectile.Speed;
            var rotated = new System.Numerics.Vector2(
                v.X * cos - v.Y * sin,
                v.X * sin + v.Y * cos
            );
            projectile.HurryTo(rotated.X, rotated.Y);
        }
