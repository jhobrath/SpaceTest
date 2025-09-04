using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerResourceManager
    {
        bool Spend(float amount);
        void Update();
        void HandleShootMeter(ButtonState shootState);

        float ShieldMeter { get; }
        float ShootMeter { get; }
    }
    public class PlayerResourceManager : IPlayerResourceManager
    {
        public const float MaxAmount = 100f;
        public const float FillRate = 5f; // per second
        public float ShieldMeter { get; set; } = 100f;

        //TODO: This is duplicate logic to whats in default shoot effect. Any idea on how to share logic?
        public float ShootMeter => Math.Clamp(_shootMeter / 3f, 0, 1);


        private const float WindowSeconds = 3f;
        private const float MaxShotCount = 12f;

        private float _amountLeftToSpend = 0f;
        private float _shootMeter = 5f;

        private List<double> _shotTimestamps = new();

        public void Update()
        {
            if(_amountLeftToSpend > 0f)
            {
                var amountToRemove = FillRate * Raylib.GetFrameTime() * 3;
                ShieldMeter -= amountToRemove;
                if (ShieldMeter < 0f) 
                    ShieldMeter = 0f;

                _amountLeftToSpend -= amountToRemove;
            }
            else 
            { 
                if(ShieldMeter < 10)
                    ShieldMeter += FillRate * Raylib.GetFrameTime()/4f;
                else if (ShieldMeter < 20)
                    ShieldMeter += FillRate * Raylib.GetFrameTime() /3f;
                else if (ShieldMeter < 30)
                    ShieldMeter += FillRate * Raylib.GetFrameTime() / 2f;
                else
                    ShieldMeter += FillRate * Raylib.GetFrameTime();

                if (ShieldMeter > MaxAmount)
                    ShieldMeter = MaxAmount;
            }

            var now = Raylib.GetTime();
            _shotTimestamps = _shotTimestamps.Where(t => now - t <= WindowSeconds).ToList();

        }

        public bool Spend(float amount)
        {
            if (amount < 0f)
                return false;
            if (amount > ShieldMeter)
                return false;

            if (amount > 20)
                _amountLeftToSpend = amount;
            else
                ShieldMeter -= amount;
                
            return true;
        }

        public void HandleShootMeter(ButtonState shootButton)
        {
            if(shootButton.IsDown)
            {
                _shootMeter = Math.Max(0, _shootMeter - Raylib.GetFrameTime());
            }
            else
            {
                _shootMeter = Math.Min(5, _shootMeter + Raylib.GetFrameTime()*2f);
            }
        }
    }
}
