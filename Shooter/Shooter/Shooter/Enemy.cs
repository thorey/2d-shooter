using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{

    class Enemy
    {
        enum EnemyAiState
        {
            // evading
            Evading,
            // enemy can't see player, wandering around.
            Wander,
            //chase!
            Chasing
        }

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

        // how fast can he turn?
        const float enemyTurnSpeed = 0.10f;

        // this value controls the distance at which the enemy will start to chase the
        // player.
        const float enemyChaseDistance = 250.0f;

        // controls the distance at which the enemy will stop because
        // he has "caught" the player.
        const float enemyCaughtDistance = 60.0f;

        // this constant is used to avoid hysteresis, which is common in ai programming.
        const float enemyHysteresis = 15.0f;

        //intitially wanders around
        EnemyAiState enemyState = EnemyAiState.Wander;

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

        float enemyOrientation;

        Vector2 enemyWanderDirection;


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
            /*
            // The enemy always moves to the left so decrement x
            Position.X -= enemyMoveSpeed;
             */
            Position = ClampToViewport(Position, graphics);

            // Update the position of the Animation
            EnemyAnimation.Position = Position;

            // Update Animation
            EnemyAnimation.Update(gameTime);

            float enemyChaseThreshold = enemyChaseDistance;
            float enemyCaughtThreshold = enemyCaughtDistance;
            // if the tank is idle, he prefers to stay idle. we do this by making the
            // chase distance smaller, so the tank will be less likely to begin chasing
            // the cat.
            if (enemyState == EnemyAiState.Wander)
            {
                enemyChaseThreshold -= enemyHysteresis / 2;
            }
            // similarly, if the tank is active, he prefers to stay active. we
            // accomplish this by increasing the range of values that will cause the
            // tank to go into the active state.
            else if (enemyState == EnemyAiState.Chasing)
            {
                enemyChaseThreshold += enemyHysteresis / 2;
                enemyCaughtThreshold -= enemyHysteresis / 2;
            }
            /*// the same logic is applied to the finished state.
            else if (enemyState == EnemyAiState.Caught)
            {
                enemyCaughtThreshold += enemyHysteresis / 2;
            }
            */
            // Second, now that we know what the thresholds are, we compare the tank's 
            // distance from the cat against the thresholds to decide what the tank's
            // current state is.
            float distanceFromPlayer = Vector2.Distance(Position, player.Position);
            if (distanceFromPlayer > enemyChaseThreshold)
            {
                // just like the mouse, if the tank is far away from the cat, it should
                // idle.
                enemyState = EnemyAiState.Wander;
            }
            else if (distanceFromPlayer > enemyCaughtThreshold)
            {
                enemyState = EnemyAiState.Chasing;
            }
            else
            {
               // enemyState = EnemyAiState.Caught;
            }

            // Third, once we know what state we're in, act on that state.
            float currentEnemySpeed;
            if (enemyState == EnemyAiState.Chasing)
            {
                // the tank wants to chase the cat, so it will just use the TurnToFace
                // function to turn towards the cat's position. Then, when the tank
                // moves forward, he will chase the cat.
                enemyOrientation = TurnToFace(Position, player.Position, enemyOrientation,
                    enemyTurnSpeed);
                currentEnemySpeed = enemyMoveSpeed;
            }
            else if (enemyState == EnemyAiState.Wander)
            {
                Wander(Position, ref enemyWanderDirection, ref enemyOrientation,
                    enemyTurnSpeed, graphics);
                currentEnemySpeed = enemyMoveSpeed;
            }
            else
            {
                // this part is different from the mouse. if the tank catches the cat, 
                // it should stop. otherwise it will run right by, then spin around and
                // try to catch it all over again. The end result is that it will kind
                // of "run laps" around the cat, which looks funny, but is not what
                // we're after.
                currentEnemySpeed = 0.0f;
            }

            // this calculation is also just like the mouse's: we construct a heading
            // vector based on the tank's orientation, and then make the tank move along
            // that heading.
            Vector2 heading = new Vector2(
                (float)Math.Cos(enemyOrientation), (float)Math.Sin(enemyOrientation));
            Position += heading * currentEnemySpeed;

            // If the enemy is past the screen or its health reaches 0, deactivate
            if (Position.X < -Width || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove
                Active = false;
            }
        }

        private void Wander(Vector2 position, ref Vector2 wanderDirection,
            ref float orientation, float turnSpeed, GraphicsDeviceManager graphics)
        {
            Random random = new Random();

            wanderDirection.X +=
                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());
            wanderDirection.Y +=
                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());

            // we'll renormalize the wander direction, ...
            if (wanderDirection != Vector2.Zero)
            {
                wanderDirection.Normalize();
            }
            // ... and then turn to face in the wander direction. We don't turn at the
            // maximum turning speed, but at 15% of it. Again, this is a bit of a magic
            // number: it works well for this sample, but feel free to tweak it.
            orientation = TurnToFace(position, position + wanderDirection, orientation,
                .15f * turnSpeed);


            // next, we'll turn the characters back towards the center of the screen, to
            // prevent them from getting stuck on the edges of the screen.
            Vector2 screenCenter = Vector2.Zero;
            screenCenter.X = graphics.GraphicsDevice.Viewport.Width / 2;
            screenCenter.Y = graphics.GraphicsDevice.Viewport.Height / 2;

            float distanceFromScreenCenter = Vector2.Distance(screenCenter, position);
            float MaxDistanceFromScreenCenter =
                Math.Min(screenCenter.Y, screenCenter.X);

            float normalizedDistance =
                distanceFromScreenCenter / MaxDistanceFromScreenCenter;

            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance *
                turnSpeed;

            // once we've calculated how much we want to turn towards the center, we can
            // use the TurnToFace function to actually do the work.
            orientation = TurnToFace(position, screenCenter, orientation,
                turnToCenterSpeed);
        }

        private Vector2 ClampToViewport(Vector2 vector, GraphicsDeviceManager graphics)
        {
            Viewport vp = graphics.GraphicsDevice.Viewport;
            vector.X = MathHelper.Clamp(vector.X, vp.X, vp.X + vp.Width);
            vector.Y = MathHelper.Clamp(vector.Y, vp.Y, vp.Y + vp.Height);
            return vector;
        }

        private static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);

            float difference = WrapAngle(desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            // so, the closest we can get to our target is currentAngle + difference.
            // return that, using WrapAngle again.
            return WrapAngle(currentAngle + difference);
        }

        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            EnemyAnimation.Draw(spriteBatch);
        }

    }
}
