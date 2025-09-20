using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerDamager
    {
        void Damage(Player player, float damage);
    }
    public class PlayerDamager : IPlayerDamager
    {
        public void Damage(Player player, float damage)
        {
            if (player.Shield > 0f)
            {
                var shieldDamage = Math.Min(player.Shield, damage);
                player.Shield -= shieldDamage;
                damage -= Math.Min(player.Shield, damage);
            }

            if(damage > 0f)
                player.Health -= Math.Min(player.Health, damage);
        }
    }
}
