using Microsoft.Extensions.DependencyInjection;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Controllers;
using System;

namespace GalagaFighter.Core
{
    public static class Registry
    {
        private static ServiceProvider? _provider;

        public static void Configure()
        {
            var services = new ServiceCollection();

            // SINGLETONS - Game-wide state managers that live for entire application
            services.AddSingleton<IObjectService, ObjectService>(); // Manages all game objects
            services.AddSingleton<IInputService, InputService>(); // Tracks input state across frames
            services.AddSingleton<IPowerUpService, PowerUpCreationService>();
            services.AddSingleton<IPowerUpControllerFactory, PowerUpControllerFactory>();

            // TRANSIENT - Stateless services that can be created fresh each time
            services.AddTransient<ICollisionCreationService, CollisionCreationService>();
            services.AddTransient<IPlayerProjectileCollisionPlanker, PlayerProjectileCollisionPlanker>();
            services.AddTransient<IPlayerProjectileCollisionService, PlayerProjectileCollisionService>();
            services.AddTransient<IProjectilePowerUpCollisionService, ProjectilePowerUpCollisionService>();
            services.AddTransient<IPlayerPowerUpCollisionService, PlayerPowerUpCollisionService>();
            services.AddTransient<IPowerUpController, PowerUpController>();
            services.AddTransient<IPlayerMover, PlayerMover>();
            services.AddTransient<IProjectileController, ProjectileController>();
            services.AddTransient<IPlayerShooter, PlayerShooter>();
            services.AddTransient<IPlayerSwitcher, PlayerSwitcher>();
            services.AddTransient<IPlayerController, PlayerController>();
            services.AddTransient<IProjectileRotator, ProjectileRotator>();
            services.AddTransient<IProjectileMover, ProjectileMover>();
            services.AddTransient<IProjectileMoverWindUpper, ProjectileMoverWindUpper>();
            services.AddTransient <IProjectileMoverPlanker, ProjectileMoverPlanker>();
            services.AddTransient <IPlayerDrawer, PlayerDrawer>();
            services.AddTransient <IMagnetProjectileService, MagnetProjectileService>();

            _provider = services.BuildServiceProvider();
        }

        public static T Get<T>() where T : notnull
        {
            if (_provider == null)
                throw new InvalidOperationException("Registry not initialized. Call Registry.Configure() first.");
            return _provider.GetRequiredService<T>();
        }
    }
}
