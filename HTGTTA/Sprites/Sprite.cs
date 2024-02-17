using HTGTTA.Manager;
using HTGTTA.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace HTGTTA.Sprites
{
    public class Sprite
    {

        #region Fields

        protected AnimationManager _animationManager;

        protected Dictionary<string, Animation> _animations;


        protected Vector2 _position;

        protected Texture2D _texture;

        public int Width { get; set; }
        public int Height { get; set; }


        #endregion


        #region Properties

        public Input Input;

        public float spriteSpeed = 5f;


        public Vector2 Velocity;



        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); }
        }

        public Sprite(Texture2D texture)
        {
            _texture = texture;
            Width = _texture.Width;
            Height = +texture.Height;
        }

        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);


            Width = _animationManager.FrameWidth;
            Height = _animationManager.FrameHeight;
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                if (_animationManager != null)
                    _animationManager.Position = _position;
            }
        }

        #endregion

        #region Methods

        public virtual void Update(GameTime gameTime, List<Sprite> sprites)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, Position, Color.White);
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch);
        }

        
        #endregion

    }
}
