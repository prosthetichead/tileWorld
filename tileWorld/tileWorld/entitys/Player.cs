﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace tileWorld
{
    class Player
    {
        public enum state { walking, swimming, idle };
        public state CurrentState = state.idle;

        public Vector2 Position = Vector2.Zero;
        public Vector2 PixelPosition = Vector2.Zero;
        public Vector2 Direction = Vector2.Zero;
        Vector2 MouseMoveToPosition = Vector2.Zero;

        private float speed;
        private float baseSpeed = 1f;
        private double FacingDeg = 0;

        private List<Cell> cellPath;
        
        public int playerSizeHeight = 32;
        public int playerSizeWidth = 32;

        private float playerDamage;

        AnimatedSprite playerSprite;
        World world;
        Pathfinder pathfinder;
        
        public Player(ContentManager Content, World world)
        {
            playerSprite = new AnimatedSprite(Content, "player", playerSizeWidth, playerSizeHeight);
            this.world = world;
            pathfinder = new Pathfinder(world);
        }

        public float playerHP()
        {
            return 200- playerDamage;
        }



        private void updatePosition(GameTime gameTime)
        {
            if (CurrentState == state.walking) //Walking Up.
            {
                speed = 50f + baseSpeed; // tiles per update
                Position += Direction * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
            }

        }

        private void updateAnimation(double FacingDeg, GameTime gameTime)
        {
            int animationFirstFrameNumber = 0;
            int animationLastFrameNumber = 3;
            int animationSpeed_Millisecs = 90;


            if (FacingDeg >= 135 | FacingDeg <= -135) //Facing Up.
            {
                switch(CurrentState)
                {
                    case state.walking:
                        animationFirstFrameNumber = 0;
                        animationLastFrameNumber = 3;
                        break;
                    case state.idle:
                        animationFirstFrameNumber = 0;
                        animationLastFrameNumber = 0;
                        break;
                    default:
                        animationFirstFrameNumber = 0;
                        animationLastFrameNumber = 0;
                        break;
                }
            }
            else if (FacingDeg >= -45 & FacingDeg <= 45) //Facing Down.
            {
                switch (CurrentState)
                {
                    case state.walking:
                        animationFirstFrameNumber = 8;
                        animationLastFrameNumber = 11;
                        break;
                    case state.idle:
                        animationFirstFrameNumber = 9;
                        animationLastFrameNumber = 9;
                        break;
                    default:
                        animationFirstFrameNumber = 9;
                        animationLastFrameNumber = 9;
                        break;
                }
            }
            else if (FacingDeg >= -135 & FacingDeg <= -45) //Facing Left.
            {
                switch (CurrentState)
                {
                    case state.walking:
                        animationFirstFrameNumber = 12;
                        animationLastFrameNumber = 15;
                        break;
                    case state.idle:
                        animationFirstFrameNumber = 12;
                        animationLastFrameNumber = 12;
                        break;
                    default:
                        animationFirstFrameNumber = 12;
                        animationLastFrameNumber = 12;
                        break;
                }
            }
            else if (FacingDeg <= 135 & FacingDeg >= 45) //Facing Right.
            {
                switch (CurrentState)
                {
                    case state.walking:
                        animationFirstFrameNumber = 4;
                        animationLastFrameNumber = 7;
                        break;
                    case state.idle:
                        animationFirstFrameNumber = 4;
                        animationLastFrameNumber = 4;
                        break;
                    default:
                        animationFirstFrameNumber = 4;
                        animationLastFrameNumber = 4;
                        break;
                }
            }

            playerSprite.setAnimation(animationFirstFrameNumber, animationLastFrameNumber, animationSpeed_Millisecs);
            playerSprite.nextFrame(gameTime);
        }

        public void Update(GameTime gameTime, InputHandler input)
        {           
            Vector2 MouseDirection = Vector2.Zero;
            double MouseDeg;
            Vector2 MousePosition = Vector2.Zero;

            //get mouse location
            MousePosition = input.mousePos();
            MousePosition = (MousePosition + Camara.Location);

            MouseDirection = (MousePosition - Position);
            MouseDirection.Normalize();
            MouseDeg = Math.Atan2(MouseDirection.X, MouseDirection.Y);
            MouseDeg = MouseDeg * 180 / Math.PI;

            if (input.mouseLeftClick())
            {
                Direction = MouseDirection;
                MouseMoveToPosition = MousePosition;
                FacingDeg = MouseDeg;
                CurrentState = state.walking;

               cellPath = pathfinder.FindCellPath(Position, MouseMoveToPosition);
                System.Console.WriteLine(world.getCenterOfTile(MousePosition));
                System.Console.WriteLine(MousePosition);
            }

            //is player moving already?
            if (CurrentState == state.walking)
            {
                if (((MouseMoveToPosition.X - Position.X) > -0.5 & (MouseMoveToPosition.X - Position.X) < 0.5) & ((MouseMoveToPosition.Y - Position.Y) > -0.5 & (MouseMoveToPosition.Y - Position.Y) < 0.5))
                {
                    CurrentState = state.idle;
                }                
            }
            if (CurrentState == state.idle)
            {
                FacingDeg = MouseDeg;
            }

            updatePosition(gameTime);
            updateAnimation(FacingDeg, gameTime);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
           PixelPosition.X = Camara.screenResWidth / 2;
           PixelPosition.Y = Camara.screenResHeight / 2;
           
           playerSprite.Draw(spriteBatch, PixelPosition);

            
        }

    }
}