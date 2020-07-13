using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace ChromeDinoAgain
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public enum state
        {
            initIdle = 1,
            run = 2,
            idle = 3,
            idleEyeClose = 4,
            duck = 5,
            over = 6
        };

        private class Obstacle
        {
            public Rectangle pos;
            public Rectangle body;
            public Obstacle()
            {
                pos = Rectangle.Empty;
                body = Rectangle.Empty;
            }
        }

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private KeyboardState kbst;
        private Texture2D asset;
        private Rectangle initStand, initStandPosition;
        private Rectangle idle, idleEyeClosed;
        private Rectangle damageHit;
        private Rectangle[] digit, smallCactus, bigCactus;
        private Rectangle charsize;
        private Rectangle duckLeftUp, duckRightUp;
        private Rectangle runLeftUp, runRightUp;
        private Rectangle character, charPosition;
        private Rectangle road, roadPosition;
        private Rectangle restartButton, gameOverText;
        private Rectangle cloud;
        private Rectangle[] cloudPosition;
        private Rectangle birdFluDown, birdFluUp, birdFluPosition;
        private Obstacle[] obstacle;
        private int obstacleCount = 2;
        private int cloudCount = 5, cloudMove = 1, cloudMoveDelay = 10, cloudMoveWait = 0;
        private int charRunDelay = 100, charRunWait = 0;
        private int roadMove = 10, roadMoveDelay = 10, roadMoveWait = 0;
        private int gravity = 0, orignalGravity = 10, gravityDelay = 5, gravityWait = 0;
        private int jumpHeight = 150;
        private int hiscore = 0, score = 0, scoreUpDelay = 50, scoreUpWait = 0;
        private bool canJump = true, roadMoving = false, isRunning = false, isGameOver = false, canJumpSound = true, isSpacePressed=false;
        private state charState = state.idle;
        private SoundEffect jump, over, step;
        private Song background_desert;
        private Random rand;
        private Color backColor;

        private bool checkPixelCollide(Rectangle r1, Obstacle r2)
        {
            bool top = r1.Y < r2.pos.Y + r2.pos.Height && r1.Y > r2.pos.Y;
            bool left = r1.X < r2.pos.X + r2.pos.Width && r1.X > r2.pos.X;
            bool bottom = r1.Y + r1.Height > r2.pos.Y && r1.Y + r1.Height < r2.pos.Y + r2.pos.Height;
            bool right = r1.X + r1.Width > r2.pos.X && r1.X + r1.Width < r2.pos.X + r2.pos.Width;

            if (top)
                if (right || left)
                {
                    return true;
                }
            if (left)
                if (top || bottom)
                {
                    return true;
                }
            if (bottom)
                if (right || left)
                {
                    /*
                    int r1x = r1.X + r1.Width - (r1.X + r1.Width - r2.X);
                    int r1y = r1.X + r1.Width - (r1.Y + r1.Height - r2.Y);

                    int r2x = r2.X + (r1.X + r1.Width - r2.X);
                    int r2y = r2.Y + (r1.Y + r1.Height - r2.Y);

                    Color[] r1c = new Color[r1.Width * r1.Height];
                    graphics.GraphicsDevice.GetBackBufferData(r1, r1c, 0, r1c.Length);

                    Color[] r2c = new Color[r2.Width * r2.Height];
                    graphics.GraphicsDevice.GetBackBufferData(r2, r2c, 0, r2c.Length);

                    if (r1c[r1x * r1y].Equals(Color.Transparent) && r2c[r2x * r2y].Equals(Color.Transparent))
                    */
                        return true;
                    /*
                    else
                        return false;
                    */
                }
            if (right)
                if (top || bottom)
                {
                    return true;
                }

            if (r1.Intersects(r2.pos))
                return true;

            return false;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            rand = new Random();
            cloudPosition = new Rectangle[cloudCount];
            backColor = new Color(Color.White, 220);
            obstacle = new Obstacle[obstacleCount];
            hiscore = Properties.Settings.Default.HiScore;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            jump = Content.Load<SoundEffect>("jump");
            over = Content.Load<SoundEffect>("over");
            step = Content.Load<SoundEffect>("step");
            background_desert = Content.Load<Song>("back");

            asset = Content.Load<Texture2D>("dino-sprite-2x");

            digit = new Rectangle[12];
            for (int i = 0; i < 12; i++)
                digit[i] = new Rectangle(953 + 20 * i, 2, 20, 21);
            smallCactus = new Rectangle[6];
            for (int i = 0; i < 6; i++)
                smallCactus[i] = new Rectangle(446 + 34 * i, 2, 34, 70);

            bigCactus = new Rectangle[3];
            for (int i = 0; i < 3; i++)
                bigCactus[i] = new Rectangle(652 + 50 * i, 2, 50, 100);

            restartButton = new Rectangle(2, 2, 72, 64);
            gameOverText = new Rectangle(954, 29, 381, 21);

            charsize = new Rectangle(0, 10, 20, 21);

            birdFluDown = new Rectangle(260, 14, 92, 68);
            birdFluUp = new Rectangle(352, 2, 92, 68);

            road = new Rectangle(2, 104, 2400, 24);
            roadPosition = new Rectangle(0, graphics.PreferredBackBufferHeight / 2 + road.Height, 2400, 24);

            initStand = new Rectangle(76, 0, 88, 96);
            initStandPosition = new Rectangle(80, roadPosition.Y - initStand.Height, 88, 96);

            idle = new Rectangle(1338, 2, 88, 94);
            idleEyeClosed = new Rectangle(1426, 2, 88, 94);

            damageHit = new Rectangle(1690, 2, 88, 94);

            cloud = new Rectangle(166, 2, 92, 27);

            duckRightUp = new Rectangle(1866, 36, 118, 60);
            duckLeftUp = new Rectangle(1984, 36, 118, 60);

            runLeftUp = new Rectangle(1514, 2, 88, 96);
            runRightUp = new Rectangle(1602, 2, 88, 96);

            character = initStand;
            charPosition = initStandPosition;

            for (int i = 0; i < cloudCount; i++)
                cloudPosition[i] = new Rectangle(graphics.PreferredBackBufferWidth + rand.Next() % graphics.PreferredBackBufferWidth, rand.Next() % graphics.PreferredBackBufferHeight / 2, cloud.Width, cloud.Height);

            for (int i = 0; i < obstacleCount; i++)
            {
                if (rand.Next() % 2 == 0)
                {
                    obstacle[i] = new Obstacle();
                    obstacle[i].body = smallCactus[rand.Next() % 6];
                    obstacle[i].pos = obstacle[i].body;
                    obstacle[i].pos.X = graphics.PreferredBackBufferWidth + obstacle[i].body.Width;
                    obstacle[i].pos.Y = roadPosition.Y - obstacle[i].body.Height + 24;
                }
                else
                {
                    obstacle[i] = new Obstacle();
                    obstacle[i].body = bigCactus[rand.Next() % 3];
                    obstacle[i].pos = obstacle[i].body;
                    obstacle[i].pos.X = graphics.PreferredBackBufferWidth + obstacle[i].body.Width;
                    obstacle[i].pos.Y = roadPosition.Y - obstacle[i].body.Height + 24;
                }
            }

            //MediaPlayer.Play(background_desert);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            kbst = Keyboard.GetState();
            charRunWait += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            gravityWait += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            roadMoveWait += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            scoreUpWait += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            cloudMoveWait += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (cloudMoveWait > cloudMoveDelay)
            {
                if (isRunning)
                {
                    for (int i = 0; i < cloudCount; i++)
                    {
                        if (cloudPosition[i].X + cloud.Width > 0)
                            cloudPosition[i].X -= cloudMove + (3 * cloudPosition[i].Y / (graphics.PreferredBackBufferHeight / 2));
                        else
                            cloudPosition[i] = new Rectangle(graphics.PreferredBackBufferWidth + rand.Next() % graphics.PreferredBackBufferWidth, rand.Next() % graphics.PreferredBackBufferHeight / 2, cloud.Width, cloud.Height);
                    }
                }
                cloudMoveWait = 0;
            }
            if (scoreUpWait > scoreUpDelay)
            {
                if (isRunning)
                    score += 1;
                scoreUpWait = 0;
            }
            if (charState == state.initIdle)
            {
                character = initStand;
            }
            else if (charState == state.idle)
            {
                character = idle;
            }
            else if (charState == state.over)
            {
                character = damageHit;
            }
            if (charRunWait > charRunDelay)
            {
                if (charState == state.run)
                {
                    if (character != runLeftUp && canJumpSound)
                    {
                        character = runLeftUp;
                    }
                    else if (character != runRightUp && canJumpSound)
                    {
                        character = runRightUp;
                    }
                    //step.Play();
                }
                if (charState == state.duck)
                {
                    if (character != duckLeftUp)
                        character = duckLeftUp;
                    else if (character != duckRightUp)
                        character = duckRightUp;
                }
                charRunWait = 0;
            }
            if (roadMoveWait > roadMoveDelay && roadMoving)
            {
                if (roadPosition.X > -road.Width)
                    roadPosition.X -= roadMove;
                else
                    roadPosition.X = 0 - roadMove;
                for (int i = 0; i < obstacleCount; i++)
                {
                    if (obstacle[i].pos.X + obstacle[i].body.Width > 0)
                    {
                        obstacle[i].pos.X -= roadMove;
                    }
                    else
                    {
                        if (rand.Next() % 2 == 0)
                        {
                            obstacle[i].body = smallCactus[rand.Next() % 6];
                            obstacle[i].pos = obstacle[i].body;
                            obstacle[i].pos.X = graphics.PreferredBackBufferWidth + obstacle[i].body.Width;
                            obstacle[i].pos.Y = roadPosition.Y - obstacle[i].body.Height + 24;
                        }
                        else
                        {
                            obstacle[i].body = bigCactus[rand.Next() % 3];
                            obstacle[i].pos = obstacle[i].body;
                            obstacle[i].pos.X = graphics.PreferredBackBufferWidth + obstacle[i].body.Width;
                            obstacle[i].pos.Y = roadPosition.Y - obstacle[i].body.Height + 24;
                        }
                    }
                }
                roadMoveWait = 0;
            }

            if (kbst.IsKeyDown(Keys.Space) && canJump)
            {
                if (isGameOver)
                {
                    for (int i = 0; i < obstacleCount; i++)
                    {
                        if (rand.Next() % 2 == 0)
                        {
                            obstacle[i].body = smallCactus[rand.Next() % 6];
                            obstacle[i].pos = obstacle[i].body;
                            obstacle[i].pos.X = graphics.PreferredBackBufferWidth + obstacle[i].body.Width;
                            obstacle[i].pos.Y = roadPosition.Y - obstacle[i].body.Height + 24;
                        }
                        else
                        {
                            obstacle[i].body = bigCactus[rand.Next() % 3];
                            obstacle[i].pos = obstacle[i].body;
                            obstacle[i].pos.X = graphics.PreferredBackBufferWidth + obstacle[i].body.Width;
                            obstacle[i].pos.Y = roadPosition.Y - obstacle[i].body.Height + 24;
                        }
                    }
                }
                if (canJumpSound)
                {
                    jump.Play();
                }
                if (!roadMoving)
                {
                    roadMoving = true;
                    isRunning = true;
                    isGameOver = false;
                }
                charState = state.run;
                gravity = -orignalGravity - 8;
                canJumpSound = false;
                isSpacePressed = true;
            }
            else
            {
                gravity = orignalGravity - 3;
            }
            if (kbst.IsKeyDown(Keys.Down) || kbst.IsKeyDown(Keys.S))
            {
                charState = state.duck;
                character = duckLeftUp;
            }
            if (gravityWait > gravityDelay)
            {
                if (gravity > 0)
                {
                    if (charPosition.Height + charPosition.Y < roadPosition.Y + 24)
                    {
                        charPosition.Y += gravity;
                    }
                    else
                    {
                        canJump = true;
                        canJumpSound = true;
                    }
                }
                else
                {
                    if (charPosition.Height + charPosition.Y > roadPosition.Y - jumpHeight)
                    {
                        charPosition.Y += gravity;
                    }
                    else
                    {
                        canJump = false;
                    }
                }
                gravityWait = 0;
            }
            for (int i = 0; i < obstacleCount; i++)
            {
                if (checkPixelCollide(charPosition,obstacle[i]) && isRunning && !isGameOver && obstacle[i].pos.X > charPosition.X)
                {
                    isGameOver = true;
                    charState = state.over;
                    isRunning = false;
                    roadMoving = false;
                    if (score > hiscore)
                    {
                        Properties.Settings.Default.HiScore = score;
                        hiscore = score;
                        Properties.Settings.Default.Save();
                    }
                    score = 0;
                    over.Play();
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backColor);
            spriteBatch.Begin();
            for (int i = 0; i < cloudCount; i++)
            {
                spriteBatch.Draw(asset, cloudPosition[i], cloud, Color.White);
            }
            spriteBatch.Draw(asset, roadPosition, road, Color.White);
            spriteBatch.Draw(asset, new Rectangle(roadPosition.X + roadPosition.Width, roadPosition.Y, roadPosition.Width, roadPosition.Height), road, Color.White);

            for (int i = 0; i < obstacleCount; i++)
            {
                spriteBatch.Draw(asset, obstacle[i].pos, obstacle[i].body, Color.White);
            }

            string sc = score.ToString();
            for (int i = 0; i < sc.Length; i++)
            {
                spriteBatch.Draw(asset, new Rectangle(charsize.X + 20 * i, charsize.Y, charsize.Width, charsize.Height), digit[sc[i] - 48], Color.White);
            }
            string hsc = hiscore.ToString();
            for (int i = hsc.Length - 1; i >= 0; i--)
            {
                spriteBatch.Draw(asset, new Rectangle(graphics.PreferredBackBufferWidth - 20 * (hsc.Length - i) - 60, charsize.Y, charsize.Width, charsize.Height), digit[hsc[i] - 48], Color.White);
                spriteBatch.Draw(asset, new Rectangle(graphics.PreferredBackBufferWidth - 20, charsize.Y, charsize.Width, charsize.Height), digit[11], Color.White);
                spriteBatch.Draw(asset, new Rectangle(graphics.PreferredBackBufferWidth - 40, charsize.Y, charsize.Width, charsize.Height), digit[10], Color.White);
            }
            spriteBatch.Draw(asset, charPosition, character, Color.White);

            if (isGameOver)
            {
                spriteBatch.Draw(asset, new Rectangle(graphics.PreferredBackBufferWidth / 2 - gameOverText.Width / 2, graphics.PreferredBackBufferHeight / 2 - gameOverText.Height / 2, gameOverText.Width, gameOverText.Height), gameOverText, Color.White);
                spriteBatch.Draw(asset, new Rectangle(graphics.PreferredBackBufferWidth / 2 - restartButton.Width / 2, graphics.PreferredBackBufferHeight / 2 + gameOverText.Height + 10, restartButton.Width, restartButton.Height), restartButton, Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}