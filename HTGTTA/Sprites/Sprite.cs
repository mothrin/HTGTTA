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

        //packages
        protected IKeyboardStateManager _keyboard { get { return ((IInputStateService)Game.Services.GetService<IInputStateService>()).KeyboardManager; } }

        protected IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }

        protected Vector2 _position;

        protected Texture2D _texture;

        public int Width { get; set; }
        public int Height { get; set; }


        #endregion


        #region Properties

        public Input Input;

        public float spriteSpeed = 5f;


        public Vector2 Velocity;

        protected string TextureAsset { get; set; }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); }
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
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            if (!string.IsNullOrEmpty(TextureAsset))
            {
                _texture = Game.Content.Load<Texture2D>(TextureAsset);
                Width = _texture.Width;
                Height = _texture.Height;
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

        //public override void Update(GameTime gameTime)
        //{

        //}

        public override void Draw(GameTime gameTime)
        {
            if (spriteBatch != null)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                if (_texture != null)
                    spriteBatch.Draw(_texture, Position, Color.White);
                else if (_animationManager != null)
                    _animationManager.Draw(spriteBatch);

                spriteBatch.End();
            }
        }

        
        #endregion

    }
}
