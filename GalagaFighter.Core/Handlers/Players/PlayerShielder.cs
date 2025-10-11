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
        //How long does it take after a hit for the shield to start recharging
        private const float _shieldRechargeTime = 1f;

        //How much more damage does a bullet do to a shield than a player
        private const float _shieldDamageMultiplier = 10f;

        private float _lastShield = _shieldRechargeTime;
        private float _lastDamageOccurred = _shieldRechargeTime;
        private float _lastHealth = 100f;
        private float _shieldRechargeStartValue = 100f; // Track what shield value was when recharge began

        public void Shield(Player player)
        {
            if (player.Shield  < _lastShield || player.Health < _lastHealth)
            {
                _lastShield = player.Shield;
                _lastHealth = player.Health;
                _lastDamageOccurred = 0f;
                _shieldRechargeStartValue = player.Shield; // Store the starting shield value for this recharge cycle
                return;
            }

            _lastDamageOccurred += Raylib.GetFrameTime();
            
            // If we haven't reached the recharge delay yet, don't start recharging
            if (_lastDamageOccurred < _shieldRechargeTime)
                return;

            if (player.Shield >= 100)
                return;

            // Calculate time elapsed since recharge delay finished (this starts at 0 when recharge begins)
            float rechargeTimeElapsed = _lastDamageOccurred - _shieldRechargeTime;
            
            // Logarithmic recharge formula: newShield = 100 - (100 - startShield) * e^(-k * timeElapsed)
            // Where k is chosen so that after _shieldRechargeTime seconds, we reach ~99.9% of full
            // Using k = 6.9 / _shieldRechargeTime gives us ~99.9% completion after _shieldRechargeTime seconds
            float k = 6.9f / _shieldRechargeTime;
            float targetShield = 100f - (100f - _shieldRechargeStartValue) * (float)Math.Exp(-k * rechargeTimeElapsed);
            
            player.Shield = Math.Clamp(targetShield, _shieldRechargeStartValue, 100f);

            _lastShield = player.Shield;
            _lastHealth = player.Health;
        }
    }
}
