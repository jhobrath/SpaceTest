using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.CPU
{
    public interface ICpuInputService
    {
        void SetPlayer(Player player);
        void Update(float frameTime);
    }

    public class CpuInputService : IInputMappings, ICpuInputService
    {
        private bool _up = false;
        private bool _shoot = true;
        private bool _shield = false;
        private bool _right = false;
        private bool _left = false;
        private bool _down = false;
        private bool _deploy = false;
        private bool _defend = false;
        private Player _player;
        private Player _opponent;

        private List<Vector2> _cellCenters;
        private Vector2 _cellSize = new(350, 200);
        private CellReaction[] _cellReactions = new CellReaction[] { 
            new(up: true),
            new(down: true)
        };

        public bool IsDefendDown() => _defend;
        public bool IsDeployTurretDown() => _deploy;
        public bool IsDownDown() => _down;
        public bool IsLeftDown() => _left;
        public bool IsRightDown() => _right;
        public bool IsShieldDown() => _shield;
        public bool IsShootDown() => _shoot;
        public bool IsUpDown() => _up;

        private readonly IObjectService _objectService;
        private readonly IGameDataRegistry _gameDataRegistry;

        public CpuInputService(IObjectService objectService, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            BuildCells();
            _gameDataRegistry = gameDataRegistry;
        }

        private void BuildCells()
        {
            var half = _cellSize / 2f;

            var centerPoints = new List<Vector2>
            {
                new(-1, -1),
                new(-1, 1),
            };

            _cellCenters = [.. centerPoints.Select(x => new Vector2(x.X * half.X, x.Y * half.Y))];
        }

        public void SetPlayer(Player player)
        {
            _player = player;
            _opponent = _objectService.GetOpponent(_player);
        }

        public void Update(float frameTime)
        {
            _up = _down = _left = _right = false;

            var cells = GetCells();
            var projectiles = _objectService.GetChildren<Projectile>(_opponent);
            var damagePerSquare = Enumerable.Range(0, cells.Count).Select(x => new CellDamage { CellIndex = x, Damage = 0 }).ToList();

            foreach(var projectile in projectiles)
            {
                var cell = GetGridIndex(projectile, cells);
                if (cell == null)
                    continue;

                damagePerSquare[cell.Value].Damage += projectile.Damage;
            }

            HandleCellResult(damagePerSquare);
            DrawCellDamage(cells, damagePerSquare);
        }

        private void DrawCellDamage(List<Rectangle> cells, List<CellDamage> damagePerSquare)
        {
            for(var i = 0;i < cells.Count;i++)
            {
                var cell = cells[i];
                var dmg = damagePerSquare[i].Damage;  

                var redPct = dmg / 10f;

                Raylib.DrawRectangleRec(cell, Color.Red.ApplyAlpha(.5f + redPct/2f));
            }
        }

        private void HandleCellResult(List<CellDamage> cellByDamage)
        {

            if (cellByDamage.All(x => x.Damage == 0))
            {
                return;
            }

            var dist = _opponent.WorldPosition - _player.WorldPosition;

            if(dist.Y < -84)
            {
                cellByDamage[0].Damage -= .5f;
            }
            else if(dist.Y > 84)
            {
                cellByDamage[1].Damage -= .5f;
            }

            var state = _gameDataRegistry.Get<GameState>();

            if (_player.WorldPosition.Y > state.ScreenSize.Y * 2f / 3f)
                cellByDamage[0].Damage -= .75f;
            else if (_player.WorldPosition.X < state.ScreenSize.Y / 3f)
                cellByDamage[1].Damage -= .75f;

            var winningCell = cellByDamage.OrderBy(x => x.Damage).First();
            var reaction = _cellReactions[winningCell.CellIndex];

            _up = reaction.Up;
            _down = reaction.Down;
            _left = reaction.Left;
            _right = reaction.Right;
        }

        private List<Rectangle> GetCells()
        {
            var half = _cellSize / 2f;
            var centerPoints = _cellCenters.Select(x => _player.WorldPosition + x);
            return [.. centerPoints.Select(x => new Rectangle(x.X - half.X, x.Y - half.Y, _cellSize.X, _cellSize.Y))];
        }

        private int? GetGridIndex(Projectile projectile, List<Rectangle> cells)
        {
            for(var i = 0;i < cells.Count; i++) {
                if (Raylib.CheckCollisionPointRec(projectile.WorldPosition, cells[i]))
                    return i;
            }

            return null;
        }

        private class CellDamage
        {
            public float Damage { get; set; }
            public int CellIndex { get; set; }
        }

        private class CellReaction
        {
            public CellReaction(bool up = false, bool right = false, bool down = false, bool left = false)
            {
                Up = up;
                Right = right;
                Down = down;
                Left = left;
            }

            public bool Up { get; set; }
            public bool Down { get; set; }
            public bool Left { get; set; }
            public bool Right { get; set; }
        }
    }
}
