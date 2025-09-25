using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerShielder
    {
        void Shield(Player player);
    }

    public class PlayerShielder : IPlayerShielder
    {
        private float _lastShield = 3f;
        private float _lastHealth = 100f;
        private float _lastDamageOccurred = 3f;

        public void Shield(Player player)
        {
            if (player.Shield  < _lastShield || player.Health < _lastHealth)
            {
                _lastShield = player.Shield;
                _lastHealth = player.Health;
                _lastDamageOccurred = 0f;
                return;
            }

            _lastDamageOccurred += Raylib.GetFrameTime();
            if (_lastDamageOccurred < 3f)
                return;

            var maxAmountGained = 50f;
            var amountGained = Math.Clamp((_lastDamageOccurred - 3f), 0, 1);
            amountGained = amountGained * maxAmountGained;

            player.Shield += amountGained * Raylib.GetFrameTime();

            if (player.Shield > 100f)
                player.Shield = 100f;

            _lastShield = player.Shield;
            _lastHealth = player.Health;
        }
    }
}
