using GalagaFighter.Core2;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services;

var game = Registry.Get<IGame>();
game.Run();