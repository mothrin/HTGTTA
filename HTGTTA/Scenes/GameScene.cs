using HTGTTA.Enums;
using HTGTTA.Models;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Randomchaos.Services.Interfaces;
using MonoGame.Randomchaos.Services.Interfaces.Enums;
using MonoGame.Randomchaos.Services.Scene.Models;
using System.Collections.Generic;

namespace HTGTTA.Scenes
{
    public class GameScene : SceneFadeBase
    {
        public int w;
        public int h;


        protected SpriteFont _font;

        private Texture2D _backgroundTexture;
        

        public Dictionary<Sprite, Dictionary<string, ObjectInterations>> Interactions = new Dictionary<Sprite, Dictionary<string, ObjectInterations>>();

        private Player player;

        public List<Sprite> _items;

        public List<Sprite> _inventory;

        public bool laptopOpened;
        public bool diaryRead;
        public bool chairGot;
        public bool chairplaced;
        public bool boxOpened;
        public bool keyGot;
        public bool drawerOpened;
        public bool paperRead;
        public bool clothesMoved;


        Vector2 lastPosition; //collision

        // packages
        /// <summary>   The input service. </summary>
        IInputStateService inputService { get { return Game.Services.GetService<IInputStateService>(); } }
        /// <summary>   State of the kB. </summary>
        IKeyboardStateManager kbState { get { return inputService.KeyboardManager; } }

        protected Hud HUD;

        private IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }

        ISceneService sceneService { get { return Game.Services.GetService<ISceneService>(); } }


        public GameScene(Game game, string name) : base(game, name)
        {
            w = GraphicsDevice.Viewport.Width;
            h = GraphicsDevice.Viewport.Height;
        }

        public override void LoadScene()
        {
            var animations = new Dictionary<string, Animation>()
            {
                { "WalkUp", new Animation(Game.Content.Load<Texture2D>("Textures/Sprite/back"), 3) },
                { "WalkDown", new Animation(Game.Content.Load<Texture2D>("Textures/Sprite/front"), 3) },
                { "WalkLeft", new Animation(Game.Content.Load<Texture2D>("Textures/Sprite/left"), 3) },
                { "WalkRight", new Animation(Game.Content.Load<Texture2D>("Textures/Sprite/right"), 3) },
            };

            // TODO: Add your initialization logic here
            Dictionary<string, ObjectInterations> nothingToDo = new Dictionary<string, ObjectInterations>()
            {
                {"Nothing to do here", new ObjectInterations(){ Active = true, Description = "Nothing to do here", Name = "Nothing to do.", InteractionType = InteractionTypeEnum.Nothing } }
            };

            //objects
            _items = new List<Sprite>()
            {
                new Sprite(Game, "Textures/Objects/Blank")
                {
                    Name = "Bed1",
                    Position = new Vector2(1500,273),
                    Width = 278,
                    Height = 328,
                    RenderBounds = true, //for bounds
                    RenderInteractionBounds = true,
                    RenderCoords = true,
                    Interaction = nothingToDo,
                },
                new Sprite(Game, "Textures/Objects/Blank")
                {
                    Name = "Bed",
                    Description = "We should probably leave that be...",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Look at bed", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Bed,Name = "Bed", Description = "Maybe we should leave this be..." }  },
                        {"Go to bed", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Sleep,Name = "Bed", Description = "I'm so so tired. Should i just go back to bed?" }  },
                        {"Read diary", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Diary, Name ="Diary",  Description = "I don't think i should read that." }  }
                    },
                    Position = new Vector2(1540,400),
                    Width = 278,
                    Height = 328,
                    RenderBounds = true, //for bounds
                    RenderInteractionBounds = true,
                    RenderCoords = true,
                },
                new Sprite(Game, "Textures/Objects/Blank")
                {
                    Name = "Bed",
                    Position = new Vector2(1580,527),
                    Width = 278,
                    Height = 328,
                    RenderBounds = true, //for bounds
                    RenderInteractionBounds = true,
                    RenderCoords = true,
                    Interaction = nothingToDo,
                },
                new Sprite (Game, "Textures/Objects/Blank")
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
                    RenderCoords = true,
                },
                new Sprite(Game,"Textures/Objects/Blank")
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
                    RenderCoords = true,
                },
                new Sprite(Game,"Textures/Objects/Blank")
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
                    RenderCoords = true,
                },
                new Sprite(Game,"Textures/Objects/Blank")
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
                    RenderCoords = true,
                },
                new Sprite(Game,"Textures/Objects/Blank")
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
                    RenderCoords = true,
                },
                new Sprite(Game,"Textures/Objects/Blank")
                {
                    Name = "Window",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Inspect window", new ObjectInterations(){InteractionType = InteractionTypeEnum.Window,Name = "Window", Description = "Ew so much mold...someone obviously hasn't cleaned in a while." } },
                        {"Open window", new ObjectInterations(){InteractionType = InteractionTypeEnum.WindowOpen, Name = "Window", Description = "It's locked. Doesn't seem like a great escape plan anyway.\nWould hurt falling down that far...or would it? I don't even touch the ground." } }
                    },
                    Position = new Vector2(0,105),
                    Width = 65,
                    Height = 560,
                    RenderBounds = true, //bounds
                    RenderInteractionBounds = true,
                    RenderCoords = true,
                }
            };

            foreach (var item in _items)
            {
                Components.Add(item);
            }

            //player
            player = new Player(Game, animations)
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
                RenderCoords = true,
            };
            Components.Add(player);

            //ui windows
            HUD = new Hud(Game);
            Components.Add(HUD);

            base.LoadScene();
        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.

        public override void Initialize()
        {
            base.Initialize();
        }

        /// LoadContent will be called once per game and is the place to load all of your content.

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundTexture = Game.Content.Load<Texture2D>("Textures/backgrounds/background"); //background

            _audio.PlaySong("Audio/Music/Drafty-Places", .05f); //music

            

            base.LoadContent();
        }


        /// UnloadContent will be called once per game and is the place to unload game-specific content

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio

        public override void Update(GameTime gameTime)
        {

            if (State == SceneStateEnum.Loaded)
            {
                lastPosition = player.Position;
            }
             
            base.Update(gameTime);
            
            if (State == SceneStateEnum.Loaded)
            {
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
            }
            if (State == SceneStateEnum.Loaded)
            {
                if (kbManager.KeyPress(Microsoft.Xna.Framework.Input.Keys.F2))
                    sceneManager.LoadScene("mainMenu");
            }

        }

        /// This is called when the game should draw itself.

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            _spriteBatch.Draw(_backgroundTexture,
                new Rectangle(0, 0, w, h),
                new Rectangle(0, 0, _backgroundTexture.Width, _backgroundTexture.Height),
               Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);

            Interactions.Clear();

            //for object interaction
            foreach (var item in _items)
            {
                _font = Game.Content.Load<SpriteFont>("Fonts/font");
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

            DrawFader(gameTime);
        }


    }
}
