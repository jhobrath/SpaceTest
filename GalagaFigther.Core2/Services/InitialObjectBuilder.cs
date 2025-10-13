using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Helpers;
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
        private IPersistentValueHandler _persistentValueHandler;

        public InitialObjectBuilder(IObjectService objectService, IPersistentValueHandler persistentValueHandler)
        {
            _objectService = objectService;
            _persistentValueHandler = persistentValueHandler;
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
            var player = new Player(pos, size, speed, new StillImageSprite("Sprites/Ships/MainShipBody.png"))
            {
                Rotation = 90
            };

            _persistentValueHandler.RegisterRange(player, p => p.X, 0, 400f);
            _persistentValueHandler.RegisterRange(player, p => p.Y, 0, screenHeight - player.Height);
            //_persistentValueHandler.RegisterRange(player, p => p.Speed, new(-1000f, -1000f), new(1000f,1000f));
            _objectService.Add(player);
        }
    }
}
