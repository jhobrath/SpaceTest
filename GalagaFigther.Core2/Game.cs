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
        private readonly IInputService _inputService;
        private readonly IPersistentValueHandler _persistentValueHandler;

        public Game(IObjectService objectService, IControllerFactory controllerFactory, IInitialObjectBuilder initialObjectBuilder,
            IInputService inputService, IPersistentValueHandler persistentValueHandler)
        {
            _objectService = objectService;
            _controllerFactory = controllerFactory;
            _initialObjectBuilder = initialObjectBuilder;
            _inputService = inputService;
            _persistentValueHandler = persistentValueHandler;
        }

        public void Run(GameState state)
        {
            CreateWindow(state);

            _initialObjectBuilder.Build();

            while(true)
            {
                var frameTime = Raylib.GetFrameTime();
                var gameObjects = _objectService.GetAll();
                
                UpdateGameObjects(frameTime, gameObjects);
                UpdateServices(frameTime);
                DrawGameObjects(frameTime, gameObjects);

                if (Raylib.WindowShouldClose())
                    break;

                if(Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    _objectService.Clear();
                    _initialObjectBuilder.Build();
                }
            }

            CloseWindow();
        }

        private void UpdateServices(float frameTime)
        {
            _inputService.Update(frameTime);
            _persistentValueHandler.Update();
        }

        private void DrawGameObjects(float frameTime, IEnumerable<GameObjects.GameObject> gameObjects)
        {
            Raylib.ClearBackground(Color.Black);
            Raylib.BeginDrawing();

            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(_controllerFactory, frameTime);
            }

            Raylib.EndDrawing();
        }

        private void UpdateGameObjects(float frameTime, IEnumerable<GameObjects.GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update(_controllerFactory, frameTime);
            }
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
