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
        public Color Colour;
        public Vector2 Origin;
        public float Rotation;
        public float Scale;

    }

    List<Message> Messages = new List<Message>();

    public void Show(string data, Vector2 pos, float duration, SpriteFont font, int alpha, Color color)
    {
        Messages.Add(new Message() {
            Data = data,
            Font = font,
            Pos = pos,
            Duration = duration,
            Alpha = alpha,
            Colour = color,
            Origin=Vector2.Zero,
            Rotation= 0f,
            Scale = 1f

        });
    }
    public void Update(GameTime gameTime) {  // == (float) gametime.ElapsedTime.TotalSeconds

       for(int i=0; i < Messages.Count(); i++)
       {
            Messages[i].Duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            Messages[i].Pos.Y -= .5f;
          if (Messages[i].Duration < 0)
           {
               Messages.RemoveAt(i);
           }
       }
    }


    public void Draw(SpriteBatch batch)
    {
        Color color;
        foreach(Message M in Messages)
        {
            color = M.Colour;
             batch.DrawString(M.Font, M.Data, M.Pos, color,M.Rotation,M.Origin,M.Scale,SpriteEffects.None,1f);
             batch.DrawString(M.Font, M.Data, new Vector2(M.Pos.X + 1, M.Pos.Y + 1), Color.Black, M.Rotation, M.Origin, M.Scale, SpriteEffects.None, .9f);
           
        }
       
    }
}
}
