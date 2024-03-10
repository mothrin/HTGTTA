using HTGTTA.Manager;
using HTGTTA.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Randomchaos.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Sprites
{
    public class Object : DrawableGameComponent
    {
        public static bool BondsOn = true;

        #region Fields

        protected SpriteBatch spriteBatch;

        protected Vector2 _position;

        protected Texture2D _texture;
        protected Texture2D _boundsTexture;

        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        #endregion


        protected string TextureAsset { get; set; }

        public Dictionary<string, ObjectInterations> Interaction { get; set; } = new Dictionary<string, ObjectInterations>();


        public Object(Game game, Texture2D textureAsset) : base(game)
        {
            TextureAsset = Convert.ToString(textureAsset);
        }

        public virtual Rectangle Bounds //for collision and bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); }
        }
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);


            _texture = Game.Content.Load<Texture2D>(TextureAsset);

            base.LoadContent();
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }

        #region Methods
        public override void Draw(GameTime gameTime)
        {
            if (spriteBatch != null)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                if (_texture != null)
                {
                    spriteBatch.Draw(_texture, Bounds, Color.White);
                }

                spriteBatch.End();
            }
        }

        #endregion

    }
}
