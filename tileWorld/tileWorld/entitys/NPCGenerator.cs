using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace tileWorld
{
    class NPCGenerator
    {
        List<NPC> NPCs;
        ContentManager Content;
        World GameWorld;

        public NPCGenerator(ContentManager content, World gameWorld)
        {
            Content = content;
            NPCs = new List<NPC>();
            GameWorld = gameWorld;
        }

        public void GenNPC_atPos(Vector2 pos)
        {
            pos.X = (int)pos.X;
            pos.Y = (int)pos.Y;
            NPCs.Add(new NPC(Content,pos,"NPC-"+pos,"npc",32,32));

        }

        public void update(GameTime gameTime, Vector2 playerPos)
        {
            foreach (NPC npc in NPCs)
            {
                //Cell cell = GameWorld.getCell((int)npc.getTilePosition().X, (int)npc.getTilePosition().Y);
                //Cell[,] cellArray = GameWorld.getCellArray((int)npc.getPosition().X, (int)npc.getPosition().Y);
                npc.update(gameTime, playerPos);
                foreach (NPC checkNpc in NPCs)
                {
                    if (checkNpc != npc) //dont check yourself!
                    {
                        npc.testBoundingBoxColision(checkNpc.BoundingBox);
                    }
                }

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (NPC npc in NPCs)
            {
                npc.draw(spriteBatch);
            }
        }
    
    }

}
