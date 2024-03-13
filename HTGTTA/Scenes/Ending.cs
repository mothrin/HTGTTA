using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Randomchaos.Services.Interfaces;
using MonoGame.Randomchaos.Services.Scene.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Scenes
{
    public class Ending : SceneFadeBase
    {

        Texture2D _bgTexture;
        protected string NextScene;

        private IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }


        public Ending(Game game, string name) : base(game, name) { }
        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            // = new SpriteBatch(GraphicsDevice); // for screen switching
            _audio.PlaySong("Audio/Music/Mysterious-Puzzle_Looping", .005f);


             _bgTexture = Game.Content.Load<Texture2D>("Textures/backgrounds/Ending");

            base.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap);

            _spriteBatch.Draw(_bgTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            _spriteBatch.End();


            base.Draw(gameTime);


            DrawFader(gameTime);
        }
    }
}
