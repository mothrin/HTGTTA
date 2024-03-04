using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Randomchaos.Services.Interfaces.Enums;
using MonoGame.Randomchaos.Services.Scene.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Scenes
{
    public class MainMenu : SceneFadeBase
    {

        private SpriteFont font;
        bool exiting;
        protected string NextScene;

        public MainMenu(Game game, string name) : base(game,name) { }


        public override void Initialize()
        {
            font = Game.Content.Load<SpriteFont>("Fonts/font");
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (State == SceneStateEnum.Loaded)
            {
                if (kbManager.KeyPress(Microsoft.Xna.Framework.Input.Keys.F10))
                {
                    exiting = true;
                    State = SceneStateEnum.Unloading;
                    UnloadScene();
                }

                if (kbManager.KeyPress(Microsoft.Xna.Framework.Input.Keys.F1))
                    sceneManager.LoadScene("optionsScene");

                if (kbManager.KeyPress(Microsoft.Xna.Framework.Input.Keys.Escape))
                    sceneManager.LoadScene("Game");
            }

            base.Update(gameTime);

            if (State == SceneStateEnum.Unloaded && exiting)
                Game.Exit();
        }
        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap);

            _spriteBatch.Draw(Game.Content.Load<Texture2D>("Screens/mainmenu"), new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),Color.White);

            string str = "This is the main menu, press Esc to exit, F1 for options, F2 to play new game.";
            Vector2 pos = (new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) - font.MeasureString(str)) / 2;

            _spriteBatch.DrawString(font, str, pos, Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);

            DrawFader(gameTime);
        }
    }
}
