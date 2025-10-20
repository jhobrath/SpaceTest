using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services;
using Raylib_cs;

namespace GalagaFighter.Core2
{
    public interface IGame
    {
        void Run();
    }

    public class Game : IGame
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        private readonly IInitialObjectBuilder _initialObjectBuilder;
        private readonly IInputService _inputService;
        private readonly IPersistentValueHandler _persistentValueHandler;
        private readonly IGameObjectUpdateService _gameObjectUpdateService;
        private readonly IPowerUpCreationService _powerUpCreationService;
        private readonly ICollisionService _collisionService;
        private readonly IGameObjectPositionService _gameObjectPositionService;
        private readonly IClearableServiceClearer _clearableServiceClearer;

        public static Guid Id => Guid.NewGuid();

        public Game(IObjectService objectService, IInitialObjectBuilder initialObjectBuilder,
            IInputService inputService, IPersistentValueHandler persistentValueHandler,
            IGameObjectUpdateService gameObjectUpdateService, IPowerUpCreationService powerUpCreationService,
            ICollisionService collisionService, IGameObjectPositionService gameObjectPositionService,
            IGameDataRegistry gameDataRegistry, IClearableServiceClearer clearableServiceClearer)
        {
            _objectService = objectService;
            _initialObjectBuilder = initialObjectBuilder;
            _inputService = inputService;
            _persistentValueHandler = persistentValueHandler;
            _gameObjectUpdateService = gameObjectUpdateService;
            _powerUpCreationService = powerUpCreationService;
            _collisionService = collisionService;
            _gameObjectPositionService = gameObjectPositionService;
            _gameDataRegistry = gameDataRegistry;
            _clearableServiceClearer = clearableServiceClearer;
        }

        public void Run()
        {
            var state = _gameDataRegistry.Get<GameState>();
            CreateWindow(state);

            _initialObjectBuilder.Build();

            while(true)
            {
                var frameTime = Raylib.GetFrameTime();
                
                UpdateGameObjects(frameTime);
                UpdateServices(frameTime);
                HandleCollisions();
                DrawGameObjects(frameTime);
                DeactivateObjects(frameTime);

                if (Raylib.WindowShouldClose())
                    break;

                if(Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    _clearableServiceClearer.Clear();
                    _initialObjectBuilder.Build();
                }
            }

            CloseWindow();
        }

        private void HandleCollisions()
        {
            _collisionService.Update();
        }

        private void UpdateGameObjects(float frameTime)
        {
            _gameObjectPositionService.Update(frameTime);
            _gameObjectUpdateService.Update(frameTime);
        }

        private void UpdateServices(float frameTime)
        {
            _inputService.Update(frameTime);
            _powerUpCreationService.Update(frameTime);
            _persistentValueHandler.Update(frameTime);
        }

        private void DrawGameObjects(float frameTime)
        {
            Raylib.ClearBackground(Color.Black);
            Raylib.BeginDrawing();

            _gameObjectUpdateService.Draw(frameTime);

            Raylib.EndDrawing();
        }


        private void DeactivateObjects(float frameTime)
        {
            _objectService.CleanUp();
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
