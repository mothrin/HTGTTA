using HTGTTA.Enums;
using HTGTTA.Services;
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

        protected string timeString;

        protected HTGTTAService _HTGTTAService { get { return Game.Services.GetService<HTGTTAService>(); } }

        public TimeSpan MaxTime { get { return _HTGTTAService.MaxTime; } set { _HTGTTAService.MaxTime = value; } } //dictates player time limit 
        public TimeSpan CurrentTime { get; set; } = TimeSpan.Zero; //this counts up while game isn't paused
        public TimeSpan CurrentTimeLeft { get; set; } //this counts down while game isn't paused
        public bool IsPaused { get; set; } = false; //when this is true the time is paused
        public bool IsRunning { get; protected set; } = false; //when true means timer is counting up or down
        public bool IsOutOfTime { get; protected set; } = false; //this triggers the game to end

        public TimerTypeEnum TimerType { get { return _HTGTTAService.TimerType; } set { _HTGTTAService.TimerType = value; } } //this dictates what type of timer is selected in timer menu
        protected Texture2D PausedTexture { get; set; } //texture of pause screen
        //protected Timer Timer;
        public GamePlayTimer(Game game) : base(game) { }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Game.Content.Load<SpriteFont>("Fonts/timeFont");
            PausedTexture = Game.Content.Load<Texture2D>("Textures/backgrounds/paused");
            base.LoadContent();
        }

        public void StartTimer()
        {
            CurrentTimeLeft = MaxTime; //sets times
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
            while (IsRunning && TimerType != TimerTypeEnum.Relaxed) 
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

            timeString = $"{CurrentTime}";
            if (IsPaused)
            {
                _spriteBatch.Draw(PausedTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White); //paused texture being drawn
            }

            switch (TimerType)
            {
                case TimerTypeEnum.Normal:
                    timeString = $"{CurrentTime}"; //counting up timer
                    break;
                case TimerTypeEnum.Tense:
                    timeString = $"{CurrentTimeLeft}"; //counting down timer
                    break;
                case TimerTypeEnum.Relaxed:
                    timeString = "";
                    break; //no timer
            }

            //timer being drawn (twice as to highlight it and make it more readable)
            float l = _font.MeasureString(timeString).X /2 ;

            _spriteBatch.DrawString(_font, timeString, new Vector2(GraphicsDevice.Viewport.Width/2 - (int)l, 8), Color.Black);
            _spriteBatch.DrawString(_font, timeString, new Vector2(GraphicsDevice.Viewport.Width/2 - (int)l, 8) - Vector2.One, Color.DarkBlue);

            _spriteBatch.End();
        }
    }
}
