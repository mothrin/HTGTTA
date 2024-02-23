using HTGTTA.Enums;
using HTGTTA.Models;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Randomchaos.Services.Audio;
using MonoGame.Randomchaos.Services.Input;
using MonoGame.Randomchaos.Services.Input.Models;
using MonoGame.Randomchaos.Services.Interfaces;
using MonoGame.Randomchaos.UI;
using System.Collections.Generic;

namespace HTGTTA
{
    public class Game1 : Game
    {
        

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public int w;
        public int h;

        protected SpriteFont _font;

        private Texture2D _backgroundTexture;

        public Dictionary<Sprite, Dictionary<string, ObjectInterations>> Interactions = new Dictionary<Sprite, Dictionary<string, ObjectInterations>>();

        private Player player;

        public List<Sprite> _items;


        Vector2 lastPosition; //collision

        // packages
        /// <summary>   The input service. </summary>
        IInputStateService inputService;
        /// <summary>   State of the kB. </summary>
        IKeyboardStateManager kbState;
        private int i;

        protected Hud HUD;

        private IAudioService _audio { get { return Services.GetService<IAudioService>(); } }

        private UIMessageBox _textbox;
        public Game1()
        {
            ////defining window width and height
            //w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //1080p
            w = 1920;
            h = 1080;

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
            HUD = new Hud(this);

            Components.Add(HUD);

            var animations = new Dictionary<string, Animation>()
            {
                { "WalkUp", new Animation(Content.Load<Texture2D>("Textures/Sprite/back"), 3) },
                { "WalkDown", new Animation(Content.Load<Texture2D>("Textures/Sprite/front"), 3) },
                { "WalkLeft", new Animation(Content.Load<Texture2D>("Textures/Sprite/left"), 3) },
                { "WalkRight", new Animation(Content.Load<Texture2D>("Textures/Sprite/right"), 3) },
            };

            // TODO: Add your initialization logic here
            Dictionary<string, ObjectInterations> nothingToDo = new Dictionary<string, ObjectInterations>()
            {
                {"Noting to do here", new ObjectInterations(){ Active = true, Description = "Nothing to do here", Name = "Nothing to do.", InteractionType = InteractionTypeEnum.Nothing } }
            };

            //objects
            _items = new List<Sprite>()
            {
                new Sprite(this, "Textures/Objects/Blank")
                {
                    Name = "Bed1",
                    Description = "We should probably leave that be...",
                    Position = new Vector2(1500,273),
                    Width = 278,
                    Height = 328,
                    RenderBounds = true, //for bounds
                    RenderInteractionBounds = true,
                    Interaction = nothingToDo,
                },
                new Sprite(this, "Textures/Objects/Blank")
                {
                    Name = "Bed",
                    Description = "We should probably leave that be...",
                    Position = new Vector2(1540,400),
                    Width = 278,
                    Height = 328,
                    RenderBounds = true, //for bounds
                    Interaction = nothingToDo,
                    RenderInteractionBounds = true,
                },
                new Sprite(this, "Textures/Objects/Blank")
                {
                    Name = "Bed",
                    Description = "We should probably leave that be...",
                    Position = new Vector2(1580,527),
                    Width = 278,
                    Height = 328,
                    RenderBounds = true, //for bounds
                    RenderInteractionBounds = true,
                    Interaction = nothingToDo,
                },
                new Sprite (this, "Textures/Objects/Blank")
                {
                    Name = "Desk",
                    Description = "The laptop is locked",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Desk", new ObjectInterations(){ InteractionType = InteractionTypeEnum.DrawsOpen,Name = "Open Desk" }  },
                        {"Laptop", new ObjectInterations(){ InteractionType = InteractionTypeEnum.LaptopCodeEnter, Name ="Laptop",  Description = "Please enter the code..." }  }
                    },
                    Position = new Vector2(102,972),
                    Width = 570,
                    Height = 75,
                    RenderBounds = true, //bounds
                    RenderInteractionBounds = true,
                },
                new Sprite(this,"Textures/Objects/Blank")
                {
                    Name = "Drawer",
                    Description = "Nothing of interest in here. Just clothes.",
                    Position = new Vector2(483,220),
                    Width = 345,
                    Height = 280,
                    RenderBounds = true, //bounds
                    RenderInteractionBounds = true,
                    Interaction = nothingToDo,
                },
                new Sprite(this,"Textures/Objects/Blank")
                {
                    Name = "Wardrobe",
                    Description = "There are too many clothes here, I can't open the door.",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Door", new ObjectInterations(){ }  },
                        {"Clothes", new ObjectInterations(){ }  },
                        {"Box", new ObjectInterations(){ }  }
                    },
                    Position = new Vector2(858,30),
                    Width = 288,
                    Height = 450,
                    RenderBounds = true, //bounds
                    RenderInteractionBounds = true,
                },
                new Sprite(this,"Textures/Objects/Blank")
                {
                    Name = "Table",
                    Description = "This bear is cute.",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Bear", new ObjectInterations(){ } },
                        {"Drawer", new ObjectInterations(){ } }
                    },
                    Position = new Vector2(1284,348),
                    Width = 171,
                    Height = 150,
                    RenderBounds = true, //bounds
                    RenderInteractionBounds = true,
                },
                new Sprite(this,"Textures/Objects/Blank")
                {
                    Name = "Door",
                    Description = "I need to find the code.",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Door", new ObjectInterations(){ } },
                        {"Code", new ObjectInterations(){ } }
                    },
                    Position = new Vector2(195,120),
                    Width = 255,
                    Height = 330,
                    RenderBounds = true, //bounds
                    RenderInteractionBounds = true,
                },
                new Sprite(this,"Textures/Objects/Blank")
                {
                    Name = "Window",
                    Description = "Locked.",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Window", new ObjectInterations(){ } },
                        {"Open", new ObjectInterations(){ } }
                    },
                    Position = new Vector2(0,105),
                    Width = 65,
                    Height = 560,
                    RenderBounds = true, //bounds
                    RenderInteractionBounds = true,
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
                RenderInteractionBounds = true,
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
                }
            }

            if (inputService.KeyboardManager.KeyPress(Keys.F1))
            {
                Sprite.BondsOn = !Sprite.BondsOn;
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

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(_backgroundTexture,
                new Rectangle(0,0, w, h), 
                new Rectangle(0, 0, _backgroundTexture.Width, _backgroundTexture.Height),
               Color.White);

            spriteBatch.End();

            base.Draw(gameTime);

            Interactions.Clear();

            //for object interaction
            foreach (var item in _items) 
            {
                _font = Content.Load<SpriteFont>("Fonts/font");
                if (player.InteractionBounds.Intersects(item.InteractionBounds))
                {
                    Interactions.Add(item,item.Interaction);                    
                }             
            }

            if (Interactions.Count > 0)
            {
                HUD.ShowInteractionOptionsWindow(Interactions);
            }
            else
            {
                HUD.CurrentInteractions = null;
            }
            
        }

       
    }
}