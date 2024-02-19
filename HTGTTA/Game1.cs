using HTGTTA.Models;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Randomchaos.Services.Audio;
using MonoGame.Randomchaos.Services.Input;
using MonoGame.Randomchaos.Services.Input.Models;
using MonoGame.Randomchaos.Services.Interfaces;
using System.Collections.Generic;

namespace HTGTTA
{ 
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public int w;
        public int h;

        private Texture2D _backgroundTexture;

        private Player player;

        public List<Sprite> _items; 

        Vector2 lastPosition; //collision

        // packages
        /// <summary>   The input service. </summary>
        IInputStateService inputService;
        /// <summary>   State of the kB. </summary>
        IKeyboardStateManager kbState;

        private IAudioService _audio { get { return Services.GetService<IAudioService>(); } }


        public Game1()
        {
            //defining window width and height
            w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics = new GraphicsDeviceManager(this);

            //window sizes
            graphics.PreferredBackBufferWidth = w;
            graphics.PreferredBackBufferHeight = h;
            graphics.ApplyChanges();

            //different features
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            //audio
            new AudioService(this);

            //input
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
                { "WalkUp", new Animation(Content.Load<Texture2D>("Textures/Sprite/back"), 3) },
                { "WalkDown", new Animation(Content.Load<Texture2D>("Textures/Sprite/front"), 3) },
                { "WalkLeft", new Animation(Content.Load<Texture2D>("Textures/Sprite/left"), 3) },
                { "WalkRight", new Animation(Content.Load<Texture2D>("Textures/Sprite/right"), 3) },
            };

            // TODO: Add your initialization logic here


            //objects
            _items = new List<Sprite>()
            {
                new Sprite(this, "Textures/Objects/bed")
                {
                    Name = "Bed",
                    Position = new Vector2(1460,260),
                    Width = 300,
                    Height = 100,
                    RenderBounds = true, //for bounds
                },
                new Sprite (this, "Textures/Objects/desk")
                {
                    Name = "Desk",
                    Position = new Vector2(100,1000),
                    RenderBounds = true, //bounds
                },
                new Sprite(this,"Textures/Objects/drawers")
                {
                    Name = "Drawer",
                    Position = new Vector2(500,200),
                    RenderBounds = true, //bounds
                },
                new Sprite(this,"Textures/Objects/wardrobe")
                {
                    Name = "Wardrobe",
                    Position = new Vector2(0,0),
                    RenderBounds = true, //bounds
                },
                new Sprite(this,"Textures/Objects/table")
                {
                    Name = "Table",
                    Position = new Vector2(1200,200),
                    RenderBounds = true, //bounds
                }
            };

            foreach (var item in _items)
            {
                Components.Add(item);
            }

            //player
            player = new Player(this, animations)
            {
                Name = "Player",
                Position = new Vector2(w / 2, h / 2),
                Input = new Input()
                {
                    Up = Keys.W,
                    Down = Keys.S,
                    Left = Keys.A,
                    Right = Keys.D,
                },
                RenderBounds = true,
            };
            Components.Add(player);

            base.Initialize();
        }

        /// LoadContent will be called once per game and is the place to load all of your content.

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _backgroundTexture = Content.Load<Texture2D>("Textures/background"); //background

            _audio.PlaySong("Audio/Music/Drafty-Places", .05f); //music
            
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

            lastPosition = player.Position;

            base.Update(gameTime);

            foreach (var item in _items)
            {
                if (player.Bounds.Intersects(item.Bounds))
                {
                    player.Position = lastPosition;
                    //player.Color = Color.Gold;
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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}