using HTGTTA.Manager;
using HTGTTA.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Sprites
{
    public class Sprite
    {

        #region Fields

        protected AnimationManager _animationManager;

        protected Dictionary<string, Animation> _animations;


        protected Vector2 _position;

        protected Texture2D _texture;


        #endregion


        #region Properties

        public Input Input;

        public float spriteSpeed = 5f;


        public Vector2 Velocity;



        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height); }
        }
        public Sprite(Texture2D texture)
        {
            _texture = texture;
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
                spriteBatch.Draw(_texture, Position, Color.White);

        }

        public Sprite(Dictionary<string, Animation> animations)
        {
            
        }



        #endregion

    }
}
