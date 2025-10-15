using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.Models.Game;
using GalagaFigther.Core2.Services;
using Raylib_cs;

namespace GalagaFigther.Core2
{
    public class Game
    {
        private readonly IObjectService _objectService;
        private readonly IInitialObjectBuilder _initialObjectBuilder;
        private readonly IInputService _inputService;
        private readonly IPersistentValueHandler _persistentValueHandler;
        private readonly IGameObjectUpdateService _gameObjectUpdateService;

        public Game(IObjectService objectService, IInitialObjectBuilder initialObjectBuilder,
            IInputService inputService, IPersistentValueHandler persistentValueHandler, IGameObjectUpdateService gameObjectUpdateService)
        {
            _objectService = objectService;
            _initialObjectBuilder = initialObjectBuilder;
            _inputService = inputService;
            _persistentValueHandler = persistentValueHandler;
            _gameObjectUpdateService = gameObjectUpdateService;
        }

        public void Run(GameState state)
        {
            CreateWindow(state);

            _initialObjectBuilder.Build();

            while(true)
            {
                var frameTime = Raylib.GetFrameTime();
                
                UpdateGameObjects(frameTime);
                UpdateServices(frameTime);
                DrawGameObjects(frameTime);

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

        private void UpdateGameObjects(float frameTime)
        {
            _gameObjectUpdateService.Update(frameTime);
        }

        private void UpdateServices(float frameTime)
        {
            _inputService.Update(frameTime);
            _persistentValueHandler.Update();
        }

        private void DrawGameObjects(float frameTime)
        {
            Raylib.ClearBackground(Color.Black);
            Raylib.BeginDrawing();

            _gameObjectUpdateService.Draw(frameTime);

            Raylib.EndDrawing();
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
