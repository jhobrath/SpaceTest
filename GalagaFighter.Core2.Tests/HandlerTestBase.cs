using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.PowerUps;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using Moq;
using System.Numerics;

namespace GalagaFighter.Core2.Tests
{
    public abstract class HandlerTestBase
    {
        protected readonly Mock<IGameDataRegistry> _gameDataRegistry = new Mock<IGameDataRegistry>();

        protected HandlerTestBase()
        {
        }

        protected void SetupDefaultData<T>()
            where T : class, IGameObjectData<Player>, new()
        {
            _gameDataRegistry.Setup(x => x.Get<T>(It.IsAny<Player>())).Returns(new T());
        }

        protected void Given<T>(Player player, T? instance = null) where T : class, IGameObjectData<Player>, new()
        {
            _gameDataRegistry.Setup(x => x.Get<T>(player)).Returns(instance ?? new T());
            _gameDataRegistry.Setup(x => x.Has<T>(player)).Returns(true);
        }

        protected void Given<T>(PowerUp powerUp, T? instance = null) where T : class, IGameObjectData<PowerUp>, new()
        {
            _gameDataRegistry.Setup(x => x.Get<T>(powerUp)).Returns(instance ?? new T());
            _gameDataRegistry.Setup(x => x.Has<T>(powerUp)).Returns(true);
        }

        protected void Given<T>(Projectile projectile, T? instance = null) where T : class, IGameObjectData<Projectile>, new()
        {
            _gameDataRegistry.Setup(x => x.Get<T>(projectile)).Returns(instance ?? new T());
            _gameDataRegistry.Setup(x => x.Has<T>(projectile)).Returns(true);
        }

        protected void Given<T>(Turret turret, T? instance = null) where T : class, IGameObjectData<Turret>, new()
        {
            _gameDataRegistry.Setup(x => x.Get<T>(turret)).Returns(instance ?? new T());
            _gameDataRegistry.Setup(x => x.Has<T>(turret)).Returns(true);
        }

        protected void Given<T>(Gun gun, T? instance = null) where T : class, IGameObjectData<Gun>, new()
        {
            _gameDataRegistry.Setup(x => x.Get<T>(gun)).Returns(instance ?? new T());
            _gameDataRegistry.Setup(x => x.Has<T>(gun)).Returns(true);
        }

        protected void Given<T>(ParticleEmitter emitter, T? instance = null) where T : class, IGameObjectData<ParticleEmitter>, new()
        {
            _gameDataRegistry.Setup(x => x.Get<T>(emitter)).Returns(instance ?? new T());
            _gameDataRegistry.Setup(x => x.Has<T>(emitter)).Returns(true);
        }

        protected Player CreatePlayer(Vector2? position = null, Vector2? size = null, Vector2? speed = null, SpriteBase? sprite = null)
        {
            return new Player(
                position ?? Vector2.Zero,
                size ?? Vector2.Zero,
                speed ?? Vector2.Zero,
                sprite ?? new StillImageSprite("UnknownSprite"));
        }

        protected DefaultGun CreateDefaultGun(GameObject owner)
        {
            return new DefaultGun(owner);
        }

        protected PlayerModifiersChildren<T> GetCollectibles<T>(PlayerEffect effect, List<T> list)
            where T : class, ICollectible
        {
            list.ForEach(i => i.CollectedFrom = effect.Id);
            var collectibles = new PlayerModifiersChildren<T>(list)
            {
                Create = new() { { effect, p => list } }
            };

            return collectibles;
        }
    }
}
