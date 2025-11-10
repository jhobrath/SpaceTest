using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Handlers.Collisions;
using GalagaFighter.Core2.Handlers.Guns;
using GalagaFighter.Core2.Handlers.ParticleEmitters;
using GalagaFighter.Core2.Handlers.Players;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.CPU;

namespace GalagaFighter.Core2
{

    public static class Registry
    {
        private static ServiceProvider? _provider;

        public static ServiceProvider Configure()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IGameDataRegistry, GameDataRegistry>();
            services.AddSingleton<IObjectService, ObjectService>();
            services.AddSingleton<IInputService, InputService>();
            services.AddSingleton<IPlayerAccelerator, PlayerMover>();
            services.AddSingleton<IPlayerRotator, PlayerRotator>();
            services.AddSingleton<IPlayerDrawer, PlayerDrawer>();
            services.AddSingleton<IPlayerAffector, PlayerAffector>();
            services.AddSingleton<IPlayerDefender, PlayerDefender>();
            services.AddSingleton<IPlayerShooter, PlayerShooter>();
            services.AddSingleton<IPlayerController, PlayerController>();
            services.AddSingleton<IGunController, GunController>();
            services.AddSingleton<IProjectileController, ProjectileController>();
            services.AddSingleton<IPowerUpController, PowerUpController>();
            services.AddSingleton<IRopeAttachmentController, RopeAttachmentController>();
            services.AddSingleton<ISpringAttachmentController, SpringAttachmentController>();

            // Particle emitter handlers with action-based names
            services.AddSingleton<IParticleEmissionTimer, ParticleDurationHandler>();
            services.AddSingleton<IParticleVelocityCalculator, ParticleVelocityCalculator>();
            services.AddSingleton<IParticleTextureSelector, ParticleTextureSelector>();
            services.AddSingleton<IParticleCreationHandler, ParticleCreationHandler>();
            services.AddSingleton<IParticleEmissionSpawner, ParticleEmissionSpawner>();
            services.AddSingleton<IParticleEmissionShepherd, ParticleUpdateHandler>();
            services.AddSingleton<IParticleController, ParticleController>();
            services.AddSingleton<IParticleEmitterController, ParticleEmitterController>();

            services.AddSingleton<IGameObjectUpdateService, GameObjectUpdateService>();
            services.AddSingleton<IPersistentValueHandler, PersistentValueHandler>();
            services.AddSingleton<IInitialObjectBuilder, InitialObjectBuilder>();
            services.AddSingleton<IPowerUpCreationService, PowerUpCreationService>();
            services.AddSingleton<IProjectilePowerUpCollisionHandler, ProjectilePowerUpCollisionHandler>();
            services.AddSingleton<IPlayerPowerUpCollisionHandler, PlayerPowerUpCollisionHandler>();
            services.AddSingleton<ICollisionService, CollisionService>();
            services.AddSingleton<IGameObjectPositionService, GameObjectPositionService>();
            services.AddSingleton<IPlayerBounder, PlayerBounder>();
            services.AddSingleton<IPlayerTurretDeployer, PlayerTurretDeployer>();
            services.AddSingleton<ITurretController, TurretController>();
            services.AddSingleton<IProjectileShooter, ProjectileShooter>();
            services.AddSingleton<ICollisionController, CollisionController>();
            services.AddSingleton<IPlayerProjectileCollisionHandler, PlayerProjectileCollisionHandler>();
            services.AddSingleton<IProjectileProjectileCollisionHandler, ProjectileProjectileCollisionHandler>();
            services.AddSingleton<ISpringEdgeCollisionHandler, SpringEdgeCollisionHandler>();
            services.AddSingleton<IProjectileEdgeCollisionHandler, ProjectileEdgeCollisionHandler>();
            services.AddSingleton<IGame, Game>();
            services.AddSingleton<IShieldController, ShieldController>();
            services.AddSingleton<IPlayerShielder, PlayerShielder>();
            services.AddSingleton<IProjectileShieldCollisionHandler, ProjectileShieldCollisionHandler>();

            services.AddSingleton<IGunRecoiler, GunRecoiler>();
            services.AddSingleton<IGunRotator, GunRotator>();
            services.AddSingleton<IGunShooter, GunShooter>();
            services.AddSingleton<IHudService, HudService>();
            
            // CPU AI System
            services.AddSingleton<ICpuInputService, CpuInputService>();
            services.AddSingleton<ICpuDecisionEngine, CpuDecisionEngine>();

            // This will automatically find all registered services that implement IClearable
            services.AddSingleton<IClearableServiceClearer>(provider =>
            {
                var clearableServices = new List<IClearable>();

                var serviceDescriptors = services.Where(s =>
                    s.Lifetime == ServiceLifetime.Singleton &&
                    typeof(IClearable).IsAssignableFrom(s.ServiceType));

                foreach (var descriptor in serviceDescriptors)
                {
                    var service = provider.GetRequiredService(descriptor.ServiceType);
                    if (service is IClearable clearable)
                        clearableServices.Add(clearable);
                }

                return new ClearableServiceClearer(clearableServices);
            });

            // Register all IProjectileBehavior implementations as singletons (as self and as interface)
            var projBehaviors = typeof(IProjectileBehavior).Assembly.GetTypes()
                .Where(t =>
                    typeof(IProjectileBehavior).IsAssignableFrom(t)
                    && !t.IsInterface
                    && !t.IsAbstract
                    && !t.IsGenericTypeDefinition
                    && t != typeof(ProjectileBehaviorBase)
                )
                .ToList();

            foreach (var type in projBehaviors)
            {
                services.AddSingleton(typeof(IProjectileBehavior), type);
            }

            // Register all IProjectilePlayerCollisionBehavior implementations as singletons (like IClearable)
            var playerCollisionBehaviors = typeof(IProjectilePlayerCollisionBehavior).Assembly.GetTypes()
                .Where(t =>
                    typeof(IProjectilePlayerCollisionBehavior).IsAssignableFrom(t)
                    && !t.IsInterface
                    && !t.IsAbstract
                    && !t.IsGenericTypeDefinition)
                .ToList();

            foreach (var type in playerCollisionBehaviors)
            {
                services.AddSingleton(typeof(IProjectilePlayerCollisionBehavior), type);
            }

            var _provider = services.BuildServiceProvider();
            return _provider;
        }

        public static T Get<T>() where T : notnull
        {
            if (_provider == null)
                _provider = Configure();

            return _provider.GetRequiredService<T>();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClearableServices(this IServiceCollection services)
        {
            

            return services;
        }
    }
}
