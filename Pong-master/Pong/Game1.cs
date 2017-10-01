using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Pong
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //public KeyboardState CurrentKBState;

        //textures
        Texture2D ball, Paddle1, Paddle2, MiddleLine, pong, bluebar, redbar;

        //object vars
        float Paddle1Y;
        float Paddle2Y;
        float BallY;
        float BallX;
        //bool BallUp = true;
        //bool BallRight = false;

        //variable vars
        int Lives1 = 3, Lives2 = 3;
        int SpeedPaddles = 10;
        float StartSpeed = 8f;
        float speed;
        Color ColorBall = Color.White;

        //general vars
        int GameOver = 0;
        int GameMode = 1;
        int StartSide;
        float BallAngle;
        float ScreenWidth;
        float ScreenHeight;
        KeyboardState currentkeyboardstate, previouskeyboardstate;

        //debug waardes (ongebruikt in de gameplay)
        float DebugX, DebugY;
        SpriteFont Font1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //CurrentKBState = Keyboard.GetState();
        }

        protected override void Initialize()
        {
            ///graphics loader
            //sprites
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ball = Content.Load<Texture2D>("ball");
            Paddle1 = Content.Load<Texture2D>("Paddle1");
            Paddle2 = Content.Load<Texture2D>("Paddle2");
            Font1 = Content.Load<SpriteFont>("Magnum");
            MiddleLine = Content.Load<Texture2D>("MiddleLine");
            pong = Content.Load<Texture2D>("pong");
            bluebar = Content.Load<Texture2D>("bluebar");
            redbar = Content.Load<Texture2D>("redbar");
            //screen
            ScreenWidth = GraphicsDevice.Viewport.Width;
            ScreenHeight = GraphicsDevice.Viewport.Height;

            ///positioning
            //ball
            BallY = (ScreenHeight / 2) - (ball.Height / 2);
            BallX = (ScreenWidth / 2) - (ball.Width / 2);
            Paddle1Y = (ScreenHeight / 2) - (Paddle1.Height / 2);
            Paddle2Y = (ScreenHeight / 2) - (Paddle2.Height / 2);
            //bal start angle
            Random random = new Random();
            BallAngle = random.Next(-45, 46);
            StartSide = random.Next(0, 2);
            if (StartSide == 1) BallAngle += 180;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //MediaPlayer.Play(Content.Load<Song>("8-Bit-Mayhem"));
        }

        protected override void Update(GameTime gameTime)
        {
            previouskeyboardstate = currentkeyboardstate;
            currentkeyboardstate = Keyboard.GetState();

            //menu controlls
            if (GameOver == 0 || GameOver == 2)
            {
                if (currentkeyboardstate.IsKeyDown(Keys.Down) && previouskeyboardstate.IsKeyUp(Keys.Down)) GameMode = MathHelper.Min(GameMode + 1, 3);
                if (currentkeyboardstate.IsKeyDown(Keys.Up) && previouskeyboardstate.IsKeyUp(Keys.Up)) GameMode = MathHelper.Max(GameMode - 1, 1);
            }

            //restart game after game end
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                GameOver = 1;
                Lives1 = 3;
                Lives2 = 3;
                BallY = 230;
                BallX = 390;
                speed = StartSpeed;

            }

            //gameover screen requisites
            if (Lives1 == 0 || Lives2 == 0) GameOver = 2;

            if (GameOver == 1)
            {
                //Controls
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                //Control Player 1
                if (Keyboard.GetState().IsKeyDown(Keys.W) && Paddle1Y > 0)
                {
                    Paddle1Y = Paddle1Y - SpeedPaddles;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S) && Paddle1Y < 383)
                {
                    Paddle1Y = Paddle1Y + SpeedPaddles;
                }

                //game mode 1 controlls load
                if (GameMode == 1 || GameMode == 3)
                {
                    //Control Player 2
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) && Paddle2Y > 0)
                    {
                        Paddle2Y = Paddle2Y - SpeedPaddles;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Down) && Paddle2Y < 383)
                    {
                        Paddle2Y = Paddle2Y + SpeedPaddles;
                    }

                }

                //gamemode 2 cpu player load
                if (GameMode == 2)
                {
                    if (BallY + ball.Height / 2 > Paddle2Y + Paddle2.Height / 2 && Paddle2Y < GraphicsDevice.Viewport.Height - Paddle2.Height) Paddle2Y += 6;
                    if (BallY + ball.Height / 2 < Paddle2Y + Paddle2.Height / 2 && Paddle2Y > 0) Paddle2Y -= 6;
                }

                //Ball movement
                //speed += 0.004f;
                BallX += speed * (float)Math.Cos(BallAngle * Math.PI / 180);
                BallY += speed * (float)Math.Sin(BallAngle * Math.PI / 180);
                
                //Debug
                DebugY = Paddle1.Height;
                DebugX = (float)Math.Cos(BallAngle * Math.PI / 180);

                //Borders
                if (BallY < 0 || BallY > GraphicsDevice.Viewport.Height - ball.Height) BallAngle = -BallAngle;

                //Lives and ball reset
                if (BallX < 0)
                {
                    BallY = 230;
                    BallX = 390;
                    speed = StartSpeed;
                    Lives1--;
                }
                if (BallX > 800)
                {
                    BallY = 230;
                    BallX = 390;
                    speed = StartSpeed;
                    Lives2--;
                }

                //Collisions etc
                Rectangle P1B = Paddle1.Bounds;
                P1B.Offset(20, Paddle1Y);
                Rectangle P2B = Paddle2.Bounds;
                P2B.Offset(ScreenWidth - Paddle2.Bounds.Width - 20, Paddle2Y);
                Rectangle ballB = ball.Bounds;
                ballB.Offset(BallX, BallY);

                if (ballB.Intersects(P1B))
                {
                    BallAngle = (BallY + ball.Height / 2) - (Paddle1Y + Paddle1.Height / 2);
                    ColorBall = Color.Blue;
                    speed += 0.5f;
                }
                if (ballB.Intersects(P2B))
                {
                    BallAngle = (Paddle2Y + Paddle2.Height / 2) - (BallY + ball.Height / 2) + 180;
                    ColorBall = Color.Red;
                    speed += 0.5f;
                }
                base.Update(gameTime);

            }

            //Debugger
            if (Keyboard.GetState().IsKeyDown(Keys.P)) Debugger.Break();
            //FullScreen
            if (Keyboard.GetState().IsKeyDown(Keys.F)) graphics.ToggleFullScreen();

        }

        protected override void Draw(GameTime gameTime)
        {
            //Background Color
            GraphicsDevice.Clear(Color.Black);

            //Draw
            spriteBatch.Begin();
            if (GameOver == 0 || GameOver == 2)
            {
                //Menu image
                spriteBatch.Draw(pong, new Vector2(), Color.White);
                //Menu text
                float DualMode = Font1.MeasureString("Dual").X;
                if (GameMode == 1)
                {
                    spriteBatch.DrawString(Font1, "Dual", new Vector2((ScreenWidth / 2) - DualMode / 2, (ScreenHeight / 2)), Color.Red);
                }
                else
                {
                    spriteBatch.DrawString(Font1, "Dual", new Vector2((ScreenWidth / 2) - DualMode / 2, (ScreenHeight / 2)), Color.White);
                }
                float CPUMode = Font1.MeasureString("VS CPU").X;
                if (GameMode == 2)
                {
                    spriteBatch.DrawString(Font1, "VS CPU", new Vector2((ScreenWidth / 2) - CPUMode / 2, (ScreenHeight / 2) + 30), Color.Red);
                }
                else
                {
                    spriteBatch.DrawString(Font1, "VS CPU", new Vector2((ScreenWidth / 2) - CPUMode / 2, (ScreenHeight / 2) + 30), Color.White);
                }
                if (GameMode == 3)
                {
                    spriteBatch.DrawString(Font1, "4 Players", new Vector2((ScreenWidth / 2) - CPUMode / 2, (ScreenHeight / 2) + 60), Color.Red);
                }
                else
                {
                    spriteBatch.DrawString(Font1, "4 Players", new Vector2((ScreenWidth / 2) - CPUMode / 2, (ScreenHeight / 2) + 60), Color.White);
                }
            }

            if (GameOver == 1)
            {
                //Draw Sprites
                spriteBatch.Draw(MiddleLine, new Vector2(ScreenWidth / 2 - MiddleLine.Bounds.Width / 2, ScreenHeight / 2 - MiddleLine.Bounds.Height / 2), Color.White);
                spriteBatch.Draw(ball, new Vector2(BallX, BallY), ColorBall);
                spriteBatch.Draw(Paddle1, new Vector2(20, Paddle1Y), Color.Blue);
                spriteBatch.Draw(Paddle2, new Vector2(ScreenWidth - Paddle2.Bounds.Width - 20, Paddle2Y), Color.Red);
                //Draw Lives Player1
                spriteBatch.Draw(bluebar, new Vector2(52, 10), Color.White);
                spriteBatch.Draw(bluebar, new Vector2(52, 42), Color.White);
                if (Lives1 >= 3) spriteBatch.Draw(ball, new Vector2(90, 20), Color.Blue);
                if (Lives1 >= 2) spriteBatch.Draw(ball, new Vector2(70, 20), Color.Blue);
                if (Lives1 >= 1) spriteBatch.Draw(ball, new Vector2(50, 20), Color.Blue);
                //Draw Lives Player2
                spriteBatch.Draw(redbar, new Vector2(712, 10), Color.White);
                spriteBatch.Draw(redbar, new Vector2(712, 42), Color.White);
                if (Lives2 >= 3) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 90, 20), Color.Red);
                if (Lives2 >= 2) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 70, 20), Color.Red);
                if (Lives2 >= 1) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 50, 20), Color.Red);
            }

            if (GameOver == 2)
            {
                //Menu GameOver text
                float YouWin = Font1.MeasureString("P1 Wins").X;
                if (Lives2 == 0) spriteBatch.DrawString(Font1, "P1 Wins", new Vector2(ScreenWidth / 2 - YouWin / 2, ScreenHeight / 2-50), Color.White);

                float YouLose = Font1.MeasureString("P2 Wins").X;
                if (Lives1 == 0) spriteBatch.DrawString(Font1, "P2 Wins", new Vector2(ScreenWidth / 2 - YouLose / 2, ScreenHeight / 2-50), Color.White);

                //float PlayAgain = Font1.MeasureString("Press Enter to play again").X;
                //spriteBatch.DrawString(Font1, "Press Enter to play again", new Vector2(ScreenWidth / 2 - PlayAgain / 2, ScreenHeight / 2 + 35), Color.White);


            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
