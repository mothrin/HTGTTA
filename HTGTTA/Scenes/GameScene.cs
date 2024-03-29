﻿using HTGTTA.Enums;
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
        public int w; //window width 
        public int h; //window height


        protected SpriteFont _font;
        protected SpriteFont _uiFont;
        protected SpriteFont _buttonfont;

        private Texture2D _backgroundTexture;
        public Texture2D chairTexture;
        public Texture2D chair2Texture;


        public Dictionary<Sprite, Dictionary<string, ObjectInterations>> Interactions = new Dictionary<Sprite, Dictionary<string, ObjectInterations>>();

        private Player player;

        public List<Sprite> _items;

        private List<Object> _objects;

        public bool choiceExit; //exit game

        protected bool UIup;

        Vector2 lastPosition; //collision repsonse (doesn't allow player to move further)


        #region packages
        // packages
        /// <summary>   The input service. </summary>
        IInputStateService inputService { get { return Game.Services.GetService<IInputStateService>(); } }
        /// <summary>   State of the kB. </summary>
        IKeyboardStateManager kbState { get { return inputService.KeyboardManager; } }
        #endregion


        private IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }

        ISceneService sceneService { get { return Game.Services.GetService<ISceneService>(); } }

        protected Hud HUD;
        protected GamePlayTimer GamePlayTimer;
        protected bool ConfirmGameExit = false;

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

            Dictionary<string, ObjectInterations> nothingToDo = new Dictionary<string, ObjectInterations>()
            {
                {"Nothing to do here", new ObjectInterations(){ Active = true, Description = "Nothing to do here", Name = "Nothing to do.", InteractionType = InteractionTypeEnum.Nothing } }
            };

            //objects on screen
            _items = new List<Sprite>()
            {
                new Sprite(Game, "Textures/Objects/Blank")
                {
                    Name = "Bed1",
                    Position = new Vector2(1500,273),
                    Width = 278,
                    Height = 328,
                    Interaction = nothingToDo,
                },
                new Sprite(Game, "Textures/Objects/Blank")
                {
                    Name = "Bed",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Look at bed", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Bed,Name = "Bed", Description = "Maybe we should leave this be..." }  },
                        {"Go to bed", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Sleep,Name = "Bed", Description = "I'm so so tired. Should i just go back to bed?" }  },
                        {"Read diary", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Diary, Name ="Diary",  Description = "I don't think i should read that." }  }
                    },
                    Position = new Vector2(1540,400),
                    Width = 278,
                    Height = 328,
                },
                new Sprite(Game, "Textures/Objects/Blank")
                {
                    Name = "Bed",
                    Interaction = new Dictionary<string, ObjectInterations> ()
                    {
                        {"Look at bed", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Bed,Name = "Bed", Description = "Maybe we should leave this be..." }  },
                        {"Go to bed", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Sleep,Name = "Bed", Description = "I'm so so tired. Should I just go back to bed?" }  }
                    },
                    Position = new Vector2(1580,527),
                    Width = 278,
                    Height = 328,
                },
                new Sprite (Game, "Textures/Objects/Blank")
                {
                    Name = "Desk",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Look at desk", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Desk,Name = "Desk", Description = "Hm, just loads of books." }  },
                        {"Look at chair", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Chair,Name = "Chair", Description = "Can't do anything with that." }  },
                        {"Open Laptop", new ObjectInterations(){ InteractionType = InteractionTypeEnum.LaptopCodeEnter, Name ="Laptop",  Description = "Needs a password. I don't remember what it as." }  },
                    },
                    Position = new Vector2(102,972),
                    Width = 570,
                    Height = 75,
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
                },
                new Sprite(Game,"Textures/Objects/Blank")
                {
                    Name = "Wardrobe",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Look at wardrobe", new ObjectInterations(){ InteractionType = InteractionTypeEnum.WarDoorOpen,Name = "Door",Description = "There are too many clothes here, I can't open the door.", }  },
                        {"Move clothes", new ObjectInterations(){ InteractionType = InteractionTypeEnum.ClothesMoved,Name = "Clothes", Description = "No point in moving these, waste of energy..." }  },
                        {"Look in box", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Box,Name = "Box",  Description = "I can't reach that.",}  },
                    },
                    Position = new Vector2(858,30),
                    Width = 288,
                    Height = 450,
                },
                new Sprite(Game,"Textures/Objects/Blank")
                {
                    Name = "Bedside Table",
                    Interaction = new Dictionary<string, ObjectInterations>()
                    {
                        {"Talk to bear", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Bear,Name = "Bear" , Description = "Hello, Mr Cluedo. How are you today?"}  },
                        {"Look at board", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Board, Name= "Board" , Description = "Cute photos, this note seems to have a code! 2215. Wonder what it's for."} },
                        {"Open drawer", new ObjectInterations(){ InteractionType = InteractionTypeEnum.Table, Name= "Drawer" , Description = "It needs a key to open."} },
                    },
                    Position = new Vector2(1284,348),
                    Width = 171,
                    Height = 150,
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
            };
            Components.Add(player);

            chairTexture = Game.Content.Load<Texture2D>("Textures/Objects/Chair");
            chair2Texture = Game.Content.Load<Texture2D>("Textures/Objects/Blank");

            Texture2D bookTexture = Game.Content.Load<Texture2D>("Textures/Objects/books");
            _objects = new List<Object>()
            {
                new Object(Game,chairTexture)
                {
                    Name = "Chair",
                    Position = new Vector2(275,790),
                    Width = 220,
                    Height = 185,
                },
                new Object(Game,bookTexture)
                {
                    Name = "Books",
                    Position = new Vector2(115,915),
                    Width = 140,
                    Height = 130,
                },
                new Object(Game,chair2Texture)
                {
                    Name = "PlaceChair",
                    Position = new Vector2(940, 330),
                    Width = 170,
                    Height = 270,

                }
            };

            foreach (var thing in _objects)
            {
                Components.Add(thing);
            }

            // Added last so it is rendered over the top of all others
            HUD = new Hud(Game);
            HUD.HidableSprites.AddRange(_objects); //for chair
            Components.Add(HUD);

            GamePlayTimer = new GamePlayTimer(Game);
            
            Components.Add(GamePlayTimer);

            GamePlayTimer.StartTimer();

            base.LoadScene();
        }

        public override void UnloadScene()
        {
            GamePlayTimer.StopTimer();
            base.UnloadScene();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundTexture = Game.Content.Load<Texture2D>("Textures/backgrounds/background2"); //background

            _audio.PlaySong("Audio/Music/Drafty-Places", .005f); //music



            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public override void Update(GameTime gameTime)
        {
            if (State == SceneStateEnum.Loaded)
            {
                lastPosition = player.Position;
            }

            base.Update(gameTime);

            if (State == SceneStateEnum.Loaded)
            {
                // Disable the player when the game is paused so they can't move about..
                player.Enabled = !GamePlayTimer.IsPaused && !ConfirmGameExit;

                //these start endings when variables are true
                if (HUD.DoorOpened)
                {
                    sceneManager.LoadScene("Ending");
                }
                if (HUD.SleepYes)
                {
                    sceneManager.LoadScene("Ending2");
                }
                if(HUD.LeaveTrapdoor)
                {
                    sceneManager.LoadScene("trapdoor"); //ending 3
                }
                if (HUD.LeaveWindow)
                {
                    sceneManager.LoadScene("Ending5");
                }
                if (GamePlayTimer.IsOutOfTime)
                {
                    sceneManager.LoadScene("Ending4");
                }


                if (!GamePlayTimer.IsPaused)
                {
                    //so player can't walk into objects
                    foreach (var item in _items)
                    {
                        if (player.Bounds.Intersects(item.Bounds))
                        {
                            player.Position = lastPosition;
                        }
                        if(HUD.UIup || HUD.Choice)
                        {
                            player.Position = lastPosition;
                        }
                    }
                    //for debugging purposes
                    if (inputService.KeyboardManager.KeyPress(Keys.F10))
                    {
                        Sprite.BondsOn = !Sprite.BondsOn;
                    }
                    //closes game
                    if (kbManager.KeyPress(Keys.F1))
                    {
                        //sceneManager.LoadScene("Options");
                        ConfirmGameExit = true;
                    }

                    if (ConfirmGameExit)
                    {
                        if (kbManager.KeyPress(Keys.Y))
                        {
                            sceneManager.LoadScene("mainMenu");
                            ConfirmGameExit = false;
                        }
                        if (kbManager.KeyPress(Keys.N))
                        {
                            ConfirmGameExit = false;
                        }
                    }
                }
                //pauses game time
                if (!ConfirmGameExit && kbManager.KeyPress(Keys.P))
                {
                    GamePlayTimer.IsPaused = !GamePlayTimer.IsPaused;
                }

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
                _uiFont = Game.Content.Load<SpriteFont>("Fonts/UIfont");
                _buttonfont = Game.Content.Load<SpriteFont>("Fonts/DoorButtons");
                if (player.InteractionBounds.Intersects(item.InteractionBounds))
                {
                    Interactions.Add(item, item.Interaction);
                }
            }

            if (!GamePlayTimer.IsPaused)
            {
                if (Interactions.Count > 0)
                {
                    HUD.ShowInteractionOptionsWindow(Interactions);
                }
                else
                {
                    HUD.CurrentInteractions = null;
                }

                if (ConfirmGameExit)
                {
                    Texture2D tmpgb = new Texture2D(GraphicsDevice, 1, 1);
                    tmpgb.SetData(new Color[] { new Color(0, 0, 0, .85f) });
                    _spriteBatch.Begin();

                    _spriteBatch.Draw(tmpgb, new Rectangle(0, 0, w,h), Color.White);

                    string exitMessage = "Are you sure you want to quit the game?";
 
                    float length = _uiFont.MeasureString(exitMessage).X / 2;

                    _spriteBatch.DrawString(_uiFont, exitMessage, (new Vector2(w, h) / 2) - new Vector2(length, 100), Color.Lavender);


                    exitMessage = "Y - Yes, N - No";
                    length = _uiFont.MeasureString(exitMessage).X / 2;

                    _spriteBatch.DrawString(_uiFont, exitMessage, (new Vector2(w, h) / 2) - new Vector2(length, 50), Color.Lavender);
                    
                    _spriteBatch.End();
                }
            }

            DrawFader(gameTime);
        }
    }
}
