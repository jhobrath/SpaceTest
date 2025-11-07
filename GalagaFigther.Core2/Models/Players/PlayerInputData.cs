using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerInputData : IGameObjectData<Player>
    {
        public ButtonData Left { get; set; } = new();
        public ButtonData Right { get; set; } = new();
        public ButtonData Up { get; set; } = new();
        public ButtonData Down { get; set; } = new();
        public ButtonData Shoot { get; set; } = new();
        public ButtonData Defend { get; set; } = new();
        public ButtonData DeployTurret { get; set; } = new();
    }
}
