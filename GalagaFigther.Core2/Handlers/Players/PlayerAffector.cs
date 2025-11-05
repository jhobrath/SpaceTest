using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using System.Runtime.InteropServices;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerAffector
    {
        void Affect(Player player, float frameTime);
    }
    public class PlayerAffector : IPlayerAffector
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        private readonly IGameObjectPositionService _gameObjectPositionService;

        public PlayerAffector(IGameDataRegistry gameDataRegistry, IObjectService objectService,
            IGameObjectPositionService gameObjectPositionService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
            _gameObjectPositionService = gameObjectPositionService;
        }

        public void Affect(Player player, float frameTime)
        {
            var effects = _gameDataRegistry.Get<PlayerEffects>(player);
            RecalculateEffectsIfNecessary(player, effects);

            foreach (var effect in effects)
                effect.Update(frameTime);
        }

        private void RecalculateEffectsIfNecessary(Player player, PlayerEffects effects)
        {
            if (!_gameDataRegistry.Has<PlayerModifiers>(player))
                RecalculateModifiers(player, effects);

            else if (effects.Any(x => !x.IsActive))
                RecalculateModifiers(player, effects);

            else if (effects.RequireRerolling)
                RecalculateModifiers(player, effects);

            effects.RequireRerolling = false;
        }

        private void RecalculateModifiers(Player player, PlayerEffects effects)
        {
            var oldModifiers = DetermineRemainingCollectibles(player, effects);
            var newModifiers = new PlayerModifiers();

            effects.RemoveAll(x => !x.IsActive);
            foreach (var effect in effects)
                effect.Apply(newModifiers);

            effects.RemoveAll(x => x.DeactivateAfterApply);

            ApplyDecorationInitialRotation(player, newModifiers.Decorations);
            PersistPriorCollectibles(oldModifiers, newModifiers, effects);

            CreateParticleEmitters(player, newModifiers, effects);
            CreateDecorations(player, newModifiers, effects);

            newModifiers.PlayerActions.ForEach(x => x(player));
            newModifiers.PlayerActions.Clear();

            foreach (var gun in player.Guns)
            { 
                if(!_objectService.ContainsKey(gun.Id))
                { 
                    _objectService.Add(gun);
                    _gameObjectPositionService.RegisterParent(player, gun);
                }
            }

            _gameDataRegistry.Set(player, newModifiers);
        }

        private void CreateParticleEmitters(Player player, PlayerModifiers newModifiers, PlayerEffects effects)
        {
            var effectIds = effects.Select(x => x.Id).ToList();
            foreach (var emitterToCreate in newModifiers.ParticleEmitters.Create)
            {
                //Only add the emitter if this effect is new
                if (newModifiers.ParticleEmitters.Any(x => effectIds.Contains(x.CollectedFrom)))
                    continue;

                var emitters = emitterToCreate.Value(player, newModifiers);
                foreach (var emitter in emitters)
                {
                    newModifiers.ParticleEmitters.Add(emitter);
                    _objectService.Add(emitter);
                    _gameObjectPositionService.RegisterParent(player, emitter);
                }
            }
        }

        private void CreateDecorations(Player player, PlayerModifiers newModifiers, PlayerEffects effects)
        {
            var effectIds = effects.Select(x => x.Id).ToList();
            foreach (var decorationToCreate in newModifiers.Decorations.Create)
            {
                //Only add the emitter if this effect is new
                if (newModifiers.Decorations.Any(x => effectIds.Contains(x.CollectedFrom)))
                    continue;

                var decorations = decorationToCreate.Value(player, newModifiers);
                foreach (var decoration in decorations)
                {
                    newModifiers.Decorations.Add(decoration);
                }
            }
        }

        private void ApplyDecorationInitialRotation(Player player, List<Decoration> decorations)
        {
            var rotationData = _gameDataRegistry.Get<PlayerRotationData>(player);
            foreach (var decoration in decorations)
                if (!decoration.MaintainRotation)
                    decoration.InitialRotation = rotationData.InitialRotation;
        }

        private void PersistPriorCollectibles(PlayerModifiers oldModifiers, PlayerModifiers newModifiers, PlayerEffects effects)
        {
            ApplyPriorCollectibles(oldModifiers.Decorations, newModifiers.Decorations, effects);
            ApplyPriorCollectibles(oldModifiers.ParticleEmitters, newModifiers.ParticleEmitters, effects);
        }

        private void ApplyPriorCollectibles<T>(PlayerModifiersChildren<T> oldCollectibles, PlayerModifiersChildren<T> newCollectibles, PlayerEffects playerEffects)
            where T : class, ICollectible
        {
            foreach (var collectible in oldCollectibles)
            {
                if (playerEffects.All(x => x.Id != collectible.CollectedFrom))
                {
                    collectible.IsActive = false;
                    continue;
                }

                newCollectibles.Add(collectible);
            }
        }

        private PlayerModifiers DetermineRemainingCollectibles(Player player, PlayerEffects effects)
        {
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);

            ClearOrphanedCollectibles(effects, modifiers.Decorations);
            ClearOrphanedCollectibles(effects, modifiers.ParticleEmitters);

            return modifiers;
        }

        private void ClearOrphanedCollectibles<T>(PlayerEffects effects, PlayerModifiersChildren<T> children)
            where T : class, ICollectible
        {
            for (var i = children.Count - 1; i >= 0; i--)
            {
                if (effects.Any(e => e.Id == children[i].CollectedFrom))
                    continue;

                children[i].IsActive = false;
                children.RemoveAt(i);
            }
        }
    }
}
