using HTGTTA.Enums;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Randomchaos.Extensions;
using MonoGame.Randomchaos.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HTGTTA.Models
{
    public class Hud : DrawableGameComponent
    {
        SpriteBatch _spritebatch;
        SpriteFont _font;

        Texture2D _bgTexture;
        Texture2D _titleBar;

        ObjectInterations interactionToDo = null;

        IKeyboardStateManager _kbState { get { return Game.Services.GetService<IInputStateService>().KeyboardManager; } }

        public Dictionary<string, ObjectInterations> CurrentInteractions = null;

        public Hud(Game game) : base(game)
        {

        }

        protected override void LoadContent()
        {
            _spritebatch = new SpriteBatch(GraphicsDevice);
            _font = Game.Content.LoadLocalized<SpriteFont>("Fonts/UIFont");


            base.LoadContent();
        }

        public void ShowInteractionOptionsWindow(Dictionary<Sprite, Dictionary<string, ObjectInterations>> interactions)
        {
            Point winSize = new Point(450, 84);
            Point winPos = new Point(8, 8);

            _spritebatch.Begin();
            DrawWindowBase(winSize,
                  winPos,
                   new Color(.5f, .5f, .5f, .5f),
                   Color.Black, 2,
                   "Interactions",
                   new Color(.5f, .5f, .9f, .5f));
           
            Vector2 center = new Vector2(winPos.X / 2, winPos.Y);

            Vector2 p = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height/4) *.5f);

            int idx = 1;
            p = new Vector2(22, p.Y + 40);
            foreach (Sprite interationObject in interactions.Keys)
            {
                string text = $"Press E to interact with {interationObject.Name}";

                DrawString(text, p, Color.Green);

                p.Y += _font.LineSpacing * idx;

                idx++;

                if (_kbState.KeyPress(Keys.E))
                {
                    interactionToDo = null;
                    CurrentInteractions = interactions[interationObject];
                }
            }


            _spritebatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentInteractions != null)
            {
                string key = null;
                if (_kbState.KeyDown(Keys.D1) && CurrentInteractions.Count > 0)
                {
                    key = CurrentInteractions.Keys.ElementAt(0);
                    interactionToDo = CurrentInteractions[key];
                }

                if (_kbState.KeyDown(Keys.D2) && CurrentInteractions.Count > 1)
                {
                    key = CurrentInteractions.Keys.ElementAt(1);
                    interactionToDo = CurrentInteractions[key];
                }

                if (_kbState.KeyDown(Keys.D3) && CurrentInteractions.Count > 2)
                {
                    key = CurrentInteractions.Keys.ElementAt(2);
                    interactionToDo = CurrentInteractions[key];
                }

                if (interactionToDo != null && interactionToDo.InteractionType == InteractionTypeEnum.Nothing)
                {
                    interactionToDo = null;
                }
            }
        }

        protected void DoInteraction(ObjectInterations interaction, int startYPos)
        {
            string textToPrint = string.Empty;

            bool userInputRequired = false;

            switch (interaction.InteractionType)
            {
                case InteractionTypeEnum.LaptopCodeEnter:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.LaptopCodeEnter;
                    break;

            }

            Point winSize = new Point(450, 64 + CurrentInteractions.Count * _font.LineSpacing);
            Point winPos = new Point(8, 16 + startYPos);

            
            DrawWindowBase(winSize,
                  winPos,
                   new Color(.5f, .5f, .5f, .5f),
                   Color.Black, 2,
                   string.IsNullOrEmpty(interaction.Name) ? "FIX THIS MOTH" : interaction.Name,
                   new Color(.5f, .5f, .9f, .5f));

            Vector2 textSize = _font.MeasureString(textToPrint);

            Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);
            txtPos = new Vector2(16, txtPos.Y + _font.LineSpacing);
            DrawString(textToPrint, txtPos, Color.Gold);

            if (userInputRequired)
            {
                // Pop up a text box :P
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (CurrentInteractions != null)
            {
                _spritebatch.Begin();

                Point winSize = new Point(450, 32 + CurrentInteractions.Count * _font.LineSpacing);
                Point winPos = new Point(8, 8 + 100);

                DrawWindowBase(winSize,
                   winPos,
                    new Color(.5f, .5f, .5f, .5f),
                    Color.Black,2,
                    "What do you want to do?",
                    new Color(.5f, .5f, .9f, .5f));


                Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);
                txtPos.X = 16;

                int idx = 1;
                foreach (string key in CurrentInteractions.Keys)
                {
                    string text = $"Press {idx} to interact with {key}";

                    if (CurrentInteractions[key].InteractionType == InteractionTypeEnum.Nothing)
                    {
                        text = "Nothing to do here....";
                    }

                    Vector2 textSize = _font.MeasureString(text);
                    txtPos += new Vector2(0, _font.LineSpacing);

                    idx++;

                    DrawString(text, txtPos, Color.Blue);
                }

                if (interactionToDo != null)
                {
                    DoInteraction(interactionToDo, winPos.Y + winSize.Y);
                }

                _spritebatch.End();
            }
        }

        protected void DrawWindowBase(Point size, Point pos, Color bgColor, Color borderCour, int borderThickness, string title, Color titleBgColor)
        {
            Point winSize = size;
            Point winPos = pos;

            _bgTexture = new Texture2D(GraphicsDevice, winSize.X, winSize.Y);
            _bgTexture.FillWithBorder(bgColor, borderCour, new Rectangle(borderThickness, borderThickness, borderThickness, borderThickness));

            _titleBar = new Texture2D(GraphicsDevice, winSize.X, 32);
            _titleBar.FillWithBorder(titleBgColor, borderCour, new Rectangle(borderThickness, borderThickness, borderThickness, borderThickness));

            _spritebatch.Draw(_bgTexture, new Rectangle(winPos.X, winPos.Y, winSize.X, winSize.Y), Color.White);
            _spritebatch.Draw(_titleBar, new Rectangle(winPos.X, winPos.Y, winSize.X, _titleBar.Height), Color.White);

            string text = title;

            Vector2 textSize = _font.MeasureString(text);
            Vector2 center = new Vector2(winPos.X / 2, winPos.Y);

            Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(textSize.X, _titleBar.Height / 4) * .5f);

            DrawString(text, txtPos, Color.Green);
        }

        protected void DrawString(string text, Vector2 pos, Color color)
        {
            Vector2 shadow = Vector2.One * -1;
            _spritebatch.DrawString(_font, text, pos, color);
            _spritebatch.DrawString(_font, text, pos + shadow, Color.Green);
        }
    }
}
