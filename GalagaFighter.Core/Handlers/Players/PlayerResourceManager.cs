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
        public const float FillRate = 10f; // per second
        public float CurrentAmount { get; private set; } = 0f;

        public void Update()
        {
            CurrentAmount += FillRate * Raylib.GetFrameTime();
            if (CurrentAmount > MaxAmount)
                CurrentAmount = MaxAmount;
        }

        public bool Spend(float amount)
        {
            if (amount < 0f)
                return false;
            if (amount > CurrentAmount)
                return false;

            CurrentAmount -= amount;
            return true;
        }
    }
}
