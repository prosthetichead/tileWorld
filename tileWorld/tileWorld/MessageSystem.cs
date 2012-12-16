using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tileWorld
{
public class MessageSystem {

    public static readonly MessageSystem Instance = new MessageSystem();

   class Message {
        public SpriteFont Font;
        public string Data;
        public Vector2 Pos;
        public float Duration;
        public float Alpha;
    }

    List<Message> Messages = new List<Message>();

    public void Show(string data, Vector2 pos, float duration, SpriteFont font, int alpha)
    {
        Messages.Add(new Message() { Data = data, Font = font, Pos = pos, Duration = duration, Alpha = alpha});
    }
    public void Update(GameTime gameTime) {  // == (float) gametime.ElapsedTime.TotalSeconds

       for (int i=0; i < Messages.Count; )
       {
           Messages[i].Duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
           if (Messages[i].Duration < 0)
           {
                Messages.RemoveAt(i);
                continue;
           }
           i++;
       }
    }


    public void Draw(SpriteBatch batch)
    {
       batch.Begin();
       for (int i=0; i<Messages.Length; )
       {
            batch.DrawString(Message[i].Font, ....)
       }
       batch.end()
    }
}
}
