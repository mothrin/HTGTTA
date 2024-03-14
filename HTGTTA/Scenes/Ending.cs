using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Randomchaos.Services.Interfaces;
using MonoGame.Randomchaos.Services.Scene.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Randomchaos.Services.Coroutine.Models;
using MonoGame.Randomchaos.Services.Interfaces.Enums;
using System.Collections;

namespace HTGTTA.Scenes
{
    public class Ending : SceneFadeBase
    {

        Texture2D _bgTexture;
        public string endingType;
        protected string NextScene;
        protected bool waiting;

        private IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }


        public Ending(Game game, string name, string nextScene) : base(game, name) { NextScene = nextScene; }
        public override void Update(GameTime gameTime)
        {
            if (State == SceneStateEnum.Loaded && !waiting)
            {
                coroutineService.StartCoroutine(WaitSecondsAndExit(10));
            }

            if (State == SceneStateEnum.Loaded && (kbManager.KeysPressed().Length > 0 || msManager.LeftButtonDown || msManager.RightButtonDown))
            {
                sceneManager.LoadScene(NextScene);
            }

            base.Update(gameTime);
        }
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
        protected IEnumerator WaitSecondsAndExit(float seconds)
        {
            waiting = true;
            yield return new WaitForSeconds(Game, seconds);

            if (State == SceneStateEnum.Loaded)
                sceneManager.LoadScene(NextScene);

        }
    }
}
