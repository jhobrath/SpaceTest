using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace GalagaFighter.Core.CPU
{
    public class GameState
    {
        public GameStateForPlayer Player1 { get; set; } = new();
        public GameStateForPlayer Player2 { get; set; } = new();
        public List<GameStateForProjectile> Player1Projectiles { get; set; } = [];
        public List<GameStateForProjectile> Player2Projectiles { get; set; } = [];

        public float[] ToFloatArray()  
        {
            var list = 
                Player1.ToFloatArray()
                .Concat(Player2.ToFloatArray());

            foreach (var projectile in Player1Projectiles)
                list = list.Concat(projectile.ToFloatArray());

            foreach (var projectile in Player2Projectiles)
                list = list.Concat(projectile.ToFloatArray());

            return [.. list];
        }
    }

    public class GameStateForPlayer
    {
        public float Health { get; set; }
        public float Resource { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }

        public float[] ToFloatArray() => [Health, Resource, PositionX, PositionY];
    }

    public class GameStateForProjectile
    {
        public float SpeedX { get; set; }
        public float SpeedY { get; set; }
        public float Damage { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
     
        public float[] ToFloatArray() => [SpeedX, SpeedY, Damage, PositionX, PositionY];
    }
}