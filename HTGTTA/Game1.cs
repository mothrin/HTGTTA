﻿using HTGTTA.Enums;
using HTGTTA.Models;
using HTGTTA.Scenes;
using HTGTTA.Scenes.StartingScreens;
using HTGTTA.Services;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Randomchaos.Services.Audio;
using MonoGame.Randomchaos.Services.Coroutine;
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
        ICoroutineService coroutineService { get { return Services.GetService<ICoroutineService>(); } }

        IInputStateService inputService;
        IKeyboardStateManager kbState;
        IMouseStateManager mouseStateManager;

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

            // Add the game service for global variables.
            Services.AddService(new HTGTTAService());

            // Set up coroutine service
            new CoroutineService(this);

            //audio
            new AudioService(this);

            //input
            kbState = new KeyboardStateManager(this);
            mouseStateManager = new MouseStateManager(this);
            inputService = new InputHandlerService(this, kbState, mouseStateManager);


            //scenes
            new SceneService(this);

        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.

        protected override void Initialize()
        {
            //scenes
            sceneService.AddScene(new GameScene(this, "Game"));
            sceneService.AddScene(new MainMenu(this, "mainMenu"));
            sceneService.AddScene(new Options(this, "Options"));
            sceneService.AddScene(new Timer(this, "Timer"));
            sceneService.AddScene(new Controls(this, "Controls"));
            sceneService.AddScene(new Help(this, "Help"));
            sceneService.AddScene(new Ending(this, "Ending","mainMenu"));
            sceneService.AddScene(new Ending2(this, "Ending2", "mainMenu"));
            sceneService.AddScene(new Trapdoor(this, "trapdoor", "Ending3"));
            sceneService.AddScene(new Ending3(this, "Ending3", "mainMenu"));
            sceneService.AddScene(new Ending4(this, "Ending4", "mainMenu"));
            sceneService.AddScene(new Ending5(this, "Ending5", "mainMenu"));
            sceneService.AddScene(new Intro1(this, "Intro1", "Intro2"));
            sceneService.AddScene(new Intro2(this, "Intro2", "Intro3"));
            sceneService.AddScene(new Intro3(this, "Intro3", "Intro4"));
            sceneService.AddScene(new Intro4(this, "Intro4", "Game"));



            base.Initialize();
        }

        /// LoadContent will be called once per game and is the place to load all of your content.

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sceneService.LoadScene("mainMenu"); //starting screen (could be changed to title or splash screen in future

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

        protected override void EndDraw() // needed for the coroutne service
        {
            base.EndDraw();

            coroutineService.UpdateEndFrame(null);
        }
    }
}