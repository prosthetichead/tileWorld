using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace tileWorld
{
    class  NPC_Manager
    {
        List<NPC> NPCs;
        ContentManager Content;
        World GameWorld;

        public NPC_Manager(ContentManager content, World gameWorld)
        {
            Content = content;
            NPCs = new List<NPC>();
            GameWorld = gameWorld;
        }

        public void GenNPC_atPos(Vector2 pos)
        {
            pos.X = (int)pos.X;
            pos.Y = (int)pos.Y;
            NPCs.Add(new NPC(Content,GameWorld, pos,"NPC-"+pos,"npc",32,32));

        }

        public NPC getNPCatPos(Vector2 pos)
        {
            foreach (NPC npc in NPCs)
            {
                if (npc.BoundingBox.Contains((int)pos.X, (int)pos.Y))
                    return npc;
            }
            return null;
        }

        public List<NPC> getNPCsInBox(Rectangle area)
        {
            List<NPC> npcList = new List<NPC>();
            foreach (NPC npc in NPCs)
            {
                if (npc.testBoundingBoxColision(area))
                {
                    npcList.Add(npc);
                }
            }
            return npcList;
        }

        public void update(GameTime gameTime, Player player)
        {
            NPC npc;
            for (int i = 0; i < NPCs.Count; i++)
            {
                npc = NPCs[i];
                npc.update(gameTime, player);
                if (npc.dead)
                    NPCs.Remove(npc);
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
