using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace tileWorld
{
    class Hud
    {
        private Texture2D curser;

        private Texture2D smallPlayerInfoBG;
        Rectangle smallPlayerInfoBG_R;
        private Texture2D largePlayerInfoBG;
        Rectangle largePlayerInfoBG_R;

        bool displayBigPlayerInfo = false;
        bool hoverSmallPlayerInfo = false;

        SpriteFont smallFont;

        GraphicsDevice graphicsDevice;
        Player player;

        Vector2 curserPos;
        Color curserColor;

        public Hud(ContentManager Content, GraphicsDevice graphicsDevice, Player player)
        {
            this.graphicsDevice = graphicsDevice;
            this.player = player;

            smallFont = Content.Load<SpriteFont>(@"Fonts/Font-PF Ronda Seven");

            curser = Content.Load<Texture2D>("HUD/curser");

            smallPlayerInfoBG = Content.Load<Texture2D>("HUD/smallPlayerInfoBG");
            smallPlayerInfoBG_R = new Rectangle(5, graphicsDevice.Viewport.Height - smallPlayerInfoBG.Height - 5, smallPlayerInfoBG.Width, smallPlayerInfoBG.Height);

            largePlayerInfoBG = Content.Load<Texture2D>("HUD/largePlayerInfoBG");
            largePlayerInfoBG_R = new Rectangle(5, graphicsDevice.Viewport.Height - smallPlayerInfoBG.Height - 10 - largePlayerInfoBG.Height, largePlayerInfoBG.Width, largePlayerInfoBG.Height);

        }

        public void Update(InputHandler input, NPC_Manager npcManger)
        {
            
            curserPos = input.mousePos();
            Vector2 curserWorldPos = curserPos + Camara.Location;
            hoverSmallPlayerInfo = false;
            
            NPC npc = npcManger.getNPCatPos(curserWorldPos);
            if (npc != null)
                curserColor = Color.OrangeRed;
            else
                curserColor = Color.BlueViolet;
            
            //hover on SmallPlayerInfo
            if (smallPlayerInfoBG_R.Contains((int)input.mousePos().X, (int)input.mousePos().Y))
            {
                hoverSmallPlayerInfo = true;
                if (input.mouseLeftClick()) //click on small player info
                    displayBigPlayerInfo = !displayBigPlayerInfo;
            }
            else
            {

            }

           
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(smallPlayerInfoBG, smallPlayerInfoBG_R, Color.White);
            if (hoverSmallPlayerInfo)
                spriteBatch.DrawString(smallFont, " "+ player.playerHP(), new Vector2(smallPlayerInfoBG_R.Location.X+10,smallPlayerInfoBG_R.Location.Y), Color.White);
            if(displayBigPlayerInfo)
                spriteBatch.Draw(largePlayerInfoBG, largePlayerInfoBG_R, Color.White);

            spriteBatch.Draw(curser, curserPos, curserColor);
        }
    }
}
