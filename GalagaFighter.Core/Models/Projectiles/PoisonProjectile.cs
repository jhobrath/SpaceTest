using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class PoisonProjectile : Projectile
    {
        public static readonly Vector2 _baseSpeed = new Vector2(500f, 0f);
        public static readonly Vector2 _baseSize = new Vector2(100f, 100f);
        private Vector2 _originalPosition;

        public override Vector2 BaseSpeed => _baseSpeed;
        public override Vector2 BaseSize => _baseSize;
        public override int BaseDamage => 0;
        public override Vector2 SpawnOffset => Vector2.Zero;
        public override bool DamageOverTime => true;

        // Animation variables (changed from constants for debugging flexibility)
        private float _bubbleFormationTime = 0.35f;
        private float _finalBubbleRadius = 35f;
        
        // Track the initial offset from owner center for movement following
        private Vector2 _initialOffsetFromOwnerCenter;
        private Player _owner;
        
        // Bubble wobble animation for realistic floating motion
        private float _wobbleFrequency1 = 2.8f;  // Primary wobble frequency (Hz)
        private float _wobbleFrequency2 = 1.7f;  // Secondary wobble frequency (Hz) 
        private float _wobbleAmplitude = 0.8f;   // Maximum wobble intensity (pixels)

        private ParticleEffect _poisonEffect;

        public PoisonProjectile(IProjectileController controller, Player owner, SpriteWrapper sprite, Vector2 initialPosition, PlayerProjectile modifiers) 
            : base(controller, owner, sprite, initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            initialPosition = new Vector2(initialPosition.X 
                + (owner.IsPlayer1 ? 0 : -1)*Rect.Width 
                + (owner.IsPlayer1 ? -1 : 1)*25, initialPosition.Y - Rect.Size.Y/ 2);
            MoveTo(initialPosition.X, initialPosition.Y);
            _originalPosition = initialPosition;
            _owner = owner;

            // Store the initial offset from the owner's center so we can follow the ship's movement
            _initialOffsetFromOwnerCenter = initialPosition - owner.Center;
            
            // Keep the projectile stationary during bubble formation
            Hurry(0f, 0f); // Set speed to 0 during formation
            
            // Add randomization for unique bubble characteristics
            RandomizeBubbleCharacteristics();

            _poisonEffect = ParticleEffectsLibrary.Get(ParticleEffectLibraryKeys.Smoke);
            _poisonEffect.UseGravity = false;
            _poisonEffect.ParticleDrag = 0f;
            _poisonEffect.Shape = EmissionShape.Point;
            _poisonEffect.ParticleStartSize = 10f;
            _poisonEffect.ParticleEndSize = 70f;
            _poisonEffect.ParticleSpeed = Vector2.Zero;// new Vector2((_owner.IsPlayer1 ? 1 : -1.2f) * _baseSpeed.X / 2, -20f);
            _poisonEffect.Offset = -Vector2.One*_poisonEffect.ParticleStartSize/2;
            _poisonEffect.ParticleStartColor = Color.DarkGreen;
            _poisonEffect.ParticleEndColor = Color.DarkGreen.ApplyAlpha(0f);
            ParticleEffects.Add(_poisonEffect);
        }
        
        private void RandomizeBubbleCharacteristics()
        {
            // Randomize wobble parameters for unique bubble behavior
            _wobbleFrequency1 = 2.3f + (float)Game.Random.NextDouble() * 1.2f; // 2.3 - 3.5 Hz
            _wobbleFrequency2 = 1.4f + (float)Game.Random.NextDouble() * 0.8f; // 1.4 - 2.2 Hz  
            _wobbleAmplitude = 0.6f + (float)Game.Random.NextDouble() * 0.4f; // 0.6 - 1.0 pixels
            
            // Randomize bubble size slightly for organic variation
            _finalBubbleRadius = 32f + (float)Game.Random.NextDouble() * 6f; // 32 - 38 pixels
            
            // Randomize formation timing for natural variety
            _bubbleFormationTime = 0.3f + (float)Game.Random.NextDouble() * 0.1f; // 0.3 - 0.4 seconds
        }

        private float _lifeTime = 0f;
        private bool _bubbleFormationComplete = false;
        private float _bubblePopTime;
        
        public void Pop()
        {
            if (_bubblePopTime > 0)
                return;

            _bubblePopTime = Raylib.GetFrameTime();

            _poisonEffect.ParticleEndSize = 300f;
            _poisonEffect.EmissionRate *= 2;
            _poisonEffect.FollowRotation = true;
            Modifiers.RotationOffsetIncrement = 5f;
            _poisonEffect.Offset += Vector2.One * -35;
            _poisonEffect.ParticleSpeed = new Vector2(Speed.X / 2.1f, -20 - (float)Game.Random.NextDouble()*40f);
            _poisonEffect.ParticleSizeVariation = 100f;
        }

        public override void Update(Game game)
        {
            _lifeTime += Raylib.GetFrameTime();

            // Check if bubble formation is complete
            if (!_bubbleFormationComplete && _lifeTime >= _bubbleFormationTime)
            {
                // Formation complete - release the bubble!
                _bubbleFormationComplete = true;

                // Set the speed to make the bubble travel
                var actualSpeed = new Vector2(
                    BaseSpeed.X * (_owner.IsPlayer1 ? 1f : -1f),
                    BaseSpeed.Y
                );
                HurryTo(actualSpeed.X, actualSpeed.Y);

                // The projectile is now at the correct position and will move independently
            }

            if (!_bubbleFormationComplete)
            {
                // During formation: physically move the projectile to follow the ship
                Vector2 newPosition = _owner.Center + _initialOffsetFromOwnerCenter;
                MoveTo(newPosition.X, newPosition.Y);
                _originalPosition = newPosition;
            }
            else
            {
                // After formation: _originalPosition follows the projectile's actual position
                _originalPosition = Position;
            }

            if(_bubblePopTime > .5f)
            {
                if(Rect.Size.X < 300f)
                { 
                    Move(-100f, -100f);
                    ScaleTo(300f, 300f);
                }
            }
            else if(_bubblePopTime == 0)
            {
                if (_bubbleFormationComplete)
                {
                    if (_owner.IsPlayer1)
                        _poisonEffect.ParticleSpeed = new Vector2(Speed.X / 2.1f, -20f);///2;
                    else
                        _poisonEffect.ParticleSpeed = new Vector2(Speed.X / 1.9f, -20f);///2;

                    _poisonEffect.Offset = -Vector2.One * _poisonEffect.ParticleStartSize / 2;
                }
                else
                {
                    //We don't want to show the poison effect yet
                    _poisonEffect.Offset = new Vector2(0f, 100000f);
                }
            }
            else
            {
                _bubblePopTime += Raylib.GetFrameTime();
                // Pop animation now handled in Draw() method
            }

            base.Update(game);
        }

        public override void Draw()
        {
            //Raylib.DrawRectangleLines((int)Rect.X, (int)Rect.Y, (int)Rect.Width, (int)Rect.Height, Color.Red);
            
            // If bubble is popping, show pop animation instead of normal bubble
            if (_bubblePopTime > 0)
            {
                DrawBubblePopping();
                return; // Don't draw normal bubble during pop animation
            }
            
            // Calculate animation progress (0.0 to 1.0)
            float animationProgress = Math.Clamp(_lifeTime / _bubbleFormationTime, 0f, 1f);
            
            // Get prong positions with player direction consideration
            float directionMultiplier = _owner.IsPlayer1 ? 1f : -1f; // Mirror for Player 2
            var spawnPoint = new Vector2(Center.X + 
                (_owner.IsPlayer1 ? -1 : 1)*Rect.Width / 2 + 
                (_owner.IsPlayer1 ? 1 : -1)*25f, (!_bubbleFormationComplete ? _owner.Center.Y : Center.Y)); 
            Vector2 topProng = spawnPoint + new Vector2(-12.78f * directionMultiplier, -9.89f);
            Vector2 bottomProng = spawnPoint + new Vector2(-12.78f * directionMultiplier, 9.89f);
            
            // Realistic bubble colors with slight randomization
            Color bubbleColor = GetRandomizedBubbleColor();
            Color soapColor = GetRandomizedSoapColor();
            Color highlightColor = GetRandomizedHighlightColor();

            if (!_bubbleFormationComplete)
            {
                // During formation animation
                DrawBubbleFormation(topProng, bottomProng, animationProgress, bubbleColor, soapColor, highlightColor, directionMultiplier);
            }
            else
            {
                // Fully formed transparent bubble traveling independently with realistic wobble
                Vector2 prongCenterPoint = (topProng + bottomProng) / 2f;
                Vector2 bubbleCenter = prongCenterPoint + new Vector2(_finalBubbleRadius * directionMultiplier, 0f);
                
                // Full wobble intensity for traveling bubble - most realistic motion
                float wobbleIntensity = 1.0f;
                
                // Create wobbled outline for the traveling bubble with randomized complexity
                int wobbleSegments = 28 + Game.Random.Next(8); // 28-35 segments for variation
                List<Vector2> wobbledPoints = new List<Vector2>();
                
                for (int i = 0; i < wobbleSegments; i++)
                {
                    float angle = (i / (float)wobbleSegments) * (float)Math.PI * 2f;
                    
                    // Complex wobble with randomized phase shifts for unique patterns
                    float randomPhase1 = (float)Math.Sin(_lifeTime * 0.1f) * 2f; // Slow phase drift
                    float randomPhase2 = (float)Math.Cos(_lifeTime * 0.07f) * 1.5f; // Different phase drift
                    float wobble1 = (float)Math.Sin(_lifeTime * _wobbleFrequency1 * Math.PI * 2 + angle * 3 + randomPhase1) * _wobbleAmplitude * wobbleIntensity;
                    float wobble2 = (float)Math.Sin(_lifeTime * _wobbleFrequency2 * Math.PI * 2 + angle * 5 + randomPhase2) * _wobbleAmplitude * wobbleIntensity * 0.7f;
                    float wobble3 = (float)Math.Sin(_lifeTime * (_wobbleFrequency1 + _wobbleFrequency2) * 0.5f * Math.PI * 2 + angle * 2) * _wobbleAmplitude * wobbleIntensity * 0.4f;
                    float totalWobble = wobble1 + wobble2 + wobble3;
                    
                    Vector2 wobbledPoint = bubbleCenter + new Vector2(
                        (float)Math.Cos(angle) * (_finalBubbleRadius + totalWobble),
                        (float)Math.Sin(angle) * (_finalBubbleRadius + totalWobble)
                    );
                    wobbledPoints.Add(wobbledPoint);
                }
                
                // Draw the transparent bubble fill (static center for stability)
                Raylib.DrawCircle((int)bubbleCenter.X, (int)bubbleCenter.Y, _finalBubbleRadius, bubbleColor);
                
                // Draw the wobbled outline
                for (int i = 0; i < wobbledPoints.Count; i++)
                {
                    Vector2 point1 = wobbledPoints[i];
                    Vector2 point2 = wobbledPoints[(i + 1) % wobbledPoints.Count];
                    Raylib.DrawLineEx(point1, point2, 2f, soapColor);
                }
                
                // Draw multiple soap film highlights with randomized wobble
                DrawRandomizedHighlights(bubbleCenter, highlightColor);
            }
        }
        
        private Color GetRandomizedBubbleColor()
        {
            // Slight color variation for each bubble
            int baseR = 180 + Game.Random.Next(-10, 11); // 170-190
            int baseG = 200 + Game.Random.Next(-15, 16); // 185-215  
            int baseB = 255; // Keep blue component stable
            int alpha = 20 + Game.Random.Next(8); // 20-27 for slight transparency variation
            
            return new Color((byte)Math.Clamp(baseR, 0, 255), (byte)Math.Clamp(baseG, 0, 255), (byte)baseB, (byte)alpha);
        }
        
        private Color GetRandomizedSoapColor()
        {
            // Slight soap film color variation
            int baseR = 160 + Game.Random.Next(-8, 9); // 152-168
            int baseG = 180 + Game.Random.Next(-10, 11); // 170-190
            int baseB = 255; // Keep blue stable for consistency
            int alpha = 115 + Game.Random.Next(10); // 115-124
            
            return new Color((byte)Math.Clamp(baseR, 0, 255), (byte)Math.Clamp(baseG, 0, 255), (byte)baseB, (byte)alpha);
        }
        
        private Color GetRandomizedHighlightColor()
        {
            // Subtle highlight color variation
            int baseR = 220 + Game.Random.Next(-5, 6); // 215-225
            int baseG = 230 + Game.Random.Next(-5, 6); // 225-235
            int baseB = 255; // Keep bright
            int alpha = 75 + Game.Random.Next(12); // 75-86
            
            return new Color((byte)Math.Clamp(baseR, 0, 255), (byte)Math.Clamp(baseG, 0, 255), (byte)baseB, (byte)alpha);
        }
        
        private void DrawRandomizedHighlights(Vector2 bubbleCenter, Color highlightColor)
        {
            // Randomize highlight positions and sizes for organic variety
            float highlight1Scale = 0.65f + (float)Game.Random.NextDouble() * 0.1f; // 0.65-0.75
            float highlight2Scale = 0.35f + (float)Game.Random.NextDouble() * 0.1f; // 0.35-0.45
            
            float highlightRadius1 = _finalBubbleRadius * highlight1Scale;
            float highlightRadius2 = _finalBubbleRadius * highlight2Scale;
            
            // Main highlight with randomized wobble and offset
            float mainOffsetX = -8f + (float)Game.Random.NextDouble() * 3f; // -8 to -5
            float mainOffsetY = -8f + (float)Game.Random.NextDouble() * 3f; // -8 to -5
            float mainWobbleX = (float)Math.Sin(_lifeTime * _wobbleFrequency1 * Math.PI) * 2f;
            float mainWobbleY = (float)Math.Sin(_lifeTime * _wobbleFrequency2 * Math.PI) * 1.5f;
            Raylib.DrawCircleLines((int)(bubbleCenter.X + mainOffsetX + mainWobbleX), (int)(bubbleCenter.Y + mainOffsetY + mainWobbleY), highlightRadius1 * 0.4f, highlightColor);
            
            // Secondary highlight with different randomized pattern
            float secOffsetX = -12f + (float)Game.Random.NextDouble() * 2f; // -12 to -10
            float secOffsetY = -5f + (float)Game.Random.NextDouble() * 2f; // -5 to -3
            float secondaryWobbleX = (float)Math.Sin(_lifeTime * _wobbleFrequency2 * Math.PI * 1.3f) * 1.8f;
            float secondaryWobbleY = (float)Math.Sin(_lifeTime * _wobbleFrequency1 * Math.PI * 0.8f) * 1.2f;
            Raylib.DrawCircleLines((int)(bubbleCenter.X + secOffsetX + secondaryWobbleX), (int)(bubbleCenter.Y + secOffsetY + secondaryWobbleY), highlightRadius2 * 0.2f, 
                new Color((byte)255, (byte)255, (byte)255, (byte)(55 + Game.Random.Next(10)))); // 55-64 alpha variation
            
            // Rim highlight with subtle randomization
            float rimOffset = (float)Game.Random.NextDouble() * 1.5f; // 0-1.5 pixel variation
            float rimWobble = (float)Math.Sin(_lifeTime * (_wobbleFrequency1 + _wobbleFrequency2) * 0.5f * Math.PI) * 0.8f;
            Raylib.DrawCircleLines((int)(bubbleCenter.X + rimOffset + rimWobble), (int)bubbleCenter.Y, _finalBubbleRadius - 1, 
                new Color((byte)200, (byte)210, (byte)255, (byte)(35 + Game.Random.Next(10)))); // 35-44 alpha variation
        }
        
        private void DrawSemiCircleStage(Vector2 centerPoint, Vector2 topProng, Vector2 bottomProng, float prongDistance, float progress, Color soapColor, Color highlightColor, float directionMultiplier)
        {
            // Start with a small semi-circle that grows
            float initialRadius = prongDistance / 3f; // Start small relative to prong distance
            float currentRadius = initialRadius * progress;
            
            // Draw the semi-circle connected to prongs (mirrored for Player 2)
            Vector2 bubbleCenter = centerPoint + new Vector2(currentRadius * directionMultiplier, 0f);
            
            // Calculate subtle wobble for realistic bubble motion with randomization
            float wobbleIntensity = progress * (0.25f + (float)Game.Random.NextDouble() * 0.1f); // 0.25-0.35 variation
            
            // Randomize segment count for organic variety
            int segments = 14 + Game.Random.Next(4); // 14-17 segments
            
            // Draw the curved part of the semi-circle with subtle wobble
            for (int i = 0; i < segments; i++)
            {
                // Draw the appropriate half based on player direction
                float startAngle = directionMultiplier > 0 ? -0.5f : 0.5f; // Right half for P1, left half for P2
                float angle1 = (startAngle + (i / (float)segments) * directionMultiplier) * (float)Math.PI;
                float angle2 = (startAngle + ((i + 1) / (float)segments) * directionMultiplier) * (float)Math.PI;
                
                // Apply subtle wobble with randomized phase shifts
                float randomPhase = (float)(Game.Random.NextDouble() * Math.PI * 2);
                float wobble1 = (float)Math.Sin(_lifeTime * _wobbleFrequency1 * Math.PI * 2 + angle1 * 3 + randomPhase) * _wobbleAmplitude * wobbleIntensity;
                float wobble2 = (float)Math.Sin(_lifeTime * _wobbleFrequency2 * Math.PI * 2 + angle2 * 3 + randomPhase) * _wobbleAmplitude * wobbleIntensity;
                
                Vector2 point1 = bubbleCenter + new Vector2(
                    (float)Math.Cos(angle1) * (currentRadius + wobble1),
                    (float)Math.Sin(angle1) * (currentRadius + wobble1)
                );
                Vector2 point2 = bubbleCenter + new Vector2(
                    (float)Math.Cos(angle2) * (currentRadius + wobble2),
                    (float)Math.Sin(angle2) * (currentRadius + wobble2)
                );
                
                Raylib.DrawLineEx(point1, point2, 2f, soapColor);
            }
            
            // Draw transparent soap film connections
            Vector2 topConnection = bubbleCenter + new Vector2(0, -currentRadius);
            Vector2 bottomConnection = bubbleCenter + new Vector2(0, currentRadius);
            
            Raylib.DrawLineEx(topProng, topConnection, 2f, soapColor);
            Raylib.DrawLineEx(bottomProng, bottomConnection, 2f, soapColor);
            
            // Add subtle soap film highlights with randomized timing
            float highlightThreshold = 0.4f + (float)Game.Random.NextDouble() * 0.2f; // 0.4-0.6 random threshold
            if (progress > highlightThreshold)
            {
                float dropletSize = 1.2f + (float)Game.Random.NextDouble() * 0.6f; // 1.2-1.8 size variation
                Raylib.DrawCircle((int)topConnection.X, (int)topConnection.Y, dropletSize, highlightColor);
                Raylib.DrawCircle((int)bottomConnection.X, (int)bottomConnection.Y, dropletSize, highlightColor);
            }
        }
        
        private void DrawSemiEllipseStage(Vector2 centerPoint, Vector2 topProng, Vector2 bottomProng, float prongDistance, float progress, Color soapColor, Color bubbleColor, Color highlightColor, float directionMultiplier)
        {
            float initialRadius = prongDistance / 3f;
            
            // Make the ellipse grow with slight randomization
            float horizontalGrowthRate = 0.55f + (float)Game.Random.NextDouble() * 0.1f; // 0.55-0.65 variation
            float verticalGrowthRate = 0.85f + (float)Game.Random.NextDouble() * 0.1f; // 0.85-0.95 variation
            
            float horizontalRadius = initialRadius + ((_finalBubbleRadius - initialRadius) * horizontalGrowthRate * progress);
            float maxVerticalGrowth = _finalBubbleRadius * verticalGrowthRate - initialRadius;
            float verticalRadius = initialRadius + (maxVerticalGrowth * progress);
            
            // Ellipse center follows player direction
            Vector2 ellipseCenter = centerPoint + new Vector2(horizontalRadius * directionMultiplier, 0f);
            
            // Calculate wobble intensity with randomization
            float wobbleIntensity = progress * (0.45f + (float)Game.Random.NextDouble() * 0.1f); // 0.45-0.55 variation
            
            // Randomize segment count for organic ellipse variation
            int segments = 18 + Game.Random.Next(4); // 18-21 segments
            
            // Draw the transparent semi-ellipse with randomized wobble
            for (int i = 0; i < segments; i++)
            {
                // Draw the appropriate half based on player direction
                float startAngle = directionMultiplier > 0 ? -0.5f : 0.5f; // Right half for P1, left half for P2
                float angle1 = (startAngle + (i / (float)segments) * directionMultiplier) * (float)Math.PI;
                float angle2 = (startAngle + ((i + 1) / (float)segments) * directionMultiplier) * (float)Math.PI;
                
                // Apply different wobble patterns with randomized complexity
                float randomPhase1 = (float)(Game.Random.NextDouble() * Math.PI);
                float randomPhase2 = (float)(Game.Random.NextDouble() * Math.PI);
                
                float wobbleH1 = (float)Math.Sin(_lifeTime * _wobbleFrequency1 * Math.PI * 2 + angle1 * 2 + randomPhase1) * _wobbleAmplitude * wobbleIntensity;
                float wobbleV1 = (float)Math.Sin(_lifeTime * _wobbleFrequency2 * Math.PI * 2 + angle1 * 4 + randomPhase2) * _wobbleAmplitude * wobbleIntensity * 0.7f;
                float wobbleH2 = (float)Math.Sin(_lifeTime * _wobbleFrequency1 * Math.PI * 2 + angle2 * 2 + randomPhase1) * _wobbleAmplitude * wobbleIntensity;
                float wobbleV2 = (float)Math.Sin(_lifeTime * _wobbleFrequency2 * Math.PI * 2 + angle2 * 4 + randomPhase2) * _wobbleAmplitude * wobbleIntensity * 0.7f;
                
                Vector2 point1 = ellipseCenter + new Vector2(
                    (float)Math.Cos(angle1) * (horizontalRadius + wobbleH1),
                    (float)Math.Sin(angle1) * (verticalRadius + wobbleV1)
                );
                Vector2 point2 = ellipseCenter + new Vector2(
                    (float)Math.Cos(angle2) * (horizontalRadius + wobbleH2),
                    (float)Math.Sin(angle2) * (verticalRadius + wobbleV2)
                );
                
                Raylib.DrawLineEx(point1, point2, 2f, soapColor);
            }
            
            // Draw transparent soap film connections
            Vector2 topConnection = ellipseCenter + new Vector2(0, -verticalRadius);
            Vector2 bottomConnection = ellipseCenter + new Vector2(0, verticalRadius);
            
            Raylib.DrawLineEx(topProng, topConnection, 2f, soapColor);
            Raylib.DrawLineEx(bottomProng, bottomConnection, 2f, soapColor);
            
            // Fill with randomized transparency timing
            float fillThreshold = 0.25f + (float)Game.Random.NextDouble() * 0.1f; // 0.25-0.35 random start
            if (progress > fillThreshold)
            {
                float fillProgress = (progress - fillThreshold) / (1f - fillThreshold);
                float transparencyVariation = 0.5f + (float)Game.Random.NextDouble() * 0.2f; // 0.5-0.7 variation
                Color fillColor = new Color(
                    bubbleColor.R,
                    bubbleColor.G,
                    bubbleColor.B,
                    (byte)(bubbleColor.A * fillProgress * transparencyVariation)
                );
                
                // Draw filled semi-ellipse with randomized density, considering direction
                int stepSize = 2 + Game.Random.Next(2); // 2-3 pixel steps for variation
                float fillStart = directionMultiplier > 0 ? 0 : -horizontalRadius;
                float fillEnd = directionMultiplier > 0 ? horizontalRadius : 0;
                
                for (float x = fillStart; (directionMultiplier > 0 ? x <= fillEnd : x >= fillEnd); x += stepSize * directionMultiplier)
                {
                    float normalizedX = Math.Abs(x) / horizontalRadius;
                    if (normalizedX <= 1f)
                    {
                        float y = verticalRadius * (float)Math.Sin(Math.Acos(normalizedX));
                        Vector2 fillPoint = ellipseCenter + new Vector2(x, 0);
                        Raylib.DrawRectangle((int)fillPoint.X, (int)(fillPoint.Y - y), stepSize, (int)(y * 2), fillColor);
                    }
                }
                
                // Add randomized highlights during growth
                float highlightThreshold = 0.55f + (float)Game.Random.NextDouble() * 0.1f; // 0.55-0.65
                if (progress > highlightThreshold)
                {
                    float highlightScale = 0.25f + (float)Game.Random.NextDouble() * 0.1f; // 0.25-0.35 variation
                    float highlightRadius = Math.Min(horizontalRadius, verticalRadius) * highlightScale;
                    float offsetX = (-6f + (float)Game.Random.NextDouble() * 2f) * directionMultiplier; // Mirror offset for Player 2
                    float offsetY = -4f + (float)Game.Random.NextDouble() * 2f; // -4 to -2
                    Raylib.DrawCircleLines((int)(ellipseCenter.X + offsetX), (int)(ellipseCenter.Y + offsetY), highlightRadius, highlightColor);
                }
            }
        }

        private void DrawSphereDetachmentStage(Vector2 centerPoint, Vector2 topProng, Vector2 bottomProng, float progress, Color bubbleColor, Color soapColor, Color highlightColor, float directionMultiplier)
        {
            // Calculate the final bubble position (detached from prongs) with player direction
            Vector2 finalBubbleCenter = centerPoint + new Vector2(_finalBubbleRadius * directionMultiplier, 0f);
            
            // Calculate the starting values from the end of stage 2
            float prongDistance = Vector2.Distance(topProng, bottomProng);
            float initialRadius = prongDistance / 3f;
            
            // The ellipse now grows to 90% of final size with randomization
            float maxVerticalGrowth = _finalBubbleRadius * (0.85f + (float)Game.Random.NextDouble() * 0.1f) - initialRadius; // 0.85-0.95 variation
            float maxVerticalRadius = initialRadius + maxVerticalGrowth;
            float maxHorizontalRadius = initialRadius + ((_finalBubbleRadius - initialRadius) * (0.55f + (float)Game.Random.NextDouble() * 0.1f)); // 0.55-0.65 variation
            
            Vector2 startCenter = centerPoint + new Vector2(maxHorizontalRadius * directionMultiplier, 0f);
            Vector2 currentCenter = Vector2.Lerp(startCenter, finalBubbleCenter, progress);
            float currentRadius = maxVerticalRadius + (_finalBubbleRadius - maxVerticalRadius) * progress;
            
            // Maximum wobble with randomization during final formation
            float wobbleIntensity = (0.75f + (float)Game.Random.NextDouble() * 0.25f) + progress * 0.2f; // 0.75-1.0 base + growth
            
            // Randomized wobble complexity for unique sphere ripples
            int wobbleSegments = 30 + Game.Random.Next(6); // 30-35 segments for variation
            List<Vector2> wobbledPoints = new List<Vector2>();
            
            for (int i = 0; i < wobbleSegments; i++)
            {
                float angle = (i / (float)wobbleSegments) * (float)Math.PI * 2f;
                
                // Multiple sine waves with randomized phases for complex, unique bubble surface ripples
                float randomPhase1 = (float)(Game.Random.NextDouble() * Math.PI * 2);
                float randomPhase2 = (float)(Game.Random.NextDouble() * Math.PI * 2);
                
                float wobble1 = (float)Math.Sin(_lifeTime * _wobbleFrequency1 * Math.PI * 2 + angle * 4 + randomPhase1) * _wobbleAmplitude * wobbleIntensity;
                float wobble2 = (float)Math.Sin(_lifeTime * _wobbleFrequency2 * Math.PI * 2 + angle * 6 + randomPhase2) * _wobbleAmplitude * wobbleIntensity * 0.6f;
                float totalWobble = wobble1 + wobble2;
                
                Vector2 wobbledPoint = currentCenter + new Vector2(
                    (float)Math.Cos(angle) * (currentRadius + totalWobble),
                    (float)Math.Sin(angle) * (currentRadius + totalWobble)
                );
                wobbledPoints.Add(wobbledPoint);
            }
            
            // Draw the filled circle with randomized transparency
            float fillAlpha = 1f + (float)Game.Random.NextDouble() * 0.1f; // 1.0-1.1 slight variation
            Color randomizedBubbleColor = new Color(bubbleColor.R, bubbleColor.G, bubbleColor.B, (byte)(bubbleColor.A * fillAlpha));
            Raylib.DrawCircle((int)currentCenter.X, (int)currentCenter.Y, currentRadius, randomizedBubbleColor);
            
            // Draw the wobbled outline
            for (int i = 0; i < wobbledPoints.Count; i++)
            {
                Vector2 point1 = wobbledPoints[i];
                Vector2 point2 = wobbledPoints[(i + 1) % wobbledPoints.Count];
                Raylib.DrawLineEx(point1, point2, 2f, soapColor);
            }
            
            // Draw diminishing transparent connections with randomized strength
            float connectionStrength = (1f - progress) * (0.9f + (float)Game.Random.NextDouble() * 0.2f); // 0.9-1.1 variation
            if (connectionStrength > 0)
            {
                Color fadingSoapColor = new Color(
                    soapColor.R,
                    soapColor.G,
                    soapColor.B,
                    (byte)(soapColor.A * connectionStrength)
                );
                
                // Connection points with slight randomization
                Vector2 topDirection = Vector2.Normalize(topProng - currentCenter);
                Vector2 bottomDirection = Vector2.Normalize(bottomProng - currentCenter);
                
                float connectionVariation = 0.9f + (float)Game.Random.NextDouble() * 0.2f; // 0.9-1.1 variation
                Vector2 topConnectionPoint = currentCenter + topDirection * currentRadius * connectionVariation;
                Vector2 bottomConnectionPoint = currentCenter + bottomDirection * currentRadius * connectionVariation;
                
                float lineThickness = 2f * connectionStrength * (0.8f + (float)Game.Random.NextDouble() * 0.4f); // 0.8-1.2 variation
                Raylib.DrawLineEx(topProng, topConnectionPoint, lineThickness, fadingSoapColor);
                Raylib.DrawLineEx(bottomProng, bottomConnectionPoint, lineThickness, fadingSoapColor);
            }
            
            // Add realistic soap film highlights with randomization and player direction
            if (progress > 0.3f)
            {
                float highlightProgress = (progress - 0.3f) / 0.7f;
                float highlightRadius = currentRadius * (0.65f + (float)Game.Random.NextDouble() * 0.1f); // 0.65-0.75 variation
                
                // Main highlight with randomized wobble and position (mirrored for Player 2)
                float highlightOffsetX = (-8f + (float)Game.Random.NextDouble() * 3f) * directionMultiplier; // Mirror offset for Player 2
                float highlightOffsetY = -8f + (float)Game.Random.NextDouble() * 3f; // -8 to -5
                float highlightWobble = (float)Math.Sin(_lifeTime * _wobbleFrequency1 * Math.PI) * (1.5f + (float)Game.Random.NextDouble() * 1f); // 1.5-2.5 wobble
                Raylib.DrawCircleLines((int)(currentCenter.X + highlightOffsetX + highlightWobble * directionMultiplier), (int)(currentCenter.Y + highlightOffsetY), highlightRadius * 0.3f, 
                    new Color(highlightColor.R, highlightColor.G, highlightColor.B, (byte)(highlightColor.A * highlightProgress)));
                
                // Secondary smaller highlight with different randomization
                if (progress > 0.6f)
                {
                    float secOffsetX = (-12f + (float)Game.Random.NextDouble() * 2f) * directionMultiplier; // Mirror offset for Player 2
                    float secOffsetY = -5f + (float)Game.Random.NextDouble() * 2f; // -5 to -3
                    float secondaryWobble = (float)Math.Sin(_lifeTime * _wobbleFrequency2 * Math.PI) * (1f + (float)Game.Random.NextDouble() * 1f); // 1-2 wobble
                    float secHighlightRadius = highlightRadius * (0.1f + (float)Game.Random.NextDouble() * 0.1f); // 0.1-0.2 size variation
                    Raylib.DrawCircleLines((int)(currentCenter.X + secOffsetX + secondaryWobble * directionMultiplier), (int)(currentCenter.Y + secOffsetY), secHighlightRadius, 
                        new Color((byte)255, (byte)255, (byte)255, (byte)(30 + Game.Random.Next(20) * highlightProgress))); // 30-49 alpha variation
                }
            }
        }
        
        private void DrawBubbleFormation(Vector2 topProng, Vector2 bottomProng, float progress, Color bubbleColor, Color soapColor, Color highlightColor, float directionMultiplier)
        {
            Vector2 centerPoint = (topProng + bottomProng) / 2f;
            float prongDistance = Vector2.Distance(topProng, bottomProng);
            
            // Stage 1 (0.0 - 0.3): Semi-circle formation connected to prongs
            if (progress <= 0.3f)
            {
                float stageProgress = progress / 0.3f;
                DrawSemiCircleStage(centerPoint, topProng, bottomProng, prongDistance, stageProgress, soapColor, highlightColor, directionMultiplier);
            }
            // Stage 2 (0.3 - 0.7): Semi-ellipse growth (vertical grows 2x faster than horizontal)
            else if (progress <= 0.7f)
            {
                float stageProgress = (progress - 0.3f) / 0.4f;
                DrawSemiEllipseStage(centerPoint, topProng, bottomProng, prongDistance, stageProgress, soapColor, bubbleColor, highlightColor, directionMultiplier);
            }
            // Stage 3 (0.7 - 1.0): Detachment and sphere formation
            else if(_bubblePopTime == 0f)
            {
                float stageProgress = (progress - 0.7f) / 0.3f;
                DrawSphereDetachmentStage(centerPoint, topProng, bottomProng, stageProgress, bubbleColor, soapColor, highlightColor, directionMultiplier);
            }
        }

        private void DrawBubblePopping()
        {
            // Calculate pop progress (0.0 to 1.0 over 0.5 seconds)
            float popProgress = Math.Clamp(_bubblePopTime / 0.5f, 0f, 1f);
            
            // Get bubble center for the popping animation
            float directionMultiplier = _owner.IsPlayer1 ? 1f : -1f;
            var spawnPoint = new Vector2(Center.X + 
                (_owner.IsPlayer1 ? -1 : 1)*Rect.Width / 2 + 
                (_owner.IsPlayer1 ? 1 : -1)*25f, Center.Y); 
            Vector2 topProng = spawnPoint + new Vector2(-12.78f * directionMultiplier, -9.89f);
            Vector2 bottomProng = spawnPoint + new Vector2(-12.78f * directionMultiplier, 9.89f);
            Vector2 prongCenterPoint = (topProng + bottomProng) / 2f;
            Vector2 bubbleCenter = prongCenterPoint + new Vector2(_finalBubbleRadius * directionMultiplier, 0f);
            
            // Pop animation in 3 stages over 0.5 seconds: burst (0-0.4), expand (0.4-0.8), fade (0.8-1.0)
            if (popProgress <= 0.4f)
            {
                // Stage 1: Explosive burst with fragments (0.0 to 0.2 seconds)
                DrawPopBurstStage(bubbleCenter, popProgress / 0.4f);
            }
            else if (popProgress <= 0.8f)
            {
                // Stage 2: Expanding poison cloud (0.2 to 0.4 seconds)
                DrawPopExpandStage(bubbleCenter, (popProgress - 0.4f) / 0.4f);
            }
            else
            {
                // Stage 3: Final fade with lingering wisps (0.4 to 0.5 seconds)
                DrawPopFadeStage(bubbleCenter, (popProgress - 0.8f) / 0.2f);
            }
        }
        
        private void DrawPopBurstStage(Vector2 bubbleCenter, float stageProgress)
        {
            // Explosive burst effect with soap bubble fragments flying outward
            
            // Draw the bubble rapidly expanding and distorting
            float burstRadius = _finalBubbleRadius * (1f + stageProgress * 0.8f); // Grows 80% larger
            float distortion = stageProgress * 15f; // Heavy distortion during burst
            
            // Get bubble colors but make them more intense during burst
            Color bubbleColor = GetRandomizedBubbleColor();
            Color soapColor = GetRandomizedSoapColor();
            Color highlightColor = GetRandomizedHighlightColor();
            
            // Make colors more intense and opaque during burst
            bubbleColor = new Color(bubbleColor.R, bubbleColor.G, bubbleColor.B, (byte)(bubbleColor.A * (2f - stageProgress)));
            soapColor = new Color(soapColor.R, soapColor.G, soapColor.B, (byte)(soapColor.A * (1.5f - stageProgress)));
            
            // Draw heavily distorted bubble outline that's breaking apart
            int segments = 20 + Game.Random.Next(8); // Irregular segments
            for (int i = 0; i < segments; i++)
            {
                float angle = (i / (float)segments) * (float)Math.PI * 2f;
                
                // Extreme distortion with fragments breaking away
                float fragmentWobble = (float)Math.Sin(_lifeTime * 25f + angle * 8) * distortion;
                float burstWobble = (float)Math.Sin(_lifeTime * 40f + angle * 12) * distortion * 0.7f;
                float totalWobble = fragmentWobble + burstWobble;
                
                // Some fragments fly further out
                float fragmentDistance = burstRadius + totalWobble + (Game.Random.NextSingle() * stageProgress * 20f);
                
                Vector2 fragmentPoint = bubbleCenter + new Vector2(
                    (float)Math.Cos(angle) * fragmentDistance,
                    (float)Math.Sin(angle) * fragmentDistance
                );
                
                // Draw fragment as small wobbling line
                if (i % 3 == 0) // Only draw some fragments for scattered effect
                {
                    float fragmentLength = 3f + stageProgress * 4f;
                    Vector2 fragmentEnd = fragmentPoint + new Vector2(
                        (float)Math.Cos(angle) * fragmentLength,
                        (float)Math.Sin(angle) * fragmentLength
                    );
                    Raylib.DrawLineEx(fragmentPoint, fragmentEnd, 1f + stageProgress, soapColor);
                }
            }
            
            // Draw central burst flash
            float flashRadius = _finalBubbleRadius * stageProgress * 0.6f;
            Color flashColor = new Color((byte)200, (byte)255, (byte)200, (byte)(100 * (1f - stageProgress)));
            Raylib.DrawCircle((int)bubbleCenter.X, (int)bubbleCenter.Y, flashRadius, flashColor);
            
            // Draw radiating shock lines
            int shockLines = 8;
            for (int i = 0; i < shockLines; i++)
            {
                float angle = (i / (float)shockLines) * (float)Math.PI * 2f;
                float shockLength = 15f + stageProgress * 25f;
                
                Vector2 shockStart = bubbleCenter + new Vector2(
                    (float)Math.Cos(angle) * _finalBubbleRadius,
                    (float)Math.Sin(angle) * _finalBubbleRadius
                );
                Vector2 shockEnd = bubbleCenter + new Vector2(
                    (float)Math.Cos(angle) * (_finalBubbleRadius + shockLength),
                    (float)Math.Sin(angle) * (_finalBubbleRadius + shockLength)
                );
                
                Color shockColor = new Color((byte)180, (byte)255, (byte)180, (byte)(120 * (1f - stageProgress)));
                Raylib.DrawLineEx(shockStart, shockEnd, 2f * (1f - stageProgress), shockColor);
            }
        }
        
        private void DrawPopExpandStage(Vector2 bubbleCenter, float stageProgress)
        {
            // Expanding poison cloud with swirling green mist
            
            // Calculate expanding cloud properties
            float cloudRadius = _finalBubbleRadius * (1.5f + stageProgress * 2f); // Expands to 3.5x original size
            float opacity = 1f - stageProgress * 0.7f; // Gradually becomes more transparent
            
            // Draw multiple layers of expanding poison cloud
            int cloudLayers = 4;
            for (int layer = 0; layer < cloudLayers; layer++)
            {
                float layerScale = 0.6f + (layer * 0.3f); // Different sizes for depth
                float layerRadius = cloudRadius * layerScale;
                float layerOpacity = opacity * (1f - layer * 0.2f); // Outer layers more transparent
                
                // Swirling motion for each layer
                float swirl = stageProgress * (layer + 1) * 180f; // Different swirl speeds per layer
                
                // Draw cloud as irregular circles with gaps
                int cloudSegments = 16 + layer * 4;
                for (int i = 0; i < cloudSegments; i++)
                {
                    float angle = (i / (float)cloudSegments) * (float)Math.PI * 2f + swirl * (float)Math.PI / 180f;
                    
                    // Create gaps and irregularities in the cloud
                    if ((i + layer) % 3 == 0) continue; // Skip some segments for wispy effect
                    
                    // Turbulent wobble for cloud edge
                    float turbulence = (float)Math.Sin(_lifeTime * 15f + angle * 6 + layer * 2) * (2f + stageProgress * 3f);
                    float cloudDistance = layerRadius + turbulence;
                    
                    Vector2 cloudPoint = bubbleCenter + new Vector2(
                        (float)Math.Cos(angle) * cloudDistance,
                        (float)Math.Sin(angle) * cloudDistance
                    );
                    
                    // Draw cloud puff as small circle
                    float puffSize = 2f + stageProgress * 3f + layer * 0.5f;
                    Color cloudColor = new Color((byte)100, (byte)160, (byte)100, (byte)(80 * layerOpacity)); // Dark green poison
                    Raylib.DrawCircle((int)cloudPoint.X, (int)cloudPoint.Y, puffSize, cloudColor);
                }
            }
            
            // Draw central poison concentration
            float centralRadius = _finalBubbleRadius * (0.8f - stageProgress * 0.3f);
            Color centralColor = new Color((byte)80, (byte)140, (byte)80, (byte)(60 * opacity));
            Raylib.DrawCircle((int)bubbleCenter.X, (int)bubbleCenter.Y, centralRadius, centralColor);
            
            // Draw swirling tendrils extending outward
            int tendrils = 6;
            for (int i = 0; i < tendrils; i++)
            {
                float tendrilAngle = (i / (float)tendrils) * (float)Math.PI * 2f + stageProgress * 90f * (float)Math.PI / 180f;
                float tendrilLength = 20f + stageProgress * 40f;
                
                // Create curving tendril path
                int tendrilSegments = 8;
                for (int seg = 0; seg < tendrilSegments - 1; seg++)
                {
                    float segProgress = seg / (float)(tendrilSegments - 1);
                    float nextSegProgress = (seg + 1) / (float)(tendrilSegments - 1);
                    
                    // Curve the tendril with sine wave
                    float curve1 = (float)Math.Sin(segProgress * Math.PI) * 8f;
                    float curve2 = (float)Math.Sin(nextSegProgress * Math.PI) * 8f;
                    
                    Vector2 tendrilStart = bubbleCenter + new Vector2(
                        (float)Math.Cos(tendrilAngle) * (centralRadius + segProgress * tendrilLength) + curve1,
                        (float)Math.Sin(tendrilAngle) * (centralRadius + segProgress * tendrilLength)
                    );
                    Vector2 tendrilEnd = bubbleCenter + new Vector2(
                        (float)Math.Cos(tendrilAngle) * (centralRadius + nextSegProgress * tendrilLength) + curve2,
                        (float)Math.Sin(tendrilAngle) * (centralRadius + nextSegProgress * tendrilLength)
                    );
                    
                    float tendrilOpacity = opacity * (1f - segProgress * 0.7f);
                    Color tendrilColor = new Color((byte)90, (byte)150, (byte)90, (byte)(100 * tendrilOpacity));
                    float tendrilThickness = 2f * (1f - segProgress * 0.5f);
                    
                    Raylib.DrawLineEx(tendrilStart, tendrilEnd, tendrilThickness, tendrilColor);
                }
            }
        }
        
        private void DrawPopFadeStage(Vector2 bubbleCenter, float stageProgress)
        {
            // Final fade with lingering wisps and sparkles
            
            float fadeOpacity = 1f - stageProgress; // Complete fade out
            
            // Draw fading poison wisps
            int wisps = 8;
            for (int i = 0; i < wisps; i++)
            {
                float wispAngle = (i / (float)wisps) * (float)Math.PI * 2f;
                float wispDistance = _finalBubbleRadius * (2f + stageProgress * 1.5f); // Continue expanding while fading
                
                // Floating motion for wisps
                float drift = (float)Math.Sin(_lifeTime * 8f + i * 2) * 5f;
                
                Vector2 wispPosition = bubbleCenter + new Vector2(
                    (float)Math.Cos(wispAngle) * wispDistance + drift,
                    (float)Math.Sin(wispAngle) * wispDistance
                );
                
                float wispSize = 3f + (float)Math.Sin(_lifeTime * 10f + i) * 1f;
                Color wispColor = new Color((byte)120, (byte)180, (byte)120, (byte)(40 * fadeOpacity));
                Raylib.DrawCircle((int)wispPosition.X, (int)wispPosition.Y, wispSize, wispColor);
            }
            
            // Draw tiny sparkles from soap film remnants
            int sparkles = 12;
            for (int i = 0; i < sparkles; i++)
            {
                float sparkleAngle = (i / (float)sparkles) * (float)Math.PI * 2f + stageProgress * 720f * (float)Math.PI / 180f;
                float sparkleDistance = _finalBubbleRadius * (0.8f + stageProgress * 2.2f);
                
                Vector2 sparklePosition = bubbleCenter + new Vector2(
                    (float)Math.Cos(sparkleAngle) * sparkleDistance,
                    (float)Math.Sin(sparkleAngle) * sparkleDistance
                );
                
                // Twinkling effect
                float twinkle = (float)Math.Sin(_lifeTime * 20f + i * 3) * 0.5f + 0.5f;
                Color sparkleColor = new Color((byte)200, (byte)220, (byte)255, (byte)(60 * fadeOpacity * twinkle));
                Raylib.DrawPixel((int)sparklePosition.X, (int)sparklePosition.Y, sparkleColor);
                
                // Occasionally draw small cross sparkle
                if (twinkle > 0.8f)
                {
                    Raylib.DrawLineEx(
                        sparklePosition + new Vector2(-1, 0), 
                        sparklePosition + new Vector2(1, 0), 
                        1f, sparkleColor);
                    Raylib.DrawLineEx(
                        sparklePosition + new Vector2(0, -1), 
                        sparklePosition + new Vector2(0, 1), 
                        1f, sparkleColor);
                }
            }
            
            // Final central glow fade
            if (fadeOpacity > 0.1f)
            {
                float glowRadius = _finalBubbleRadius * 0.3f;
                Color glowColor = new Color((byte)150, (byte)200, (byte)150, (byte)(30 * fadeOpacity));
                Raylib.DrawCircle((int)bubbleCenter.X, (int)bubbleCenter.Y, glowRadius, glowColor);
            }
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return [new PoisonedEffect()];
        }
    }
}
