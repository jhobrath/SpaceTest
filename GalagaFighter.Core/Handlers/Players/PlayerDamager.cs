using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerDamager
    {
        void Damage(Player player, float damage, Vector2? point = null);
    }
    public class PlayerDamager : IPlayerDamager
    {
        private readonly IPlayerManagerFactory _playerManagerFactory;

        public PlayerDamager(IPlayerManagerFactory playerManagerFactory)
        {
            _playerManagerFactory = playerManagerFactory;
        }

        public void Damage(Player player, float damage, Vector2? point = null)
        {
            point = point ?? player.Center;

            if (player.Shield > 0f)
            {
                var shieldDamage = Math.Min(player.Shield, damage);
                player.Shield -= shieldDamage;
                damage -= Math.Min(player.Shield, damage);
                AddEffect(player, new ShieldTakeDamageEffect(point.Value - player.Center));
            }

            if (damage > 0f)
            {
                player.Health -= Math.Min(player.Health, damage);
                AddEffect(player, new HealthTakeDamageEffect(point.Value - player.Center));
            }
        }

        private void AddEffect(Player player, StatusEffect effect)
        {
            var effectManager = _playerManagerFactory.GetEffectManager(player);
            effectManager.AddEffect(effect);
        }
    }
}
