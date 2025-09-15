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
        private int _animationFrames = 10;
        private float _finalBubbleRadius = 35f;

        public PoisonProjectile(IProjectileController controller, Player owner, SpriteWrapper sprite, Vector2 initialPosition, PlayerProjectile modifiers) 
            : base(controller, owner, sprite, initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            _originalPosition = initialPosition;
        }

        private float _lifeTime = 0f;
        public override void Update(Game game)
        {
            _lifeTime += Raylib.GetFrameTime();
            base.Update(game);
        }

        public override void Draw()
        {
            //Poison projectile is a bubble filled with poison gas
            //The space ship for this power up contains two prongs at its tip that dispense 
            //  poison soap to form the bubbles
            //The two prong tips are at _originalPosition + (-12.78, -9.89) and _originalPosition + (-12.78, 9.89). 
            //We need an animation that draws the frames of the bubble forming from the prong tips
            //Once the bubble is formed, I will fade in an actual png sprite to replace it
            //The bubble should take .35 seconds to form over about 8-12 frames.
            //Do not animate the poison inside the bubble, only draw the bubble

            // Calculate animation progress (0.0 to 1.0)
            float animationProgress = Math.Clamp(_lifeTime / _bubbleFormationTime, 0f, 1f);
            
            // Get prong positions
            Vector2 topProng = _originalPosition + new Vector2(-5f, -9.89f);
            Vector2 bottomProng = _originalPosition + new Vector2(-5, 9.89f);
            
            // Colors for the bubble
            Color bubbleColor = new Color((byte)100, (byte)255, (byte)100, (byte)180); // Translucent green
            Color soapColor = new Color((byte)200, (byte)255, (byte)200, (byte)255);   // Bright soap green

            if (animationProgress < 1f)
            {
                // During formation animation
                DrawBubbleFormation(topProng, bottomProng, animationProgress, bubbleColor, soapColor);
            }
            else
            {
                // Fully formed bubble - draw complete circle
                // Position bubble so its leftmost point is at the center between prongs
                Vector2 prongCenterPoint = (topProng + bottomProng) / 2f;
                Vector2 bubbleCenter = prongCenterPoint + new Vector2(_finalBubbleRadius, 0f);
                
                Raylib.DrawCircle((int)bubbleCenter.X, (int)bubbleCenter.Y, _finalBubbleRadius, bubbleColor);
                Raylib.DrawCircleLines((int)bubbleCenter.X, (int)bubbleCenter.Y, _finalBubbleRadius, soapColor);
                
                // Draw some soap film highlights for realism
                float highlightRadius = _finalBubbleRadius * 0.7f;
                Raylib.DrawCircleLines((int)(bubbleCenter.X - 8), (int)(bubbleCenter.Y - 8), highlightRadius * 0.3f, 
                    new Color((byte)255, (byte)255, (byte)255, (byte)100));
            }
        }

        private void DrawBubbleFormation(Vector2 topProng, Vector2 bottomProng, float progress, Color bubbleColor, Color soapColor)
        {
            // Calculate center point between prongs
            Vector2 centerPoint = (topProng + bottomProng) / 2f;
            
            // Early stage (0.0 - 0.3): Draw initial soap streams from prongs
            if (progress <= 0.3f)
            {
                float streamProgress = progress / 0.3f;
                float streamLength = streamProgress * 20f;
                
                // Draw soap streams extending from prongs toward center
                Vector2 topDirection = Vector2.Normalize(centerPoint - topProng);
                Vector2 bottomDirection = Vector2.Normalize(centerPoint - bottomProng);
                
                Vector2 topStreamEnd = topProng + topDirection * streamLength;
                Vector2 bottomStreamEnd = bottomProng + bottomDirection * streamLength;
                
                Raylib.DrawLineEx(topProng, topStreamEnd, 3f, soapColor);
                Raylib.DrawLineEx(bottomProng, bottomStreamEnd, 3f, soapColor);
                
                // Add some droplets at the ends
                Raylib.DrawCircle((int)topStreamEnd.X, (int)topStreamEnd.Y, 2f, soapColor);
                Raylib.DrawCircle((int)bottomStreamEnd.X, (int)bottomStreamEnd.Y, 2f, soapColor);
            }
            // Middle stage (0.3 - 0.7): Streams connect and start forming arc
            else if (progress <= 0.7f)
            {
                float arcProgress = (progress - 0.3f) / 0.4f;
                
                // Draw connected soap film as partial arcs
                float arcRadius = _finalBubbleRadius * arcProgress;
                float arcAngle = 180f * arcProgress;
                
                // Calculate bubble center - leftmost point should be at centerPoint
                Vector2 bubbleCenter = centerPoint + new Vector2(_finalBubbleRadius, 0f);
                
                // Draw partial arcs forming the bubble outline
                for (int i = 0; i < (int)(arcAngle / 10f); i++)
                {
                    float angle = (i * 10f) * (float)Math.PI / 180f;
                    float nextAngle = ((i + 1) * 10f) * (float)Math.PI / 180f;
                    
                    Vector2 point1 = bubbleCenter + new Vector2(
                        (float)Math.Cos(angle) * arcRadius,
                        (float)Math.Sin(angle) * arcRadius
                    );
                    Vector2 point2 = bubbleCenter + new Vector2(
                        (float)Math.Cos(nextAngle) * arcRadius,
                        (float)Math.Sin(nextAngle) * arcRadius
                    );
                    
                    Raylib.DrawLineEx(point1, point2, 2f, soapColor);
                }
                
                // Still show connection to prongs
                Raylib.DrawLineEx(topProng, bubbleCenter + new Vector2(-arcRadius, 0), 2f, soapColor);
                Raylib.DrawLineEx(bottomProng, bubbleCenter + new Vector2(-arcRadius, 0), 2f, soapColor);
            }
            // Final stage (0.7 - 1.0): Complete bubble formation
            else
            {
                float finalProgress = (progress - 0.7f) / 0.3f;
                Vector2 bubbleCenter = centerPoint + new Vector2(_finalBubbleRadius, 0f);
                float currentRadius = _finalBubbleRadius * finalProgress;
                
                // Draw the forming bubble with increasing opacity
                Color currentBubbleColor = new Color(
                    bubbleColor.R, 
                    bubbleColor.G, 
                    bubbleColor.B, 
                    (byte)(bubbleColor.A * finalProgress)
                );
                
                // Draw filled circle for bubble interior
                Raylib.DrawCircle((int)bubbleCenter.X, (int)bubbleCenter.Y, currentRadius, currentBubbleColor);
                
                // Draw bubble outline
                Raylib.DrawCircleLines((int)bubbleCenter.X, (int)bubbleCenter.Y, currentRadius, soapColor);
                
                // Add forming highlights
                if (finalProgress > 0.5f)
                {
                    float highlightRadius = currentRadius * 0.7f;
                    Raylib.DrawCircleLines((int)(bubbleCenter.X - 8), (int)(bubbleCenter.Y - 8), highlightRadius * 0.3f, 
                        new Color((byte)255, (byte)255, (byte)255, (byte)(100 * finalProgress)));
                }
            }
        }
    }
}
