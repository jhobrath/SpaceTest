using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.Handlers.Players;
using GalagaFigther.Core2.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2
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
            services.AddSingleton<IPlayerMover, PlayerMover>();
            services.AddSingleton<IPlayerRotator, PlayerRotator>();
            services.AddSingleton<IPlayerDrawer, PlayerDrawer>();
            services.AddSingleton<IPlayerAffector, PlayerAffector>();
            services.AddSingleton<IPlayerShooter, PlayerShooter>();
            services.AddSingleton<IPlayerController, PlayerController>();
            services.AddSingleton<IProjectileController, ProjectileController>();
            services.AddSingleton<IPowerUpController, PowerUpController>();
            services.AddSingleton<IGameObjectUpdateService, GameObjectUpdateService>();
            services.AddSingleton<IPersistentValueHandler, PersistentValueHandler>();
            services.AddSingleton<IInitialObjectBuilder, InitialObjectBuilder>();
            services.AddSingleton<IPowerUpCreationService, PowerUpCreationService>();
            services.AddSingleton<IGame, Game>();

            _provider = services.BuildServiceProvider();
            return _provider;
        }

        public static T Get<T>() where T : notnull
        {
            if (_provider == null)
                _provider = Configure();

            return _provider.GetRequiredService<T>();
        }
    }
}
