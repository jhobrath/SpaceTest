// See https://aka.ms/new-console-template for more information
using GalagaFigther.Core2;
using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.Handlers.Players;
using GalagaFigther.Core2.Models;
using GalagaFigther.Core2.Models.Game;
using GalagaFigther.Core2.Services;
using System.Numerics;

var gameDataRegistry = new GameDataRegistry();
var gameState = gameDataRegistry.Get<GameState>();
var objectService = new ObjectService();
var inputService = new InputService();
var playerMover = new PlayerMover(gameDataRegistry, inputService);
var playerRotator = new PlayerRotator(gameDataRegistry);
var playerDrawer = new PlayerDrawer(gameDataRegistry);
var playerAffector = new PlayerAffector(gameDataRegistry);
var playerShooter = new PlayerShooter(inputService, gameDataRegistry, objectService);
var playerController = new PlayerController(playerMover, playerRotator, playerAffector, playerDrawer, playerShooter);
var projectileController = new ProjectileController(gameDataRegistry);
var powerUpController = new PowerUpController(gameDataRegistry);
var gameObjectUpdateService = new GameObjectUpdateService(objectService, playerController, projectileController, powerUpController);
var persistentValueHandler = new PersistentValueHandler();
var initialObjectBuilder = new InitialObjectBuilder(objectService, persistentValueHandler, gameDataRegistry);
var powerUpCreationService = new PowerUpCreationService(gameDataRegistry, objectService);

var game = new Game(objectService, initialObjectBuilder, inputService, persistentValueHandler, gameObjectUpdateService, powerUpCreationService);
game.Run(gameState);