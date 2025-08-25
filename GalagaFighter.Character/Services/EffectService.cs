using GalagaFighter.CharacterScreen.Models;
using System.Collections.Generic;

namespace GalagaFighter.CharacterScreen.Services
{
    public interface IEffectService
    {
        List<OffensiveEffect> GetAvailableEffects();
        OffensiveEffect? GetEffectById(string id);
    }

    public class EffectService : IEffectService
    {
        private readonly List<OffensiveEffect> _effects;

        public EffectService()
        {
            _effects = InitializeOffensiveEffects();
        }

        public List<OffensiveEffect> GetAvailableEffects()
        {
            return _effects;
        }

        public OffensiveEffect? GetEffectById(string id)
        {
            return _effects.Find(e => e.Id == id);
        }

        private List<OffensiveEffect> InitializeOffensiveEffects()
        {
            return new List<OffensiveEffect>
            {
                new OffensiveEffect
                {
                    Id = "surprise_shot",
                    Name = "Surprise Shot",
                    Description = "Randomly fires additional bullets during combat",
                    IconPath = "Sprites/Effects/surprise.png",
                    EffectClassName = "GalagaFighter.Core.Models.Effects.SurpriseShotEffect",
                    Category = EffectCategory.Offensive
                },
                new OffensiveEffect
                {
                    Id = "timed_barrage",
                    Name = "Timed Barrage",
                    Description = "Periodically unleashes rapid-fire bullet bursts",
                    IconPath = "Sprites/Effects/barrage.png",
                    EffectClassName = "GalagaFighter.Core.Models.Effects.TimedBarrageEffect",
                    Category = EffectCategory.Offensive
                },
                new OffensiveEffect
                {
                    Id = "ricochet",
                    Name = "Ricochet",
                    Description = "Bullets bounce off walls to hit enemies",
                    IconPath = "Sprites/Effects/ricochet.png",
                    EffectClassName = "GalagaFighter.Core.Models.Effects.RicochetEffect",
                    Category = EffectCategory.Offensive
                },
                new OffensiveEffect
                {
                    Id = "splitter",
                    Name = "Splitter",
                    Description = "Bullets split into multiple projectiles on impact",
                    IconPath = "Sprites/Effects/splitter.png",
                    EffectClassName = "GalagaFighter.Core.Models.Effects.SplitterEffect",
                    Category = EffectCategory.Offensive
                }
                // TODO: Add 4 more effects later to make 8 total
            };
        }
    }
}