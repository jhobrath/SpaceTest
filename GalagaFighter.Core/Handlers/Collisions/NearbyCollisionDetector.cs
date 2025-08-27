using System.Numerics;

namespace GalagaFighter.Core.Handlers.Collisions
{
    public interface INearbyCollisionDetector
    {
        bool HasCollision(GameObject obj1, GameObject obj2);
    }
    
    public class NearbyCollisionDetector : INearbyCollisionDetector
    {
        public bool HasCollision(GameObject obj1, GameObject obj2)
        {
            // Implementation depends on specific business logic requirements
            // This could be distance-based proximity detection
            var distance = Vector2.Distance(obj1.Center, obj2.Center);
            
            // Example: objects are "nearby" if within 100 units of each other
            return distance <= 100f;
        }
    }
}
