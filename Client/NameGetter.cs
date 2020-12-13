using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    class NameGetter:Component
    {
        private SpriteFont font;
        private string sceneName;
        private ClientManager clientManager;
        private Texture2D background;
        private Color textColor;

        private string nameInput;
        private SpriteBatch spriteBatch;
        private List<Keys> oldKeys;

        private const int cMaxLength = 10;
        private const int cAsciiOffset = 32;

        public NameGetter(GraphicsDevice graphicsDevice, SpriteFont font, string sceneName, ClientManager clientManager, Texture2D background, Color textColor)
        {
            this.font = font;
            this.sceneName = sceneName;
            this.clientManager = clientManager;
            this.background = background;
            this.textColor = textColor;

            nameInput = "";
            spriteBatch = new SpriteBatch(graphicsDevice);
            oldKeys = new List<Keys>();
        }

        public override void Update(float deltaTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] keys = keyboardState.GetPressedKeys();

            foreach (Keys key in keys)
            {
                if (!oldKeys.Contains(key))
                {
                    if (key >= Keys.A && key <= Keys.Z && nameInput.Length <= cMaxLength)
                    {
                        char letter;
                        if (!keyboardState.CapsLock && !keyboardState.IsKeyDown(Keys.LeftShift))
                            letter = (char)((int)key + cAsciiOffset); //If player isn't holding shift, or the capslock isn't down, add 32 as Keys.A maps to ascii A, and A->a = +32
                        else
                            letter = (char)key;

                        nameInput += letter;
                    }
                    if (key == Keys.Back && nameInput.Length > 0)
                        nameInput = nameInput.Remove(nameInput.Length - 1);
                    else if (key == Keys.Enter)
                    { 
                        clientManager.SetName(nameInput);
                        SceneManager.ChangeScene(sceneName);
                    }
                }
            }

            oldKeys.Clear();
            oldKeys.AddRange(keys);

            base.Update(deltaTime);
        }

        public override void Draw(Matrix view, Matrix projection)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "Please enter a name, then press enter when you are done", new Vector2(20, 50), textColor, 0.0f, Vector2.Zero, 0.5f, 0, 0);
            spriteBatch.DrawString(font, "Name:", new Vector2(20, 100), textColor, 0.0f, Vector2.Zero, 0.5f, 0, 0);
            spriteBatch.DrawString(font, nameInput, new Vector2(120, 100), textColor, 0.0f, Vector2.Zero, 0.5f, 0, 0);
            spriteBatch.End();

            base.Draw(view, projection);
        }
    }
}
