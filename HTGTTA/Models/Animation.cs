using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Models
{
    public class Animation
    {

        public int CurrentFrame { get; set; } // viewing
        public int FrameCount { get; private set; } // amount of sprites in loop
        public int FrameHeight {get {return Texture.Height;} } // height of texture
        public int FrameWidth { get { return Texture.Width / FrameCount; } } //weidth of texture
        public float FrameSpeed { get; set; } //speed of animation
        public bool IsLooping { get; set; }
        public Texture2D Texture { get; private set; } 

        public Animation (Texture2D texture, int frameCount) //contructor
        {
            Texture = texture;
            FrameCount = frameCount;
            IsLooping = true;
            FrameSpeed = 0.2f; // animation speed
        }
    }
}
