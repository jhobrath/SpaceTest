// See https://aka.ms/new-console-template for more information
using GalagaFigther.Core2;
using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.Models;
using GalagaFigther.Core2.Services;
using System.Numerics;

var gameState = new GameState
{
    ScreenSize = new Vector2(1920, 1080)
};

var objectService = new ObjectService();
var gameDataRegistry = new GameDataRegistry();
var inputService = new InputService();
var controllerFactory = new ControllerFactory(gameDataRegistry, inputService);
var persistentValueHandler = new PersistentValueHandler();
var initialObjectBuilder = new InitialObjectBuilder(objectService, persistentValueHandler);

var game = new Game(objectService, controllerFactory, initialObjectBuilder, inputService, persistentValueHandler);
game.Run(gameState);