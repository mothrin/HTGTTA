using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Randomchaos.Extensions;
using MonoGame.Randomchaos.Services.Interfaces;
using MonoGame.Randomchaos.Services.Interfaces.Enums;
using MonoGame.Randomchaos.Services.Scene.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Scenes
{
    public class MainMenu : SceneFadeBase
    {

        Texture2D _bgTexture;
        Texture2D _titleBar;
        Texture2D _backgroundTexture;

        private SpriteFont _font;
        bool exiting;
        protected string NextScene;

        #region packages
        private IKeyboardStateManager _kbState { get { return Game.Services.GetService<IInputStateService>().KeyboardManager; } }
        private IMouseStateManager _msState { get { return Game.Services.GetService<IInputStateService>().MouseManager; } }
        private IInputStateService inputService { get { return Game.Services.GetService<IInputStateService>(); } }
        private ISceneService sceneService { get { return Game.Services.GetService<ISceneService>(); } }
        #endregion

        Dictionary<string, Rectangle> ButtonBounds = new Dictionary<string, Rectangle>();
        public MainMenu(Game game, string name) : base(game,name) { }


        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            // = new SpriteBatch(GraphicsDevice); // for screen switching
            _font = Game.Content.LoadLocalized<SpriteFont>("Fonts/UIFont");
            //_spritebatch = new SpriteBatch(GraphicsDevice); // for ui
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (State == SceneStateEnum.Loaded)
            {
                if (kbManager.KeyPress(Microsoft.Xna.Framework.Input.Keys.F10))
                {
                    exiting = true;
                    State = SceneStateEnum.Unloading;
                    UnloadScene();
                }

            }

            base.Update(gameTime);

            if (State == SceneStateEnum.Unloaded && exiting)
                Game.Exit();
        }
        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap);

            _spriteBatch.Draw(Game.Content.Load<Texture2D>("Screens/mainmenu"), new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),Color.White);

            MenuButtons();

            _spriteBatch.End();


            base.Draw(gameTime);


            DrawFader(gameTime);
        }

        public void MenuButtons()
        {
            Point keySize = new Point(400, 100);
            Point keyStartPos = new Point((GraphicsDevice.Viewport.Width/2 - 200), 400);
            string keyText = "";


            Vector2 strSize = _font.MeasureString(keyText);
            for (int i = 0; i < 3; i++)
            {
                Point keyPos = keyStartPos;

                Color keyColor = Color.Gray;
                Color keyBorder = Color.DimGray;
                if (i == 0)
                {
                    keyText = "Start New Game";
                }
                if (i == 1)
                {
                    keyText = "Options";
                    keyPos.Y += (keySize.Y + 25);
                }
                if (i == 2)
                {
                    keyText = "Quit";
                    keyPos.Y += (keySize.Y + 150);
                }

                if (!ButtonBounds.ContainsKey(keyText))
                {
                    ButtonBounds.Add(keyText, new Rectangle(keyPos.X, keyPos.Y, 400, 100));
                }
                if (_msState.PositionRect.Intersects(ButtonBounds[keyText])) // Check mouse over and if it is button click event.
                {
                    keyColor = Color.LavenderBlush;
                    keyBorder = Color.Gray;
                }

                strSize = _font.MeasureString(keyText) / 2;

                DrawBox(keySize, (keyPos + new Point(-5, 7)), Color.Black, Color.Black, 1);
                DrawBox(keySize, keyPos, keyColor, keyBorder, 1);
                DrawString(keyText, (keyPos + new Point(200, 50)).ToVector2() - strSize, Color.Navy);

                foreach (var button in ButtonBounds.Keys)
                {
                    if (_msState.PositionRect.Intersects(ButtonBounds[button]) && _msState.LeftClicked)
                    {
                        if (button == "Start New Game")
                        {
                            sceneManager.LoadScene("Game");
                        }
                        if (button == "Options")
                        {
                            sceneManager.LoadScene("Options");
                        }
                        if (button == "Quit")
                        {
                            exiting = true;
                            State = SceneStateEnum.Unloading;
                            UnloadScene();
                        }
                    }
                }
            }
        }

        //UI
        protected void DrawBox(Point size, Point pos, Color bgColor, Color borderCour, int borderThickness)
        {
            _bgTexture = new Texture2D(GraphicsDevice, size.X, size.Y);
            _bgTexture.FillWithBorder(bgColor, borderCour, new Rectangle(borderThickness, borderThickness, borderThickness, borderThickness));

            _spriteBatch.Draw(_bgTexture, new Rectangle(pos.X, pos.Y, size.X, size.Y), Color.White);
        }
        protected void DrawWindowBase(Point size, Point pos, Color bgColor, Color borderCour, int borderThickness, string title, Color titleBgColor, Color? titleTextColor = null)
        {
            if (titleTextColor == null)
            {
                titleTextColor = Color.Navy;
            }

            _titleBar = new Texture2D(GraphicsDevice, size.X, 40);
            _titleBar.FillWithBorder(titleBgColor, borderCour, new Rectangle(borderThickness, borderThickness, borderThickness, borderThickness));

            DrawBox(size, pos, bgColor, borderCour, borderThickness);
            _spriteBatch.Draw(_titleBar, new Rectangle(pos.X, pos.Y, size.X, _titleBar.Height), Color.White);

            string text = title;

            Vector2 textSize = _font.MeasureString(text);
            Vector2 center = new Vector2(pos.X / 2, pos.Y);

            Vector2 txtPos = (pos.ToVector2() + new Vector2(size.X / 2, 0)) - (new Vector2(textSize.X, _titleBar.Height / 4) * .5f);

            DrawString(text, txtPos, titleTextColor.Value);
        }
        protected void DrawString(string text, Vector2 pos, Color color, SpriteFont font = null)
        {
            if (font == null)
            {
                font = _font;
            }

            Vector2 shadow = Vector2.One * -1;
            _spriteBatch.DrawString(font, text, pos, Color.Gray);
            _spriteBatch.DrawString(font, text, pos + shadow, color);
        }
    }
}
