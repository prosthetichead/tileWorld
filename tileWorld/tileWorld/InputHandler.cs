using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace tileWorld
{
    class InputHandler
    {
        MouseState mouseState;
        KeyboardState keyState;
        MouseState previousMouseState;
        KeyboardState previousKeyState;

        public bool used = false;

        public float holdTimer = 0;

        Game game;
        

        public InputHandler(Game game)
        {
            this.game = game;

            game.IsMouseVisible = false;
            mouseState = Mouse.GetState();
            keyState = Keyboard.GetState();
        }

        public void Update(GameTime gameTime)
        {
             // New Tick (this bool makes sure an input isnt used twice.)
            if (game.IsActive)
            {
                previousMouseState = mouseState;
                mouseState = Mouse.GetState();

                previousKeyState = keyState;
                keyState = Keyboard.GetState();

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    holdTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if(mouseState.LeftButton == ButtonState.Released)
                    holdTimer = 0f;

            }
            used = false;
        }

        public bool IsMouseInsideWindow()
        {
            return game.GraphicsDevice.Viewport.Bounds.Contains(mouseState.X, mouseState.Y);
        }
        public Vector2 mousePos()
        {
            return new Vector2(mouseState.X, mouseState.Y);
        }

        public bool mouseLeftHold()
        {
            if (!used & IsMouseInsideWindow())
            {
                if (mouseState.LeftButton == ButtonState.Pressed && holdTimer > .5f)
                {
                    used = true;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool mouseLeftClick()
        {
            if (!used & IsMouseInsideWindow())
            {
                if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                {
                    used = true;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool mouseLeftRelease()
        {
            if (mouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
            {
                return false;
            }
        }

        public bool mouseRightClick()
        {
            if (!used & IsMouseInsideWindow())
            {
                if (mouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released)
                {
                    used = true;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool keyBoardKeyPress(Keys key)
        {
            if (!used)
            {
                if (keyState.IsKeyDown(key) & !previousKeyState.IsKeyDown(key))
                {
                    used = true;
                    return true;
                }
                else
                    return false;

            }
            else
                return false;
        }
    }
}
