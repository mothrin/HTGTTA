using HTGTTA.Enums;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Randomchaos.Extensions;
using MonoGame.Randomchaos.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography;

namespace HTGTTA.Models
{
    public class Hud : DrawableGameComponent
    {
        SpriteBatch _spritebatch;
        SpriteFont _font;

        Texture2D _bgTexture;
        Texture2D _titleBar;
        Texture2D _backgroundTexture;

        Color EnterPin;

        ObjectInterations interactionToDo = null;
        int maxPinLength = 4;

        #region packages
        private ISceneService sceneService { get { return Game.Services.GetService<ISceneService>(); } }
        private IKeyboardStateManager _kbState { get { return Game.Services.GetService<IInputStateService>().KeyboardManager; } }
        private IMouseStateManager _msState { get { return Game.Services.GetService<IInputStateService>().MouseManager; } }
        private IInputStateService inputService { get { return Game.Services.GetService<IInputStateService>(); } }
        private IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }
        #endregion

        protected string currentPinValue = string.Empty;

        Dictionary<string, Rectangle> keysBounds = new Dictionary<string, Rectangle>();
        Dictionary<string, Rectangle> Options = new Dictionary<string, Rectangle>();

        public Dictionary<string, ObjectInterations> CurrentInteractions = null;

        protected int puzzleNum;

        protected bool OpenLaptop = false;

        protected bool LaptopLocked = true;

        protected bool DiaryOpen = false;

        protected bool ChairGot = false;

        protected bool Choice = false;

        protected bool laptopOpened;
        protected bool ReadDiary = false;


        protected bool sleepOptions;

        Texture2D laptopBorder;

        protected bool UIup;

        public Hud(Game game) : base(game)
        {

        }

