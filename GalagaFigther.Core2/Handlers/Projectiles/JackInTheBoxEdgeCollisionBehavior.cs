using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using System.Diagnostics;
using System.Numerics;

namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public class JackInTheBoxState : IGameObjectData<Projectile>
    {
        public bool HasLanded { get; set; }
        public float DropTime { get; set; }
        public bool HasLaunched { get; set; }
        public Vector2 LaunchDirection { get; set; }
        public NonRepeatingAnimatedImageSprite? Sprite { get; set; }
    }

    public class JackInTheBoxBehavior : ProjectileBehaviorBase
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        public JackInTheBoxBehavior(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public override void Update(Projectile projectile, float frameTime)
        {
            var launchSpeed = 10000f;
            var launchDuration = .5f;
            var waitTime = 1.5f;

            DebugWriter.Write("      " + (int)projectile.X + "|" + (int)projectile.Y);


            var state = _gameDataRegistry.Get<JackInTheBoxState>(projectile);
            if (!state.HasLanded)
                return;

            state.DropTime += frameTime;
            if (state.DropTime < waitTime)
                return;

            if(!state.HasLaunched)
            {
                state.HasLaunched = true;
                projectile.ScaleTo(500, 50);

                if(state.LaunchDirection.Y != 0)
                {
                    projectile.Move(x: projectile.Width / 2f);
                }

                projectile.Sprite = state.Sprite = new NonRepeatingAnimatedImageSprite("Sprites/Projectiles/JackInTheBox.png", 20, 500, 50, launchDuration / 20f);
            }
            else
            {
                var frame = state.Sprite!.CurrentFrame;
                var speed = 0f;
                if(frame < 20f/3f)
                {
                    speed = 0f;
                }    
                else if(frame < 20f*2f/3f)
                {
                    var firstHalfPct = (frame - (20f/3f)) / (20f/3f);
                    speed = launchSpeed - launchSpeed * firstHalfPct;
                }
                else
                {
                    speed = 0f;
                    var secondHalfPct = (frame - (20f * 2f / 3f)) / (20f / 3f);
                    //speed = launchSpeed * secondHalfPct;
                    projectile.Color = projectile.Color.ApplyAlpha(1-secondHalfPct);
                }

                projectile.HurryTo(state.LaunchDirection.X * speed, state.LaunchDirection.Y * speed);
            }

            if (state.DropTime > waitTime + launchDuration - .1f)
            {
                projectile.IsActive = false;
                return;
            }
        }
    }

    public class JackInTheBoxEdgeCollisionBehavior : ProjectileBehaviorBase
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public JackInTheBoxEdgeCollisionBehavior(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public override void Update(Projectile projectile, float frameTime)
        {
            var state = _gameDataRegistry.Get<JackInTheBoxState>(projectile);

            MoveToClosestEdge(projectile);
            RotateProjectileToFaceInwards(projectile, state);

            projectile.HurryTo(0, 0);
            projectile.AngularVelocity = 0;
            state.HasLanded = true;
        }

        private void RotateProjectileToFaceInwards(Projectile projectile, JackInTheBoxState state)
        {
            var screenSize = _gameDataRegistry.Get<GameState>();

            if (projectile.X < 50)
            {
                projectile.Rotation = 0;
                state.LaunchDirection = new(1, 0);
            }
            else if (projectile.X > screenSize.ScreenSize.X - 50)
            {
                projectile.Rotation = 180;
                state.LaunchDirection = new(-1, 0);
            }
            else if (projectile.Y < 50)
            {
                projectile.Rotation = 90;
                state.LaunchDirection = new(0, 1);
            }
            else
            {
                projectile.Rotation = -90;
                state.LaunchDirection = new(0, -1);
            }
        }

        private void MoveToClosestEdge(Projectile projectile)
        {
            var screenSize = _gameDataRegistry.Get<GameState>();

            if (projectile.X < 50)
                projectile.MoveTo(12);
            else if (projectile.X > screenSize.ScreenSize.X)
                projectile.MoveTo((screenSize.ScreenSize.X) - 12);
            else if (projectile.Y < 50)
                projectile.MoveTo(y: 12);
            else
                projectile.MoveTo(y: (screenSize.ScreenSize.Y) - (50+12));
        }
    }
}
