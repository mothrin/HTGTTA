using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Randomchaos.Services.Coroutine.Models;
using MonoGame.Randomchaos.Services.Interfaces;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace HTGTTA.Models
{
    public class GamePlayTimer : DrawableGameComponent
    {
        ICoroutineService coroutineService { get { return Game.Services.GetService<ICoroutineService>(); } }

        protected SpriteBatch _spriteBatch;
        protected SpriteFont _font;
        public TimeSpan MaxTime { get; set; } = new TimeSpan(0, 15, 0);
        public TimeSpan CurrentTime { get; set; } = TimeSpan.Zero;
        public TimeSpan CurrentTimeLeft { get; set; }
        public bool IsPaused { get; set; } = false;

        public bool IsRunning { get; protected set; } = false;
        public bool IsOutOfTime { get; protected set; } = false;

        protected Texture2D PausedTexture { get; set; }

        public GamePlayTimer(Game game, TimeSpan? maxTime = null) : base(game)
        {
            if (maxTime != null)
            {
                MaxTime = maxTime.Value;
            }
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Game.Content.Load<SpriteFont>("Fonts/timeFont");
            PausedTexture = Game.Content.Load<Texture2D>("Textures/backgrounds/paused");
            base.LoadContent();
        }

        public void StartTimer()
        {
            CurrentTimeLeft = MaxTime;
            CurrentTime = TimeSpan.Zero;

            IsRunning = true;
            coroutineService.StartCoroutine(TimeTicker());
        }

        public void StopTimer()
        {
            IsRunning = false;
        }

        protected IEnumerator TimeTicker()
        {
            while (IsRunning)
            {
                if (!IsPaused && !IsOutOfTime) // If it's not paused, wait for a second and move the time on.
                {
                    yield return new WaitForSeconds(Game, 1);

                    if (!IsPaused) // Pause may be hit while we are waiting...
                    {
                        CurrentTime += TimeSpan.FromSeconds(1);
                    }

                    CurrentTimeLeft = MaxTime - CurrentTime;

                    IsOutOfTime = CurrentTimeLeft.TotalSeconds <= 0;
                }
                else // If it is paused, do nothing, wait until the next frame to check again.
                {
                    yield return new WaitForEndOfFrame(Game);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (IsPaused)
            {
                _spriteBatch.Draw(PausedTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            }

            string timeString = $"{CurrentTime}";
            //string timeString = $"{CurrentTime} / {MaxTime} = {CurrentTimeLeft}";
            //string timeString = $"{CurrentTimeLeft}";
            float l = _font.MeasureString(timeString).X /2 ;

            _spriteBatch.DrawString(_font, timeString, new Vector2(GraphicsDevice.Viewport.Width/2 - (int)l, 8), Color.Black);
            _spriteBatch.DrawString(_font, timeString, new Vector2(GraphicsDevice.Viewport.Width/2 - (int)l, 8) - Vector2.One, Color.DarkBlue);

            _spriteBatch.End();
        }
    }
}
