using GalagaFighter.Core2;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services;

var gameDataRegistry = Registry.Get<IGameDataRegistry>();
var gameState = gameDataRegistry.Get<GameState>();
var game = Registry.Get<IGame>();
game.Run(gameState);