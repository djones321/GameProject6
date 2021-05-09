using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GameProject6
{
    public class TrailParticleSystem : ParticleSystem
    {
        Vector2 velocity;

        public TrailParticleSystem(Game game, int maxExplosions) : base(game, maxExplosions * 25) { }

        protected override void InitializeConstants()
        {
            textureFilename = "drop";

            minNumParticles = 20;
            maxNumParticles = 25;

            blendState = BlendState.AlphaBlend;
            DrawOrder = AdditiveBlendDrawOrder;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            velocity.Y = 20; //RandomHelper.NextDirection() * RandomHelper.NextFloat(1, 25);

            var lifetime = RandomHelper.NextFloat(0.5f, 1.0f);

            var acceleration = new Vector2(0, 2);

            var rotation = 0; // RandomHelper.NextFloat(0, MathHelper.TwoPi);

            var angularVelocity = RandomHelper.NextFloat(MathHelper.PiOver4, MathHelper.PiOver4);

            p.Initialize(where, velocity, acceleration, lifetime: lifetime, rotation: rotation, angularVelocity: angularVelocity);

        }

        protected override void UpdateParticle(ref Particle particle, float dt)
        {
            base.UpdateParticle(ref particle, dt);

            float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;

            float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);

            particle.Color = Color.Blue * alpha;

            particle.Scale = .15f + .05f * normalizedLifetime;

            
        }

        public void PlaceTrail(Vector2 where) => AddParticles(where);
    }
}
