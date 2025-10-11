using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Helper;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Services
{
    public interface IInitialObjectBuilder
    {
        void Build();
    }
    public class InitialObjectBuilder : IInitialObjectBuilder
    {
        private IObjectService _objectService;

        public InitialObjectBuilder(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Build()
        {
            var screenHeight = Raylib.GetScreenHeight();
            var screenWidth = Raylib.GetScreenHeight();
            CreatePlayer(screenWidth, screenHeight);
        }

        private void CreatePlayer(int screenWidth, int screenHeight)
        {
            var playerHeight = 168f;
            var pos = new Vector2(0f, (screenHeight - playerHeight) / 2);
            var size = Vector2.One * 168;
            var speed = Vector2.Zero;
            var player = new Player(pos, size, speed, new StillImageSprite("Sprites/Ships/MainShip.png"))
            {
                Rotation = 90
            };
            _objectService.Add(player);
        }
    }
}