        protected override void LoadContent()
        {
            _spritebatch = new SpriteBatch(GraphicsDevice);
            _font = Game.Content.LoadLocalized<SpriteFont>("Fonts/UIFont");

            base.LoadContent();
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
                if (DiaryOpen)
                {
                    _backgroundTexture = Game.Content.Load<Texture2D>("Textures/backgrounds/backgrounddiary");
                    //_audio.PlaySFX("Audio/SFX/book_flip2");
                }
                //pin code
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
                                EnterPin = Color.Gray;
                                currentPinValue = currentPinValue.Substring(0, currentPinValue.Length - 1);
                                _audio.PlaySFX("Audio/SFX/menu_select");
                            }
                            else if (lapTopKey == "X")
                            {
                                OpenLaptop = false;
                                interactionToDo = null;
                            }
                            else if (lapTopKey == "Ent.")
                            {

                                if (currentPinValue == "2215") // correct code
                                {
                                    OpenLaptop = false;
                                    interactionToDo = null;
                                    _audio.PlaySFX("Audio/SFX/menu_change");
                                    LaptopLocked = false;

                                }
                                else
                                {
                                    EnterPin = Color.DarkRed;
                                    _audio.PlaySFX("Audio/SFX/menu_cancel");
                                }
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
                    Color.Black, 2,
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

                //puzzles
                if (OpenLaptop)
                {
                    DrawLaptopKeyPad();
                }

                if (!LaptopLocked)
                {
                    DrawLaptop();
                }

                if (DiaryOpen)
                {
                    DiaryRead();
                }
                if (Choice)
                {
                    YesOrNo();
                }

                _spritebatch.End();
            }
        }
        protected void DoInteraction(ObjectInterations interaction, int startYPos)
        {
            string textToPrint = string.Empty;


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
                    if (!OpenLaptop)
                    {
                        currentPinValue = string.Empty; // reset the pin value.
                    }
                    OpenLaptop = true;
                    break;
                case InteractionTypeEnum.Chair:
                    textToPrint = interaction.Description;

                    if (ReadDiary)
                    {
                        textToPrint = "Maybe i could use this for something after all. Do i take it?";
                        Choice = true;
                        //ChairGot = true; 
                        //_backgroundTexture = Game.Content.Load<Texture2D>("Textures/backgrounds/backgroundchair"); //background
                    }
                    interaction.InteractionType = InteractionTypeEnum.Chair;
                    break;

                //door
                case InteractionTypeEnum.Door:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Door;
                    break;
                case InteractionTypeEnum.DoorCode:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.DoorCode;
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
                    if (laptopOpened)
                    {
                        DiaryOpen = true;
                    }
                    break;
                case InteractionTypeEnum.Sleep:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Sleep;
                    Choice = true;
                    break;

                //wardrobe
                case InteractionTypeEnum.WarDoorOpen:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.WarDoorOpen;
                    break;
                case InteractionTypeEnum.Box:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Box;
                    if (ChairGot)
                    {
                        interaction.InteractionType = InteractionTypeEnum.Chair;
                    }
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
                    switch (puzzleNum)
                    {
                        case 0:
                            textToPrint = interaction.Description;
                            break;
                        case 1:
                            textToPrint = "Cludeo: You're best friend knew everything about you, maybe you should read her messages again.";
                            break;
                        case 2:
                            textToPrint = "Cludeo: Look at the photos in the diary more closely, there might be something you're missing.";
                            break;
                        case 3:
                            textToPrint = "Cludeo: Pretty sure you put a key in that box, maybe you should try opening something.";
                            break;
                        case 4:
                            textToPrint = "Cludeo: There is a code for the door, retrace you're steps. There is probably something you're missing...";
                            break;
                    }
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

            //Description box
            Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);
            txtPos = new Vector2(215, txtPos.Y + _font.LineSpacing);
            DrawString(textToPrint, txtPos, Color.Navy); //shadow colour
            //txtPos = new Vector2(215, txtPos.Y + 2*_font.LineSpacing);
        }

        //Screens
        protected void DrawLaptopKeyPad()
        {

            // Draw laptop locked image
            Rectangle laptopRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spritebatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/LaptopLocked"), laptopRec, Color.White);

            UIup = true;

            Point size = new Point(360, 64);
            //Point pos = new Point(765, 630);


            // Characters entered..
            Point textBoxPos = new Point(775, 625);
            DrawBox(new Point(size.X, size.Y), textBoxPos, Color.DimGray, Color.Black, 1);

            string str = currentPinValue;

            if (string.IsNullOrEmpty(str))
            {
                str = "Enter Pin...";
            }

            Vector2 strSize = _font.MeasureString(str);
            DrawString(str, (textBoxPos + new Point(12, (int)strSize.Y / 4)).ToVector2(), EnterPin);

            // keys 0-9 + enter and delete
            Point keySize = new Point(112, 40);
            Point keyStartPos = textBoxPos + new Point(0, 25);

            for (int i = 0; i < 12; i++)
            {
                Point keyPos = keyStartPos;

                keyPos.X += ((keySize.X + 12) * (i % 3));
                keyPos.Y += (keySize.Y + 12) * (i / 3);


                Color keyColor = Color.DimGray;
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
                    keysBounds.Add(keyText, new Rectangle(keyPos.X, keyPos.Y + 50, 112, 40));
                }


                if (_msState.PositionRect.Intersects(keysBounds[keyText])) // Check mouse over and if it is button click event.
                {
                    keyColor = Color.DarkGray;
                    keyBorder = Color.Black;
                }

                strSize = _font.MeasureString(keyText) / 2;

                DrawBox(keySize, keyPos + new Point(0, 50), keyColor, keyBorder, 1);
                DrawString(keyText, (keyPos + new Point(56, 64)).ToVector2() - strSize, Color.Gray);
            }
            if (inputService.KeyboardManager.KeyPress(Keys.F4)) //close laptop
            {
                OpenLaptop = false;
                interactionToDo = null;
                UIup = false;
            }

        }
        protected void DrawLaptop()
        {
            Rectangle laptopRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spritebatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/laptop"), laptopRec, Color.White);

            UIup = true;

            if (inputService.KeyboardManager.KeyPress(Keys.F4)) //close laptop
            {
                LaptopLocked = true;
                laptopOpened = true;
                UIup = false;
                puzzleNum = 1;
            }

        }
        protected void DiaryRead()
        {
            Rectangle laptopRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spritebatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/laptop"), laptopRec, Color.White);

            UIup = true;

            if (inputService.KeyboardManager.KeyPress(Keys.F4)) //close diary
            {
                DiaryOpen = false;
                ReadDiary = true;
                puzzleNum = 2;
                UIup = false;
                interactionToDo = null;
            }

            _backgroundTexture = Game.Content.Load<Texture2D>("Textures/backgrounds/backgrounddiary"); //background


        }


        protected void YesOrNo()
        {

            Point keySize = new Point(400, 100);
            Point keyStartPos = new Point((GraphicsDevice.Viewport.Width / 2) - 425, 500);
            string keyText = "";



            //DrawBox(keySize, keyPos + new Point(440,0), keyColor, keyBorder, 1);
            //DrawString("No", (keyPos + new Point(638,0)).ToVector2(), Color.Navy);

            Vector2 strSize = _font.MeasureString(keyText);
            for (int i = 0; i < 2; i++)
            {
                Point keyPos = keyStartPos;

                Color keyColor = Color.Lavender;
                Color keyBorder = Color.Navy;
                if (i == 0)
                {
                    keyText = "Yes";
                }
                if (i == 1)
                {
                    keyText = "No";
                    keyPos.X += (keySize.X + 50);
                }

                if (!Options.ContainsKey(keyText))
                {
                    Options.Add(keyText, new Rectangle(keyPos.X, keyPos.Y, 400, 100));
                }
                if (_msState.PositionRect.Intersects(Options[keyText])) // Check mouse over and if it is button click event.
                {
                    keyColor = Color.LavenderBlush;
                    keyBorder = Color.Navy;
                }

                strSize = _font.MeasureString(keyText) / 2;

                DrawBox(keySize, keyPos, keyColor, keyBorder, 1);
                DrawString(keyText, (keyPos + new Point(200,50)).ToVector2() - strSize, Color.Navy);

                foreach (var button in Options.Keys)
                {
                    if (_msState.PositionRect.Intersects(Options[button]) && _msState.LeftClicked)
                    {
                        if (button == "Yes")
                        { 
                            //end game
                        }
                        if (button == "No")
                        {
                            Choice = false;
                            interactionToDo = null;
                        }
                    }
                }
            }








        }
        protected void Sleep()
        {

        }
        protected void Chair()
        {

        }
        public void ShowInteractionOptionsWindow(Dictionary<Sprite, Dictionary<string, ObjectInterations>> interactions)
        {
            Point winSize = new Point(500, 100);
            Point winPos = new Point(8, 8);

            _spritebatch.Begin();

            //interact box (E)
            DrawWindowBase(winSize,
                  winPos,
                   new Color(.5f, .5f, .5f, .5f),
                   Color.Black, 2,
                   "Interactions",
                   new Color(1f, 1f, 1.25f, 1f));
            //size, position, base window, border, border thickness, Title, header colour

            Vector2 center = new Vector2(winPos.X / 2, winPos.Y);

            Vector2 p = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);

            int idx = 1;
            p = new Vector2(22, p.Y + 40);
            string text;
            foreach (Sprite interationObject in interactions.Keys)
            {
                if (UIup)
                {
                    text = "Press to F4 to leave";
                }
                else
                {
                    text = $"Press E to interact with {interationObject.Name}";
                }

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

        //UI
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
