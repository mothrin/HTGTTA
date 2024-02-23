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
using System;
using System.Collections.Generic;
using System.Xml.Linq;

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

        public string[] Interaction;

        private Player player;

        public List<Sprite> _items;


        Vector2 lastPosition; //collision

        // packages
        /// <summary>   The input service. </summary>
        IInputStateService inputService;
        /// <summary>   State of the kB. </summary>
        IKeyboardStateManager kbState;
        private int i;

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
                new Sprite(this, "Textures/Objects/Blank")
                {
                    Name = "Bed1",
                    Description = "We should probably leave that be...",
                    
                    Position = new Vector2(1500,273),
                    Width = 278,
                    Height = 328,
                    RenderBounds = true, //for bounds
                    RenderInteractionBounds = true,
                },
                new Sprite(this, "Textures/Objects/Blank")
                {
                    Name = "Bed",
                    Description = "We should probably leave that be...",
                    Position = new Vector2(1540,400),
                    Width = 278,
                    Height = 328,
                    RenderBounds = true, //for bounds
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
                },
                new Sprite (this, "Textures/Objects/Blank")
                {
                    Name = "Desk",
                    Description = "The laptop is locked",
                    Interaction = new string [] {"Desk","Laptop"},
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
                    Interaction = new string [] {"Bear","Drawer"},
                    Position = new Vector2(483,220),
                    Width = 345,
                    Height = 280,
                    RenderBounds = true, //bounds
                    RenderInteractionBounds = true,
                },
                new Sprite(this,"Textures/Objects/Blank")
                {
                    Name = "Wardrobe",
                    Description = "There are too many clothes here, I can't open the door.",
                    Interaction = new string [] {"Door","Clothes","Box"},
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
                    Interaction = new string [] {"Bear","Drawer"},
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
                    Interaction = new string [] {"Door","Code"},
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
                    Interaction = new string [] {"Window","Open"},
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

            //for object interaction
            foreach (var item in _items) 
            {
                _font = Content.Load<SpriteFont>("Fonts/font");
                if (player.InteractionBounds.Intersects(item.InteractionBounds))
                {
                    string textToPrint = $"E to interact with [{item.Name}]";
                    Vector2 textSize = _font.MeasureString(textToPrint);
                    Vector2 txtPos = player.Position + (new Vector2(player.Width / 2, _font.LineSpacing * -3) - (textSize * .5f));

                    

                    spriteBatch.Begin();
                    spriteBatch.DrawString(_font, textToPrint, txtPos, Color.Black);
                    spriteBatch.DrawString(_font, textToPrint, txtPos + new Vector2(-1, -1), Color.White);
                    //(_textbox.Draw(_font, textToPrint, txtPos + new Vector2(-1, -1), Color.White)

                    for (int i=0; i < 10; i++)
                    {
                        spriteBatch.DrawString(_font, Interaction[i], txtPos + new Vector2(0, 0), Color.White);
                    }


                    if (inputService.KeyboardManager.KeyDown(Keys.E))
                    {
                        textToPrint = $"{item.Name} - {item.Description}";
                        textSize = _font.MeasureString(textToPrint);
                        txtPos = player.Position + (new Vector2(player.Width / 2, _font.LineSpacing * -2) - (textSize * .5f));

                        spriteBatch.DrawString(_font, textToPrint, txtPos, Color.Black);
                    }
                    spriteBatch.End();
                }

             
            }
            

        }
    }
}