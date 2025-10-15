using GalagaFigther.Core2;
using GalagaFigther.Core2.Models.Game;
using GalagaFigther.Core2.Services;

var gameDataRegistry = Registry.Get<IGameDataRegistry>();
var gameState = gameDataRegistry.Get<GameState>();
var game = Registry.Get<IGame>();
game.Run(gameState);