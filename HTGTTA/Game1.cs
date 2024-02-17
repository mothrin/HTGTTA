using HTGTTA.Models;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Randomchaos.Services.Audio;
using MonoGame.Randomchaos.Services.Input.Models;
using MonoGame.Randomchaos.Services.Input;
using MonoGame.Randomchaos.Services.Interfaces;
using System.Collections.Generic;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using Vector2 = System.Numerics.Vector2;

namespace HTGTTA
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public int w;
        public int h;

        private Texture2D _backgroundTexture;
        private Texture2D one;

        private List<Sprite> _sprites;

        /// <summary>   The input service. </summary>
        IInputStateService inputService;
        /// <summary>   State of the kB. </summary>
        IKeyboardStateManager kbState;

        private IAudioService _audio { get { return Services.GetService<IAudioService>(); } }


        public Game1()
        {
            w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = w;
            graphics.PreferredBackBufferHeight = h;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = false;


            new AudioService(this);

            kbState = new KeyboardStateManager(this);
            inputService = new InputHandlerService(this, kbState);

        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.

        protected override void Initialize()
        {
            var animations = new Dictionary<string, Animation>()
            {
                { "WalkUp", new Animation(Content.Load<Texture2D>("Textures/back"), 3) },
                { "WalkDown", new Animation(Content.Load<Texture2D>("Textures/front"), 3) },
                { "WalkLeft", new Animation(Content.Load<Texture2D>("Textures/left"), 3) },
                { "WalkRight", new Animation(Content.Load<Texture2D>("Textures/right"), 3) },
            };

            // TODO: Add your initialization logic here
            _sprites = new List<Sprite>()
            {

                new Player(this, animations)
                {
                    Position = new Vector2(w/2, h/2),
                    Input = new Input()
                    {
                        Up = Keys.W,
                        Down = Keys.S,
                        Left = Keys.A,
                        Right = Keys.D,
                    },
                },
                /*new Player(animations)
                {
                    Position = new Vector2(w/2, h/2),
                    Input = new Input()
                    {
                        Up = Keys.Up,
                        Down = Keys.Down,
                        Left = Keys.Left,
                        Right = Keys.Right,
                    },
                },*/
                new Sprite(this, "Textures/bed")
                {
                    Position = new Vector2(20,10),
                },
            };

            foreach (var sprite in _sprites)
            {
                Components.Add(sprite);
            }

            base.Initialize();
        }

        /// LoadContent will be called once per game and is the place to load all of your content.

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _backgroundTexture = Content.Load<Texture2D>("Textures/background");

            

            

            _audio.PlaySong("Audio/Music/Drafty-Places", .0125f);
            
            base.LoadContent();
        }


        /// UnloadContent will be called once per game and is the place to unload game-specific content

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

       
        /// Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio

        protected override void Update(GameTime gameTime)
        {

            inputService.PreUpdate(gameTime);
            base.Update(gameTime);

            foreach (var sprite in _sprites)
            {
                foreach (var sprite2 in _sprites)
                {
                    if (sprite != sprite2)
                    {
                        if (sprite.Rectangle.Intersects(sprite2.Rectangle))
                        {
                            sprite.Position -= sprite.Velocity;
                        }
                    }
                }
            }

            if (inputService.KeyboardManager.KeyPress(Keys.Escape))
            {
                Exit();
            }
        }

       /// This is called when the game should draw itself.

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(_backgroundTexture,
                new Rectangle(0,0, w, h), 
                new Rectangle(0, 0, _backgroundTexture.Width, _backgroundTexture.Height),
               Color.White);

            //foreach (var sprite in _sprites)
            //    sprite.Draw(spriteBatch);

                

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}