using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Projectiles;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Handlers.Players;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using Moq;
using Xunit;

namespace GalagaFighter.Core2.Tests.Handlers.Players
{
    public class PlayerAffectorTestFixture : HandlerTestBase
    {
        private readonly Player _player;
        private readonly IPlayerAffector _affector;

        private PlayerModifiers? _assignedModifiers = null;

        private readonly Mock<IObjectService> _objectService = new Mock<IObjectService>();
        private readonly Mock<IGameObjectPositionService> _gameObjectPositionService = new Mock<IGameObjectPositionService>();
        private readonly List<GameObject> _addedGameObjects = [];
        private readonly Dictionary<GameObject, List<GameObject>> _registeredParents = [];

        public PlayerAffectorTestFixture()
        {
            _affector = new PlayerAffector(_gameDataRegistry.Object, _objectService.Object, _gameObjectPositionService.Object);
            _player = CreatePlayer();

            SetupDefaultData<PlayerModifiers>();
            SetupDefaultData<PlayerEffects>();

            _gameDataRegistry.Setup(x => x.Set(_player, It.IsAny<PlayerModifiers>()))
                .Callback<Player, PlayerModifiers>((p, m) => _assignedModifiers = m);

            _objectService.Setup(x => x.Add(It.IsAny<GameObject>()))
                .Callback<GameObject>(g => _addedGameObjects.Add(g));

            _gameObjectPositionService.Setup(x => x.RegisterParent(It.IsAny<GameObject>(), It.IsAny<GameObject[]>()))
                .Callback<GameObject, GameObject[]>((p,c) => {
                     if (!_registeredParents.ContainsKey(p))
                         _registeredParents[p] = [];

                     _registeredParents[p].AddRange(c);
                 });
        }

        [Fact]
        public void WhenPlayerHasNoModifiers_ThenRecalculateModifiers()
        {
            _affector.Affect(_player, 0);

            Assert.NotNull(_assignedModifiers);
        }

        [Fact]
        public void WhenPlayerHasEffects_AndRequiresRerollingIsTrue_ThenRecalculateModifiers()
        {
            Given<PlayerModifiers>(_player, new());
            Given<PlayerEffects>(_player, new()
            {
                RequireRerolling = true,
            });

            _affector.Affect(_player, 0);

            Assert.NotNull(_assignedModifiers);
        }

        [Fact]
        public void WhenPlayerHasEffects_AndRequiresRerollingIsNotTrue_ThenRecalculateModifiers()
        {
            Given<PlayerModifiers>(_player, new());
            Given<PlayerEffects>(_player, new()
            {
                RequireRerolling = false,
            });

            _affector.Affect(_player, 0);

            Assert.Null(_assignedModifiers);
        }

        [Fact]
        public void WhenPlayerHasEffect_AndEffectCountMatches_AndEffectsAreAllActive_ThenDoNotRecalculateModifiers()
        {
            Given<PlayerModifiers>(_player, new());
            Given<PlayerEffects>(_player, new([new DefaultShootEffect()])
            {
                RequireRerolling = false
            });

            _affector.Affect(_player, 0);

            Assert.Null(_assignedModifiers);
        }

        [Fact]
        public void WhenPlayerHasNewEffect_ThenEffectsAreApplied()
        {
            Given<PlayerModifiers>(_player, new());
            Given<PlayerEffects>(_player, new([new TestEffect((t,m) => m.Stats.SpeedMultiplier = 10)])
            {
                RequireRerolling = true
            });

            _affector.Affect(_player, 0);

            Assert.NotNull(_assignedModifiers);
            Assert.Equal(10, _assignedModifiers!.Stats.SpeedMultiplier);
        }

        [Fact]
        public void WhenRerolling_AndCollectibleRemains_ThenCollectibleInstancePersists_AndNoGunsAreAdded()
        {
            var existingGun = CreateDefaultGun(_player);
            var testEffect = new TestEffect((t,m) => m.Guns.Create[t] = p => [existingGun]);
            var guns = GetCollectibles<Gun>(testEffect, [existingGun]);

            Given<PlayerModifiers>(_player, new() { Guns = guns });
            Given<PlayerEffects>(_player, new([testEffect])
            {
                RequireRerolling = true
            });

            _affector.Affect(_player, 0);

            Assert.NotNull(_assignedModifiers);
            Assert.Equal(1, _assignedModifiers!.Guns.Count);
            Assert.Equal(existingGun, _assignedModifiers!.Guns[0]);
            Assert.Equal(0, _addedGameObjects.Count);
            Assert.Equal(0, _registeredParents.Count);
        }

        [Fact]
        public void WhenRerolling_AndCollectibleIsNew_ThenCollectibleInstanceAdded()
        {
            var newGun = CreateDefaultGun(_player);
            var testEffect = new TestEffect((t, m) => m.Guns.Create[t] = p => [newGun]);
            
            Given<PlayerModifiers>(_player, new() { Guns = [] });
            Given<PlayerEffects>(_player, new([testEffect])
            {
                RequireRerolling = true
            });

            _affector.Affect(_player, 0);

            Assert.NotNull(_assignedModifiers);
            Assert.Equal(1, _assignedModifiers!.Guns.Count);
            Assert.Equal(newGun, _assignedModifiers!.Guns[0]);
            Assert.Equal(1, _addedGameObjects.Count);
            Assert.Equal(newGun, _addedGameObjects[0]);
            Assert.Equal(1, _registeredParents.Count);
            Assert.Equal(newGun, _registeredParents[_player][0]);
        }

        private class TestEffect : PlayerEffect
        {
            private readonly Action<TestEffect, PlayerModifiers> _apply;

            public TestEffect(Action<TestEffect, PlayerModifiers> apply)
            {
                _apply = apply;
            }

            public override void Apply(PlayerModifiers modifiers)
            {
                _apply(this, modifiers);
            }
        }
    }
}
