using GalagaFighter.Core.Handlers.Collisions;
using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using Raylib_cs;
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

        private readonly Player _player;
        private readonly PhaseShifter _shifter;
        private readonly SpriteDecorations _decorations;
        private Color _blue;
        private Color _red;
        private EffectModifiers _modifiers;

        public PhaseShiftedEffect(Player player, PhaseShifter shifter)
        {
            _player = player;
            _shifter = shifter;
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

            _red = Color.White.ApplyRed(.5f).ApplyAlpha(.8f);
            _blue = Color.White.ApplyBlue(.5f).ApplyGreen(.2f).ApplyAlpha(.8f);
        }

        public override void Apply(EffectModifiers modifiers)
        {
            _modifiers = modifiers;

            modifiers.Decorations.Apply(_decorations);
            modifiers.Display.Opacity *= .5f;
            modifiers.Stats.Shield /= 3f;
            SetUntouchable();
        }

        public override void OnUpdate(float frameTime)
        {
            SetUntouchable();
            base.OnUpdate(frameTime);
        }

        private void SetUntouchable()
        {
            var untouchable = GetUntouchable();
            _modifiers.Untouchable = untouchable;
            _decorations["PhaseShift"].Sprite.Color = untouchable
                ? _blue
                : _red;
        }

        private bool GetUntouchable()
        {
            if (_shifter.IsActive == false)
                return true;

            if (ContactCollisionDetector.HasCollision(_player, _shifter))
                return false;

            return true;
        }
    }
}
