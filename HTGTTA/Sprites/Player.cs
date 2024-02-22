using HTGTTA.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace HTGTTA.Sprites
{
    
    public class Player : Sprite
    {
        public Player(Game game, string textureAsset) : base(game, textureAsset) { }

        public Player(Game game, Dictionary<string, Animation> animations) : base(game, animations) { }

        public List<Sprite> _sprites;

        public virtual void Move() //movement code
        {
            if (_keyboard.KeyDown(Keys.W))
            {
                Velocity.Y -= spriteSpeed;
            }
            if (_keyboard.KeyDown(Keys.S))
            {
                Velocity.Y += spriteSpeed;
            }
            if (_keyboard.KeyDown(Keys.A))
            {
                Velocity.X -= spriteSpeed;
            }
            if (_keyboard.KeyDown(Keys.D))
            {
                Velocity.X += spriteSpeed;
            }

            CheckPosition(Width, Height);
        }


        protected void CheckPosition(int width, int height)
        {
            if (Position.X > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - width) // so ghost can't go out the sides of window
            {
                Position = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - width, Position.Y);
            }
            else if (Position.X < 0) 
            {
                Position = new Vector2(0, Position.Y);
            }
            if (Position.Y < GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2  - height)   // so ghost can't go out bottom of window , 
            {
                Position = new Vector2(Position.X, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - height);
            }
            else if (Position.Y > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - height) //ghost can't go onto wall
            {
                Position = new Vector2(Position.X, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - height);
            }
        }

        public override void Update(GameTime gameTime)
        {
            Move();
            SetAnimations();

            Position += Velocity;

            Velocity = Vector2.Zero;



            _animationManager.Update(gameTime);
        }
        
        protected virtual void SetAnimations()
        {
            if (Velocity.X > 0)
                _animationManager.Play(_animations["WalkRight"]);
            else if (Velocity.X < 0)
                _animationManager.Play(_animations["WalkLeft"]);
            else if (Velocity.Y > 0)
                _animationManager.Play(_animations["WalkDown"]);
            else if (Velocity.Y < 0)
                _animationManager.Play(_animations["WalkUp"]);
            else _animationManager.Stop();
        }

    }
}
