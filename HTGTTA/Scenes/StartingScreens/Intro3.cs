using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Randomchaos.Services.Coroutine.Models;
using MonoGame.Randomchaos.Services.Coroutine;
using MonoGame.Randomchaos.Services.Interfaces.Enums;
using MonoGame.Randomchaos.Services.Interfaces;
using MonoGame.Randomchaos.Services.Scene.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Scenes.StartingScreens
{
    public class Intro3 : SceneFadeBase
    {

        Texture2D _bgTexture;
        public string endingType;
        protected string NextScene;
        protected bool waiting;

        private IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }


        public Intro3(Game game, string name, string nextScene) : base(game, name) { NextScene = nextScene; }
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
            _bgTexture = Game.Content.Load<Texture2D>("Textures/backgrounds/Intro/startscreen3");


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

