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
        public enum State {movingToPoint, freeMoving, idle, meleeAttack};
        public State state = State.idle;
      
        public Vector2 Position = Vector2.Zero;
        public Vector2 PixelPosition = Vector2.Zero;
        public Vector2 Direction = Vector2.Zero;
        public Vector2 MoveToPos = Vector2.Zero;

        private float speed = 60f;
        private float attackTimer = .5f;
        private float attackTimerBase = .5f;
        private double FacingDeg = 0;

        private List<Cell> cellPath;
        
        public int Height = 32;
        public int Width = 32;

        private float playerDamage;
        private float maxHP = 100;

        AnimatedSprite playerSprite;
        AnimatedSprite playerMeleeAttackSprite;
        World world;
        Pathfinder pathfinder;
        SpriteFont damageFont;
        Texture2D debugRec; 

        private int damage = 0;


        public Player(ContentManager Content, World world)
        {
            playerSprite = new AnimatedSprite(Content, "player", Width, Height);
            playerMeleeAttackSprite = new AnimatedSprite(Content, "player", Width*2, Height*2, new Vector2(Width, Height + (Height/2)));
            
            
            this.world = world;
            pathfinder = new Pathfinder(world);
            damageFont = Content.Load<SpriteFont>(@"Fonts/Font-8bitoperator JVE");
            debugRec = Content.Load<Texture2D>(@"debugRec");
        }

        public float playerHP()
        {
            return maxHP - playerDamage;
        }

        private void updateAnimation(GameTime gameTime)
        {
            int animationFirstFrameNumber=0;
            int animationLastFrameNumber=0;
            int animationSpeed_Millisecs = 90;


            if (FacingDeg >= 135 | FacingDeg <= -135) //Facing Up.
            {
                switch(state)
                {
                    case State.meleeAttack:
                        animationFirstFrameNumber = 10;
                        animationLastFrameNumber = 12;
                        break;
                    case State.movingToPoint:
                        animationFirstFrameNumber = 0;
                        animationLastFrameNumber = 3;
                        break;
                    case State.freeMoving:
                        goto case State.movingToPoint;
                    default:
                        animationFirstFrameNumber = 1;
                        animationLastFrameNumber = 1;
                        break;
                }
            }
            else if (FacingDeg >= -45 & FacingDeg <= 45) //Facing Down.
            {
                switch (state)
                {
                    case State.meleeAttack:
                        animationFirstFrameNumber = 20;
                        animationLastFrameNumber = 22;
                        break;
                    case State.movingToPoint:
                        animationFirstFrameNumber = 20;
                        animationLastFrameNumber = 23;
                        break;
                    case State.freeMoving:
                        goto case State.movingToPoint;
                    default:
                        animationFirstFrameNumber = 21;
                        animationLastFrameNumber = 21;
                        break;
                }
            }
            else if (FacingDeg >= -135 & FacingDeg <= -45) //Facing Left.
            {
                switch (state)
                {
                    case State.meleeAttack:
                        animationFirstFrameNumber = 25;
                        animationLastFrameNumber = 27;
                        break;
                    case State.movingToPoint:
                        animationFirstFrameNumber = 30;
                        animationLastFrameNumber = 33;
                        break;
                    case State.freeMoving:
                        goto case State.movingToPoint;
                    default:
                        animationFirstFrameNumber = 31;
                        animationLastFrameNumber = 31;
                        break;
                }
            }
            else if (FacingDeg <= 135 & FacingDeg >= 45) //Facing Right.
            {
                switch (state)
                {
                    case State.meleeAttack:
                        animationFirstFrameNumber = 15;
                        animationLastFrameNumber = 17;
                        break;
                    case State.movingToPoint:
                        animationFirstFrameNumber = 10;
                        animationLastFrameNumber = 13;
                        break;
                    case State.freeMoving:
                        goto case State.movingToPoint;
                    default:
                        animationFirstFrameNumber = 11;
                        animationLastFrameNumber = 11;
                        break;
                }
            }


            if (state == State.meleeAttack)
            {
                playerMeleeAttackSprite.setAnimation(animationFirstFrameNumber, animationLastFrameNumber, animationSpeed_Millisecs);
                playerMeleeAttackSprite.nextFrame(gameTime);
            }
            else
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
                state = State.freeMoving;
            }
            else if (input.mouseLeftClick())
            {
                npc = npcManager.getNPCatPos(MousePosition);
                if (npc != null) // CLICKED ON NPC
                {
                    if (Vector2.Distance(Position, npc.getPosition()) <= 20)
                    {
                        state = State.meleeAttack;
                    }

                }
                else
                {
                    cellPath = pathfinder.FindCellPath(Position, MousePosition);
                    if (cellPath != null)
                        state = State.movingToPoint;
                    else
                        state = State.idle;
                }
            }
            else if (input.mouseRightClick())
            {
                state = State.meleeAttack;
            }   
            else if (input.mouseLeftRelease() && state == State.freeMoving)
            {
                state = State.idle;
            }


            if (state == State.freeMoving)
            {
                Direction = MouseDirection;
                FacingDeg = MouseDeg;
                Position += Direction * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
            }
            else if (state == State.meleeAttack)
            {
                if (attackTimer == attackTimerBase)
                {
                    FacingDeg = MouseDeg;
                    meleeAttack(FacingDeg, npcManager);
                    System.Console.WriteLine("ATTACK");
                }
                
                attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (attackTimer <= 0)
                {
                    state = State.idle;
                    attackTimer = attackTimerBase;
                }
            }
            else if (state == State.movingToPoint)
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
                    state = State.idle;
                }
            }
            else if (state == State.idle)
            {
                FacingDeg = MouseDeg;
            }

            updateAnimation(gameTime);
        }

        private void meleeAttack(double FacingDeg, NPC_Manager npcManager)
        {
            Rectangle attackArea;
            List<NPC> npcList;
            //workout box attack area
            attackArea = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            //check What is in the box attack area
            npcList = npcManager.getNPCsInBox(attackArea);
            foreach (NPC npc in npcList)
            {
                npc.attacked(5);
            }
            //damage what is in the box attack area

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PixelPosition.X = Camara.screenResWidth / 2;
            PixelPosition.Y = Camara.screenResHeight / 2;

            spriteBatch.Draw(debugRec, new Rectangle((int)PixelPosition.X - Width, (int)PixelPosition.Y - (Height / 2) + 5, Width + Width, Height - (Height / 4)), null, Color.Tomato, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            if(state == State.meleeAttack)
                playerMeleeAttackSprite.Draw(spriteBatch, PixelPosition);
            else
                playerSprite.Draw(spriteBatch, PixelPosition);
        }

    }
}
