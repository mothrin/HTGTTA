using HTGTTA.Enums;
using HTGTTA.Models;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame.Randomchaos.Services.Audio;
using MonoGame.Randomchaos.Services.Input.Models;
using MonoGame.Randomchaos.Services.Input;
using MonoGame.Randomchaos.Services.Interfaces;
using MonoGame.Randomchaos.Services.Scene.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Scenes
{
    public class GameScene : Game1
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public int w;
        public int h;


        protected SpriteFont _font;

        private Texture2D _backgroundTexture;
        public Texture2D _laptopTexture;

        public Dictionary<Sprite, Dictionary<string, ObjectInterations>> Interactions = new Dictionary<Sprite, Dictionary<string, ObjectInterations>>();

        private Player player;

        public List<Sprite> _items;

        public List<Sprite> _inventory;

        public Boolean laptopOpened;
        public Boolean diaryRead;
        public Boolean chairGot;
        public Boolean chairplaced;
        public Boolean boxOpened;
        public Boolean keyGot;
        public Boolean drawerOpened;
        public Boolean paperRead;
        public Boolean clothesMoved;


        Vector2 lastPosition; //collision

        // packages
        /// <summary>   The input service. </summary>
        IInputStateService inputService;
        /// <summary>   State of the kB. </summary>
        IKeyboardStateManager kbState;
        private int i;

        protected Hud HUD;

        private IAudioService _audio { get { return Services.GetService<IAudioService>(); } }

        ISceneService sceneService { get { return Services.GetService<ISceneService>(); } }


        public GameScene(Game game, string name)
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
            IsMouseVisible = true;

            //audio
            new AudioService(this);

            //input
            kbState = new KeyboardStateManager(this);
            inputService = new InputHandlerService(this, kbState);

            //scenes
            new SceneService(this);


        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.

        protected override void Initialize()
        {
            sceneService.AddScene(new MainMenu(this, "mainMenu"));
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
                {"Nothing to do here", new ObjectInterations(){ Active = true, Description = "Nothing to do here", Name = "Nothing to do.", InteractionType = InteractionTypeEnum.Nothing } }
            };

            //objects
            _items = new List<Sprite>()
            {
                new Sprite(this, "Textures/Objects/Blank")
                {
                    Name = "Bed1",
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
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Look at bed", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Bed,Name = "Bed", Description = "Maybe we should leave this be..." }  },
                        {"Go to bed", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Sleep,Name = "Bed", Description = "I'm so so tired. Should i jsut go back to bed?" }  },
                        {"Read diary", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Diary, Name ="Diary",  Description = "I don't think i should read that." }  }
                    },
                    Position = new Vector2(1540,400),
                    Width = 278,
                    Height = 328,
                    RenderBounds = true, //for bounds
                    RenderInteractionBounds = true,
                },
                new Sprite(this, "Textures/Objects/Blank")
                {
                    Name = "Bed",
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
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Look at desk", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Desk,Name = "Desk", Description = "Hm, just loads of books." }  },
                        {"Look at paper", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Paper,Name = "Paper", Description = "Just some homework, I think." }  },
                        {"Open Laptop", new ObjectInterations(){ InteractionType = InteractionTypeEnum.LaptopCodeEnter, Name ="Laptop",  Description = "Needs a password. I don't remember what it as." }  }
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
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                       {"Look at drawers", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Drawers,Name = "Drawers",Description = "Eh, just clothes and stuff. Nothing of interest.", }  },
                        {"Look at plant", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Plant,Name = "Plant",Description = "Aw poor plant, someone didn't look after you very well.", }  },
                        {"Look at mirror", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Mirror,Name = "Mirror",Description = "Well someone had a tantrum...was it you mr plant?", }  }
                    },
                    Position = new Vector2(483,220),
                    Width = 345,
                    Height = 280,
                    RenderBounds = true, //bounds
                    RenderInteractionBounds = true,
                },
                new Sprite(this,"Textures/Objects/Blank")
                {
                    Name = "Wardrobe",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Open door", new ObjectInterations(){ InteractionType = InteractionTypeEnum.WarDoorOpen,Name = "Door",Description = "There are too many clothes here, I can't open the door.", }  },
                        {"Move clothes", new ObjectInterations(){ InteractionType = InteractionTypeEnum.ClothesMoved,Name = "Clothes", Description = "No point in moving these, waste of energy..." }  },
                        {"Look in box", new ObjectInterations(){ InteractionType = InteractionTypeEnum.ChairPlaced,Name = "Box",  Description = "I can't reach that.",}  },
                    },
                    Position = new Vector2(858,30),
                    Width = 288,
                    Height = 450,
                    RenderBounds = true, //bounds
                    RenderInteractionBounds = true,
                },
                new Sprite(this,"Textures/Objects/Blank")
                {
                    Name = "Bedside Table",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Talk to bear", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Bear1,Name = "Bear" , Description = "Hello Mr Cluedo. How are you today?"}  },
                        {"Look at board", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Board, Name= "Board" , Description = "Cute photos, this note seems to have a code, wonder what it does."} },
                        {"Open drawer", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Table, Name= "Drawer" , Description = "It needs a key to open."} },
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
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Open door", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Door,Name = "Door", Description = "Hm, It's locked. I think it needs a code." }  },
                        {"Enter code", new ObjectInterations(){ InteractionType= InteractionTypeEnum.DoorCode,Name = "Door" , Description = "What's the code?"} },
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
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Inspect window", new ObjectInterations(){InteractionType = InteractionTypeEnum.Window,Name = "Window", Description = "Ew so much mold...someone obviously hasn't cleaned in a while." } },
                        {"Open window", new ObjectInterations(){InteractionType = InteractionTypeEnum.WindowOpen, Name = "Window", Description = "It's locked. Doesn't seem like a great escape plan anyway. Would hurt falling down that far...or would it? I don't even touch the ground." } }
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

            _laptopTexture = Content.Load<Texture2D>("Textures/Puzzle UI/laptop"); // ui

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
                new Rectangle(0, 0, w, h),
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
                    Interactions.Add(item, item.Interaction);
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
