using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace tileWorld
{
    public class AnimatedSprite
    {
        //The current position of the Sprite
        // public Vector2 Position = new Vector2(0, 0);
        private Texture2D SpriteTextureMap;
        private int SpriteWidth;
        private int SpriteHeight;
        private int FrameNumberFirst = 0;
        private int FrameNumberLast  = 0;
        private int FrameCurrent = 0;

        Vector2 origin;
       
        private int elapsedTime = 0; //times since Last frame change
        private int FrameTime = 1; // how often to change frames in milliseconds

        //private bool animation = false; //does this sprite animate?
        

        //Load the texture for the sprite using the Content Pipeline
        public AnimatedSprite(ContentManager Content, string theAssetName, int spriteWidth, int spriteHeight)
        {
            SpriteWidth = spriteWidth;
            SpriteHeight = spriteHeight;
            SpriteTextureMap = Content.Load<Texture2D>(theAssetName);
            origin = new Vector2(SpriteWidth / 2, SpriteHeight);
        }

        public AnimatedSprite(ContentManager Content, string theAssetName, int spriteWidth, int spriteHeight, Vector2 origin)
        {
            SpriteWidth = spriteWidth;
            SpriteHeight = spriteHeight;
            SpriteTextureMap = Content.Load<Texture2D>(theAssetName);
            this.origin = origin;
        }


        public void setAnimation(int frameNumberFirst, int frameNumberLast, int frameTimeMilliseconds)
        {
           // animation = true;
            FrameNumberFirst = frameNumberFirst;
            FrameNumberLast = frameNumberLast;
            FrameTime = frameTimeMilliseconds;
            //FrameCurrent = frameNumberFirst;
        }



        public void nextFrame(GameTime gameTime)
        {
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime > FrameTime)
            { //Move to the next frame
                if (FrameCurrent < FrameNumberFirst || FrameCurrent >= FrameNumberLast)
                {
                    FrameCurrent = FrameNumberFirst;
                }
                else
                {
                    FrameCurrent++;
                }
                elapsedTime = 0; //Reset elapsedTime
            }
        }

        /// <summary>
        /// Draw the sprite to the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="pixelPosition">position in pixels to draw the sprite</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 pixelPosition)
        {
            float rotation = 0f;

            int rectangleX = FrameCurrent % (SpriteTextureMap.Width / SpriteWidth);
            int rectangleY = FrameCurrent / (SpriteTextureMap.Width / SpriteWidth);

            Rectangle sRectangle = new Rectangle(rectangleX * SpriteWidth, rectangleY * SpriteHeight, SpriteWidth, SpriteHeight);
            Rectangle dRectangle = new Rectangle((int)pixelPosition.X, (int)pixelPosition.Y, SpriteWidth, SpriteHeight);

            spriteBatch.Draw(SpriteTextureMap, dRectangle, sRectangle, Color.White, rotation, origin,SpriteEffects.None, Camara.calculateDepth(pixelPosition));
            
         //   spriteBatch.Draw(SpriteTexture, pixelPosition, Color.White);
        }



    }
}