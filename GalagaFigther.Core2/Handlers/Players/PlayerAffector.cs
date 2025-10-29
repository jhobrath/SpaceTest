using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerAffector
    {
        void Affect(Player player, float frameTime);
    }
    public class PlayerAffector : IPlayerAffector
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public PlayerAffector(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Affect(Player player, float frameTime)
        {
            var effects = _gameDataRegistry.Get<PlayerEffects>(player);
            RecalculateEffectsIfNecessary(player, effects);

            foreach(var effect in effects)
                effect.Update(frameTime);
        }

        private void RecalculateEffectsIfNecessary(Player player, PlayerEffects effects)
        {
            if (!_gameDataRegistry.Has<PlayerModifiers>(player))
                RecalculateModifiers(player, effects);

            else if (effects.Any(x => !x.IsActive))
                RecalculateModifiers(player, effects);

            else if(effects.RequireRerolling)
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

            ApplyDecorationInitialRotation(player, newModifiers.Decorations);
            ApplyPriorCollectibles(oldModifiers, newModifiers, effects);

            _gameDataRegistry.Set(player, newModifiers);
        }

        private void ApplyDecorationInitialRotation(Player player, List<Decoration> decorations)
        {
            var rotationData = _gameDataRegistry.Get<PlayerRotationData>(player);
            foreach (var decoration in decorations)
                if (!decoration.MaintainRotation)
                    decoration.InitialRotation = rotationData.InitialRotation;
        }

        private void ApplyPriorCollectibles(PlayerModifiers oldModifiers, PlayerModifiers newModifiers, PlayerEffects effects)
        {
            ApplyPriorCollectibles(oldModifiers.Guns, newModifiers.Guns, effects);
            ApplyPriorCollectibles(oldModifiers.Decorations, newModifiers.Decorations, effects);
            ApplyPriorCollectibles(oldModifiers.Turrets, newModifiers.Turrets, effects);
            ApplyPriorCollectibles(oldModifiers.ParticleEmitters, newModifiers.ParticleEmitters, effects);
        }

        private void ApplyPriorCollectibles<T>(PlayerModifiersChildren<T> oldCollectibles, PlayerModifiersChildren<T> newCollectibles, PlayerEffects playerEffects) 
            where T : class, ICollectible
        {
            foreach(var collectible in oldCollectibles)
            {
                if (playerEffects.All(x => x.Id != collectible.CollectedFrom))
                    continue;

                newCollectibles.Add(collectible);
            }
        }

        private PlayerModifiers DetermineRemainingCollectibles(Player player, PlayerEffects effects)
        {
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);

            ClearOrphanedCollectibles(effects, modifiers.Guns);
            ClearOrphanedCollectibles(effects, modifiers.Decorations);
            ClearOrphanedCollectibles(effects, modifiers.Turrets);
            ClearOrphanedCollectibles(effects, modifiers.ParticleEmitters);

            return modifiers;
        }

        private void ClearOrphanedCollectibles<T>(PlayerEffects effects, PlayerModifiersChildren<T> children)
            where T : class, ICollectible
        {
            for(var i = children.Count - 1;i>=0;i--)
            {
                if (effects.Any(e => e.Id == children[i].CollectedFrom))
                    continue;

                children[i].IsActive = false;
                children.RemoveAt(i);
            }
        }
    }
}
