// Projectile.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{
    class Projectile
    {
        // Image representing the Projectile
        public Texture2D Texture;

        // Position of the Projectile relative to the upper left side of the screen
        public Vector2 Position;
        public Vector2 FireDirection;

        // State of the Projectile
        public bool Active;

        // The amount of damage the projectile can inflict to an enemy
        public int Damage;
        public float angle2;
        public float angle3;
        public Vector2 direction;
        // Represents the viewable boundary of the game
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


        public void Initialize(Viewport viewport, Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            this.viewport = viewport;

            Active = true;

            Damage = 10;

            projectileMoveSpeed = 20f;
        }
        public void EnemyInitialize(Viewport viewport, Texture2D texture, Vector2 position, Vector2 heading)
        {
            Texture = texture;
            Position = position;
            this.viewport = viewport;

            Active = true;

            Damage = 10;
            heading.Normalize();
            FireDirection = heading;
            angle3 = (float)(Math.Atan2(FireDirection.Y, FireDirection.X));
            if (FireDirection != Vector2.Zero)
                FireDirection.Normalize();
            angle3 = (float)(Math.Atan2(FireDirection.Y, FireDirection.X));
            projectileMoveSpeed = 20f;
        }
		public void Initialize(Viewport viewport, Texture2D texture, Vector2 position,Vector2 dest)
        {
            Texture = texture;
            Position = position;
            this.viewport = viewport;
            direction = dest-Position;
            //angle2 = (float)(Math.Atan2(direction.Y, direction.X));
            if (direction != Vector2.Zero)
                direction.Normalize();
            angle2 = (float)(Math.Atan2(direction.Y, direction.X));
            Active = true;
            Damage = 10;

            projectileMoveSpeed = 15f;
        }
        public void Update()
        {
            // Projectiles always move to the right
            Position += direction*projectileMoveSpeed;

            // Deactivate the bullet if it goes out of screen
            if (Position.X + Texture.Width / 2 > viewport.Width)
                Active = false;
			if (Position.Y + Texture.Height / 2 > viewport.Height)
				Active = false;
        }

        public void EnemyUpdate()
        {
            //
            Position += FireDirection * projectileMoveSpeed * .5f;

            // Deactivate the bullet if it goes out of screen
            if (Position.X + Texture.Width / 2 > viewport.Width ||
                Position.Y + Texture.Height / 2 > viewport.Height)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
			spriteBatch.Draw(Texture, Position, null, Color.White, angle2,new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);

           // spriteBatch.Draw(Texture, Position, null, Color.White, 0f,new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
        }
        public void EnemyDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, angle3, new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
