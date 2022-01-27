using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{

    class Asteroid
    {
        // Animation representing the enemy
        public Animation EnemyAnimation;

        // The position of the enemy ship relative to the top left corner of thescreen
        public Vector2 Position;

        // The state of the Enemy Ship
        public bool Active;

        // The hit points of the enemy, if this goes to zero the enemy dies
        public int Health;

        // The amount of damage the enemy inflicts on the player ship
        public int Damage;

        // The amount of score the enemy will give to the player
        public int Value;

        //set how fast the enemy moves
        float enemyMoveSpeed = 5.0f;


        // Get the width of the enemy ship
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }

        // Get the height of the enemy ship
        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }


        public void Initialize(Animation animation, Vector2 position)
        {
            // Load the enemy ship texture
            EnemyAnimation = animation;

            // Set the position of the enemy
            Position = position;

            // We initialize the enemy to be active so it will be updated
            Active = true;


            // Set the health of the enemy
            Health = 10;

            // Set the amount of damage the enemy can do
            Damage = 10;

            // Set the score value of the enemy
            Value = 100;
        }


        public void Update(GameTime gameTime, GraphicsDeviceManager graphics, Player player)
        {
           
            // The enemy always moves down so increment y
            Position.Y += enemyMoveSpeed;

            // Update the position of the Animation
            EnemyAnimation.Position = Position;

            // Update Animation
            EnemyAnimation.Update(gameTime);

            // If the enemy is past the screen or its health reaches 0, deactivate
            if (Position.Y > graphics.GraphicsDevice.Viewport.Height || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            EnemyAnimation.Draw(spriteBatch);
        }

    }
}
