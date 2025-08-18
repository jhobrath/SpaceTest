using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players.Updates
{
    public class PlayerInputUpdate
    {
        public ButtonState Left { get; set; } = false;
        public ButtonState Right { get; set; } = false;
        public ButtonState Shoot { get; set; } = false;
        public ButtonState Switch { get; set; } = false;
    }

    public class ButtonState
    {
        public bool IsPressed { get; set; } = false;
        public bool IsDown { get; set; } = false;
        public float HeldDuration { get; set; } = 0f;

        public static implicit operator bool(ButtonState state)
        {
            return state.IsDown;
        }

        public static implicit operator ButtonState(bool val)
        {
            return new ButtonState
            {
                IsPressed = false,
                IsDown = val,
                HeldDuration = 0f
            };
        }
    }
}
