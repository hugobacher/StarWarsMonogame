﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StarWars.Content;

namespace StarWars
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //Class objects
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Player player;
        private EnemyHandler enemyHandler;
        private GameObject background;
        private ExplosionHandler explosionHandler;

        //Declare variables for window size
        private static int windowWidth;
        private static int windowHeight;

        //Textures for the gameObjects
        private Texture2D xwingImg, tieFighterImg, devastatorImg, laser, backgroundImg, tieBomberImg, tieInterceptorImg, tieAdvancedImg, expolsionImg;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Gamewindow size and fullscreen mode
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.IsFullScreen = false;

            //Give the window size variables their value
            windowHeight = graphics.PreferredBackBufferHeight;
            windowWidth = graphics.PreferredBackBufferWidth;
        }
        public static int WindowWidth { get => windowWidth; }
        public static int WindowHeight { get => windowHeight; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load in textures for the gameObjects
            backgroundImg = Content.Load<Texture2D>("stars");
            xwingImg = Content.Load<Texture2D>("xwing");
            tieFighterImg = Content.Load<Texture2D>("tiefighter");
            tieBomberImg = Content.Load<Texture2D>("tiebomber");
            tieInterceptorImg = Content.Load<Texture2D>("tieinterceptor");
            devastatorImg = Content.Load<Texture2D>("devastator");
            tieAdvancedImg = Content.Load<Texture2D>("tieadvanced");
            laser = Content.Load<Texture2D>("laser");
            expolsionImg = Content.Load<Texture2D>("explosions");

            //Creates the player
            player = new Player(xwingImg, (xwingImg.Width / 5), (xwingImg.Height / 5), 10f, 3, laser);
            //Creates enemyHandler object and sending in all enemy textures
            enemyHandler = new EnemyHandler(tieFighterImg, devastatorImg, tieBomberImg, tieInterceptorImg, tieAdvancedImg);
            //Background image that sizes to fit window size
            background = new GameObject(backgroundImg, windowWidth, windowHeight);
            //Creates explosionHandler and sending in image, column and rows
            explosionHandler = new ExplosionHandler(expolsionImg, 8, 8);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Update the player
            player.Update();

            //Spawn enemies and update them
            enemyHandler.Spawn();
            enemyHandler.Update();

            explosionHandler.Update();

            //Check collisions between gameobjects
            Collisions();
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //Start of drawing all gameObjects on the screen
            spriteBatch.Begin();

            //Draw the background stars
            background.Draw(spriteBatch);

            //Draw the xwing
            player.Draw(spriteBatch);

            //Draw the enemies
            enemyHandler.Draw(spriteBatch);

            explosionHandler.Draw(spriteBatch, new Vector2(400, 200));

            spriteBatch.End();
            base.Draw(gameTime);
        }
        /// <summary>
        /// This is checking if gameObjects are intersecting with eachother
        /// and removes hitpoints and marks objects as no longer alive so they
        /// will get removed from the game.
        /// </summary>
        private void Collisions()
        {
            foreach (Enemy enemy in enemyHandler.Enemies)
            {
                foreach (Laser laser in player.LaserHandler.Lasers)
                {
                    //Remove lasers that hits enemies and remove the enemy that gets hit
                    if (laser.Hitbox.Intersects(enemy.Hitbox))
                    {
                        laser.Alive = false;
                        enemy.Hitpoints--;
                    }
                }

                //Remove enemies that the player hits
                if (player.Hitbox.Intersects(enemy.Hitbox))
                {
                    enemy.Hitpoints--;
                    player.Hitpoints--;
                }
            }
        }
    }
}
