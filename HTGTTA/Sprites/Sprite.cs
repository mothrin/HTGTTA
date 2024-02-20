using HTGTTA.Manager;
using HTGTTA.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Randomchaos.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HTGTTA.Sprites
{
    public class Sprite : DrawableGameComponent
    {

        #region Fields

        protected SpriteBatch spriteBatch;

        protected AnimationManager _animationManager;

        protected Dictionary<string, Animation> _animations;

        protected Vector2 _position;

        protected Texture2D _texture;
        protected Texture2D _boundsTexture;

        protected SpriteFont _font;


        public int Width { get; set; }
        public int Height { get; set; }

        //packages
        protected IKeyboardStateManager _keyboard { get { return ((IInputStateService)Game.Services.GetService<IInputStateService>()).KeyboardManager; } }
        protected IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }

        public Input Input;

        public float spriteSpeed = 5f; //player speed

        public Vector2 Velocity; //player position

        #endregion

        #region Properties

        public string Name { get; set; }

        public bool RenderBounds { get; set; }

        protected string TextureAsset { get; set; }

        public Rectangle Bounds //for collision and bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); }
        }
        public Rectangle BoundsPlayer
        {
            get { return new Rectangle((int)Position.X+(Width/4), (int)Position.Y + (2* (Height/3)), Width/2, Height / 4); }
        }


        public Sprite(Game game, string textureAsset) : base(game)
        {
            TextureAsset = textureAsset;
        }

        public Sprite(Game game, Dictionary<string, Animation> animations) : base(game)
        {
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);
        }

        protected override void LoadContent()
        {
            //text
            _font = Game.Content.Load<SpriteFont>("Fonts/font");

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

             //bounds
            _boundsTexture = new Texture2D(GraphicsDevice, 1, 1);
            _boundsTexture.SetData(new Color[] {new Color(1f, .1f, .1f, .25f) });

            if (!string.IsNullOrEmpty(TextureAsset))
            {
                _texture = Game.Content.Load<Texture2D>(TextureAsset);
                //Width = _texture.Width;
                //Height = _texture.Height;
            }
            if (_animations != null)
            {
                Width = _animationManager.FrameWidth;
                Height = _animationManager.FrameHeight;
            }

            base.LoadContent();
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
        public override void Draw(GameTime gameTime)
        {
            if (spriteBatch != null)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                if (RenderBounds)
                {
                    if (_texture != null)
                        spriteBatch.Draw(_boundsTexture, Bounds, Color.White);
                    else
                        spriteBatch.Draw(_boundsTexture, BoundsPlayer, Color.White);

                }

                if (_texture != null)
                {
                    spriteBatch.Draw(_texture, Bounds, Color.White);
                }
                else if (_animationManager != null)
                {                 
                    _animationManager.Draw(spriteBatch);
                }

                // Draw text for coords
                string textToPrint = $"{Name} - {Position}";
                Vector2 textSize = _font.MeasureString(textToPrint);
                Vector2 txtPos = Position + (new Vector2(Width / 2, 0) - (textSize * .5f));

                spriteBatch.DrawString(_font, textToPrint, txtPos, Color.Black);
                spriteBatch.DrawString(_font, textToPrint, txtPos + new Vector2(-2,-2), Color.Gold);

                spriteBatch.End();
            }
        }

        #endregion

    }
}
