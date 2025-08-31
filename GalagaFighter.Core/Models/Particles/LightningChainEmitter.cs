using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Particles
{
    /// <summary>
    /// Creates connected lightning bolt chains that form continuous electrical discharge
    /// </summary>
    public class LightningChainEmitter : GameObject
    {
        private readonly IObjectService _objectService;
        private readonly List<LightningChain> _chains;
        private readonly LightningChainConfig _config;
        private float _emissionTimer;
        private bool _isEmitting;

        public bool IsEmitting => _isEmitting;
        public int ActiveChainCount => _chains.Count;

        private Vector2 _lastCenter;

        public LightningChainEmitter(Guid owner, IObjectService objectService, Vector2 position, 
                                   LightningChainConfig config) 
            : base(owner, null, position, Vector2.One, Vector2.Zero)
        {
            _objectService = objectService;
            _config = config ?? new LightningChainConfig();
            _chains = new List<LightningChain>();
            _emissionTimer = 0f;
            _isEmitting = _config.EmitOnStart;
            SetDrawPriority(0.4);
            _lastCenter = position;
        }

        public void MoveTo(float x, float y)
        {
            var newCenter = new Vector2(x, y);
            var offset = newCenter - _lastCenter;
            _lastCenter = newCenter;
            // Move all chains and their segments
            foreach (var chain in _chains)
            {
                chain.MoveAllSegments(offset);
            }
        }

        public override void Update(Game game)
        {
            float frameTime = Raylib.GetFrameTime();

            // Always follow the owning projectile's position
            MoveTo(Center.X, Center.Y);

            // Emit new chains
            if (_isEmitting)
            {
                _emissionTimer += frameTime;
                float emissionInterval = 1f / _config.EmissionRate;
                
                while (_emissionTimer >= emissionInterval && _chains.Count < _config.MaxChains)
                {
                    EmitLightningChain();
                    _emissionTimer -= emissionInterval;
                }
            }

            // Update existing chains
            for (int i = _chains.Count - 1; i >= 0; i--)
            {
                _chains[i].Update(frameTime);
                if (!_chains[i].IsActive)
                {
                    _chains[i].Cleanup(_objectService);
                    _chains.RemoveAt(i);
                }
            }
        }

        private void EmitLightningChain()
        {
            // Create a new lightning chain starting from this emitter
            var chain = new LightningChain(Id, _objectService, Center, _config);
            _chains.Add(chain);
        }

        public override void Draw()
        {
            // Chains handle their own drawing through their segments
        }

        public void StartEmission() => _isEmitting = true;
        public void StopEmission() => _isEmitting = false;
        
        public void ClearChains()
        {
            foreach (var chain in _chains)
            {
                chain.Cleanup(_objectService);
            }
            _chains.Clear();
        }
    }

    /// <summary>
    /// Configuration for lightning chain behavior
    /// </summary>
    public class LightningChainConfig
    {
        public float EmissionRate { get; set; } = 10f; // Chains per second
        public int MaxChains { get; set; } = 3; // Max simultaneous chains
        public int ChainLength { get; set; } = 5; // Number of segments per chain
        public float SegmentLength { get; set; } = 32f; // Length of each segment (sprite size)
        public float ChainLifetime { get; set; } = 0.3f; // How long each chain lasts
        public float BranchChance { get; set; } = 0.3f; // Chance to branch (30%)
        public Vector2 ChainDirection { get; set; } = new Vector2(1, 0); // Base direction
        public Color StartColor { get; set; } = Color.White;
        public Color EndColor { get; set; } = Color.DarkBlue;
        public bool EmitOnStart { get; set; } = true;
        public List<string> LightningSprites { get; set; } = new() { "lightning_1", "lightning_2" };
    }

    /// <summary>
    /// A single lightning chain made of connected segments
    /// </summary>
    public class LightningChain
    {
        private readonly List<Particle> _segments;
        private readonly LightningChainConfig _config;
        private float _lifetime;
        private bool _isActive;

        public bool IsActive => _isActive;

        public LightningChain(Guid owner, IObjectService objectService, Vector2 startPosition, 
                            LightningChainConfig config)
        {
            _segments = new List<Particle>();
            _config = config;
            _lifetime = 0f;
            _isActive = true;

            CreateConnectedSegments(owner, objectService, startPosition);
        }

        private void CreateConnectedSegments(Guid owner, IObjectService objectService, Vector2 startPosition)
        {
            // Give each chain a random starting direction for variety
            float randomStartAngle = Game.Random.NextSingle() * 2f * (float)Math.PI; // 0 to 360 degrees
            CreateLightningBranch(owner, objectService, startPosition, randomStartAngle, _config.ChainLength);
        }

        private void CreateLightningBranch(Guid owner, IObjectService objectService, Vector2 startPosition, 
                                         float currentAngle, int remainingSegments)
        {
            if (remainingSegments <= 0) return;

            Vector2 currentPosition = startPosition;
            
            for (int i = 0; i < remainingSegments; i++)
            {
                // Decide whether to branch or continue straight
                bool shouldBranch = Game.Random.NextSingle() < _config.BranchChance && i > 0; // Don't branch on first segment
                
                string spriteToUse;
                float rotation;
                bool createSecondBranch = false;
                
                if (shouldBranch)
                {
                    // Use lightning_2 for branching
                    spriteToUse = "lightning_2";
                    rotation = currentAngle;
                    createSecondBranch = true;
                }
                else
                {
                    // Use lightning_1 for straight continuation
                    spriteToUse = "lightning_1";
                    rotation = currentAngle;
                }
                
                // Create the lightning segment
                var sprite = new SpriteWrapper($"Sprites/Particles/{spriteToUse}.png", _config.StartColor);
                
                var segment = new Particle(
                    owner,
                    sprite,
                    currentPosition,
                    new Vector2(_config.SegmentLength, _config.SegmentLength),
                    Vector2.Zero,
                    _config.ChainLifetime,
                    _config.StartColor,
                    _config.EndColor,
                    _config.SegmentLength,
                    _config.SegmentLength
                )
                {
                    Rotation = rotation * 180f / (float)Math.PI
                };

                _segments.Add(segment);
                objectService.AddGameObject(segment);
                
                // Calculate next position
                Vector2 direction = new Vector2((float)Math.Cos(currentAngle), (float)Math.Sin(currentAngle));
                currentPosition += direction * _config.SegmentLength;
                
                // If we used lightning_2, create a new branch
                if (createSecondBranch)
                {
                    // Create a perpendicular branch (90 degrees up or down randomly)
                    float branchAngle = currentAngle + (Game.Random.NextSingle() > 0.5f ? 1f : -1f) * (float)Math.PI / 2f;
                    Vector2 branchDirection = new Vector2((float)Math.Cos(branchAngle), (float)Math.Sin(branchAngle));
                    Vector2 branchStartPos = currentPosition - direction * (_config.SegmentLength * 0.5f); // Start branch from middle of current segment
                    
                    // Create recursive branch with remaining segments
                    CreateLightningBranch(owner, objectService, branchStartPos, branchAngle, remainingSegments - i - 1);
                    break; // End main chain when we branch
                }
                
                // Add small random angle variation for natural zigzag
                currentAngle += (Game.Random.NextSingle() - 0.5f) * 0.2f; // ±0.1 radians (~6 degrees)
            }
        }

        public void Update(float frameTime)
        {
            _lifetime += frameTime;
            
            if (_lifetime >= _config.ChainLifetime)
            {
                _isActive = false;
            }
        }

        public void Cleanup(IObjectService objectService)
        {
            foreach (var segment in _segments)
            {
                segment.IsActive = false;
                objectService.RemoveGameObject(segment);
            }
            _segments.Clear();
        }

        public void MoveAllSegments(Vector2 offset)
        {
            foreach (var segment in _segments)
            {
                segment.Move(offset.X, offset.Y);
            }
        }
    }
}