using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class PlankedEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/Statuses/planked.png";
        public override int MaxCount => 1;
        protected override float Duration => 7f;

        private readonly SpriteDecoration _decoration;
        private readonly SpriteDecoration _underDeco;

        public PlankedEffect()
        {
            _decoration = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipBody_Planked.png"), Vector2.Zero, new Vector2(160, 160));
            _underDeco = new SpriteDecoration(new SpriteWrapper("Sprites/Projectiles/wooden_plank_2_planked.png"), new Vector2(0f, 0f), new Vector2(30,150)) { FollowRotation = true };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Decorations["Planked"] = _decoration;
            //modifiers.Decorations["PlankedWood"] = _underDeco;
        }

        private float _lifetime = 0f;
        public override void OnUpdate(float frameTime)
        {
            _lifetime += frameTime;
            base.OnUpdate(frameTime);
        }
    }
}
