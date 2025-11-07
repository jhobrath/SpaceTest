using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Defensives;
using GalagaFighter.Core2.Effects.Projectiles;
using GalagaFighter.Core2.Effects.Statuses;
using GalagaFighter.Core2.Effects.Turrets;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core2.Services
{
    public interface IInitialObjectBuilder
    {
        void Build();
    }
    public class InitialObjectBuilder : IInitialObjectBuilder
    {
        private IObjectService _objectService;
        private IPersistentValueHandler _persistentValueHandler;
        private IGameDataRegistry _gameDataRegistry;
        private IInputService _inputService;
        private IGameObjectPositionService _gameObjectPositionService;

        public InitialObjectBuilder(IObjectService objectService, IPersistentValueHandler persistentValueHandler, 
            IGameDataRegistry gameDataRegistry, IInputService inputService, IGameObjectPositionService gameObjectPositionService)
        {
            _objectService = objectService;
            _persistentValueHandler = persistentValueHandler;
            _gameDataRegistry = gameDataRegistry;
            _inputService = inputService;
            _gameObjectPositionService = gameObjectPositionService;
        }

        public void Build()
        {
            var screenWidth = Raylib.GetScreenWidth();
            var screenHeight = Raylib.GetScreenHeight();
            CreatePlayers(screenWidth, screenHeight);

            _gameObjectPositionService.Update(Raylib.GetFrameTime());
        }

        private void CreatePlayers(int screenWidth, int screenHeight)
        {
            var player1 = CreatePlayer(Game.Player1Id, 100, 90, new(95,95), new(550f+95, screenHeight+95), ShipPalettes.AzureWing);
            var player2 = CreatePlayer(Game.Player2Id, screenWidth - 168, -90, new(screenWidth-550f-95, 95), new(screenWidth- 95,screenHeight+95), ShipPalettes.VoidHunter);

            //AddTether(player1, player2);
            AddSpring(player1);

            _objectService.Add(player1);
            _objectService.Add(player2);

            RegisterInputMappings(player1, player2);
        }

        private void AddSpring(GameObject start)
        {
            // Rope attached to nose of ship
            var springAttachmentBack = new SpringAttachment(start.Id, new(0, 50));
            var springAttachmentLeft = new SpringAttachment(start.Id, new(-50, 0));
            var springAttachmentRight = new SpringAttachment(start.Id, new(50, 00));

            foreach(var attachment in new List<SpringAttachment>([springAttachmentLeft, springAttachmentBack, springAttachmentRight]))
            {
                var spring = new Spring(attachment, 100, 500f, 0.05f); // Visual stiffness for easier compression
                var initialPos = attachment.WorldPosition;
                foreach (var pt in spring.Points)
                    pt.CurrentPosition = pt.OldPosition = initialPos;

                attachment.Spring = spring;
                _objectService.Add(attachment);
                _gameObjectPositionService.RegisterParent(start, attachment);
            }
        }

        private void AddTether(GameObject start, GameObject end)
        {
            //Rope attached to nose of ship
            var ropeAttachment = new RopeAttachment(start.Id, new(0, -50));
            var ropeAttachment2 = new RopeAttachment(end.Id, new(0, -50));

            var rope = new Rope(ropeAttachment, 700, ropeAttachment2);
            ropeAttachment.Rope = rope;
            _objectService.Add(ropeAttachment);
            _objectService.Add(ropeAttachment2);

            _gameObjectPositionService.RegisterParent(start, ropeAttachment);
            _gameObjectPositionService.RegisterParent(end, ropeAttachment2);
        }

        private void RegisterInputMappings(Player player1, Player player2)
        {
            var player1Mappings = new KeyMappings(
                KeyboardKey.W,    // Forward
                KeyboardKey.S,    // Back
                KeyboardKey.A,    // Left
                KeyboardKey.D,    // Right
                KeyboardKey.K,    // Shoot
                KeyboardKey.J,    // Defend
                KeyboardKey.U     // Deploy Turret
            );

            var player2Mappings = new GamepadMappings(
                GamepadButton.LeftFaceUp,
                GamepadButton.LeftFaceDown,
                GamepadButton.LeftFaceLeft,    // Left (TODO: Update with different keys)
                GamepadButton.LeftFaceRight,   // Right (TODO: Update with different keys)
                GamepadButton.RightTrigger2,   // Shoot (TODO: Update with different keys)
                GamepadButton.LeftTrigger2,    // Defend (TODO: Update with different keys)
                GamepadButton.RightFaceUp      // Deploy Turret (TODO: Update with different keys)
            );

            _inputService.AddPlayer(player1.Id, player1Mappings);
            _inputService.AddPlayer(player2.Id, player2Mappings);
        }

        private Player CreatePlayer(Guid playerId, int x, float rotation, Vector2 min, Vector2 max, Color palette)
        {
            var playerHeight = 168f;
            var pos = new Vector2(x, 450);
            var size = Vector2.One * 168;
            var speed = Vector2.Zero;

            // Create sprite without palette swap initially
            var shipSprite = new StillImageSprite("Sprites/Ships/MainShipBody.png");

            var player = new Player(pos, size, speed, shipSprite)
            {
                Id = playerId,
                Rotation = rotation,
                Palette = palette // Set specific palette color
            };

            var bounds = _gameDataRegistry.Get<PlayerBoundsData>(player);
            bounds.Min = min;
            bounds.Max = max;

            bounds.MaxSpeed = new(3000,3000);

            var playerRotation = _gameDataRegistry.Get<PlayerRotationData>(player);
            playerRotation.InitialRotation = rotation;

            var effects = _gameDataRegistry.Get<PlayerEffects>(player);
            effects.Add(new DefaultShootEffect());
            //effects.Add(new NinjaTurretEffect());
            //effects.Add(new AddWeaponEffect());
            //effects.Add(new AddWeaponEffect());
            //effects.Add(new AddWeaponEffect());
            //effects.Add(new AddWeaponEffect());
            effects.RequireRerolling = true;

            // Create engine trail emitter
            var engineConfig = ParticleEffectTemplates.Get("EngineTrail");
            var engineTrail = new ParticleEmitter(
                owner: player.Id,
                position: new Vector2(0, 76),
                size: 1
            )
            { Config = engineConfig };

            // Create engine trail emitter
            var smokeConfig = ParticleEffectTemplates.Get("SmokeTrail");
            var smokeTrail = new ParticleEmitter(
                owner: player.Id,
                position: new Vector2(0, 76),
                size: 1
            )
            { Config = smokeConfig };

            _objectService.Add(engineTrail);
            _objectService.Add(smokeTrail);
            _gameObjectPositionService.RegisterParent(player, engineTrail);
            _gameObjectPositionService.RegisterParent(player, smokeTrail);

            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
            _persistentValueHandler.Register(
                () => engineTrail.Enabled = smokeTrail.Enabled = true, 
                () => engineTrail.Enabled = smokeTrail.Enabled = false, 
                () => player.Acceleration.Length() > 1);


            return player;
        }
    }
}
