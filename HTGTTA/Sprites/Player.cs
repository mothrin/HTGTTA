using HTGTTA.Manager;
using HTGTTA.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Sprites
{
    public class Player : Sprite
    {
        public Player(Texture2D texture)
                : base(texture)
        {
            _texture = texture;
        }

        public Player(Dictionary<string, Animation> animations) 
            : base(animations)
        {
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);
        }


        public virtual void Move()
        {

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W))
            {
                Velocity.Y -= spriteSpeed;
            }
            if (kstate.IsKeyDown(Keys.S))
            {
                Velocity.Y += spriteSpeed;
            }
            if (kstate.IsKeyDown(Keys.A))
            {
                Velocity.X -= spriteSpeed;
            }
            if (kstate.IsKeyDown(Keys.D))
            {
                Velocity.X += spriteSpeed;
            }
            if (Velocity.X > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - _texture.Width / 2)
            {
                Velocity.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
            }
            else if (Velocity.X < _texture.Width / 2)
            {
                Velocity.X = _texture.Width / 2;
            }
            if (Velocity.Y > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - _texture.Width / 2)
            {
                Velocity.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - _texture.Height / 2;
            }
            else if (Velocity.Y < _texture.Height / 2)
            {
                Velocity.Y = _texture.Height / 2;
            }


        }

        protected bool isTouchingLeft(Sprite sprite)
        {
            return this.Rectangle.Right + this.Velocity.X > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Left &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        protected bool isTouchingRight(Sprite sprite)
        {
            return this.Rectangle.Left + this.Velocity.X < sprite.Rectangle.Right &&
                this.Rectangle.Right > sprite.Rectangle.Right &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        protected bool isTouchingTop(Sprite sprite)
        {
            return this.Rectangle.Bottom + this.Velocity.Y > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Top &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }

        protected bool isTouchingBottom(Sprite sprite)
        {
            return this.Rectangle.Top + this.Velocity.Y < sprite.Rectangle.Bottom &&
              this.Rectangle.Bottom > sprite.Rectangle.Bottom &&
              this.Rectangle.Right > sprite.Rectangle.Left &&
              this.Rectangle.Left < sprite.Rectangle.Right;
        }
    



        

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            Move();
            SetAnimations();


            foreach (var sprite in sprites)
            {
                if (sprite == this)
                    continue;

                if ((this.Velocity.X > 0 && this.isTouchingLeft(sprite)) ||
                    (this.Velocity.X < 0 & this.isTouchingRight(sprite)))
                    this.Velocity.X = 0;

                if ((this.Velocity.Y > 0 && this.isTouchingTop(sprite)) ||
                    (this.Velocity.Y < 0 & this.isTouchingBottom(sprite)))
                    this.Velocity.Y = 0;
            }

            Position += Velocity;

            Velocity = Vector2.Zero;


            _animationManager.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, Position, Color.White);
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch);


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
