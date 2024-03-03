using HTGTTA.Enums;
using HTGTTA.Models;
using HTGTTA.Scenes;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Randomchaos.Services.Audio;
using MonoGame.Randomchaos.Services.Input;
using MonoGame.Randomchaos.Services.Input.Models;
using MonoGame.Randomchaos.Services.Interfaces;
using MonoGame.Randomchaos.Services.Scene.Services;
using MonoGame.Randomchaos.UI;
using System;
using System.Collections.Generic;

namespace HTGTTA
{
    public class Game1 : Game
    {

        public int w;
        public int h;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ISceneService sceneService { get { return Services.GetService<ISceneService>(); } }

        IInputStateService inputService;
        IKeyboardStateManager kbState;
        public Game1()
        {
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
            sceneService.AddScene(new GameScene(this, "Game"));
            sceneService.AddScene(new MainMenu(this, "mainMenu"));




            base.Initialize();
        }

        /// LoadContent will be called once per game and is the place to load all of your content.

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


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
        }

        /// This is called when the game should draw itself.

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            base.Draw(gameTime);
        }
    }
}