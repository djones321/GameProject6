using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject6
{
    public enum AnimationsType
    {
        Spread = 0, 
        Rapid
    }


    public class Animations
    {
        private double animationTimer;
        public int animationFrame;
        private int animationCount;
        public AnimationsType animationType;
        //private GameTime gameTime;

        public Animations()
        {
            animationTimer = 0;
            animationFrame = 0;
            animationCount = 0;
            animationType = AnimationsType.Spread;
        }

        public void NextFrame(double gameTime)
        {
            //this.gameTime = gameTime;

            if (animationType == AnimationsType.Spread && animationCount < 3)
            {
                animationTimer += gameTime;
                SpreadFrame();
            }
            else if (animationType == AnimationsType.Rapid && animationCount < 12)
            {
                animationTimer += gameTime;
                RapidFrame();
            }
            else
            {
                if (animationType == AnimationsType.Spread) animationType = AnimationsType.Rapid;
                else animationType = AnimationsType.Spread;
                animationCount = 0;
                NextFrame(gameTime);
            }
        }

        public void SpreadFrame()
        {
            if (animationTimer > .9f && animationTimer < 1f)
            {
                animationFrame = 0;
            }
            else if (animationTimer > 1.1f)
            {
                animationFrame = 1;
                animationTimer -= 1.1f;
                animationCount++;

            }

        }

        public void RapidFrame()
        {
            if (animationTimer > .1f && animationTimer < .2f)
            {
                animationFrame = 0;
            }
            else if (animationTimer > .3f)
            {
                animationFrame = 1;
                animationTimer -= .3f;
                animationCount++;

            }
        }
    }
}
