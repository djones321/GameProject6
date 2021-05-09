using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameProject6.Collisions;

namespace GameProject6
{
    class PenguinKing
    {
        public Vector2 position;

        //private Vector2 playerPosition;

        private Texture2D texture;

        private BoundingRectangle headBounds;

        //public int animationFrame = 1;

        public double rotation;

        public bool Killed;

        //private double animationTimer;

        public Color color = Color.White;

        public Animations animations;

        private bool left;

        private bool flipped;


        public BoundingRectangle HeadBounds => headBounds;

        /// <summary>
        /// Creates a new coin sprite
        /// </summary>
        /// <param name="position">The position of the sprite in the game</param>
        public PenguinKing(Vector2 position)
        {
            this.position = position;
            this.headBounds = new BoundingRectangle(new Vector2(position.X, position.Y-60), 48, 224);
            animations = new Animations();

            left = true;//facing left
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("PenguinKing");
            
        }

        public void Update(Vector2 playerP, float width)
        {
            //position.X = Lerp(position.X, playerP.X, .005f);
            //position.Y = Lerp(position.Y, playerP.Y-32, .005f);
            //position = Vector2.Lerp(position, playerP, 50);

            //rotation = (Math.Atan2((double)playerP.Y - position.Y, (double)playerP.X - position.X));
            rotation = Math.Atan2((double)playerP.Y - position.Y, (double)playerP.X - position.X) + Math.PI;
            color = Color.White;

            if (rotation > 1.4 && rotation < 4.71) flipped = true;
            else flipped = false;

            if (left)
            {
                if (position.X > 0)
                {
                    position.X -= 2;
                    headBounds.X = position.X;
                }
                else left = false;
            }
            else
            {
                if(position.X < width)
                {
                    position.X += 2;
                    headBounds.X = position.X;
                }
                else left = true;
            }
            //position += new Vector2((float)Math.Cos(rotation) / 2, (float)Math.Sin(rotation) / 2);
            //headBounds.Center = position;
        }


        


        /// <summary>
        /// Draws the animated sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //if (Killed) return;
            /*animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if(animationTimer > 1.1f && animationTimer < 1.3f)
            {
                animationFrame = 0;
            }
            else if (animationTimer > 1.5f)
            {
                animationFrame = 1;
                animationTimer -= 1.5f;

            }*/

            animations.NextFrame(gameTime.ElapsedGameTime.TotalSeconds);

            //var source = new Rectangle(animationFrame * 16, 0, 16, 16);


            SpriteEffects headFlip = (flipped) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            SpriteEffects bodyFlip = (flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, new Vector2(position.X-((flipped)? 32 : 16), position.Y), new Rectangle(0, 64, 128, 256), color, 0, new Vector2(32, 32), 1f, bodyFlip, 0);
            spriteBatch.Draw(texture, new Vector2(position.X, position.Y), new Rectangle(animations.animationFrame * 64, 0, 64, 64), color, (float)rotation, new Vector2(32, 32), 1.5f, headFlip, 0);
            

        }
    }
}

