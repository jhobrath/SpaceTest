using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerRotationData : IGameObjectData<Player>
    {
        public float MaxRotationDueToMovement => 10f;
        public float InitialRotation { get; set; } = float.MinValue;
        public Vector2 DirectionalVector => new Vector2(
            (float)_directionalVector.Real,
            (float)_directionalVector.Imaginary);

        private Complex _directionalVector => Complex.Exp(Complex.ImaginaryOne * InitialRotation * MathF.PI / 180f);
    }
}
