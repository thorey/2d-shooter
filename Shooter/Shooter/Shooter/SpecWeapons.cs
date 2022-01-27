using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class SpecWeapons //deals with palyer specaial weapons
    {
        public Texture2D Texture;
        public Vector2 Position;
        public int Spec;
        public bool Active;
        public int Damage;
        public float angle;
        public Vector2 Direction;
        Viewport viewport;
        // Get the width of the projectile ship
        public int Width
        {
            get { return Texture.Width; }
        }

        // Get the height of the projectile ship
        public int Height
        {
            get { return Texture.Height; }
        }

        // Determines how fast the projectile moves
        float projectileMoveSpeed;

        public void Initialize(Viewport viewport, Texture2D texture, Vector2 position, Vector2 dest,int type)
        {
            Texture = texture;
            Position = position;
            this.viewport = viewport;
            
            Spec = type;
           
                Direction = dest - Position;
                //angle2 = (float)(Math.Atan2(direction.Y, direction.X));
                if (Direction != Vector2.Zero)
                    Direction.Normalize();
                angle = (float)(Math.Atan2(Direction.Y, Direction.X));
                Active = true;
                Damage = 20;
                
                projectileMoveSpeed = 15f;
            
            
        }
        public void Update()
        {
            // Projectiles always move to the right
            Position += Direction * projectileMoveSpeed;

            // Deactivate the bullet if it goes out of screen
            if (Position.X + Texture.Width / 2 > viewport.Width)
                Active = false;
            if (Position.Y + Texture.Height / 2 > viewport.Height)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
                spriteBatch.Draw(Texture, Position, null, Color.White, angle, new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
            
        }
    }
}
