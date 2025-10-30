using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface IHudService
    {
        void Update();
    }

    public class HudService : IHudService
    {
        private readonly IObjectService _objectService;
        private readonly IGameDataRegistry _gameDataRegistry;

        public HudService(IObjectService objectService, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
        }

        public void Update()
        {
            var player1 = _objectService.Get<Player>(Game.Player1Id);
            var player2 = _objectService.Get<Player>(Game.Player2Id);

            RenderHealthBars(player1, player2); // Put this in the top-left corner and top right corner respectively
            RenderBaseStats(player1, player2); //Put Shield, Damage, Speed, FireRate (show firerate as 1/Firerate)
        }



        private void RenderBaseStats(Player player1, Player player2)
        {
            RenderPlayerBaseStats(player1, 50, false);
            RenderPlayerBaseStats(player1, 50, true);
        }

        private void RenderPlayerBaseStats(Player player, int yStart, bool isRightJustified)
        {
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
            var baseStats = _gameDataRegistry.Get<PlayerBaseStats>(player);

            var speedValue = baseStats.Speed * modifiers.Stats.SpeedMultiplier;
            var shield = baseStats.Shield * modifiers.Stats.ShieldMultiplier;
            var damage = baseStats.Damage * modifiers.Stats.DamageMultiplier;
            var fireRate = baseStats.FireRate * modifiers.Stats.FireRateMultiplier;

            //Render them on one line, evenly space beneath the health bar
        }

        private void RenderHealthBars(Player player1, Player player2)
        {
            RenderHealthBar(player1, 0, false);
            RenderHealthBar(player1, 0, true);
        }

        private void RenderHealthBar(Player player, int yStart, bool isRightJustified)
        {
            //Render player.Health as a health bar. If player.Health is over 100,
            //  render a translucent darker red over the bar for
            //  the base 100. As health decreases, the darker bar should
            //  get smaller and then when health is < 100, the red
            //  behind it starts disappearing
        }
    }
}
