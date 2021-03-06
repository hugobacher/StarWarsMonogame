﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace StarWars
{
    class EnemyManager
    {
        //Random number generator
        private Random random = new Random();

        //Add a stopwatch timer that will keep track of time
        private Stopwatch devastatorTimer = Stopwatch.StartNew();
        private Stopwatch lambdaTimer = Stopwatch.StartNew();
        private Stopwatch elapsedTimer = Stopwatch.StartNew();

        //Bool changes to true when a mustdie enemy goes outside of the screen
        private bool gameOver = false;

        //List containing all enemies
        private List<Enemy> enemies = new List<Enemy>();

        private Texture2D tieFighterTexture, devastatorTexture, bomberTexture, interceptorTexture, lambdaTexture;

        /// <summary>
        /// List containing all enemies
        /// </summary>
        public List<Enemy> Enemies { get => enemies; set => enemies = value; }

        /// <summary>
        /// Bool changes to true when a mustdie enemy goes outside of the screen
        /// and the game should be ended
        /// </summary>
        public bool GameOver { get => gameOver; }

        /// <summary>
        /// Constructor for <c>EnemyManager</c>
        /// </summary>
        /// <param name="tieFighterTexture">Texture for Tie Fighter</param>
        /// <param name="devastatorTexture">Texture for Devastator</param>
        /// <param name="bomberTexture">Texture for Tie Bomber</param>
        /// <param name="interceptorTexture">Texture for Tie Interceptor</param>
        /// <param name="lambdaTexture">Texture for Lambda T4-A</param>
        public EnemyManager(Texture2D tieFighterTexture, Texture2D devastatorTexture, Texture2D bomberTexture, Texture2D interceptorTexture, Texture2D lambdaTexture)
        {
            //Add the textures for all enemies
            this.tieFighterTexture = tieFighterTexture;
            this.devastatorTexture = devastatorTexture;
            this.bomberTexture = bomberTexture;
            this.interceptorTexture = interceptorTexture;
            this.lambdaTexture = lambdaTexture;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public void Update()
        {
            //Spawn enemies
            Spawn();

            //Update the enemies
            foreach (Enemy enemy in enemies)
                enemy.Update();

            CheckHitpoints();
            CheckIfOutside();
            RemoveEnemies();

            //ElapsedTimer is not used after 30 seconds of gameplay, so stop it
            if (elapsedTimer.Elapsed.Seconds > 30)
                elapsedTimer.Stop();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the lasers
            foreach (Enemy enemy in enemies)
                enemy.Draw(spriteBatch);
        }

        /// <summary>
        /// Reseting the <c>enemyManager</c>, so the game can be played again
        /// </summary>
        public void Reset()
        {
            gameOver = false;

            //Restart all spawning timers
            devastatorTimer.Restart();
            lambdaTimer.Restart();
            elapsedTimer.Restart();

            //Empty the enemies list containing all enemies
            enemies.Clear();
        }

        /// <summary>
        /// Spawns enemies using <c>SpawnNormal()</c> and <c>SpawnBoss()</c> methods
        /// </summary>
        private void Spawn()
        {
            //Spawn normal enemies
            //Layout: texture, spawnAmount, hitBoxX, hitBoxY, speed, lives
            SpawnNormal(tieFighterTexture, 15, 80, 64, 7.5f, 1);
            SpawnNormal(bomberTexture, 50, 95, 90, 4f, 5);
            SpawnNormal(interceptorTexture, 60, 70, 85, 10f, 3);

            //Spawn boss enemies (that must die)
            //Layout: texture, timer, spawnTime, hitBoxX, hitBoxY, speed, lives
            SpawnBoss(devastatorTexture, devastatorTimer, 35, 350, 600, 0.5f, 75);
            SpawnBoss(lambdaTexture, lambdaTimer, 75, 150, 146, 3.5f, 10);
        }

        /// <summary>
        /// Spawn normal enemies that is not needed to be killed
        /// before they exit the bottom of the screen
        /// </summary>
        /// <param name="texture">Texture of the enemy</param>
        /// <param name="spawnAmount">How many enemies that should spawn,
        /// lower value = more spawns, higher value = less spawns</param>
        /// <param name="hitBoxX">Hitbox size on the x axis</param>
        /// <param name="hitBoxY">Hitbox size on the y axis</param>
        /// <param name="speed">The movement speed of the enemy</param>
        /// <param name="lives">How many lives the enemy have</param>
        private void SpawnNormal(Texture2D texture, int spawnAmount, int hitBoxX, int hitBoxY, float speed, int lives)
        {
            //Temporary spawnAmount saver if the new spawnAmount is less than the original spawnAmount
            int originalSpawnAmount = spawnAmount;

            //Make the spawnAmount grow over time, but never bigger than the set original spawnAmount
            spawnAmount = 150 - (spawnAmount * (elapsedTimer.Elapsed.Seconds / 2));
            if (spawnAmount < originalSpawnAmount)
                spawnAmount = originalSpawnAmount;

            int spawnrate = random.Next(spawnAmount);
            //Spawns normal enemies
            if (spawnrate == 0)
            {
                //Get a random position over the screen
                int positionX = random.Next(Game1.WindowWidth - hitBoxX);

                //Add the enemy
                enemies.Add(new Enemy(texture, hitBoxX, hitBoxY, speed, positionX, lives, false));
            }
        }

        /// <summary>
        /// Spawn in boss enemies that must be killed before the exit
        /// the bottom of the screen
        /// </summary>
        /// <param name="texture">Texture of the enemy</param>
        /// <param name="timer">The spawing timer that is used</param>
        /// <param name="spawnTime">When the enemy should be spawned, compared to the timer</param>
        /// <param name="hitBoxX">Hitbox size on the x axis</param>
        /// <param name="hitBoxY">Hitbox size on the y axis</param>
        /// <param name="speed">The movement speed of the enemy</param>
        /// <param name="lives">How many lives the enemy have</param>
        private void SpawnBoss(Texture2D texture, Stopwatch timer, int spawnTime, int hitBoxX, int hitBoxY, float speed, int lives)
        {
            spawnTime = random.Next(spawnTime - 5, spawnTime + 5);
            //Spawns the boss enemies
            if (timer.Elapsed.Seconds >= spawnTime)
            {
                //Get a random position over the screen
                int positionX = random.Next(Game1.WindowWidth - hitBoxX);

                enemies.Add(new Enemy(texture, hitBoxX, hitBoxY, speed, positionX, lives, true));
                timer.Restart();
            }
        }

        /// <summary>
        /// Checking how many hitpoints the enmey has left,
        /// if it's less or equal than 0 then mark the enemy as no longer alive
        /// </summary>
        private void CheckHitpoints()
        {
            //Check if the enemy has or equal than 0  hitpoint and if so mark is as not alive
            foreach (Enemy enemy in enemies)
            {
                if (enemy.Hitpoints <= 0)
                    enemy.Alive = false;
            }
        }

        /// <summary>
        /// Checks if a enemy is below the window, if so mark is as no longer alive
        /// If the enemy must die before it gets outside, change to game over scene
        /// </summary>
        private void CheckIfOutside()
        {
            //Set the enemy alive state to false if the enemy is outside of the screen
            foreach (Enemy enemy in enemies)
            {
                if (enemy.Position.Y >= Game1.WindowHeight + 10 + enemy.Hitbox.Height && !enemy.MustDie)
                    enemy.Alive = false;
                else if (enemy.Position.Y >= Game1.WindowHeight + 10 + enemy.Hitbox.Height && enemy.MustDie)
                {
                    enemy.Alive = false;
                    gameOver = true;
                }
            }
        }

        /// <summary>
        /// Removing the enemies that no longer is alive
        /// </summary>
        private void RemoveEnemies()
        {
            //A temporary list to fill with enemies
            List<Enemy> tempEnemies = new List<Enemy>();

            //If the enemy is alive, add them to the temp list
            foreach (Enemy enemy in enemies)
            {
                if (enemy.Alive)
                    tempEnemies.Add(enemy);
            }
            //Overwrite the enemies list with the temp list
            enemies = tempEnemies;
        }
    }
}
