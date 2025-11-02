using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerRenderEffects : List<PlayerRenderEffect>, IList<PlayerRenderEffect>, IGameObjectData<Player>
    {
        public bool IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class PlayerRenderEffect
    {
        private Action<RenderDetails, float, float> _action;
        private readonly float _time;
        private readonly float _intensity;
        private float _lifetime = 0;

        public bool IsActive { get; set; } = true;

        public PlayerRenderEffect(Action<RenderDetails, float, float> action, float time, float intensity)
        {
            _action = action;
            _time = time;
            _intensity = intensity;
            _lifetime = 0f;
        }

        public void Apply(RenderDetails renderDetails, float frameTime)
        {
            var pct = _lifetime / _time;
            _action(renderDetails, pct, _intensity);
         
            _lifetime += frameTime;
            if (_lifetime > _time)
                IsActive = false;
        }
    }

    public static class RenderEffectActions
    {
        private static Random _random = new Random();

        /// <summary>
        /// Randomly jiggles the position of the rendered object
        /// </summary>
        public static Action<RenderDetails, float, float> Jiggle => (d, p, i) =>
        {
            d.X += (float)(_random.NextDouble() * i * 2 - i); // Center around original position
            d.Y += (float)(_random.NextDouble() * i * 2 - i);
        };

        /// <summary>
        /// Rotates the object back and forth with easing in the last quarter
        /// Intensity specifies number of rotation cycles within the given time
        /// </summary>
        public static Action<RenderDetails, float, float> Sputter => (d, p, i) =>
        {
            // Calculate how many complete cycles we've gone through
            float cycleProgress = p * i; // p goes 0->1, multiply by number of cycles
            
            // Easing in last quarter (0.75 to 1.0)
            float easeFactor = 1f;
            if (p > 0.75f)
            {
                float easeProgress = (p - 0.75f) / 0.25f; // 0 to 1 in last quarter
                easeFactor = 1f - easeProgress; // Gradually reduce to 0
            }
            
            // Sine wave for smooth back-and-forth rotation
            float rotationAmount = (float)Math.Sin(cycleProgress * Math.PI * 2) * 45f * easeFactor; // ±45 degrees
            d.Rotation += rotationAmount;
        };

        /// <summary>
        /// Cycles through the full rainbow spectrum by directly manipulating RGB values
        /// Intensity specifies the speed of color cycling (number of complete cycles)
        /// </summary>
        public static Action<RenderDetails, float, float> Rainbow => (d, p, i) =>
        {
            float angle = p * i * 360f;
            d.Red = (float)(Math.Sin(angle * Math.PI / 180f) * 127.5f + 127.5f);
            d.Green = (float)(Math.Sin((angle + 120f) * Math.PI / 180f) * 127.5f + 127.5f);
            d.Blue = (float)(Math.Sin((angle + 240f) * Math.PI / 180f) * 127.5f + 127.5f);
        };

        /// <summary>
        /// Applies blue alpha on and off
        /// </summary>
        public static Action<RenderDetails, float, float> Flash(Color color) 
        {
            return (d, p, i) =>
            {
                var val = p * i * 2;

                if (Math.Floor(val) % 2 == 0)
                {
                    d.Red = color.R;
                    d.Green = color.G;
                    d.Blue = color.B;
                }
            };
        }


        /// <summary>
        /// Randomly flickers the object in and out of visibility
        /// Intensity is the chance (0-1) that the object will be hidden on any given frame
        /// </summary>
        public static Action<RenderDetails, float, float> Flicker => (d, p, i) =>
        {
            if (_random.NextDouble() < i)
            {
                d.Alpha = 0; // Hide completely
            }
            // Otherwise leave alpha as-is
        };

        /// <summary>
        /// Inverts the rotation of the object (makes it face the opposite direction)
        /// Intensity is ignored
        /// </summary>
        public static Action<RenderDetails, float, float> Confusion => (d, p, i) =>
        {
            d.Rotation = -d.Rotation; // Negate rotation
        };

        /// <summary>
        /// Scales the object up and down in a heartbeat pattern (beat-beat-pause)
        /// Intensity specifies number of complete heartbeat cycles within the given time
        /// </summary>
        public static Action<RenderDetails, float, float> Heartbeat => (d, p, i) =>
        {
            // Each heartbeat cycle: beat (0.0-0.15), beat (0.15-0.3), pause (0.3-1.0)
            float cycleProgress = (p * Math.Abs(i)) % 1f; // Get position within current cycle
            
            float scale = 1;
            
            if (cycleProgress < 0.15f) // First beat
            {
                float beatProgress = cycleProgress / 0.15f; // 0 to 1
                scale = 1f + (float)Math.Sin(beatProgress * Math.PI) * 0.2f; // Pulse up to 1.2x
            }
            else if (cycleProgress < 0.3f) // Second beat
            {
                float beatProgress = (cycleProgress - 0.15f) / 0.15f; // 0 to 1
                scale = 1f + (float)Math.Sin(beatProgress * Math.PI) * 0.2f; // Pulse up to 1.2x
            }
            // else: pause at normal size (scale = 1f)

            var newWidth = d.Width * scale;
            var widthDiff = newWidth - d.Width;
            var newHeight = d.Height * scale;
            var heightDiff = newHeight - d.Height;

            // Apply scale to width and height
            d.Width += (i < 0 ? -1 : 1) * widthDiff;
            d.Height += (i < 0 ? -1 : 1) * heightDiff;
        };
    }

    public class RenderDetails
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Rotation { get; set; }
        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }
        public float Alpha { get; set; }
    }
}
