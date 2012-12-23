using System;
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
        public enum state { walkingToPoint, walking, swimming, idle, attacking };
        public state CurrentState = state.idle;

        public Vector2 Position = Vector2.Zero;
        public Vector2 PixelPosition = Vector2.Zero;
        public Vector2 Direction = Vector2.Zero;
        public Vector2 MoveToPos = Vector2.Zero;

        private float speed = 60f;
        private double FacingDeg = 0;

        private List<Cell> cellPath;
        
        public int playerSizeHeight = 32;
        public int playerSizeWidth = 32;

        private float playerDamage;
        private float maxHP = 100;

        AnimatedSprite playerSprite;
        World world;
        Pathfinder pathfinder;
        SpriteFont damageFont;

        private int damage = 0;


        public Player(ContentManager Content, World world)
        {
            playerSprite = new AnimatedSprite(Content, "player", playerSizeWidth, playerSizeHeight);
            this.world = world;
            pathfinder = new Pathfinder(world);
            damageFont = Content.Load<SpriteFont>(@"Fonts/Font-8bitoperator JVE");
        }

        public float playerHP()
        {
            return maxHP - playerDamage;
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
                    case state.walkingToPoint:
                        goto case state.walking;
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
                    case state.walkingToPoint:
                        goto case state.walking;
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
                    case state.walkingToPoint:
                        goto case state.walking;
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
                    case state.walkingToPoint:
                        goto case state.walking;
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

        public bool attacked(int attackRoll)
        {
            Random roll = new Random((int)DateTime.Now.Ticks);
            int Defend = roll.Next(1, 8);
            damage = Math.Max(attackRoll - Defend, 0) ;
            playerDamage = playerDamage + damage;

            if (damage > 0)
            {
                MessageSystem.Instance.Show("" + damage, new Vector2(PixelPosition.X, PixelPosition.Y - 5), 1f, damageFont, 1, Color.Red);
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update(GameTime gameTime, NPC_Manager npcManager, InputHandler input)
        {           
            Vector2 MouseDirection = Vector2.Zero;
            double MouseDeg;
            Vector2 MousePosition = Vector2.Zero;
            NPC npc ;
            

            //get mouse location
            MousePosition = input.mousePos();
            MousePosition = (MousePosition + Camara.Location);

            MouseDirection = (MousePosition - Position);
            MouseDirection.Normalize();
            MouseDeg = Math.Atan2(MouseDirection.X, MouseDirection.Y);
            MouseDeg = MouseDeg * 180 / Math.PI;

            if (input.mouseLeftHold())
            {
                CurrentState = state.walking;
            }

            if (input.mouseLeftClick())
            {
                npc = npcManager.getNPCatPos(MousePosition);
                if (npc != null) // CLICKED ON NPC
                {
                    if (Vector2.Distance(Position, npc.getPosition()) <= 20)
                    {
                        System.Console.WriteLine("ATTACKed");
                    }

                }
                else
                {
                    cellPath = pathfinder.FindCellPath(Position, MousePosition);
                    if (cellPath != null)
                        CurrentState = state.walkingToPoint;
                    else
                        CurrentState = state.idle;
                }
            }



            else if (input.mouseRightClick())
            {
                if (npcManager.getNPCatPos(MousePosition) != null)
                {
                    
                    //check if distance between NPC clicked and player;

                }
            }




            //is player moving already?
            if (CurrentState == state.walkingToPoint)
            {
                MoveToPos = cellPath[0].pixelPositionCenter;
                if (Vector2.Distance(Position, MoveToPos) > 1)
                {
                    Direction = MoveToPos - Position;
                    Direction.Normalize();
                    FacingDeg = Math.Atan2(Direction.X, Direction.Y);
                    FacingDeg = FacingDeg * 180 / Math.PI;
                    Position += Direction * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
                }
                else
                {
                    cellPath[0].color = Color.White;
                    cellPath.RemoveAt(0);                    
                }
                if (cellPath.Count == 0)
                {
                    CurrentState = state.idle;
                }
            }
            if (CurrentState == state.idle)
            {
                FacingDeg = MouseDeg;
            }

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
