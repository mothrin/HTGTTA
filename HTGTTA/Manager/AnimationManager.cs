using HTGTTA.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Manager
{
    public class AnimationManager //keeps track of one animimation at a time
    {
        private Animation _animation;

        private float _timer; // to know when to incremenet current frame based on spritespeed
        public Vector2 Position { get; set; }

        protected Texture2D _CurrentFrame;
        public int FrameWidth { get { return _animation.FrameWidth; } }
        public int FrameHeight { get { return _animation.FrameHeight; } }

        public AnimationManager(Animation animation)
        {
            _animation = animation;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_animation.Texture,
                             Position,
                             new Rectangle(_animation.CurrentFrame * _animation.FrameWidth, // as currentframe increments, the width shifts to along sprite sheet
                                           0,
                                           _animation.FrameWidth,
                                           _animation.FrameHeight),
                             Color.White);
        }

        public void Play(Animation animation)
        {
            if (_animation == animation) //if at the start, animation continues
                return;
             
            _animation = animation; //else it resets it from start

            _animation.CurrentFrame = 0;

            _timer = 0;
        }

        public void Stop() //stops animation
        {
            _timer = 0f;

            _animation.CurrentFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > _animation.FrameSpeed)
            {
                _timer = 0f;

                _animation.CurrentFrame++;

                if (_animation.CurrentFrame >= _animation.FrameCount)
                    _animation.CurrentFrame = 0; //loops round to first frame
            }
        }
    }
}