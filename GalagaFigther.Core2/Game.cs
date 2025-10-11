using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.Models;
using GalagaFigther.Core2.Services;
using Raylib_cs;

namespace GalagaFigther.Core2
{
    public class Game
    {
        private readonly IObjectService _objectService;
        private readonly IControllerFactory _controllerFactory;
        private readonly IInitialObjectBuilder _initialObjectBuilder;

        public Game(IObjectService objectService, IControllerFactory controllerFactory, IInitialObjectBuilder initialObjectBuilder)
        {
            _objectService = objectService;
            _controllerFactory = controllerFactory;
            _initialObjectBuilder = initialObjectBuilder;
        }

        public void Run(GameState state)
        {
            CreateWindow(state);
            RegisterControllers();

            _initialObjectBuilder.Build();

            while(true)
            {
                var frameTime = Raylib.GetFrameTime();
                var gameObjects = _objectService.GetAll();
                
                foreach(var gameObject in gameObjects)
                {
                    gameObject.Update(_controllerFactory, frameTime);
                }

                Raylib.ClearBackground(Color.Black);
                Raylib.BeginDrawing();

                foreach (var gameObject in gameObjects)
                {
                    gameObject.Draw(_controllerFactory, frameTime);
                }

                Raylib.EndDrawing();

                if(Raylib.WindowShouldClose())
                    break;
            }

            CloseWindow();
        }

        private void RegisterControllers()
        {
        }

        private static void CreateWindow(GameState state)
        {
            Raylib.InitWindow((int)state.ScreenSize.X, (int)state.ScreenSize.Y, "Protexius");
            Raylib.SetTargetFPS(60);
        }

        private static void CloseWindow()
        {
            Raylib.CloseWindow();
        }
    }
}
