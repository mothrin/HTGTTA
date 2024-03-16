using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Randomchaos.Services.Interfaces.Enums;
using MonoGame.Randomchaos.Services.Interfaces;
using MonoGame.Randomchaos.Services.Scene.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Randomchaos.Extensions;

namespace HTGTTA.Scenes
{
    public class Help : SceneFadeBase
    {
        private SpriteFont _font;

        Dictionary<string, Rectangle> ButtonBounds = new Dictionary<string, Rectangle>();
        public Help(Game game, string name) : base(game, name) { }

        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            _font = Game.Content.LoadLocalized<SpriteFont>("Fonts/UIFont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (State == SceneStateEnum.Loaded)
            {
                if (kbManager.KeyPress(Microsoft.Xna.Framework.Input.Keys.F1))
                {
                    sceneManager.LoadScene("Options");
                }
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap);

            _spriteBatch.Draw(Game.Content.Load<Texture2D>("Screens/HelpMenu"), new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            DrawString("Press F1 to go back.", new Point(10, 10).ToVector2(), Color.Gray);

            _spriteBatch.End();


            base.Draw(gameTime);


            DrawFader(gameTime);
        }


        protected void DrawString(string text, Vector2 pos, Color color, SpriteFont font = null)
        {
            if (font == null)
            {
                font = _font;
            }

            Vector2 shadow = Vector2.One * -1;
            _spriteBatch.DrawString(font, text, pos, Color.Gray);
            _spriteBatch.DrawString(font, text, pos + shadow, color);
        }
    }
}
