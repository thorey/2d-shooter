using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;

namespace Shooter
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        Player player;                      // Represents the player 
        float playerMoveSpeed;              // A movement speed for the player
        
        Texture2D cursorTexture;            //mouse cursor
        Vector2 cursorPosition;
        MouseState previousMouseState;        //state of mouse
        MouseState currentMouseState;
        KeyboardState currentKeyboardState; // Keyboard states used to determine key presses
        KeyboardState previousKeyboardState;

        ParallaxingBackground mainBackground;
        
        Vector2 direction;
        float angle;
        int type;
        int shield;
        int currentWeapon;// which special weapon are you using 1,2,3
        int ammo;
        // Enemies
        Texture2D enemyTexture;
        Texture2D enemyTexture2;
        Texture2D BossTexture;
        Texture2D asteroidTexture1;
        Texture2D asteroidTexture2;
        Texture2D asteroidTexture3;
        List<Enemy> enemies;
        List<Enemy2> enemies2;
        List<PowerUp> powerups;
        List<Boss> bosses;
        List<Asteroid> asteroids;
        Texture2D specWeapon1;
        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousEnemySpawnTime;
        TimeSpan asteroidSpawnTime;
        TimeSpan asteroidFieldSpawnTime;
        TimeSpan previousAsteroidSpawnTime;
        TimeSpan spawnDelay;
        TimeSpan resetTime;
        TimeSpan startBossFight;
        TimeSpan startAsteroidField;
        TimeSpan endAsteroidField;
        int startAsteroid;
        int endAsteroid;
        int startBoss;
        int enemySpawn;
        int multiplier;

        Random random;              // A random number generator
        Texture2D winTexture;
        Texture2D projectileTexture;
        Texture2D projectileTexture2;
        Texture2D bossMissiles;
        List<Projectile> projectiles;
        List<Projectile> enemyprojectiles;
        List<Projectile> bossprojectiles;
        List<SpecWeapons> specweapons;
		Vector2 dest;
        // The rate of fire of the player laser
        TimeSpan fireTime;
        TimeSpan previousFireTime;
        // drops
        Texture2D drop1;
        Texture2D drop2;
        Texture2D drop3;
        // Explosion graphics list
        Texture2D explosionTexture;
        List<Animation> explosions;

        // The sound that is played when a laser is fired
        SoundEffect laserSound;

        // The sound used when the player or an enemy dies
        SoundEffect explosionSound;

        Song gameplayMusic;     // The music played during gameplay
        bool isWin=false;
        int score;              //Number that holds the player score
        SpriteFont font;        // The font used to display UI elements
        bool isPaused;
        Texture2D pauseTexture;
        bool isMain;
        Texture2D mainTexture;
        bool isBoss;    //determines if there is an active boss
        bool noEnemies; //no enemies to be updated
        bool asteroid;  //do asteroids exist?
        Texture2D gameoverTexture;
        bool isGameOver = false;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 1024;

            //IsMouseVisible = true;
            this.graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            player = new Player();                          // Initialize the player class
            playerMoveSpeed = 8.0f;                         // Set a constant player move speed

            mainBackground = new ParallaxingBackground();

            // Initialize the enemies list
            enemies = new List<Enemy>();
            enemies2 = new List<Enemy2>();
            bosses = new List<Boss>();
            asteroids = new List<Asteroid>();
            specweapons=new List<SpecWeapons>();
            previousEnemySpawnTime = TimeSpan.Zero;              // Set the time keepers to zero
            enemySpawnTime = TimeSpan.FromSeconds(0.2);    // Used to determine how fast enemy respawns
            resetTime = TimeSpan.Zero;
            spawnDelay = TimeSpan.FromSeconds(4.0);
            previousAsteroidSpawnTime = TimeSpan.Zero;
            asteroidFieldSpawnTime = TimeSpan.FromSeconds(.2);
            asteroidSpawnTime = TimeSpan.FromSeconds(1.5);
            startAsteroidField = TimeSpan.FromSeconds(30.0);
            endAsteroidField = TimeSpan.FromSeconds(65.0);
            startBossFight = TimeSpan.FromSeconds(120.0);
       /*     startAsteroid = 8;
            endAsteroid = 158;
            startBoss = 324;
            enemySpawn = 0;
        */
            //one enemy group spawns every 4 seconds and during the asteroid field one
            //asteroid spawns every .2 seconds.  based on that, interger values are calculated
            //to time certain events.
            startAsteroid = 24;
            endAsteroid = 174;
            startBoss = 372;
            enemySpawn = 0;
            multiplier = 1;

            isPaused = true;
            isMain = true;
            isBoss = false;    //determines if there is an active boss
            noEnemies = false; //no enemies to be updated
            asteroid = false;  //do asteroids exist?

            shield = 0;
            currentWeapon = 0;// which special weapon are you using 1,2,3
            ammo = 0;

            random = new Random();                          // Initialize our random number generator

            projectiles = new List<Projectile>();
            enemyprojectiles = new List<Projectile>();
            bossprojectiles = new List<Projectile>();
            powerups = new List<PowerUp>();
            fireTime = TimeSpan.FromSeconds(.15f);          // Set the laser to fire every quarter second

            explosions = new List<Animation>();              // Initialize the explosion list

            score = 0;                                      //Set player's score to zero

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, 2 * GraphicsDevice.Viewport.Height / 3);
            
            player.Initialize(playerAnimation, playerPosition);

            mainBackground.Initialize(Content, "starfieldBig", GraphicsDevice.Viewport.Height, 1);
            mainTexture = Content.Load<Texture2D>("mainMenu2");
            pauseTexture = Content.Load<Texture2D>("pauseMenu");
            gameoverTexture = Content.Load<Texture2D>("GameOver");
            winTexture = Content.Load<Texture2D>("win");
            enemyTexture = Content.Load<Texture2D>("saucer");
            enemyTexture2 = Content.Load<Texture2D>("EnemyShip001");
            BossTexture = Content.Load<Texture2D>("boss");
            asteroidTexture1 = Content.Load<Texture2D>("asteroid1");
            asteroidTexture2 = Content.Load<Texture2D>("asteroids2");
            asteroidTexture3 = Content.Load<Texture2D>("asteroid3");
            cursorTexture = Content.Load<Texture2D>("mousecross");
            projectileTexture = Content.Load<Texture2D>("laser");
            projectileTexture2 = Content.Load<Texture2D>("laser3");
            bossMissiles = Content.Load<Texture2D>("LightningBall");
            explosionTexture = Content.Load<Texture2D>("explosion");
            specWeapon1 = Content.Load<Texture2D>("laser2");
            gameplayMusic = Content.Load<Song>("sound/greyhound");
            
            drop1 = Content.Load<Texture2D>("flame");
            drop2 = Content.Load<Texture2D>("burst");
            drop3 = Content.Load<Texture2D>("shield");

            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");

            font = Content.Load<SpriteFont>("gameFont");

            PlayMusic(gameplayMusic);            // Start the music right away
        }

        private void Reset(GameTime gameTime)
        {
            player = new Player();                          // Initialize the player class
            playerMoveSpeed = 8.0f;                         // Set a constant player move speed

            mainBackground = new ParallaxingBackground();

            // Initialize the enemies list
            enemies = new List<Enemy>();
            enemies2 = new List<Enemy2>();
            bosses = new List<Boss>();
            asteroids = new List<Asteroid>();
            specweapons = new List<SpecWeapons>();
            previousEnemySpawnTime = TimeSpan.Zero;              // Set the time keepers to zero
            enemySpawnTime = TimeSpan.FromSeconds(0.2);    // Used to determine how fast enemy respawns
            resetTime = TimeSpan.Zero;
            spawnDelay = TimeSpan.FromSeconds(4.0);
            previousAsteroidSpawnTime = TimeSpan.Zero;
            asteroidFieldSpawnTime = TimeSpan.FromSeconds(.2);
            asteroidSpawnTime = TimeSpan.FromSeconds(1.5);
            startAsteroidField = TimeSpan.FromSeconds(10.0);
            endAsteroidField = TimeSpan.FromSeconds(20.0);
            startBossFight = TimeSpan.FromSeconds(60.0);

            isPaused = true;
            isMain = true;
            isBoss = false;    //determines if there is an active boss
            noEnemies = false; //no enemies to be updated
            asteroid = false;  //do asteroids exist?

            shield = 0;
            currentWeapon = 0;// which special weapon are you using 1,2,3
            ammo = 0;

            startAsteroid = 24;
            endAsteroid = 174;
            startBoss = 372;
            enemySpawn = 0;
            multiplier = 1;

            random = new Random();                          // Initialize our random number generator

            projectiles = new List<Projectile>();
            enemyprojectiles = new List<Projectile>();
            bossprojectiles = new List<Projectile>();
            powerups = new List<PowerUp>();
            fireTime = TimeSpan.FromSeconds(.15f);          // Set the laser to fire every quarter second

            explosions = new List<Animation>();              // Initialize the explosion list

            score = 0;                                      //Set player's score to zero

            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin();
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, 2 * GraphicsDevice.Viewport.Height / 3);

            player.Initialize(playerAnimation, playerPosition);

            mainBackground.Initialize(Content, "starfieldBig", GraphicsDevice.Viewport.Height, 1);
            mainTexture = Content.Load<Texture2D>("mainMenu2");
            pauseTexture = Content.Load<Texture2D>("pauseMenu");

            enemyTexture = Content.Load<Texture2D>("saucer");
            enemyTexture2 = Content.Load<Texture2D>("EnemyShip001");
            BossTexture = Content.Load<Texture2D>("boss");
            asteroidTexture1 = Content.Load<Texture2D>("asteroid1");
            asteroidTexture2 = Content.Load<Texture2D>("asteroids2");
            asteroidTexture3 = Content.Load<Texture2D>("asteroid3");
            cursorTexture = Content.Load<Texture2D>("mousecross");
            projectileTexture = Content.Load<Texture2D>("laser");
            projectileTexture2 = Content.Load<Texture2D>("laser3");
            bossMissiles = Content.Load<Texture2D>("LightningBall");
            explosionTexture = Content.Load<Texture2D>("explosion");
            specWeapon1 = Content.Load<Texture2D>("laser2");
            gameplayMusic = Content.Load<Song>("sound/greyhound");

            drop1 = Content.Load<Texture2D>("flame");
            drop2 = Content.Load<Texture2D>("burst");
            drop3 = Content.Load<Texture2D>("shield");

            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");

            font = Content.Load<SpriteFont>("gameFont");

            PlayMusic(gameplayMusic);            // Start the music right away
            // Save the previous state of the keyboard so we can determinesingle key presses
            previousKeyboardState = currentKeyboardState;
            //
            //currentMouseState = Mouse.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            //mouse postion
            cursorPosition.X = Mouse.GetState().X;
            cursorPosition.Y = Mouse.GetState().Y;

            dest = cursorPosition;
            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape) && isPaused == false)
                isPaused = true;

            if (!isPaused)
            {
                UpdatePlayer(gameTime);
                mainBackground.Update();
                // Update the enemies
                if (!isBoss && enemySpawn >= endAsteroid)
                {
                    noEnemies = false;
                }
                if (!noEnemies)
                    UpdateEnemies(gameTime, graphics, player);

                //Update asteroids
                if (asteroid)
                    UpdateAsteroids(gameTime, graphics, player);

                //Update boss
                if (isBoss)
                    UpdateBoss(gameTime, graphics, player);
                UpdateCollision();
                UpdateProjectiles();
                UpdateSpecweapons();
                UpdateEnemyProjectiles();
                UpdateBossProjectiles();
                UpdatePower();
                UpdateExplosions(gameTime);
            }
        }

        private void PlayMusic(Song song)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(song);


                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);

        }

        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();
            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 49, 55, 1, 30, Color.White, 1f, true);
            // Randomly generate the position of the enemy
            Vector2 position;
            if (random.Next(3) == 0)
            {
                if (random.Next(2) == 0)
                    position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100) / 3);
                else
                    position = new Vector2(-GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100) / 3);
            }
            else
                position = new Vector2(random.Next(GraphicsDevice.Viewport.Width), -GraphicsDevice.Viewport.Height + enemyTexture2.Height / 2);
            // Create an enemy
            Enemy enemy = new Enemy();
            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);
            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }

        private void AddEnemy2()
        {
            if (enemies2.Count < 4)
            {
                Animation enemyAnimation2 = new Animation();
                enemyAnimation2.Initialize(enemyTexture2, Vector2.Zero, 112, 150, 1, 30, Color.White, 1f, true);
                Vector2 position2;
                if (random.Next(3) == 0)
                {
                    if (random.Next(2) == 0)
                        position2 = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100) / 2);
                    else
                        position2 = new Vector2(-GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100) / 2);
                }
                else
                    position2 = new Vector2(random.Next(GraphicsDevice.Viewport.Width), -GraphicsDevice.Viewport.Height + enemyTexture2.Height / 2);
                Enemy2 enemy2 = new Enemy2();
                enemy2.Initialize(enemyAnimation2, position2);
                enemies2.Add(enemy2);
            }
        }

        private void UpdateEnemies(GameTime gameTime, GraphicsDeviceManager graphics, Player player)
        {
            if ((enemySpawn < startBoss) && (enemySpawn < startAsteroid || enemySpawn > endAsteroid)) //during gameplay not during asteroid field or before the boss fight
            {
                //Every 4 seconds spawn four new enemies
                while (gameTime.TotalGameTime - resetTime >= spawnDelay)
                {
                    if (gameTime.TotalGameTime - previousEnemySpawnTime > enemySpawnTime)
                    {
                        previousEnemySpawnTime = gameTime.TotalGameTime;

                        // Add an Enemy
                        AddEnemy();
                        AddEnemy();
                        AddEnemy();
                        AddEnemy();
                        AddEnemy2();
                        AddEnemy2();
                        AddEnemy2();
                        AddEnemy2();
                        resetTime = gameTime.TotalGameTime;
                        enemySpawn++;
                    }
                }
                if (gameTime.TotalGameTime - previousEnemySpawnTime > enemySpawnTime)
                {
                    AddEnemy2();
                }
            }

            // Update the Enemies
            for (int i = enemies2.Count - 1; i >= 0; i--)
            {
                if (random.Next(200) == 0)//shoot randomly every now and then
                {
                    int k = random.Next(enemies2.Count - 1);
                    enemies2[k].FireDirection = enemies2[k].getFireDirection(player, enemies2[k].enemyOrientation);
                    AddEnemyProjectile(enemies2[k].Position, k, enemies2[k].FireDirection);
                }
            }
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime, graphics, player);

                if (enemies[i].Active == false)
                {
                    // If not active and health <= 0
                    if (enemies[i].Health <= 0)
                    {
                        AddDrop(enemies[i].Position, random.Next(100));
                        // Add an explosion
                        AddExplosion(enemies[i].Position);
                        
                        // Play the explosion sound
                        explosionSound.Play();

                        //Add to the player's score
                        score += enemies[i].Value * multiplier;
                        multiplier++;
                    }

                    enemies.RemoveAt(i);
                }
            }
            for (int i = enemies2.Count - 1; i >= 0; i--)
            {
                enemies2[i].Update(gameTime, graphics, player);

                if (enemies2[i].Active == false)
                {
                    // If not active and health <= 0
                    if (enemies2[i].Health <= 0)
                    {
                        // Add an explosion
                        AddExplosion(enemies2[i].Position);
                        
                        AddDrop(enemies2[i].Position,random.Next(50));
                        // Play the explosion sound
                        explosionSound.Play();

                        //Add to the player's score
                        score += enemies2[i].Value * multiplier;
                        multiplier++;
                    }

                    enemies2.RemoveAt(i);
                }
            }
            if ((enemySpawn >= startBoss) && enemies.Count - 1 < 0 && enemies2.Count - 1 < 0)
            {
                noEnemies = true;
                AddBoss();
                isBoss = true;
            }
            else if ((enemySpawn >= startAsteroid && enemySpawn < endAsteroid) && enemies.Count - 1 < 0 && enemies2.Count - 1 < 0)
            {
                noEnemies = true;
                AddAsteroid();
                asteroid = true;
            }
            else if (!isBoss && enemySpawn >= endAsteroid)
            {
                //30 second intervals
             //   startAsteroid = 166;
             //   endAsteroid = 316;
                startAsteroid = 198;
                endAsteroid = 348;
            }
        }

        private void AddBoss()
        {
            // Create the animation object
            Animation bossAnimation = new Animation();
            // Initialize the animation with the correct animation information
            bossAnimation.Initialize(BossTexture, Vector2.Zero, 130, 160, 1, 30, Color.White, 1.5f, true);
            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(random.Next(100, GraphicsDevice.Viewport.Height - 100), -GraphicsDevice.Viewport.Width + enemyTexture2.Width / 2);
            // Create an enemy
            Boss boss = new Boss();

            // Initialize the enemy
            boss.Initialize(bossAnimation, position);

            // Add the enemy to the active enemies list
            bosses.Add(boss);

        }

        private void UpdateBoss(GameTime gameTime, GraphicsDeviceManager graphics, Player player)
        {
            for (int i = bosses.Count - 1; i >= 0; i--)
            {
                if (random.Next(100) == 0)//shoot randomly every now and then
                {
                    bosses[i].FireDirection = bosses[i].getFireDirection(player, bosses[i].enemyOrientation);
                    AddBossProjectile(bosses[i].Position, bosses[i].FireDirection, bossMissiles);
                }
            }
            for (int i = 0; i < bosses.Count; i++)
            {
                bosses[i].Update(gameTime, graphics, player);
                if (bosses[i].Active == false)
                {
                    // If not active and health <= 0
                    if (bosses[i].Health <= 0)
                    {
                        // Add an explosion
                        AddExplosion(bosses[i].Position);

                        // Play the explosion sound
                        explosionSound.Play();

                        //Add to the player's score
                        score += bosses[i].Value * multiplier;
                        isWin = true;
                    }

                    bosses.RemoveAt(i);
                }
            }
        }

        private void AddAsteroid()
        {
            int choose = random.Next(100);

            if (choose % 3 == 0)
            {
                // Create the animation object
                Animation asteroidAnimation = new Animation();
                // Initialize the animation with the correct animation information
                asteroidAnimation.Initialize(asteroidTexture1, Vector2.Zero, 64, 64, 1, 30, Color.White, 1f, true);
                // Randomly generate the position of the enemy
                Vector2 position = new Vector2(random.Next(GraphicsDevice.Viewport.Width), -GraphicsDevice.Viewport.Height + enemyTexture2.Height / 2);
                // Create an enemy
                Asteroid asteroid = new Asteroid();
                // Initialize the enemy
                asteroid.Initialize(asteroidAnimation, position);
                // Add the enemy to the active enemies list
                asteroids.Add(asteroid);
            }
            else if (choose % 3 == 1)
            {
                // Create the animation object
                Animation asteroidAnimation = new Animation();
                // Initialize the animation with the correct animation information
                asteroidAnimation.Initialize(asteroidTexture2, Vector2.Zero, 77, 75, 1, 30, Color.White, 1f, true);
                // Randomly generate the position of the enemy
                Vector2 position = new Vector2(random.Next(GraphicsDevice.Viewport.Width), -GraphicsDevice.Viewport.Height + enemyTexture2.Height / 2);
                // Create an enemy
                Asteroid asteroid = new Asteroid();
                // Initialize the enemy
                asteroid.Initialize(asteroidAnimation, position);
                // Add the enemy to the active enemies list
                asteroids.Add(asteroid);
            }
            else
            {
                // Create the animation object
                Animation asteroidAnimation = new Animation();
                // Initialize the animation with the correct animation information
                asteroidAnimation.Initialize(asteroidTexture3, Vector2.Zero, 32, 32, 1, 30, Color.White, 1f, true);
                // Randomly generate the position of the enemy
                Vector2 position = new Vector2(random.Next(GraphicsDevice.Viewport.Width), -GraphicsDevice.Viewport.Height + enemyTexture2.Height / 2);
                // Create an enemy
                Asteroid asteroid = new Asteroid();
                // Initialize the enemy
                asteroid.Initialize(asteroidAnimation, position);
                // Add the enemy to the active enemies list
                asteroids.Add(asteroid);
            }

        }

        private void UpdateAsteroids(GameTime gameTime, GraphicsDeviceManager graphics, Player player)
        {
            if (enemySpawn >= startAsteroid && enemySpawn <= endAsteroid) //during the asteroid field
            {
                if (gameTime.TotalGameTime - previousAsteroidSpawnTime > asteroidFieldSpawnTime)
                {
                    previousAsteroidSpawnTime = gameTime.TotalGameTime;
                    AddAsteroid();
                    enemySpawn++;
                }
            }
            if (gameTime.TotalGameTime - previousAsteroidSpawnTime > asteroidSpawnTime) //at all other times after the first asteroid field
            {
                previousAsteroidSpawnTime = gameTime.TotalGameTime;
                AddAsteroid();
            }
            for (int i = 0; i < asteroids.Count; i++)
            {
                asteroids[i].Update(gameTime, graphics, player);
                if (asteroids[i].Active == false)
                {
                    // If not active and health <= 0
                    if (asteroids[i].Health <= 0)
                    {
                        // Add an explosion
                        AddExplosion(asteroids[i].Position);

                        // Play the explosion sound
                        explosionSound.Play();

                        //Add to the player's score
                        score += asteroids[i].Value * multiplier;
                    }

                    asteroids.RemoveAt(i);
                }
            }
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }
        
        private void AddSpecWeapon(Vector2 position,Vector2 dest)
        {
            SpecWeapons specweapon = new SpecWeapons();
            specweapon.Initialize(GraphicsDevice.Viewport, specWeapon1, position, dest, currentWeapon);
            specweapons.Add(specweapon);
        }
        
        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
            projectiles.Add(projectile);
        }
        
        private void AddEnemyProjectile(Vector2 position, Vector2 dest)
        {

            Projectile projectile = new Projectile();
            projectile.EnemyInitialize(GraphicsDevice.Viewport, projectileTexture, position, dest);
            projectiles.Add(projectile);
        }

        private void AddBossProjectile(Vector2 position, Vector2 FireDirection, Texture2D projectileTexture)
        {
            Projectile projectile = new Projectile();
            projectile.EnemyInitialize(GraphicsDevice.Viewport, projectileTexture, position, FireDirection);
            bossprojectiles.Add(projectile);
        }

        private void AddProjectile(Vector2 position, Vector2 dest)
        {

            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture2, position, dest);
            projectiles.Add(projectile);
        }
        
        private void AddProjectile2(Vector2 position, Vector2 dest)
        {

            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, specWeapon1, position, dest);
            projectiles.Add(projectile);
        }
        
        private void UpdateProjectiles()
        {
            // Update the Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();

                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        private void UpdateBossProjectiles()
        {
            // Update the Projectiles
            for (int i = bossprojectiles.Count - 1; i >= 0; i--)
            {
                bossprojectiles[i].EnemyUpdate();

                if (bossprojectiles[i].Active == false)
                {
                    bossprojectiles.RemoveAt(i);
                }
            }
        }
        
        private void AddDrop(Vector2 position1,int chance)
        {
            
            if (chance==1)
            {
                PowerUp pw = new PowerUp();
                pw.Initialize(GraphicsDevice.Viewport,drop1,position1,1);
                powerups.Add(pw);
            }
            if (chance == 2)
            {
                PowerUp pw = new PowerUp();
                pw.Initialize(GraphicsDevice.Viewport, drop2, position1,2);
                powerups.Add(pw);
            }
            if (chance == 3)
            {
                PowerUp pw = new PowerUp();
                pw.Initialize(GraphicsDevice.Viewport, drop3, position1,3);
                powerups.Add(pw);
            }

        }
        
        private void UpdatePower()
        {
            for (int i = powerups.Count - 1; i >= 0; i--)
            {
                powerups[i].Update();

                if (powerups[i].Active == false)
                {
                    powerups.RemoveAt(i);
                }
            }
        }
        
        private void UpdateSpecweapons()
        {
            // Update the Projectiles
            for (int i = specweapons.Count - 1; i >= 0; i--)
            {
                specweapons[i].Update();

                if (specweapons[i].Active == false)
                {
                    specweapons.RemoveAt(i);
                }
            }
        }
        
        private void AddEnemyProjectile(Vector2 position, int k, Vector2 FireDirection)
        {
            Projectile projectile = new Projectile();
            projectile.EnemyInitialize(GraphicsDevice.Viewport, projectileTexture, position, FireDirection);
            enemyprojectiles.Add(projectile);
        }

        private void UpdateEnemyProjectiles()
        {
            // Update the Projectiles
            for (int i = enemyprojectiles.Count - 1; i >= 0; i--)
            {
                enemyprojectiles[i].EnemyUpdate();

                if (enemyprojectiles[i].Active == false)
                {
                    enemyprojectiles.RemoveAt(i);
                }
            }

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Save the previous state of the keyboard so we can determinesingle key presses
            previousKeyboardState = currentKeyboardState;
            //
            //currentMouseState = Mouse.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            //mouse postion
            cursorPosition.X = Mouse.GetState().X;
            cursorPosition.Y = Mouse.GetState().Y;

            dest = cursorPosition;
            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape) && isPaused == false)
                isPaused = true;

            if (!isPaused&&!isGameOver&&!isMain&&!isWin)
            {
                UpdatePlayer(gameTime);
                mainBackground.Update();
                // Update the enemies
                if (!isBoss && enemySpawn >= endAsteroid)
                {
                    noEnemies = false;
                }
                if (!noEnemies)
                    UpdateEnemies(gameTime, graphics, player);

                //Update asteroids
                if (asteroid)
                    UpdateAsteroids(gameTime, graphics, player);

                //Update boss
                if (isBoss)
                    UpdateBoss(gameTime, graphics, player);
                UpdateCollision();
                UpdateProjectiles();
                UpdateSpecweapons();
                UpdateEnemyProjectiles();
                UpdateBossProjectiles();
                UpdatePower();
                UpdateExplosions(gameTime);
            }

            base.Update(gameTime);
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);

            // Use the Keyboard
            if (currentKeyboardState.IsKeyDown(Keys.A))
            {
                player.Position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                player.Position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                player.Position.Y += playerMoveSpeed;
            }
            
            //gets angle to rotate player
            direction = (player.Position) - cursorPosition;
            angle = (float)(Math.Atan2(direction.Y, direction.X));
            
            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0 + player.Width / 2, GraphicsDevice.Viewport.Width - player.Width / 2);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0 + player.Height / 2, GraphicsDevice.Viewport.Height - player.Height / 2);

            // Fire only every interval we set as the fireTime
            if (gameTime.TotalGameTime - previousFireTime > fireTime)
            {
                // Reset our current time
                previousFireTime = gameTime.TotalGameTime;

                // Add the projectile, but add it to the front and center of the player
                // AddProjectile(player.Position + new Vector2(player.Width / 2, 0));
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {

                    AddProjectile(player.Position - new Vector2(player.Width / 2, player.Height / 4), dest);
                    laserSound.Play();

                }


            }
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released && ammo!=0)
            {
                if (currentWeapon == 1)
                {
                    AddSpecWeapon(player.Position - new Vector2(player.Width / 2, 3 * (player.Height / 4)), dest);
                    laserSound.Play();
                    ammo--;
                }

                if (currentWeapon == 2)
                {
                    AddSpecWeapon(player.Position - new Vector2(player.Width / 2, 3 * (player.Height / 4)), dest);
                    AddSpecWeapon(player.Position - new Vector2(player.Width / 2, 3 * (player.Height / 4)), dest+new Vector2(100,0));
                    AddSpecWeapon(player.Position - new Vector2(player.Width / 2, 3 * (player.Height / 4)), dest + new Vector2(-100,0));
                    ammo--;
                }
            }
            // Play the laser sound
            //laserSound.Play();


            // reset score if player health goes to zero
            if (player.Health <= 0)
            {
                if (player.Lives > 0)
                {
                    player.Lives--;
                    player.Health = 100;
                }
                if (player.Lives == 0)
                { // include game over here
                   player.Health = 100;
                   player.Lives = 3;
                   score = 0;
                    isGameOver = true;
                }
            }

        }
        
        private void UpdateLife(int damage)
        {
            int carry=0;
            multiplier = 1;
            if (shield == 0)
                player.Health -= damage;
            else
            {
                shield -= damage;
                if (shield < 0)
                {
                    carry = shield;
                    shield = 0;
                    player.Health -= carry;
                }
            }
        }
        
        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect functionto 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;
            Vector2 pos=new Vector2(player.Width , player.Height );
            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X - player.Width,
                (int)player.Position.Y - player.Height,
                player.Width, player.Height);

            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X,
                (int)enemies[i].Position.Y,
                enemies[i].Width,
                enemies[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    UpdateLife(enemies[i].Damage);

                    // Since the enemy collided with the player
                    // destroy it
                    enemies[i].Health = 0;

                    // If the player health is less than zero we died
                    
                }

            }

            // Projectile vs Enemy Collision
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }
            // Do the collision between the player and the enemies2
            for (int i = 0; i < enemies2.Count; i++)
            {
                rectangle1= new Rectangle((int)player.Position.X - player.Width,
                (int)player.Position.Y - player.Height,
                player.Width, player.Height);
                rectangle2 = new Rectangle((int)enemies2[i].Position.X - enemies2[i].Width / 2,
                    (int)enemies2[i].Position.Y - enemies2[i].Height / 2,
                    enemies2[i].Width, enemies2[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    UpdateLife(enemies2[i].Damage);

                    // Since the enemy collided with the player
                    // destroy it
                    enemies2[i].Health = 0;

                    // If the player health is less than zero we died
                    
                }
            }
            // Projectile vs Enemy2 Collision
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies2.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies2[j].Position.X - enemies2[j].Width / 2,
                    (int)enemies2[j].Position.Y - enemies2[j].Height / 2,
                    enemies2[j].Width, enemies2[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies2[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }
            // Do the collision between the player and the asteroids
            for (int i = 0; i < asteroids.Count; i++)
            {
                rectangle1 = new Rectangle((int)player.Position.X - player.Width,
                (int)player.Position.Y - player.Height,
                player.Width, player.Height);
                rectangle2 = new Rectangle((int)asteroids[i].Position.X,
                (int)asteroids[i].Position.Y,
                asteroids[i].Width,
                asteroids[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    UpdateLife(asteroids[i].Damage);

                    // Since the enemy collided with the player
                    // destroy it
                    asteroids[i].Health = 0;

                    // If the player health is less than zero we died
                    if (player.Health <= 0)
                        player.Active = false;
                }
            }// Projectile vs Asteroid Collision
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < asteroids.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)asteroids[j].Position.X - asteroids[j].Width / 2,
                    (int)asteroids[j].Position.Y - asteroids[j].Height / 2,
                    asteroids[j].Width, asteroids[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        asteroids[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }
            // Do the collision between the player and the boss
            for (int i = 0; i < bosses.Count; i++)
            {
                rectangle1 = new Rectangle((int)player.Position.X - player.Width,
                (int)player.Position.Y - player.Height,
                player.Width, player.Height);
                rectangle2 = new Rectangle((int)bosses[i].Position.X,
                (int)bosses[i].Position.Y,
                bosses[i].Width,
                bosses[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    UpdateLife(bosses[i].Damage * 1/10);


                    // If the player health is less than zero we died
                    
                }

            }
            // Projectile vs Boss Collision
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < bosses.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)bosses[j].Position.X - bosses[j].Width / 2,
                    (int)bosses[j].Position.Y - bosses[j].Height / 2,
                    bosses[j].Width, bosses[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        bosses[j].Health -= projectiles[i].Damage;
                        AddExplosion(projectiles[i].Position);
                        projectiles[i].Active = false;
                    }
                }
            }
            for (int i = 0; i < specweapons.Count; i++)
            {
                for (int j = 0; j < bosses.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)specweapons[i].Position.X -
                    specweapons[i].Width / 2, (int)specweapons[i].Position.Y -
                    specweapons[i].Height / 2, specweapons[i].Width, specweapons[i].Height);

                    rectangle2 = new Rectangle((int)bosses[j].Position.X - bosses[j].Width / 2,
                    (int)bosses[j].Position.Y - bosses[j].Height / 2,
                    bosses[j].Width, bosses[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        bosses[j].Health -= specweapons[i].Damage;
                        AddExplosion(specweapons[i].Position);
                        specweapons[i].Active = false;
                    }
                }
            }
            // BossProjectile vs Player Collision
            for (int i = 0; i < bossprojectiles.Count; i++)
            {
                // Create the rectangles we need to determine if we collided with each other
                rectangle1 = new Rectangle((int)bossprojectiles[i].Position.X -
                bossprojectiles[i].Width / 2, (int)bossprojectiles[i].Position.Y -
                bossprojectiles[i].Height / 2, bossprojectiles[i].Width, bossprojectiles[i].Height);

                rectangle2 = new Rectangle((int)player.Position.X - player.Width / 2,
                (int)player.Position.Y - player.Height / 2,
                player.Width, player.Height);

                // Determine if the two objects collided with each other
                if (rectangle1.Intersects(rectangle2))
                {
                    UpdateLife(bossprojectiles[i].Damage);
                    AddExplosion(bossprojectiles[i].Position);
                    bossprojectiles[i].Active = false;
                }
            }
            // Projectile vs Player Collision
            for (int i = 0; i < enemyprojectiles.Count; i++)
            {
                // Create the rectangles we need to determine if we collided with each other
                rectangle1 = new Rectangle((int)enemyprojectiles[i].Position.X -
                enemyprojectiles[i].Width / 2, (int)enemyprojectiles[i].Position.Y -
                enemyprojectiles[i].Height / 2, enemyprojectiles[i].Width, enemyprojectiles[i].Height);

                rectangle2 = new Rectangle((int)player.Position.X - player.Width ,
                (int)player.Position.Y - player.Height ,
                player.Width, player.Height);

                // Determine if the two objects collided with each other
                if (rectangle1.Intersects(rectangle2))
                {
                    UpdateLife(enemyprojectiles[i].Damage/2);
                    enemyprojectiles[i].Active = false;
                }
            }

            for (int i = 0; i < specweapons.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)specweapons[i].Position.X -
                    specweapons[i].Width / 2, (int)specweapons[i].Position.Y -
                    specweapons[i].Height / 2, specweapons[i].Width, specweapons[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= specweapons[i].Damage;
                        specweapons[i].Active = false;
                    }
                }
            }

            for (int i = 0; i < specweapons.Count; i++)
            {
                for (int j = 0; j < enemies2.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)specweapons[i].Position.X -
                    specweapons[i].Width / 2, (int)specweapons[i].Position.Y -
                    specweapons[i].Height / 2, specweapons[i].Width, specweapons[i].Height);

                    rectangle2 = new Rectangle((int)enemies2[j].Position.X - enemies2[j].Width / 2,
                    (int)enemies2[j].Position.Y - enemies2[j].Height / 2,
                    enemies2[j].Width, enemies2[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies2[j].Health -= specweapons[i].Damage;
                        specweapons[i].Active = false;
                    }
                }
            }

            for (int i = 0; i < specweapons.Count; i++)
            {
                for (int j = 0; j < asteroids.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)specweapons[i].Position.X -
                    specweapons[i].Width / 2, (int)specweapons[i].Position.Y -
                    specweapons[i].Height / 2, specweapons[i].Width, specweapons[i].Height);

                    rectangle2 = new Rectangle((int)asteroids[j].Position.X - asteroids[j].Width / 2,
                    (int)asteroids[j].Position.Y - asteroids[j].Height / 2,
                    asteroids[j].Width, asteroids[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        asteroids[j].Health -= specweapons[i].Damage;
                        specweapons[i].Active = false;
                    }
                }
            }
            for (int i = 0; i < specweapons.Count; i++)
            {
                for (int j = 0; j < bosses.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)specweapons[i].Position.X -
                    specweapons[i].Width / 2, (int)specweapons[i].Position.Y -
                    specweapons[i].Height / 2, specweapons[i].Width, specweapons[i].Height);

                    rectangle2 = new Rectangle((int)bosses[j].Position.X - bosses[j].Width / 2,
                    (int)bosses[j].Position.Y - bosses[j].Height / 2,
                    bosses[j].Width, bosses[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        bosses[j].Health -= specweapons[i].Damage;
                        specweapons[i].Active = false;
                    }
                }
            }
            for (int i = 0; i < powerups.Count; i++)
            {
                rectangle1 = new Rectangle((int)player.Position.X - player.Width/2,
                   (int)player.Position.Y - player.Height/2,
                   player.Width, player.Height);
                rectangle2 = new Rectangle((int)powerups[i].Position.X,
                    (int)powerups[i].Position.Y ,
                    powerups[i].Width, powerups[i].Height);
                if (rectangle1.Intersects(rectangle2))
                {

                    if (powerups[i].type != 3)
                    {
                        currentWeapon = powerups[i].type;
                        ammo = 30;
                    }
                    else
                        shield = 30;

                    powerups[i].Active = false;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing
            spriteBatch.Begin();
			
            // Draw the moving background
            mainBackground.Draw(spriteBatch);

            // Draw the Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            for (int i = 0; i < enemies2.Count; i++)
            {
                enemies2[i].Draw(spriteBatch);
            }
            //Draw the asteroids
            for (int i = 0; i < asteroids.Count; i++)
            {
                asteroids[i].Draw(spriteBatch);
            }
            //Draw the boss
            for (int i = 0; i < bosses.Count; i++)
            {
                bosses[i].Draw(spriteBatch);
            }
            // Draw the Projectiles
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw(spriteBatch);
            }
            for (int i = 0; i < bossprojectiles.Count; i++)
            {
                bossprojectiles[i].EnemyDraw(spriteBatch);
            }
            for (int i = 0; i < specweapons.Count; i++)// draw special weapons
            {
                specweapons[i].Draw(spriteBatch);
            }
            for (int i = 0; i < enemyprojectiles.Count; i++)
            {
                enemyprojectiles[i].EnemyDraw(spriteBatch);
            }
            // Draw the explosions
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);
            }
            for (int i = 0; i < powerups.Count; i++)
            {
                powerups[i].Draw(spriteBatch);
            }
            // Draw the Player
            player.Draw(spriteBatch, angle);

            // Draw the score
            spriteBatch.DrawString(font, "Score: " + score + "  x" + multiplier, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.LightBlue);
            spriteBatch.DrawString(font, "Health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.LightBlue);
            spriteBatch.DrawString(font, "Lives: " + player.Lives, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 60), Color.LightBlue);
            spriteBatch.DrawString(font, "Shield: " + shield, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 90), Color.LightBlue);
            spriteBatch.DrawString(font, "Ammo: " + ammo, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 120), Color.LightBlue);
            
            if (isMain)
            {

                spriteBatch.Draw(mainTexture, Vector2.Zero, Color.White);

                Vector2 menu1Position = new Vector2(GraphicsDevice.Viewport.Width / 1.22F, GraphicsDevice.Viewport.Height / 1.32F);
                Vector2 menu2Position = new Vector2(GraphicsDevice.Viewport.Width / 1.22F, GraphicsDevice.Viewport.Height / 1.2F);

                Rectangle menu1 = new Rectangle((int)menu1Position.X, (int)menu1Position.Y, 270, 30);
                Rectangle menu2 = new Rectangle((int)menu2Position.X, (int)menu2Position.Y, 310, 30);

                if (menu1.Contains((int)cursorPosition.X, (int)cursorPosition.Y))
                {
                    spriteBatch.DrawString(font, "Start", menu1Position, Color.Orange);
                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        Reset(gameTime);
                        isPaused = false;
                        isMain = false;
                        isWin = false;
                    }
                }
                else
                    spriteBatch.DrawString(font, "Start", menu1Position, Color.LightBlue);

                if (menu2.Contains((int)cursorPosition.X, (int)cursorPosition.Y))
                {
                    spriteBatch.DrawString(font, "Quit", menu2Position, Color.Orange);
                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                        this.Exit();
                }
                else
                    spriteBatch.DrawString(font, "Quit", menu2Position, Color.LightBlue);
            }
            else if (isPaused)
            {
                Vector2 menuPosition = new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 4);
                spriteBatch.Draw(pauseTexture, menuPosition, Color.White);

                Vector2 menu1Position = new Vector2(GraphicsDevice.Viewport.Width / 2.4F, GraphicsDevice.Viewport.Height / 2.6F);
                Vector2 menu2Position = new Vector2(GraphicsDevice.Viewport.Width / 2.5F, GraphicsDevice.Viewport.Height / 2.0F);
                Vector2 menu3Position = new Vector2(GraphicsDevice.Viewport.Width / 2.4F, GraphicsDevice.Viewport.Height / 1.6F);

                Rectangle menu1 = new Rectangle((int)menu1Position.X, (int)menu1Position.Y, 270, 30);
                Rectangle menu2 = new Rectangle((int)menu2Position.X, (int)menu2Position.Y, 310, 30);
                Rectangle menu3 = new Rectangle((int)menu3Position.X, (int)menu3Position.Y, 270, 30);

                if (menu1.Contains((int)cursorPosition.X, (int)cursorPosition.Y))
                {
                    spriteBatch.DrawString(font, "Resume Game", menu1Position, Color.Orange);
                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                        isPaused = false;
                }
                else
                    spriteBatch.DrawString(font, "Resume Game", menu1Position, Color.LightBlue);

                if (menu2.Contains((int)cursorPosition.X, (int)cursorPosition.Y))
                {
                    spriteBatch.DrawString(font, "Exit to Main Menu", menu2Position, Color.Orange);
                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                        isMain = true;
                }
                else
                    spriteBatch.DrawString(font, "Exit to Main Menu", menu2Position, Color.LightBlue);

                if (menu3.Contains((int)cursorPosition.X, (int)cursorPosition.Y))
                {
                    spriteBatch.DrawString(font, "Quit to Desktop", menu3Position, Color.Orange);
                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                        this.Exit();
                }
                else
                    spriteBatch.DrawString(font, "Quit to Desktop", menu3Position, Color.LightBlue);
            }
            else if(isGameOver)
            {
                spriteBatch.Draw(gameoverTexture, Vector2.Zero, Color.White);

                if (currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    isMain = true;
                    isGameOver = false;
                    isWin = false;
                }
            }
            else if (isWin)
            {
                spriteBatch.Draw(winTexture, Vector2.Zero, Color.White);
                if (currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    isMain = true;
                    isWin = false;
                }
            }

            spriteBatch.Draw(cursorTexture, cursorPosition - new Vector2(cursorTexture.Width / 2, cursorTexture.Height / 2), Color.White);
            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
