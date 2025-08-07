using Raylib_cs;

namespace GalagaFighter.Models
{
    public abstract class GameObject
    {
        public Rectangle Rect;
        public bool IsActive;

        public GameObject(Rectangle rect)
        {
            Rect = rect;
            IsActive = true;
        }

        public abstract void Update(Game game);
        public abstract void Draw();
    }
}
