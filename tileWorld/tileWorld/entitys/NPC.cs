using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace tileWorld
{
    [Serializable]
    public struct NPCData
    {
        public Vector2 Position;
        public Vector2 OriginalPosition;
        public Vector2 PixelPosition;
        public Vector2 Direction;


        public string Name;
        public string TextureName;
        
        public int NpcHeight; //pixels
        public int NpcWidth; //pixels
        
        public float speed;
        public int visiblityRange;

        public bool hostile; // hostile NPCs do not attck each other only Non Hostile NPCs and players
        public bool hostileToPlayer; // may still be not hostile but hostile to player. (an attacked vilager for example)
        
        public int level;     

        public int strength; // physical
        public int dexterity; // armor, ranged 
        public int constitution; // HP. level * con = HP
        public int intelligence; // mana. Int * 10 = Mana

        public int maxHP;
        public int damage;
     }


    class NPC
    {
        private NPCData npcData;
        public AnimatedSprite NpcSprite;
        private Pathfinder pathfinder;
        private World world;
        private List<Cell> cellPath;

        private Cell previousCell;
        private Cell currentCell;
        private Cell previousPlayerCell;
        private Cell currentPlayerCell;
        
        public enum state {walkingToPlayer, swimming, idle, returnHome, attacking};
        public state CurrentState = state.idle;

        public bool dead = false;

        private float attackSpeedTimer = 2f;
        private float attackSpeed = 2f;
        private int attackRange = 20;
        Random rand = new Random();
        int randomStep;
        int randomSpeed;


        SpriteFont fontTiny;
        Texture2D debugRec;

        /// <summary>
        /// defualt constructer
        /// </summary>
        /// <param name="tilePosition">starting position of the npc</param>
        /// <param name="name">a name for the pc</param>
        /// <param name="textureName">name of the content this NPC will use</param>
        /// <param name="npcHeight">height in pixels</param>
        /// <param name="npcWidth">width in pixels</param>
        public NPC(ContentManager Content, World world, Vector2 position, string name, string textureName, int npcHeight, int npcWidth)
        {
            npcData.Name = name;
            npcData.NpcHeight = npcHeight;
            npcData.NpcWidth = npcWidth;
            npcData.Position = position;
            npcData.OriginalPosition = npcData.Position;
            npcData.PixelPosition = npcData.Position * 32;
            npcData.visiblityRange = 300;
            npcData.speed = 30;
            npcData.Direction.X = 1;

            npcData.maxHP = 10;

            NpcSprite = new AnimatedSprite(Content, textureName, npcHeight, npcWidth);
            fontTiny = Content.Load<SpriteFont>(@"Fonts/Font-PF Arma Five");
            pathfinder = new Pathfinder(world);
            this.world = world;
            cellPath = new List<Cell>();

            randomStep = rand.Next(-15, 15);
            randomSpeed = rand.Next(0, 5);

            debugRec = Content.Load<Texture2D>(@"debugRec");

        }

        public Vector2 getPixelPosition()
        {
            return npcData.PixelPosition;
        }
        public Vector2 getPosition()
        {
            return npcData.Position;
        }

        public float HP()
        {
            return npcData.maxHP - npcData.damage;
        }
        public bool attacked(int attackRoll)
        {
            Random roll = new Random((int)DateTime.Now.Ticks);
            int Defend = roll.Next(1, 8);
            int damageTaken = Math.Max(attackRoll - Defend, 0);
            npcData.damage = damageTaken + npcData.damage;

            if (damageTaken > 0)
            {
                MessageSystem.Instance.Show("" + damageTaken, new Vector2(npcData.PixelPosition.X, npcData.PixelPosition.Y - 5),1f, fontTiny, 1, Color.Cyan);

                return true;
            }
            else
            {
                return false;
            }
        }
        
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)npcData.PixelPosition.X - npcData.NpcWidth/2, (int)npcData.PixelPosition.Y - npcData.NpcHeight, npcData.NpcWidth, npcData.NpcHeight);
            }
        }

        public void update(GameTime gameTime, Player player)
        {
            if (HP() <= 0)
            {
                dead = true;
            }
            else
            {

                float distanceToPlayer = Vector2.Distance(player.Position, npcData.Position);
                Vector2 walktopos;

                if (distanceToPlayer <= npcData.visiblityRange)
                {
                    CurrentState = state.walkingToPlayer;
                }
                if (distanceToPlayer <= attackRange)
                {
                    CurrentState = state.attacking;    
                }



                if (CurrentState == state.walkingToPlayer)
                {
                    if (world.getCellFromPixelPos(player.Position) != currentPlayerCell)
                    {
                        randomStep = rand.Next(-15, 15);
                        currentPlayerCell = world.getCellFromPixelPos(player.Position);
                        cellPath = pathfinder.FindCellPath(npcData.Position, player.Position);
                        if (cellPath != null)
                            cellPath.RemoveAt(0);
                    }

                    if (cellPath != null && cellPath.Count > 0)
                    {

                        walktopos = new Vector2(cellPath[0].pixelPositionCenter.X + randomStep, cellPath[0].pixelPositionCenter.Y + randomStep);
                        if (Vector2.Distance(walktopos, npcData.Position) > 1)
                        {
                            npcData.Direction = walktopos - npcData.Position;
                            npcData.Direction.Normalize();
                        }
                        else
                            cellPath.RemoveAt(0);

                        npcData.Position += npcData.Direction * ((float)gameTime.ElapsedGameTime.TotalSeconds * (npcData.speed + randomSpeed));
                    }
                }
                if (CurrentState == state.idle)
                {
                    npcData.Direction = Vector2.Zero;
                }
                if (CurrentState == state.attacking)
                {
                    npcData.Direction = Vector2.Zero;
                    attackSpeedTimer = attackSpeedTimer - (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (attackSpeedTimer <= 0)
                    {
                        player.attacked(attackRoll());
                        attackSpeedTimer = attackSpeed;

                    }
                }


            }
        }
        private int attackRoll()
        {
            // CALC A REAL ATTACK ROLL HERE!
            Random ran = new Random();
            int attackRoll = ran.Next(1, 8);            
            return attackRoll;

        }

        public void draw(SpriteBatch spriteBatch)
        {
            npcData.PixelPosition.X = (npcData.Position.X);
            npcData.PixelPosition.Y = (npcData.Position.Y);

            npcData.PixelPosition -= Camara.Location;
            //System.Console.WriteLine("X, Y " + PixelPosition.X + "," + PixelPosition.Y);

           //spriteBatch.Draw(debugRec, BoundingBox, null, Color.Tomato, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            NpcSprite.Draw(spriteBatch, npcData.PixelPosition);
        }
    }
}






//        if (cellPath != null && cellPath[0] == currentCell)
//        {
//            cellPath[0].exitCellintoPath();
//            cellPath.RemoveAt(0); //dont walk to the cell your currenly on
//        }
//        updatePathTime = baseUpdatePathTime;
//    }

//    if (cellPath != null && cellPath.Count != 0)
//    {

//        if (world.getCellFromPixelPos(npcData.Position) == currentCell)
//        {
//            npcData.Direction = cellPath[0].pixelPositionCenter - npcData.Position;
//            npcData.Direction.Normalize();
//        }
//        else
//        {
//            cellPath[0].exitCellintoPath();
//            cellPath.RemoveAt(0);
//        }
//    }
//}

//if (CurrentState == state.returnHome)
//{
//    npcData.Direction = npcData.OriginalPosition - npcData.Position;
//    if (Math.Abs(npcData.Direction.X) <= 5 | Math.Abs(npcData.Direction.Y) <= 5)
//        CurrentState = state.idle;
//    npcData.Direction.Normalize();
//}