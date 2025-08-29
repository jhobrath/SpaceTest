using Raylib_cs;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerResourceManager
    {
        bool Spend(float amount);
        void Update();
        float CurrentAmount { get; }
    }
    public class PlayerResourceManager : IPlayerResourceManager
    {
        public const float MaxAmount = 100f;
        public const float FillRate = 5f; // per second
        public float CurrentAmount { get; private set; } = 100f;

        private float _amountLeftToSpend = 0f;

        public void Update()
        {
            if(_amountLeftToSpend > 0f)
            {
                var amountToRemove = FillRate * Raylib.GetFrameTime() * 3;
                CurrentAmount -= amountToRemove;
                if (CurrentAmount < 0f) 
                    CurrentAmount = 0f;

                _amountLeftToSpend -= amountToRemove;
            }
            else 
            { 
                if(CurrentAmount < 10)
                    CurrentAmount += FillRate * Raylib.GetFrameTime()/4f;
                else if (CurrentAmount < 20)
                    CurrentAmount += FillRate * Raylib.GetFrameTime() /3f;
                else if (CurrentAmount < 30)
                    CurrentAmount += FillRate * Raylib.GetFrameTime() / 2f;
                else
                    CurrentAmount += FillRate * Raylib.GetFrameTime();

                if (CurrentAmount > MaxAmount)
                    CurrentAmount = MaxAmount;
            }
        }

        public bool Spend(float amount)
        {
            if (amount < 0f)
                return false;
            if (amount > CurrentAmount)
                return false;

            if (amount > 20)
                _amountLeftToSpend = amount;
            else
                CurrentAmount -= amount;
                
            return true;
        }
    }
}
