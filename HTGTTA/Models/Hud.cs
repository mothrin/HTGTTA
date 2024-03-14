using HTGTTA.Enums;
using HTGTTA.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Randomchaos.Extensions;
using MonoGame.Randomchaos.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace HTGTTA.Models
{
    public class Hud : DrawableGameComponent
    {
        SpriteBatch _spritebatch;
        SpriteFont _font;
        Texture2D _bgTexture;

        Texture2D _boxBG;
        Texture2D _drawerBG;
        Texture2D _titleBar;


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
        protected string DoorCodeInput = string.Empty;

        Dictionary<string, Rectangle> keysBounds = new Dictionary<string, Rectangle>();
        Dictionary<string, Rectangle> BoxitemBounds = new Dictionary<string, Rectangle>();
        Dictionary<string, Rectangle> DraweritemBounds = new Dictionary<string, Rectangle>();
        Dictionary<string, Rectangle> Options = new Dictionary<string, Rectangle>();
        Dictionary<string, Rectangle> DoorKey = new Dictionary<string, Rectangle>();

        public Dictionary<string, ObjectInterations> CurrentInteractions = null;

        protected int puzzleNum; //hints

        protected bool OpenLaptop = false; //Brings up laptop UI if true
        protected bool LaptopLocked = true; //Shows locked laptop screen if true
        protected bool laptopOpened = false; //shows unlocked laptop screen is true

        protected bool ReadDiary; // if true, diary ui pops up
        protected bool DiaryOpen; // allows player to progress to next puzzle

        public bool chairGot; //if true player can place infront of wardrobe
        protected bool PlacedChair; //if true player can continuously look in box without yes or no option 
        protected bool chairTook; // if player has taken chair, they can't continuously take it after

        protected bool itemDescBox; //if true description for item is shown
        protected string itemDescription; //decription for item
        public string itemText = ""; //Name of item

        protected bool BoxGot; //if true player can
        protected string BoxType; //determines if player can taken key or not so correct UI can be shown
        protected bool KeyClicked; //if true the boxtype changes to the one without a key

        protected bool boxLooked; //if true the player can continue to next puzzle
        protected bool KeyTaken; //if true the player can open the drawer with yes or no buttons

        protected bool DrawerOpened; //if true drawer UI pops up
        protected string DrawerType; //determines which UI is to be shown
        protected bool PaperClicked; //if clicked UI changes 

        protected bool drawerLooked; //if true player can progress to next puzzle
        protected bool PaperRead; //when true the player can put in the code to the door

        protected bool DoorCode; //when true door UI pops up
        public bool DoorOpened; //when true game ends 
        protected int codeCount = 0; //keeps track of code length
        protected string icon1; //door code
        protected string icon2; //door code
        protected string icon3; //door code
        protected string icon4; //door code

        protected bool moveClothes;
        public bool LeaveTrapdoor;


        public bool SleepYes; // when true game ends

        public bool Choice; //for yes and no buttons
        protected string typeChoice; //to dictate what happens when yes is pressed depending on puzzle type



        public bool UIup; //changes interaction key

        private List<string> doorKeyTextures = new List<string>()
        {
            "book", "circle",
            "triangle", "tree",
            "hexagon", "square",
            "heart", "rabbit",
            "flag", "star"
        };

        public List<Object> HidableSprites = new List<Object>();

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
                foreach (var item in BoxitemBounds.Keys)
                {
                    if (_msState.PositionRect.Intersects(BoxitemBounds[item]) && _msState.LeftClicked && !KeyClicked)
                    {
                        if (item == "Key")
                        {
                            _boxBG = Game.Content.Load<Texture2D>("Textures/Puzzle UI/BoxNoKey");
                            KeyTaken = true;
                            PlacedChair = true;
                            KeyClicked = true;
                            BoxType = "Key";

                        }
                    }
                }
                foreach (var item in DraweritemBounds.Keys)
                {
                    if (_msState.PositionRect.Intersects(DraweritemBounds[item]) && _msState.LeftClicked && !PaperClicked)
                    {
                        if (item == "Paper")
                        {
                            _boxBG = Game.Content.Load<Texture2D>("Textures/Puzzle UI/DrawerWithNote");
                            PaperRead = true;
                            DrawerOpened = true;
                            PaperClicked = true;
                            DrawerType = "Paper";
                            _audio.PlaySFX("Audio/SFX/book_flip.2");
                        }
                    }
                }


                //pin code
                foreach (var lapTopKey in keysBounds.Keys)
                {
                    if (_msState.PositionRect.Intersects(keysBounds[lapTopKey]) && _msState.LeftClicked && LaptopLocked)
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
                            else if (lapTopKey == "Ent.")
                            {

                                if (currentPinValue == "2215") // correct code
                                {

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

                if (DoorCode)
                {
                    foreach (var button in DoorKey.Keys)
                    {
                        if (_msState.PositionRect.Intersects(DoorKey[button]) && _msState.LeftClicked && DoorCode)
                        {
                            codeCount++;
                            _audio.PlaySFX("Audio/SFX/menu_select");
                            switch (codeCount)
                            {
                                case 1:
                                    icon1 = button;
                                    break;
                                case 2:
                                    icon2 = button;
                                    break;
                                case 3:
                                    icon3 = button;
                                    break;
                                case 4:
                                    icon4 = button;
                                    break;
                            }
                            DoorCodeInput = DoorCodeInput + button;
                            if (DoorCodeInput == "Textures/Puzzle UI/DoorIcons/triangleTextures/Puzzle UI/DoorIcons/circleTextures/Puzzle UI/DoorIcons/heartTextures/Puzzle UI/DoorIcons/star")
                            {
                                _audio.PlaySFX("Audio/SFX/menu_change");
                                DoorOpened = true;
                                DoorCode = false;
                                PaperRead = false;
                            }
                            if (codeCount > 4)
                            {
                                codeCount = 0;
                                DoorCodeInput = "";
                            }
                        }
                    }

                    if (inputService.KeyboardManager.KeyPress(Keys.Q)) //close Door UI
                    {
                        DoorCode = false;
                        UIup = false;
                        interactionToDo = null;
                        codeCount = 0;
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
                    if (LaptopLocked)
                    {
                        DrawLaptopKeyPad();
                    }
                    if (!LaptopLocked)
                    {
                        DrawLaptop();
                    }
                }
                if (DiaryOpen)
                {
                    DiaryRead();
                }
                if (PlacedChair)
                {
                    LookInBox();
                }
                if (DrawerOpened)
                {
                    Drawer();
                }
                if (DoorCode)
                {
                    DoorLock();
                }
                if (Choice)
                {
                    YesOrNo();
                }
                if (moveClothes)
                {
                    Trapdoor();
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
                    if (chairTook)
                    {
                        textToPrint = "I have the chair on me.";
                        break;
                    }
                    if (ReadDiary)
                    {
                        textToPrint = "Maybe i could use this for something after all. Do i take it?";
                        typeChoice = "Chair";
                        Choice = true;
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
                    if (PaperRead)
                    {
                        DoorCode = true;
                    }
                    if (DoorOpened)
                    {
                        textToPrint = "I unlocked it!";
                    }
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
                    typeChoice = "Sleep";
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
                    if (chairGot)
                    {
                        textToPrint = "I might be able to reach that box now. Do I place the chair?";
                        typeChoice = "PlaceChair";
                        Choice = true;
                    }
                    if (boxLooked)
                    {
                        PlacedChair = true;
                    }
                    break;
                case InteractionTypeEnum.ClothesMoved:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.ClothesMoved;
                    if (PaperRead)
                    {
                        textToPrint = "Do I move the clothes?";
                        typeChoice = "clothes";
                        Choice = true;
                    }
                    break;

                //bedside table
                case InteractionTypeEnum.Board:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Board;
                    break;
                case InteractionTypeEnum.Table:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Table;
                    if (KeyTaken)
                    {
                        textToPrint = "Pretty sure I got a key for this. Do I use it?";
                        typeChoice = "UseKey";
                        Choice = true;
                    }
                    if (drawerLooked)
                    {
                        DrawerOpened = true;
                    }
                    break;
                case InteractionTypeEnum.Bear:
                    switch (puzzleNum)
                    {
                        case 0:
                            textToPrint = interaction.Description;
                            break;
                        case 1:
                            textToPrint = "Cludeo: Your best friend knew everything about you, maybe you should read her messages again.";
                            break;
                        case 2:
                            textToPrint = "Cludeo: Read the diary more closely, there might be something you're missing. That chair might be useful for something.";
                            break;
                        case 3:
                            textToPrint = "Cludeo: Pretty sure you put a key in that box when you were alive, maybe you should try opening something with it.";
                            break;
                        case 4:
                            textToPrint = "Cludeo: There is a code for the door, retrace you're steps. There seems to be a symbol representing each puzzle.";
                            break;
                    }
                    interaction.InteractionType = InteractionTypeEnum.Bear;
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
                   new Color(1f, 1f, 1.25f, .5f));

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
            if (inputService.KeyboardManager.KeyPress(Keys.Q)) //close laptop
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

            laptopOpened = true;
            puzzleNum = 1;

            UIup = true;

            if (inputService.KeyboardManager.KeyPress(Keys.Q)) //close laptop
            {
                OpenLaptop = false;
                interactionToDo = null;
                UIup = false;
            }

        }
        protected void DiaryRead()
        {
            Rectangle diaryRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spritebatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/Diary"), diaryRec, Color.White);

            puzzleNum = 2;
            ReadDiary = true;
            UIup = true;

            if (inputService.KeyboardManager.KeyPress(Keys.Q)) //close diary
            {
                DiaryOpen = false;
                UIup = false;
                interactionToDo = null;
            }


        }
        protected void LookInBox()
        {
            Rectangle boxRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            if (BoxType == "Key")
            {
                _boxBG = Game.Content.Load<Texture2D>("Textures/Puzzle UI/BoxNoKey");
                _spritebatch.Draw(_boxBG, boxRec, Color.White);
            }
            else
            {
                _boxBG = Game.Content.Load<Texture2D>("Textures/Puzzle UI/Box");

                _spritebatch.Draw(_boxBG, boxRec, Color.White);

                Point size = new Point(112, 40);
                Point keyPos = new Point(112, 40);

                for (int i = 0; i < 8; i++)
                {
                    if (i == 0)
                    {
                        itemText = "Buttons";
                        itemDescription = "Buttons. Wow so interesting...";
                        keyPos = new Point(395, 750);
                        size = new Point(105, 120);
                    }

                    if (i == 1)
                    {
                        itemText = "Coins";
                        itemDescription = "1 pound and 35p! Wow rich!";
                        keyPos = new Point(395, 915);
                        size = new Point(145, 40);
                    }

                    if (i == 2)
                    {
                        itemText = "Folder";
                        itemDescription = "Yawnnnn, Homework...";
                        keyPos = new Point(130, 560);
                        size = new Point(250, 385);
                    }

                    if (i == 3)
                    {
                        itemText = "Scrolls";
                        itemDescription = "Some silly doodles, guess I was an artist when I was alive...";
                        keyPos = new Point(855, 330);
                        size = new Point(460, 320);
                    }

                    if (i == 4)
                    {
                        itemText = "Bunny";
                        itemDescription = "Cute! You must be the Muffin I heard about. Nice to meet you! ";
                        keyPos = new Point(1070, 700);
                        size = new Point(115, 220);
                    }
                    if (i == 5)
                    {
                        itemText = "Key";
                        itemDescription = "A key is always useful in these situations";
                        keyPos = new Point(880, 860);
                        size = new Point(160, 90);
                    }
                    if (i == 6) // Enter
                    {
                        itemText = "Roll";
                        itemDescription = "I don't even know what that is.";
                        keyPos = new Point(515, 375);
                        size = new Point(320, 530);
                    }
                    if (i == 7)
                    {
                        itemText = "Book";
                        itemDescription = "\"The complex nature of Project:Zorya by Elle Boseley\"\n Hm. Seems like a pretty cool book, I must've liked it in life.";
                        keyPos = new Point(1495, 630);
                        size = new Point(425, 450);
                    }
                    if (!BoxitemBounds.ContainsKey(itemText))
                    {
                        BoxitemBounds.Add(itemText, new Rectangle(keyPos.X, keyPos.Y, size.X, size.Y));
                    }
                    foreach (var item in BoxitemBounds.Keys)
                    {
                        Point winSize = new Point(1500, 150);
                        Point winPos = new Point(210, 200);
                        if (_msState.PositionRect.Intersects(BoxitemBounds[itemText]))
                        {
                            DrawWindowBase(winSize,
                            winPos,
                            new Color(.5f, .5f, .5f, .5f),
                            Color.Black, 2,
                            itemText,
                            new Color(1f, 1f, 1.25f, 1f));

                            //Description box
                            Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);
                            txtPos = new Vector2(215, txtPos.Y + _font.LineSpacing);
                            DrawString(itemDescription, txtPos, Color.Navy); //shadow colour

                        }
                    }
                }
            }
            puzzleNum = 3;

            UIup = true;
            if (inputService.KeyboardManager.KeyPress(Keys.Q)) //close UI
            {
                PlacedChair = false;
                UIup = false;
                interactionToDo = null;
            }

        }
        protected void Drawer()
        {
            Rectangle DrawerRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            if (DrawerType == "Paper")
            {
                _drawerBG = Game.Content.Load<Texture2D>("Textures/Puzzle UI/DrawerWithNote");
                _spritebatch.Draw(_drawerBG, DrawerRec, Color.White);
            }
            else
            {
                _drawerBG = Game.Content.Load<Texture2D>("Textures/Puzzle UI/Drawer");
                _spritebatch.Draw(_drawerBG, DrawerRec, Color.White);

                Point size = new Point(112, 40);
                Point keyPos = new Point(112, 40);
                //buttons used to dictate where items are
                for (int i = 0; i < 8; i++)
                {
                    if (i == 0)
                    {
                        itemText = "Bracelet";
                        itemDescription = "Cute Bracelet.";
                        keyPos = new Point(295, 5);
                        size = new Point(240, 270);
                    }

                    if (i == 1)
                    {
                        itemText = "Photos";
                        itemDescription = "Oh so that's what I looked like, glad I didn't get to all the photos of me...";
                        keyPos = new Point(515, 390);
                        size = new Point(555, 380);
                    }

                    if (i == 2)
                    {
                        itemText = "Photo";
                        itemDescription = "She must be my best friend.";
                        keyPos = new Point(1300, 10);
                        size = new Point(240, 300);
                    }

                    if (i == 3)
                    {
                        itemText = "Paper";
                        itemDescription = "This is the missing paper in my diary! Do I read it?";
                        keyPos = new Point(1280, 475);
                        size = new Point(295, 380);
                    }

                    if (i == 4)
                    {
                        itemText = "Pen";
                        itemDescription = "The imfamous diary writing pen!";
                        keyPos = new Point(1135, 315);
                        size = new Point(165, 170);
                    }
                    if (i == 5)
                    {
                        itemText = "Stationary";
                        itemDescription = "Ooo drawing utensils.";
                        //typeChoice = "Key";
                        //Choice = true;
                        keyPos = new Point(635, 20);
                        size = new Point(570, 245);
                    }
                    if (i == 6)
                    {
                        itemText = "Makeup";
                        itemDescription = "Purple nail polish...I expect no less.";
                        keyPos = new Point(275, 530);
                        size = new Point(240, 265);
                    }
                    if (!DraweritemBounds.ContainsKey(itemText))
                    {
                        DraweritemBounds.Add(itemText, new Rectangle(keyPos.X, keyPos.Y, size.X, size.Y));
                    }
                    //checks to see if player's mouse hovers over item box
                    foreach (var item in DraweritemBounds.Keys)
                    {
                        Point winSize = new Point(1500, 150);
                        Point winPos = new Point(210, 200);
                        if (_msState.PositionRect.Intersects(DraweritemBounds[itemText]))
                        {
                            DrawWindowBase(winSize,
                            winPos,
                            new Color(.5f, .5f, .5f, .5f),
                            Color.Black, 2,
                            itemText,
                            new Color(1f, 1f, 1.25f, 1f));

                            //Description box
                            Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);
                            txtPos = new Vector2(215, txtPos.Y + _font.LineSpacing);
                            DrawString(itemDescription, txtPos, Color.Navy); //shadow colour

                        }
                    }
                }
            }
            puzzleNum = 4;
            UIup = true;


            //check if player wants to leave ui
            if (inputService.KeyboardManager.KeyPress(Keys.Q)) //close diary
            {
                DrawerOpened = false;
                UIup = false;
                interactionToDo = null;
            }
        }
        protected void DoorLock()
        {
            Rectangle laptopRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spritebatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/PadLock"), laptopRec, Color.White);

            puzzleNum = 5;
            UIup = true;

            Point keyPos = new Point(160, 120);
            Point keySize = new Point(150, 150);

            #region code input
            Point Size1 = new Point(170, 155);
            Point textBoxPos1 = new Point(585, 705);
            DrawBox(new Point(Size1.X, Size1.Y), textBoxPos1, Color.DimGray, Color.DimGray, 1);

            Point Size2 = new Point(160, 155);
            Point textBoxPos2 = new Point(800, 705);
            DrawBox(new Point(Size2.X, Size2.Y), textBoxPos2, Color.DimGray, Color.DimGray, 1);

            Point Size3 = new Point(160, 155);
            Point textBoxPos3 = new Point(1015, 705);
            DrawBox(new Point(Size3.X, Size3.Y), textBoxPos3, Color.DimGray, Color.DimGray, 1);

            Point Size4 = new Point(160, 155);
            Point textBoxPos4 = new Point(1220, 705);
            DrawBox(new Point(Size4.X, Size4.Y), textBoxPos4, Color.DimGray, Color.DimGray, 1);

            if (codeCount == 1)
            {
                _spritebatch.Draw(Game.Content.Load<Texture2D>(icon1), new Rectangle(textBoxPos1.X, textBoxPos1.Y, 150, 145), Color.White);
            }
            if (codeCount == 2)
            {
                _spritebatch.Draw(Game.Content.Load<Texture2D>(icon1), new Rectangle(textBoxPos1.X, textBoxPos1.Y, 150, 145), Color.White);
                _spritebatch.Draw(Game.Content.Load<Texture2D>(icon2), new Rectangle(textBoxPos2.X, textBoxPos2.Y, 150, 145), Color.White);
            }
            if (codeCount == 3)
            {
                _spritebatch.Draw(Game.Content.Load<Texture2D>(icon1), new Rectangle(textBoxPos1.X, textBoxPos1.Y, 150, 145), Color.White);
                _spritebatch.Draw(Game.Content.Load<Texture2D>(icon2), new Rectangle(textBoxPos2.X, textBoxPos2.Y, 150, 145), Color.White);
                _spritebatch.Draw(Game.Content.Load<Texture2D>(icon3), new Rectangle(textBoxPos3.X, textBoxPos3.Y, 150, 145), Color.White);
            }
            if (codeCount == 4)
            {
                _spritebatch.Draw(Game.Content.Load<Texture2D>(icon1), new Rectangle(textBoxPos1.X, textBoxPos1.Y, 150, 145), Color.White);
                _spritebatch.Draw(Game.Content.Load<Texture2D>(icon2), new Rectangle(textBoxPos2.X, textBoxPos2.Y, 150, 145), Color.White);
                _spritebatch.Draw(Game.Content.Load<Texture2D>(icon3), new Rectangle(textBoxPos3.X, textBoxPos3.Y, 150, 145), Color.White);
                _spritebatch.Draw(Game.Content.Load<Texture2D>(icon4), new Rectangle(textBoxPos4.X, textBoxPos4.Y, 150, 145), Color.White);
            }
            #endregion

            for (int i = 0; i < 10; i++)
            {
                Color keyColor = Color.DimGray;
                Color keyBorder = Color.Black;

                string keyText = $"Textures/Puzzle UI/DoorIcons/{doorKeyTextures[i]}";

                keyPos.X = 150 + ((keySize.X + 12) * (i % 2));
                keyPos.Y = 120 + (keySize.Y + 12) * (i / 2);


                if (!DoorKey.ContainsKey(keyText))
                {
                    DoorKey.Add(keyText, new Rectangle(keyPos.X, keyPos.Y, 150, 150));
                }


                if (_msState.PositionRect.Intersects(DoorKey[keyText])) // Check mouse over and if it is button click event.
                {
                    keyColor = Color.DarkGray;
                    keyBorder = Color.Black;
                }

                DrawBox(keySize, keyPos, keyColor, keyBorder, 1);
                _spritebatch.Draw(Game.Content.Load<Texture2D>(keyText), new Rectangle(keyPos.X + 43, keyPos.Y + 43, 64, 64), Color.White);
            }
        }
        protected void Trapdoor()
        {
            Rectangle diaryRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spritebatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/trapdoor"), diaryRec, Color.White);



            UIup = true;
            LeaveTrapdoor = true;


            if (inputService.KeyboardManager.KeyPress(Keys.Q)) //close diary
            {
                moveClothes = false;
                UIup = false;
                interactionToDo = null;
            }
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
                            if(typeChoice=="Sleep")
                            {
                                SleepYes = true;
                            }
                            if(typeChoice=="Chair")
                            {
                                chairGot = true;
                                var chair = HidableSprites.FirstOrDefault(f => f.Name == typeChoice);

                                if (chair != null)
                                {
                                    chair.Visible = false;
                                }

                                Choice = false;
                                interactionToDo = null;
                                chairTook = true;
                            }
                            if (typeChoice== "PlaceChair")
                            {
                                chairGot = false;
                                var chair2 = HidableSprites.FirstOrDefault(f => f.Name == typeChoice);
                                if (chair2 == null)
                                {
                                    chair2.Visible = true;
                                }
                                PlacedChair = true;
                                Choice = false;
                                interactionToDo = null;
                                boxLooked = true;
                            }
                            if(typeChoice== "UseKey")
                            {
                                KeyTaken = false;
                                DrawerOpened = true;
                                Choice = false;
                                interactionToDo = null;
                                drawerLooked = true;
                            }
                            if(typeChoice=="clothes")
                            {
                                moveClothes = true;
                                Choice = false;
                                interactionToDo = null;
                            }
                            if(typeChoice=="trapdoor")
                            {
                                LeaveTrapdoor = true;
                                Choice = false;
                                interactionToDo = null;
                            }

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
        public void ShowInteractionOptionsWindow(Dictionary<Sprite, Dictionary<string, ObjectInterations>> interactions)
        {
            Point winSize = new Point(600, 100);
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
                    text = "Press to Q to leave";
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
