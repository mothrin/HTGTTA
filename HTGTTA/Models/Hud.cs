﻿using HTGTTA.Enums;
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
        Texture2D _titleBar;

        //chnaginG UIs
        Texture2D _boxBG;
        Texture2D _drawerBG;


        ObjectInterations interactionToDo = null;
        public Dictionary<string, ObjectInterations> CurrentInteractions = null;

        //packages
        #region packages 
        private ISceneService sceneService { get { return Game.Services.GetService<ISceneService>(); } }
        private IKeyboardStateManager _kbState { get { return Game.Services.GetService<IInputStateService>().KeyboardManager; } }
        private IMouseStateManager _msState { get { return Game.Services.GetService<IInputStateService>().MouseManager; } }
        private IInputStateService inputService { get { return Game.Services.GetService<IInputStateService>(); } }
        private IAudioService _audio { get { return Game.Services.GetService<IAudioService>(); } }
        #endregion

        protected string currentPinValue = string.Empty;
        protected string DoorCodeInput = string.Empty;

        //buttons that player can press
        Dictionary<string, Rectangle> Options = new Dictionary<string, Rectangle>(); //yes and no buttons
        Dictionary<string, Rectangle> keysBounds = new Dictionary<string, Rectangle>(); //laptop keys
        Dictionary<string, Rectangle> BoxitemBounds = new Dictionary<string, Rectangle>(); //box items
        Dictionary<string, Rectangle> DraweritemBounds = new Dictionary<string, Rectangle>(); //drawer items
        Dictionary<string, Rectangle> DoorKey = new Dictionary<string, Rectangle>(); //door keys


        Color EnterPin; //changes colour of laptop pin input depending if wrong input or not
        int maxPinLength = 4; //pinlength for laptopcode

        //boolean for puzzles
        protected int puzzleNum= 2; //hints
        protected bool keyPress;
        //laptop
        protected bool OpenLaptop; //Brings up laptop UI if true
        protected bool LaptopLocked = true; //Shows locked laptop screen if true
        protected bool laptopOpened ; //shows unlocked laptop screen is true
        //diary
        protected bool ReadDiary; // allows player to progress to next puzzle
        protected bool DiaryOpen; // if true, diary ui pops up
        //desk
        public bool chairGot; //if true player can place infront of wardrobe
        protected bool PlacedChair; //if true player can continuously look in box without yes or no option 
        protected bool chairTook; // if player has taken chair, they can't continuously take it after
        //box and drawer items
        protected bool itemDescBox; //if true description for item is shown
        protected string itemDescription; //decription for item
        public string itemText = ""; //Name of item
        //box
        protected bool BoxGot; //if true player can
        protected string BoxType; //determines if player can taken key or not so correct UI can be shown
        protected bool KeyClicked; //if true the boxtype changes to the one without a key
        protected bool boxLooked; //if true the player can continue to next puzzle
        protected bool KeyTaken =true; //if true the player can open the drawer with yes or no buttons
        //drawer
        protected bool DrawerOpened; //if true drawer UI pops up
        protected string DrawerType; //determines which UI is to be shown
        protected bool PaperClicked; //if clicked UI changes 
        protected bool drawerLooked; //if true player can progress to next puzzle
        protected bool PaperRead; //when true the player can put in the code to the door
        //door
        protected bool DoorCode; //when true door UI pops up
        public bool DoorOpened; //when true game ends 
        protected int codeCount = 0; //keeps track of code length
        protected string icon1; //door code
        protected string icon2; //door code
        protected string icon3; //door code
        protected string icon4; //door code
        //wardrobe
        protected bool moveClothes;
        public bool LeaveTrapdoor;
        //window
        protected bool windowkey;
        public bool LeaveWindow;
        //bed
        public bool SleepYes; // when true game ends
        //yes or no buttons
        public bool Choice; //for yes and no buttons
        protected string typeChoice; //to dictate what happens when yes is pressed depending on puzzle type
        //changes key player is told to use in top left corner
        public bool UIup; //changes interaction key

        //door code button symbols
        private List<string> doorKeyTextures = new List<string>()
        {
            "book", "circle",
            "triangle", "tree",
            "hexagon", "square",
            "heart", "rabbit",
            "flag", "star"
        };
        //hidable sprites that are moved
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
            //this is to see what number the player presses to interact with the object
            if (CurrentInteractions != null)
            {
                string key = null;
                if (_kbState.KeyDown(Keys.D1) && CurrentInteractions.Count > 0 && !Choice)
                {
                    key = CurrentInteractions.Keys.ElementAt(0);
                    interactionToDo = CurrentInteractions[key];
                }

                if (_kbState.KeyDown(Keys.D2) && CurrentInteractions.Count > 1 && !Choice)
                {
                    key = CurrentInteractions.Keys.ElementAt(1);
                    interactionToDo = CurrentInteractions[key];
                }

                if (_kbState.KeyDown(Keys.D3) && CurrentInteractions.Count > 2 && !Choice)
                {
                    key = CurrentInteractions.Keys.ElementAt(2);
                    interactionToDo = CurrentInteractions[key];
                }

                if (interactionToDo != null && interactionToDo.InteractionType == InteractionTypeEnum.Nothing) //for uninteractable objects that require collison
                {
                    interactionToDo = null;
                    
                }
                // this is to see if the player has clicked on the key within the box so that the ui can be changed accordingly
                foreach (var item in BoxitemBounds.Keys)
                {
                    if (_msState.PositionRect.Intersects(BoxitemBounds[item]) && _msState.LeftClicked && !KeyClicked)
                    {
                        if (item == "Key")//item that player needs to click
                        {
                            _boxBG = Game.Content.Load<Texture2D>("Textures/Puzzle UI/BoxNoKey");
                            KeyTaken = true; //So player can progress to next puzzle
                            PlacedChair = true; //recalls procedure that draws UI
                            KeyClicked = true; //so ui changes
                            BoxType = "Key";
                            _audio.PlaySFX("Audio/SFX/keys_05");
                        }
                    }
                }
                //this is to see if the player has clicked the paper within the box so the ui can be changed accordingly
                foreach (var item in DraweritemBounds.Keys)
                {
                    if (_msState.PositionRect.Intersects(DraweritemBounds[item]) && _msState.LeftClicked && !PaperClicked)
                    {
                        if (item == "Paper")//item that player needs to click
                        {
                            _boxBG = Game.Content.Load<Texture2D>("Textures/Puzzle UI/DrawerWithNote");
                            PaperRead = true; //So player can progress to next puzzle
                            DrawerOpened = true; //recalls procedure that draws UI
                            PaperClicked = true; //so ui changes
                            DrawerType = "Paper";
                            _audio.PlaySFX("Audio/SFX/book_flip.2"); 
                        }
                    }
                }


                //this codes is to check what laptop buttons the player is presses so it can respond accordingly
                foreach (var lapTopKey in keysBounds.Keys)
                {
                    if (_msState.PositionRect.Intersects(keysBounds[lapTopKey]) && _msState.LeftClicked && LaptopLocked)
                    {
                        if (lapTopKey != "Del." && lapTopKey != "Ent.") // means button is being pressed
                        {
                            if (currentPinValue.Length < maxPinLength) // limits player input to max of 4 numbers
                            {
                                _audio.PlaySFX("Audio/SFX/menu_select");
                                currentPinValue += lapTopKey; //adds to pin
                            }
                            else //player can't type any more numbers
                            {
                                _audio.PlaySFX("Audio/SFX/menu_cancel"); 
                            }
                        }
                        else
                        {
                            if (lapTopKey == "Del." && currentPinValue.Length > 0) //removes last entered digit 
                            {
                                EnterPin = Color.Gray;
                                currentPinValue = currentPinValue.Substring(0, currentPinValue.Length - 1);
                                _audio.PlaySFX("Audio/SFX/menu_select");
                            }
                            else if (lapTopKey == "Ent.") //inputs code that player has entered
                            {

                                if (currentPinValue == "2215") // correct code
                                {

                                    _audio.PlaySFX("Audio/SFX/menu_change");
                                    LaptopLocked = false;

                                }
                                else //incorrect code
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
                //this codes if to check for the door padlock button presses and respond accordingly
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
                                DoorOpened = true; //ends game
                                DoorCode = false; //closes door UI
                                PaperRead = false; 
                            }
                            if (codeCount > 4) //resets code
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

                //defines options window
                Point winSize = new Point(500, 45 + CurrentInteractions.Count * _font.LineSpacing);
                Point winPos = new Point(8, 8 + 110);

                //draws option window
                DrawWindowBase(winSize,winPos,new Color(.5f, .5f, .5f, .5f),Color.Black, 2,"What do you want to do?",new Color(1f, 1f, 1.25f, 1f)); //draws box for options

                Vector2 txtPos = winPos.ToVector2() - (new Vector2(0, _titleBar.Height / 4) * .5f); //where box is to be drawn
                txtPos.X = 16;

                int idx = 1;
                foreach (string key in CurrentInteractions.Keys) // draws interaction options
                {
                    string text = $"Press {idx} - {key}"; //tells player what number to press

                    if (CurrentInteractions[key].InteractionType == InteractionTypeEnum.Nothing)//if there is nothing to interact with 
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
                    DoInteraction(interactionToDo, winPos.Y + winSize.Y); //this goes to switch case within method
                }

                //puzzles (boolean logic)
                if (OpenLaptop) 
                {
                    if (LaptopLocked) //will show locked laptop ui
                    {
                        DrawLaptopKeyPad();
                    }
                    if (!LaptopLocked) //if laptop has been unlocked open laptop 
                    {
                        DrawLaptop();
                    }
                }
                if (DiaryOpen) //opens diary ui
                {
                    DiaryRead();
                }
                if (PlacedChair) //can only look in box if chair is placed
                {
                    LookInBox();
                }
                if (DrawerOpened) //can only look in drawer if unlocked
                {
                    Drawer();
                }
                if (DoorCode) //if true door UI opens
                {
                    DoorLock();
                }
                if (Choice) //triggers yes or no buttons
                {
                    YesOrNo();
                }


                _spritebatch.End();
            }
        }
        protected void DoInteraction(ObjectInterations interaction, int startYPos)
        {
            //in here is where the decription box text is determined based on what the player chooses to interact with
            //it can either trigger interaction, ui to pop up or for yes or no buttons to pop up
            string textToPrint = string.Empty;

            //switch case determines what interaction, easy to add to
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
                        currentPinValue = string.Empty; // resets the pin value.
                    }
                    OpenLaptop = true;
                    break;
                case InteractionTypeEnum.Chair:
                    textToPrint = interaction.Description;
                    if (chairTook) //if player tried to interact with chair after it's taken
                    {
                        textToPrint = "I have the chair on me.";
                        break;
                    }
                    if (ReadDiary && !UIup) //opens yes or no buttons
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
                    if (PaperRead) //opens door UI
                    {
                        DoorCode = true;
                    }
                    if (DoorOpened) //Displays this before ending game
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
                    if(windowkey) //checks if player has required key
                    {
                        textToPrint = "I have a key. Do I try using it?";
                        typeChoice = "window";
                        Choice= true;
                    }
                    else if(KeyTaken) //if player has the wrong key
                    {
                        textToPrint = "Hmm, this key isn't for this.";
                    }
                    break;

                //bed
                case InteractionTypeEnum.Bed:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Bed;
                    break;
                case InteractionTypeEnum.Diary:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Diary;
                    if (laptopOpened) //checks if player can read diary yet
                    {
                        DiaryOpen = true;
                    }
                    break;
                case InteractionTypeEnum.Sleep:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Sleep;
                    if(!UIup) //opens yes or no buttons
                    {
                        typeChoice = "Sleep";
                        Choice = true;
                    }
                    break;

                //wardrobe
                case InteractionTypeEnum.WarDoorOpen:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.WarDoorOpen;
                    break;
                case InteractionTypeEnum.Box:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Box;
                    if (chairGot && !UIup) //opens yes or no buttons
                    {
                        textToPrint = "I might be able to reach that box now. Do I place the chair?";
                        typeChoice = "PlaceChair";
                        Choice = true;
                    }
                    if (boxLooked) //so yes or no buttons don't continue to pop up after inital answer
                    {
                        PlacedChair = true;
                    }
                    break;
                case InteractionTypeEnum.ClothesMoved:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.ClothesMoved;
                    if (PaperRead && !UIup) //opens yes or no buttons
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
                    if (KeyTaken && !UIup) //opens yes or no buttons
                    {
                        textToPrint = "Pretty sure I got a key for this. Do I use it?";
                        typeChoice = "UseKey";
                        Choice = true;
                    }
                    if (drawerLooked) //so yes or no buttons don't continue to pop up after inital answer
                    {
                        DrawerOpened = true;
                    }
                    break;
                case InteractionTypeEnum.Bear: //hints
                    switch (puzzleNum)
                    {
                        case 0:
                            textToPrint = interaction.Description;
                            break;
                        case 1:
                            textToPrint = "Cluedo: Your best friend knew everything about you, maybe you should read her messages again.";
                            break;
                        case 2:
                            textToPrint = "Cluedo: Read the diary more closely, there might be something you're missing. \nThat chair might be useful for something.";
                            break;
                        case 3:
                            textToPrint = "Cluedo: Pretty sure you put a key in that box when you were alive, \nMaybe you should try opening something with it.";
                            break;
                        case 4:
                            textToPrint = "Cluedo: There is a code for the door, retrace you're steps.\n There seems to be a symbol representing each puzzle.";
                            break;
                    }
                    interaction.InteractionType = InteractionTypeEnum.Bear;
                    break;

                //drawers
                case InteractionTypeEnum.Drawers:
                    textToPrint = interaction.Description;
                    interaction.InteractionType = InteractionTypeEnum.Drawers;
                    if(ReadDiary) //gives player window key for alt ending
                    {
                        textToPrint = "Oh cool, guess there was something in here! I found a key!";
                        windowkey = true;
                    }
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

            //defines description box
            Point winSize = new Point(1500, 150);
            Point winPos = new Point(210, 600 + startYPos);

            DrawWindowBase(winSize,winPos,new Color(.5f, .5f, .5f, .5f),Color.Black, 2, string.IsNullOrEmpty(interaction.Name) ? "FIX THIS" : interaction.Name, new Color(1f, 1f, 1.25f, .5f));
            Vector2 textSize = _font.MeasureString(textToPrint);

            //draws description box
            Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);
            txtPos = new Vector2(215, txtPos.Y + _font.LineSpacing);
            DrawString(textToPrint, txtPos, Color.Navy);
        }

        //UIs
        protected void DrawLaptopKeyPad()
        {

            // Draws laptop locked image
            Rectangle laptopRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spritebatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/LaptopLocked"), laptopRec, Color.White);

            UIup = true;

            Point size = new Point(360, 64);

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

            //draws pin buttons
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
                //changes button colour when player hovers over with mouse
                if (_msState.PositionRect.Intersects(keysBounds[keyText])) // Check mouse over and if it is button click event.
                {
                    keyColor = Color.DarkGray;
                    keyBorder = Color.Black;
                }

                strSize = _font.MeasureString(keyText) / 2;

                //draws pin key buttons
                DrawBox(keySize, keyPos + new Point(0, 50), keyColor, keyBorder, 1);
                DrawString(keyText, (keyPos + new Point(56, 64)).ToVector2() - strSize, Color.Gray);
            }
            if (inputService.KeyboardManager.KeyPress(Keys.Q)) //closes laptop
            {
                OpenLaptop = false;
                interactionToDo = null;
                UIup = false;
            }

        }
        protected void DrawLaptop()
        {
            //draws unlocked laptop
            Rectangle laptopRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spritebatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/laptop"), laptopRec, Color.White);

            laptopOpened = true;
            puzzleNum = 1; //increases puzzle number for hint system

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
            //draws diary
            Rectangle diaryRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spritebatch.Draw(Game.Content.Load<Texture2D>("Textures/Puzzle UI/Diary"), diaryRec, Color.White);

            puzzleNum = 2;  //increases puzzle number for hint system
            ReadDiary = true; 
            UIup = true;

            if (inputService.KeyboardManager.KeyPress(Keys.Q)) //closes diary
            {
                DiaryOpen = false;
                UIup = false;
                interactionToDo = null;
            }


        }
        protected void LookInBox()
        {
            //draws box UI
            Rectangle boxRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            //defines what UI background is displayed
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

                //buttons used to dicatate what item desciption to display
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
                            //draws description box
                            DrawWindowBase(winSize,
                            winPos,
                            new Color(.5f, .5f, .5f, .5f),
                            Color.Black, 2,
                            itemText,
                            new Color(1f, 1f, 1.25f, 1f));

                            Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);
                            txtPos = new Vector2(215, txtPos.Y + _font.LineSpacing);
                            DrawString(itemDescription, txtPos, Color.Navy); 
                        }
                    }
                }
            }
            puzzleNum = 3;  //increases puzzle number for hint system

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
            //draws drawer UI
            Rectangle DrawerRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            //dictates which UI to display
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
                            //Draws description box
                            DrawWindowBase(winSize,
                            winPos,
                            new Color(.5f, .5f, .5f, .5f),
                            Color.Black, 2,
                            itemText,
                            new Color(1f, 1f, 1.25f, 1f));

                            Vector2 txtPos = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);
                            txtPos = new Vector2(215, txtPos.Y + _font.LineSpacing);
                            DrawString(itemDescription, txtPos, Color.Navy);

                        }
                    }
                }
            }
            puzzleNum = 4;  //increases puzzle number for hint system
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

            puzzleNum = 5;  //increases puzzle number for hint system
            UIup = true;

            Point keyPos = new Point(160, 120);
            Point keySize = new Point(150, 150);

            //draws buttons for player input to be displayed
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

            //draws symbols in boxes as player inputs symbols
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

            //draws buttons for player to press
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
                if (_msState.PositionRect.Intersects(DoorKey[keyText])) // Check mouse over and changes button colour
                {
                    keyColor = Color.DarkGray;
                    keyBorder = Color.Black;
                }

                DrawBox(keySize, keyPos, keyColor, keyBorder, 1);
                _spritebatch.Draw(Game.Content.Load<Texture2D>(keyText), new Rectangle(keyPos.X + 43, keyPos.Y + 43, 64, 64), Color.White);
            }
        }
    

        protected void YesOrNo()
        {
            // displays yes and no buttons
            Point keySize = new Point(400, 100);
            Point keyStartPos = new Point((GraphicsDevice.Viewport.Width / 2) - 425, 500);
            string keyText = "";

            Vector2 strSize = _font.MeasureString(keyText);
            //defines buttons
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
                if (_msState.PositionRect.Intersects(Options[keyText])) // Check mouse over and chaneges the colour of them
                {
                    keyColor = Color.LavenderBlush;
                    keyBorder = Color.Navy;
                }

                strSize = _font.MeasureString(keyText) / 2;

                //draws buttons
                DrawBox(keySize, keyPos, keyColor, keyBorder, 1);
                DrawString(keyText, (keyPos + new Point(200,50)).ToVector2() - strSize, Color.Navy);

                //checks player button press and repsonds accordingly
                foreach (var button in Options.Keys)
                {
                    if (_msState.PositionRect.Intersects(Options[button]) && _msState.LeftClicked)
                    {
                        if (button == "Yes")
                        { 
                            if(typeChoice=="Sleep") //ending triggered
                            {
                                SleepYes = true;
                            }
                            if(typeChoice=="Chair") //player takes chair and it is removed 
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
                            if(typeChoice== "UseKey") //opens drawer for user
                            {
                                KeyTaken = false;
                                DrawerOpened = true;
                                Choice = false;
                                interactionToDo = null;
                                drawerLooked = true;
                            }
                            if(typeChoice=="clothes") //ending triggered
                            {
                                LeaveTrapdoor = true;
                                Choice = false;
                                interactionToDo = null;
                            }
                            if(typeChoice== "window") //ending triggered
                            {
                                LeaveWindow = true;
                                Choice = false;
                                interactionToDo = null;
                            }
                        }
                        if (button == "No") //for all presses of no, button UI closes
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

            //interact box (E) (changes to Q when UI open
            DrawWindowBase(winSize,
                  winPos,
                   new Color(.5f, .5f, .5f, .5f),
                   Color.Black, 2,
                   "Interactions",
                   new Color(1f, 1f, 1.25f, 1f));

            Vector2 center = new Vector2(winPos.X / 2, winPos.Y);

            Vector2 p = (winPos.ToVector2() + new Vector2(winSize.X / 2, 0)) - (new Vector2(0, _titleBar.Height / 4) * .5f);

            int idx = 1;
            p = new Vector2(22, p.Y + 40);
            string text;
            //changes interaction box depending if UI is open or not
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
            //draws box (for buttons)
            _bgTexture = new Texture2D(GraphicsDevice, size.X, size.Y);
            _bgTexture.FillWithBorder(bgColor, borderCour, new Rectangle(borderThickness, borderThickness, borderThickness, borderThickness));

            _spritebatch.Draw(_bgTexture, new Rectangle(pos.X, pos.Y, size.X, size.Y), Color.White);
        }
        protected void DrawWindowBase(Point size, Point pos, Color bgColor, Color borderCour, int borderThickness, string title, Color titleBgColor, Color? titleTextColor = null)
        {
            //draws box for windows (interaction key prompt, options window, desciprion box)
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
            //draws string
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
