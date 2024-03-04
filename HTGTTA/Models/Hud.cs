using HTGTTA.Enums;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Randomchaos.Extensions;
using MonoGame.Randomchaos.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HTGTTA.Models
{
    public class Hud : DrawableGameComponent
    {
        SpriteBatch _spritebatch;
        SpriteFont _font;
       
        Texture2D _bgTexture;
        Texture2D _titleBar;

        ObjectInterations interactionToDo = null;
        int maxPinLength = 4;

        private  ISceneService sceneService { get { return Game.Services.GetService<ISceneService>(); } }
        private IKeyboardStateManager _kbState { get { return Game.Services.GetService<IInputStateService>().KeyboardManager; } }
        private IMouseStateManager _msState {get { return Game.Services.GetService< IInputStateService>().MouseManager; } }

        private IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }

        public Dictionary<string, ObjectInterations> CurrentInteractions = null;

        protected bool ShowKeyPad = false;

        Texture2D laptopBorder;

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
            Point winSize = new Point(500, 100);
            Point winPos = new Point(8, 8);

            _spritebatch.Begin();

            //interact box
            DrawWindowBase(winSize,
                  winPos,
                   new Color(.5f, .5f, .5f, .5f),
                   Color.Black, 2,
                   "Interactions",
                   new Color(1f, 1f, 1.25f, 1f));
            //size, position, base window, border, border thickness, Title, header colour

            Vector2 center = new Vector2(winPos.X / 2, winPos.Y);

            Vector2 p = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height/4) *.5f);

            int idx = 1;
            p = new Vector2(22, p.Y + 40);
            foreach (Sprite interationObject in interactions.Keys)
            {
                string text = $"Press E to interact with {interationObject.Name}";

                DrawString(text, p, Color.Navy);

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

                foreach (var lapTopKey in keysBounds.Keys)
                {
                    if (_msState.PositionRect.Intersects(keysBounds[lapTopKey]) && _msState.LeftClicked)
                    {
                        if (lapTopKey != "Del." && lapTopKey != "Ent.")
                        {
                            if (currentPinValue.Length < maxPinLength)
                            {
                                _audio.PlaySFX("Audio/SFX/menu_select");
                                currentPinValue += lapTopKey;
                            }
                            else
                            {
                                _audio.PlaySFX("Audio/SFX/menu_cancel");
                            }
                        }
                        else
                        {
                            if (lapTopKey == "Del." && currentPinValue.Length > 0)
                            {
                                currentPinValue = currentPinValue.Substring(0, currentPinValue.Length - 1);
                                _audio.PlaySFX("Audio/SFX/menu_select");
                            }
                            else if (lapTopKey == "Ent.")
                            {
                                // Close the laptop.
                                ShowKeyPad = false;
                                interactionToDo = null;
                                _audio.PlaySFX("Audio/SFX/menu_change");

                                // Moth, in here you need to check if the code is correct, if it is, 
                                // when they go to open the laptop, they dont load the key pad, but see what's on the laptop
                            }
                            else
                            {
                                _audio.PlaySFX("Audio/SFX/menu_cancel");
                            }
                        }
                    }
                }
            }
        }

        protected void DoInteraction(ObjectInterations interaction, int startYPos)
        {
            string textToPrint = string.Empty;

            bool userInputRequired = false;

            switch (interaction.InteractionType)
            {
                //desk
                case InteractionTypeEnum.Desk:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Desk;
                    break;
                case InteractionTypeEnum.LaptopCodeEnter:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.LaptopCodeEnter;
                    userInputRequired = true;
                    if (!ShowKeyPad)
                    {
                        currentPinValue = string.Empty; // reset the pin value.
                    }
                    ShowKeyPad = true;
                    //sceneService.LoadScene("laptop");
                    break;
                case InteractionTypeEnum.Paper:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Paper;
                    userInputRequired = true;
                    break;
                //door
                case InteractionTypeEnum.Door:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Door;
                    break;
                case InteractionTypeEnum.DoorCode:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.DoorCode;
                    userInputRequired = true;
                    break;
                //window
                case InteractionTypeEnum.Window:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Window;
                    break;
                case InteractionTypeEnum.WindowOpen:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.WindowOpen;
                    break;
                //bed
                case InteractionTypeEnum.Bed:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Bed;
                    break;
                case InteractionTypeEnum.Diary:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Diary;
                    break;
                case InteractionTypeEnum.Sleep:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Sleep;
                    userInputRequired = true;
                    break;

                //wardrobe
                case InteractionTypeEnum.WarDoorOpen:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.WarDoorOpen;
                    break;
                case InteractionTypeEnum.ChairPlaced:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.ChairPlaced;
                    break;
                case InteractionTypeEnum.ClothesMoved:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.ClothesMoved;
                    break;

                //bedside table
                case InteractionTypeEnum.Board:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Board;
                    break;
                case InteractionTypeEnum.Table:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Table;
                    break;
                case InteractionTypeEnum.Bear1:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Bear1;
                    break;
                case InteractionTypeEnum.DrawsOpen:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.DrawsOpen;
                    break;
                case InteractionTypeEnum.DrawsClose:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.DrawsClose;
                    break;

                //drawers
                case InteractionTypeEnum.Drawers:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Drawers;
                    break;
                case InteractionTypeEnum.Plant:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Plant;
                    break;
                case InteractionTypeEnum.Mirror:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Mirror;
                    break;

            }

            //Description box
            Point winSize = new Point(1500, 150);
            Point winPos = new Point(210, 600 + startYPos);

            DrawWindowBase(winSize,
                  winPos,
                   new Color(.5f, .5f, .5f, .5f),
                   Color.Black, 2,
                   string.IsNullOrEmpty(interaction.Name) ? "FIX THIS" : interaction.Name,
                   new Color(1f, 1f, 1.25f, 1f));

            //size, position, base window, border, border thickness, interaction Name, header colour

            Vector2 textSize = _font.MeasureString(textToPrint);

            //description
            Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);
            txtPos = new Vector2(215, txtPos.Y + _font.LineSpacing);
            DrawString(textToPrint, txtPos, Color.Navy); //shadow colour
            //txtPos = new Vector2(215, txtPos.Y + 2*_font.LineSpacing);

            if (userInputRequired)
            {
                string answer;
                switch (interaction.InteractionType)
                {
                    case InteractionTypeEnum.LaptopCodeEnter:
                        textToPrint = "Please enter code";
                        //DrawString(textToPrint, txtPos, Color.Gray); //shadow colour
                        answer = Console.ReadLine();
                        break;
                    case InteractionTypeEnum.DoorCode:
                        textToPrint = "Input code for lock";
                        answer = Console.ReadLine();
                        break;
                    case InteractionTypeEnum.Sleep:
                        textToPrint = "Do I go to sleep?";
                        answer = Console.ReadLine();
                        break;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (CurrentInteractions != null)
            {
                _spritebatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap);

                //option window
                Point winSize = new Point(500, 45 + CurrentInteractions.Count * _font.LineSpacing);
                Point winPos = new Point(8, 8 + 110);

                DrawWindowBase(winSize,
                   winPos,
                    new Color(.5f, .5f, .5f, .5f),
                    Color.Black,2,
                    "What do you want to do?",
                    new Color(1f, 1f, 1.25f, 1f));
                //size, position ,bgColour, borderColour, borderThickness, title,titleBgColour


                Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);
                txtPos.X = 16;

                int idx = 1;
                foreach (string key in CurrentInteractions.Keys)
                {
                    string text = $"Press {idx} - {key}";

                    if (CurrentInteractions[key].InteractionType == InteractionTypeEnum.Nothing)
                    {
                        text = "Nothing to do here....";
                    }

                    Vector2 textSize = _font.MeasureString(text);
                    txtPos += new Vector2(0, _font.LineSpacing);

                    idx++;

                    DrawString(text, txtPos, Color.Navy);
                }

                if (interactionToDo != null)
                {
                    DoInteraction(interactionToDo, winPos.Y + winSize.Y);
                }

                if (ShowKeyPad)
                {
                    DrawLaptopKeyPad();
                }

                _spritebatch.End();
            }
        }

        protected string currentPinValue = string.Empty;

        Dictionary<string, Rectangle> keysBounds = new Dictionary<string, Rectangle>();

        protected void DrawLaptopKeyPad()
        {
            Point size = new Point(420, 672);
            Point pos = new Point(GraphicsDevice.Viewport.Width - 528, 8);

            DrawWindowBase(size, pos, Color.Silver, Color.Black, 2, "Laptop", new Color(.2f,.2f,.2f,1), Color.Silver);

            // Characters entered..
            Point textBoxPos = pos + new Point(6, 46);
            DrawBox(new Point(size.X-12, 64), textBoxPos, Color.White, Color.Navy, 1);

            string str = currentPinValue;

            if (string.IsNullOrEmpty(str))
            {
                str = "Enter Pin...";
            }

            Vector2 strSize = _font.MeasureString(str);
            DrawString(str, (textBoxPos + new Point(12, (int)strSize.Y/4)).ToVector2(), Color.DarkGray);
            // keys 0-9 + enter and delete
            Point keySize = new Point(128, 128);
            Point keyStartPos = textBoxPos + new Point(0,72);

            for (int i = 0; i < 12; i++)
            {
                Point keyPos = keyStartPos;

                keyPos.X += ((keySize.X + 12) * (i % 3));
                keyPos.Y += (keySize.Y + 12) * (i / 3);



                Color keyColor = Color.DarkGray;
                Color keyBorder = Color.Black;

                string keyText = (i + 1).ToString();

                if (i == 9) // Delete
                {
                    keyText = "Del.";
                }

                if (i == 10)
                {
                    keyText = "0";
                }

                if (i == 11) // Enter
                {
                    keyText = "Ent.";
                }

                if (!keysBounds.ContainsKey(keyText))
                {
                    keysBounds.Add(keyText, new Rectangle(keyPos.X, keyPos.Y, 128, 128));
                }

                if (_msState.PositionRect.Intersects(keysBounds[keyText])) // Check mouse over and if it is button click event.
                {
                    keyColor = Color.Gray;
                    keyBorder = Color.WhiteSmoke;
                }

                strSize = _font.MeasureString(keyText) / 2;

                DrawBox(keySize, keyPos, keyColor, keyBorder, 1);
                DrawString(keyText, (keyPos + new Point(64, 64)).ToVector2() - strSize, Color.Navy);
            }

            // Draw laptop locked image
            Rectangle laptopRec = new Rectangle(pos.X - 808, pos.Y, 800, 512);
            _spritebatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/LaptopLocked"), laptopRec, Color.White);
            // put a boarder around the laptop image.
            if (laptopBorder == null)
            {
                laptopBorder = new Texture2D(GraphicsDevice, 800, 512);
                laptopBorder.FillWithBorder(Color.Transparent, Color.Navy, new Rectangle(1, 1, 1, 1));
            }
            _spritebatch.Draw(laptopBorder, laptopRec, Color.White);

            // Draw text on laptop screen
            Vector2 lapTopTextPos = new Vector2((laptopRec.X + (laptopRec.Width / 2)) - 60, (laptopRec.Y + (laptopRec.Height / 2)) + 48);
            DrawString(currentPinValue, lapTopTextPos, Color.White, Game.Content.Load<SpriteFont>("Fonts/laptopFont"));
        }

        protected void DrawBox(Point size, Point pos, Color bgColor, Color borderCour, int borderThickness)
        {
            _bgTexture = new Texture2D(GraphicsDevice, size.X, size.Y);
            _bgTexture.FillWithBorder(bgColor, borderCour, new Rectangle(borderThickness, borderThickness, borderThickness, borderThickness));

            _spritebatch.Draw(_bgTexture, new Rectangle(pos.X, pos.Y, size.X, size.Y), Color.White);
        }



        protected void DrawWindowBase(Point size, Point pos, Color bgColor, Color borderCour, int borderThickness, string title, Color titleBgColor, Color? titleTextColor = null)
        {
            if (titleTextColor == null)
            {
                titleTextColor = Color.Navy;
            }

            _titleBar = new Texture2D(GraphicsDevice, size.X, 40);
            _titleBar.FillWithBorder(titleBgColor, borderCour, new Rectangle(borderThickness, borderThickness, borderThickness, borderThickness));

            DrawBox(size,pos,bgColor,borderCour, borderThickness);
            _spritebatch.Draw(_titleBar, new Rectangle(pos.X, pos.Y, size.X, _titleBar.Height), Color.White);

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
            _spritebatch.DrawString(font, text, pos, Color.Gray);
            _spritebatch.DrawString(font, text, pos + shadow, color);
        }
    }
}
