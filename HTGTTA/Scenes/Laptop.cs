using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Randomchaos.Services.Interfaces;
using MonoGame.Randomchaos.Services.Interfaces.Enums;
using MonoGame.Randomchaos.Services.Scene.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Scenes
{
    public class Laptop : SceneFadeBase
    {
        private SpriteFont font;

        public int w;
        private int h;

        private IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }
        public Laptop(Game game, string name) : base(game, name)
        {

        }
        protected override void LoadContent()
        {
            w = GraphicsDevice.Viewport.Width;
            h = GraphicsDevice.Viewport.Height;

            _audio.PlaySong("Audio/Music/Drafty-Places", .05f); //music

        }

        public override void Initialize()
        {
            font = Game.Content.Load<SpriteFont>("Fonts/font");
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            if (State == SceneStateEnum.Loaded)
            {
                if (kbManager.KeyPress(Microsoft.Xna.Framework.Input.Keys.Escape))
                    sceneManager.LoadScene("Game");
            }

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap);

            _spriteBatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/laptop"), new Rectangle(0, 0, w, h), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);

            DrawFader(gameTime);
        }
    }
}
