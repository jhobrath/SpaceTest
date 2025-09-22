using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    internal class PhaseShiftedEffect : StatusEffect
    {
        public override string IconPath => "Sprites/Effects/statuses/phaseshifted.png";
        protected override float Duration => 2f;

        private SpriteDecorations _decorations;

        public PhaseShiftedEffect()
        {
            _decorations = new SpriteDecorations {
                { 
                    "PhaseShift",
                    new SpriteDecoration(SpriteGenerationService3.CreatePhaseShifterOverlay())
                    {
                        Size = new Vector2(160, 160),
                        Offset = Vector2.Zero,
                        FollowRotation = true
                    }
                }
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Untouchable = true;
            modifiers.Decorations.Apply(_decorations);
            modifiers.Display.Opacity *= .5f;
        }
    }
}
