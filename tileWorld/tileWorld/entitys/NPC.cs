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

     }


    class NPC
    {
        private NPCData npcData;
        public AnimatedSprite NpcSprite;
        private Pathfinder pathfinder;
        private List<Cell> cellPath;
        private Vector2 LastPlayerPos;
        private Vector2 WalkToPos;
        
        public enum state {walkingToPlayer, swimming, idle, returnHome, attacking};
        public state CurrentState = state.idle;

        SpriteFont fontTiny;

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
            npcData.speed = 20;
            npcData.Direction.X = 1; 

            NpcSprite = new AnimatedSprite(Content, textureName, npcHeight, npcWidth);
            fontTiny = Content.Load<SpriteFont>(@"Fonts/Font-PF Arma Five");
            pathfinder = new Pathfinder(world);
            cellPath = new List<Cell>();
        }
        public NPC(ContentManager Content, NPCData npcData)
        {
            this.npcData = npcData;

            NpcSprite = new AnimatedSprite(Content, npcData.TextureName, npcData.NpcHeight, npcData.NpcWidth);
            fontTiny = Content.Load<SpriteFont>(@"Fonts/Font-PF Arma Five");
        }

        public Vector2 getPixelPosition()
        {
            return npcData.PixelPosition;
        }
        public Vector2 getPosition()
        {
            return npcData.Position;
        }

        

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)npcData.Position.X, (int)npcData.Position.Y, npcData.NpcWidth, npcData.NpcHeight);
            }
        }

        public void testBoundingBoxColision(Rectangle rectangle)
        {
            if (rectangle.Intersects(this.BoundingBox))
            {
                //System.Console.WriteLine("INTERSECTS");
                //npcData.Direction =
            }
        }


        public void update(GameTime gameTime, Vector2 playerPos)
        {
             float distanceToPlayer;

             distanceToPlayer = Vector2.Distance(playerPos, npcData.Position);

             if (distanceToPlayer <= npcData.visiblityRange)
             {
                 CurrentState = state.walkingToPlayer; //Change State To walking
             }
             else
                 CurrentState = state.returnHome;
             
            if (distanceToPlayer <= 20)
                 CurrentState = state.attacking; 


            if (CurrentState == state.walkingToPlayer)
            {
                if (cellPath.Count == 0)
                {
                    cellPath = pathfinder.FindCellPath(npcData.Position, playerPos);
                }
                if (Vector2.Distance(cellPath[0].pixelPositionCenter, npcData.Position) > 1)
                {

                    npcData.Direction = cellPath[0].pixelPositionCenter - npcData.Position;
                    npcData.Direction.Normalize();
                }
                else
                    cellPath.RemoveAt(0); 
            }
            
            if (CurrentState == state.returnHome)
            {
                npcData.Direction = npcData.OriginalPosition - npcData.Position;
                if (Math.Abs(npcData.Direction.X) <= 5 | Math.Abs(npcData.Direction.Y) <= 5)
                    CurrentState = state.idle;
                npcData.Direction.Normalize();
            }
         
            
            if (CurrentState == state.idle)
            {
                npcData.Direction = Vector2.Zero;
            }
            if (CurrentState == state.attacking)
            {
                npcData.Direction = Vector2.Zero;
            }

            npcData.Position += npcData.Direction * ((float)gameTime.ElapsedGameTime.TotalSeconds * npcData.speed);
         }

        public void draw(SpriteBatch spriteBatch)
        {
            npcData.PixelPosition.X = (npcData.Position.X);
            npcData.PixelPosition.Y = (npcData.Position.Y);

            npcData.PixelPosition -= Camara.Location;
            //System.Console.WriteLine("X, Y " + PixelPosition.X + "," + PixelPosition.Y);

            NpcSprite.Draw(spriteBatch, npcData.PixelPosition);
            spriteBatch.DrawString(fontTiny, CurrentState.ToString(), new Vector2(npcData.PixelPosition.X + 2, npcData.PixelPosition.Y - 29), Color.Black);
            spriteBatch.DrawString(fontTiny, CurrentState.ToString(), new Vector2(npcData.PixelPosition.X, npcData.PixelPosition.Y - 30), Color.White);

        }
    }
}
