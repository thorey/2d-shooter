using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class PowerUp
    {
        
        Texture2D Texture;
        public Vector2 Position;
        public bool Active;
        Viewport viewport;
        public Vector2 Direction;
        float movespeed;
        public int type;
        
        public int Width
        {
            get { return Texture.Width; }
        }
        public int Height
        {
            get { return Texture.Height; }
        }


        public void Initialize(Viewport viewport, Texture2D texture, Vector2 position,int type)
        {
            this.viewport = viewport;
            Texture = texture;
            Position = position;
            this.type = type;
            movespeed = 5f;
            Active = true;

        }

        public void Update()
        {
            // Projectiles always move to the right
            Position += new Vector2(0,1) * movespeed;

            // Deactivate the bullet if it goes out of screen
            if (Position.X + Texture.Width / 2 > viewport.Width)
                Active = false;
            if (Position.Y + Texture.Height / 2 > viewport.Height)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(Texture, Position, null, Color.White, 0,new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);

        }
    }
}
