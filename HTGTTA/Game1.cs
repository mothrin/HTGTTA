using HTGTTA.Models;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here


            base.Initialize();
        }

        /// LoadContent will be called once per game and is the place to load all of your content.

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            one = Content.Load<Texture2D>("bed");
            _backgroundTexture = Content.Load<Texture2D>("background");

            var animations = new Dictionary<string, Animation>()
            {
                { "WalkUp", new Animation(Content.Load<Texture2D>("back"), 3) },
                { "WalkDown", new Animation(Content.Load<Texture2D>("front"), 3) },
                { "WalkLeft", new Animation(Content.Load<Texture2D>("left"), 3) },
                { "WalkRight", new Animation(Content.Load<Texture2D>("right"), 3) },
            };

            _sprites = new List<Sprite>()
            {
                
                new Player(animations)
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
                new Sprite(one)
                {
                    Position = new Vector2(20,10),
                
                },
            };
            

        }


        /// UnloadContent will be called once per game and is the place to unload game-specific content

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

       
        /// Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio

        protected override void Update(GameTime gameTime)
        {

            foreach (var sprite in _sprites)
                sprite.Update(gameTime, _sprites);

            base.Update(gameTime);
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

            foreach (var sprite in _sprites)
                sprite.Draw(spriteBatch);

                

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}