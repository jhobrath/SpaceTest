using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class PoisonProjectile : Projectile
    {
        public static readonly Vector2 _baseSpeed = new Vector2(100f, 0f);
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

        // Random seed for consistent animation per projectile
        private Random _animationRandom;
        
        // Cached curve points for smooth animation
        private List<Vector2> _topStreamPath;
        private List<Vector2> _bottomStreamPath;
        private List<Vector2> _bubbleEdgePoints;

        public PoisonProjectile(IProjectileController controller, Player owner, SpriteWrapper sprite, Vector2 initialPosition, PlayerProjectile modifiers) 
            : base(controller, owner, sprite, initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            _originalPosition = initialPosition;
            _owner = owner;
            
            // Store the initial offset from the owner's center so we can follow the ship's movement
            _initialOffsetFromOwnerCenter = initialPosition - owner.Center;
            
            // Keep the projectile stationary during bubble formation
            Hurry(0f, 0f); // Set speed to 0 during formation
        }

        private float _lifeTime = 0f;
        private bool _bubbleFormationComplete = false;
        
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
            
            base.Update(game);
        }

        public override void Draw()
        {
            // Calculate animation progress (0.0 to 1.0)
            float animationProgress = Math.Clamp(_lifeTime / _bubbleFormationTime, 0f, 1f);
            
            // Get prong positions
            Vector2 topProng = _originalPosition + new Vector2(-12.78f, -9.89f);
            Vector2 bottomProng = _originalPosition + new Vector2(-12.78f, 9.89f);
            
            // Realistic bubble colors - transparent with subtle blue/purple hue
            Color bubbleColor = new Color((byte)180, (byte)200, (byte)255, (byte)25); // Very faint blue-purple, highly transparent
            Color soapColor = new Color((byte)160, (byte)180, (byte)255, (byte)120);   // Subtle blue-purple soap film
            Color highlightColor = new Color((byte)220, (byte)230, (byte)255, (byte)80); // Soft white-blue highlights

            if (!_bubbleFormationComplete)
            {
                // During formation animation
                DrawBubbleFormation(topProng, bottomProng, animationProgress, bubbleColor, soapColor, highlightColor);
            }
            else
            {
                // Fully formed transparent bubble traveling independently
                Vector2 prongCenterPoint = (topProng + bottomProng) / 2f;
                Vector2 bubbleCenter = prongCenterPoint + new Vector2(_finalBubbleRadius, 0f);
                
                // Draw the transparent bubble with subtle hue
                Raylib.DrawCircle((int)bubbleCenter.X, (int)bubbleCenter.Y, _finalBubbleRadius, bubbleColor);
                Raylib.DrawCircleLines((int)bubbleCenter.X, (int)bubbleCenter.Y, _finalBubbleRadius, soapColor);
                
                // Draw multiple soap film highlights for realistic bubble appearance
                float highlightRadius1 = _finalBubbleRadius * 0.7f;
                float highlightRadius2 = _finalBubbleRadius * 0.4f;
                
                // Main highlight - larger and softer
                Raylib.DrawCircleLines((int)(bubbleCenter.X - 8), (int)(bubbleCenter.Y - 8), highlightRadius1 * 0.4f, highlightColor);
                
                // Secondary highlight - smaller and sharper for realism
                Raylib.DrawCircleLines((int)(bubbleCenter.X - 12), (int)(bubbleCenter.Y - 5), highlightRadius2 * 0.2f, 
                    new Color((byte)255, (byte)255, (byte)255, (byte)60));
                
                // Rim highlight - very subtle edge lighting
                Raylib.DrawCircleLines((int)bubbleCenter.X, (int)bubbleCenter.Y, _finalBubbleRadius - 1, 
                    new Color((byte)200, (byte)210, (byte)255, (byte)40));
            }
        }

        private void DrawBubbleFormation(Vector2 topProng, Vector2 bottomProng, float progress, Color bubbleColor, Color soapColor, Color highlightColor)
        {
            Vector2 centerPoint = (topProng + bottomProng) / 2f;
            float prongDistance = Vector2.Distance(topProng, bottomProng);
            
            // Stage 1 (0.0 - 0.3): Semi-circle formation connected to prongs
            if (progress <= 0.3f)
            {
                float stageProgress = progress / 0.3f;
                DrawSemiCircleStage(centerPoint, topProng, bottomProng, prongDistance, stageProgress, soapColor, highlightColor);
            }
            // Stage 2 (0.3 - 0.7): Semi-ellipse growth (vertical grows 2x faster than horizontal)
            else if (progress <= 0.7f)
            {
                float stageProgress = (progress - 0.3f) / 0.4f;
                DrawSemiEllipseStage(centerPoint, topProng, bottomProng, prongDistance, stageProgress, soapColor, bubbleColor, highlightColor);
            }
            // Stage 3 (0.7 - 1.0): Detachment and sphere formation
            else
            {
                float stageProgress = (progress - 0.7f) / 0.3f;
                DrawSphereDetachmentStage(centerPoint, topProng, bottomProng, stageProgress, bubbleColor, soapColor, highlightColor);
            }
        }

        private void DrawSemiCircleStage(Vector2 centerPoint, Vector2 topProng, Vector2 bottomProng, float prongDistance, float progress, Color soapColor, Color highlightColor)
        {
            // Start with a small semi-circle that grows
            float initialRadius = prongDistance / 3f; // Start small relative to prong distance
            float currentRadius = initialRadius * progress;
            
            // Draw the semi-circle (right half of a circle) connected to prongs
            Vector2 bubbleCenter = centerPoint + new Vector2(currentRadius, 0f);
            
            // Draw the curved part of the semi-circle with subtle transparency
            int segments = 16;
            for (int i = 0; i < segments; i++)
            {
                // Draw from -90 degrees to +90 degrees (right semi-circle)
                float angle1 = ((i / (float)segments) - 0.5f) * (float)Math.PI;
                float angle2 = (((i + 1) / (float)segments) - 0.5f) * (float)Math.PI;
                
                Vector2 point1 = bubbleCenter + new Vector2(
                    (float)Math.Cos(angle1) * currentRadius,
                    (float)Math.Sin(angle1) * currentRadius
                );
                Vector2 point2 = bubbleCenter + new Vector2(
                    (float)Math.Cos(angle2) * currentRadius,
                    (float)Math.Sin(angle2) * currentRadius
                );
                
                Raylib.DrawLineEx(point1, point2, 2f, soapColor);
            }
            
            // Draw transparent soap film connections
            Vector2 topConnection = bubbleCenter + new Vector2(0, -currentRadius);
            Vector2 bottomConnection = bubbleCenter + new Vector2(0, currentRadius);
            
            Raylib.DrawLineEx(topProng, topConnection, 2f, soapColor);
            Raylib.DrawLineEx(bottomProng, bottomConnection, 2f, soapColor);
            
            // Add subtle soap film highlights for early formation
            if (progress > 0.5f)
            {
                Raylib.DrawCircle((int)topConnection.X, (int)topConnection.Y, 1.5f, highlightColor);
                Raylib.DrawCircle((int)bottomConnection.X, (int)bottomConnection.Y, 1.5f, highlightColor);
            }
        }

        private void DrawSemiEllipseStage(Vector2 centerPoint, Vector2 topProng, Vector2 bottomProng, float prongDistance, float progress, Color soapColor, Color bubbleColor, Color highlightColor)
        {
            float initialRadius = prongDistance / 3f;
            
            // Make the ellipse grow much larger - closer to the final bubble size
            // Horizontal radius grows moderately
            float horizontalRadius = initialRadius + ((_finalBubbleRadius - initialRadius) * 0.6f * progress);
            
            // Vertical radius grows to nearly the final size (90% of target)
            float maxVerticalGrowth = _finalBubbleRadius * 0.9f - initialRadius;
            float verticalRadius = initialRadius + (maxVerticalGrowth * progress);
            
            Vector2 ellipseCenter = centerPoint + new Vector2(horizontalRadius, 0f);
            
            // Draw the transparent semi-ellipse (right half)
            int segments = 20;
            for (int i = 0; i < segments; i++)
            {
                float angle1 = ((i / (float)segments) - 0.5f) * (float)Math.PI;
                float angle2 = (((i + 1) / (float)segments) - 0.5f) * (float)Math.PI;
                
                Vector2 point1 = ellipseCenter + new Vector2(
                    (float)Math.Cos(angle1) * horizontalRadius,
                    (float)Math.Sin(angle1) * verticalRadius
                );
                Vector2 point2 = ellipseCenter + new Vector2(
                    (float)Math.Cos(angle2) * horizontalRadius,
                    (float)Math.Sin(angle2) * verticalRadius
                );
                
                Raylib.DrawLineEx(point1, point2, 2f, soapColor);
            }
            
            // Draw transparent soap film connections
            Vector2 topConnection = ellipseCenter + new Vector2(0, -verticalRadius);
            Vector2 bottomConnection = ellipseCenter + new Vector2(0, verticalRadius);
            
            Raylib.DrawLineEx(topProng, topConnection, 2f, soapColor);
            Raylib.DrawLineEx(bottomProng, bottomConnection, 2f, soapColor);
            
            // Fill the semi-ellipse with very transparent color as it grows
            if (progress > 0.3f)
            {
                float fillProgress = (progress - 0.3f) / 0.7f;
                Color fillColor = new Color(
                    bubbleColor.R,
                    bubbleColor.G,
                    bubbleColor.B,
                    (byte)(bubbleColor.A * fillProgress * 0.6f) // Even more transparent for interior
                );
                
                // Draw filled semi-ellipse using multiple circles to approximate it
                for (int x = 0; x <= (int)horizontalRadius; x += 3) // Slightly larger steps for less fill
                {
                    float normalizedX = x / horizontalRadius;
                    if (normalizedX <= 1f) // Prevent Math.Acos domain errors
                    {
                        float y = verticalRadius * (float)Math.Sin(Math.Acos(normalizedX));
                        
                        Vector2 fillPoint = ellipseCenter + new Vector2(x, 0);
                        Raylib.DrawRectangle((int)fillPoint.X, (int)(fillPoint.Y - y), 2, (int)(y * 2), fillColor);
                    }
                }
                
                // Add subtle highlights during growth
                if (progress > 0.6f)
                {
                    float highlightRadius = Math.Min(horizontalRadius, verticalRadius) * 0.3f;
                    Raylib.DrawCircleLines((int)(ellipseCenter.X - 6), (int)(ellipseCenter.Y - 4), highlightRadius, highlightColor);
                }
            }
        }

        private void DrawSphereDetachmentStage(Vector2 centerPoint, Vector2 topProng, Vector2 bottomProng, float progress, Color bubbleColor, Color soapColor, Color highlightColor)
        {
            // Calculate the final bubble position (detached from prongs)
            Vector2 finalBubbleCenter = centerPoint + new Vector2(_finalBubbleRadius, 0f);
            
            // Calculate the starting values from the end of stage 2
            float prongDistance = Vector2.Distance(topProng, bottomProng);
            float initialRadius = prongDistance / 3f;
            
            // The ellipse now grows to 90% of final size
            float maxVerticalGrowth = _finalBubbleRadius * 0.9f - initialRadius;
            float maxVerticalRadius = initialRadius + maxVerticalGrowth; // ~31.5f instead of ~19.78f
            float maxHorizontalRadius = initialRadius + ((_finalBubbleRadius - initialRadius) * 0.6f); // ~23.65f
            
            Vector2 startCenter = centerPoint + new Vector2(maxHorizontalRadius, 0f);
            
            Vector2 currentCenter = Vector2.Lerp(startCenter, finalBubbleCenter, progress);
            float currentRadius = maxVerticalRadius + (_finalBubbleRadius - maxVerticalRadius) * progress;
            
            // Draw the transparent sphere
            Raylib.DrawCircle((int)currentCenter.X, (int)currentCenter.Y, currentRadius, bubbleColor);
            Raylib.DrawCircleLines((int)currentCenter.X, (int)currentCenter.Y, currentRadius, soapColor);
            
            // Draw diminishing transparent connections to prongs that disappear as bubble detaches
            float connectionStrength = 1f - progress;
            if (connectionStrength > 0)
            {
                Color fadingSoapColor = new Color(
                    soapColor.R,
                    soapColor.G,
                    soapColor.B,
                    (byte)(soapColor.A * connectionStrength)
                );
                
                // Connection points on the sphere closest to the prongs
                Vector2 topConnectionPoint = currentCenter + Vector2.Normalize(topProng - currentCenter) * currentRadius;
                Vector2 bottomConnectionPoint = currentCenter + Vector2.Normalize(bottomProng - currentCenter) * currentRadius;
                
                Raylib.DrawLineEx(topProng, topConnectionPoint, 2f * connectionStrength, fadingSoapColor);
                Raylib.DrawLineEx(bottomProng, bottomConnectionPoint, 2f * connectionStrength, fadingSoapColor);
            }
            
            // Add realistic soap film highlights during detachment
            if (progress > 0.3f)
            {
                float highlightProgress = (progress - 0.3f) / 0.7f;
                float highlightRadius = currentRadius * 0.7f;
                
                // Main highlight
                Raylib.DrawCircleLines((int)(currentCenter.X - 8), (int)(currentCenter.Y - 8), highlightRadius * 0.3f, 
                    new Color(highlightColor.R, highlightColor.G, highlightColor.B, (byte)(highlightColor.A * highlightProgress)));
                
                // Secondary smaller highlight for realism
                if (progress > 0.6f)
                {
                    Raylib.DrawCircleLines((int)(currentCenter.X - 12), (int)(currentCenter.Y - 5), highlightRadius * 0.15f, 
                        new Color((byte)255, (byte)255, (byte)255, (byte)(40 * highlightProgress)));
                }
            }
        }
    }
}
