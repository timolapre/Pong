/*

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

using System.Drawing;

namespace Pong
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //public KeyboardState CurrentKBState;

        //textures
        Texture2D ball;
        Texture2D Paddle1;
        Texture2D Paddle2;
        Texture2D MiddleLine;

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

        //general vars
        int GameMode = 0;
        int StartSide;
        float BallAngle;
        float ScreenWidth;
        float ScreenHeight;

        //debug waardes (ongebruikt in de gameplay)
        float DebugX, DebugY;
        SpriteFont Font1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //CurrentKBState = Keyboard.GetState();
        }

        //Initialize Initialize Initialize Initialize Initialize Initialize
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

        }

        //UPDATE UPDATE UPDATE UPDATE UPDATE UPDATE UPDATE
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                GameMode = 1;
                Lives1 = 3;
                Lives2 = 3;
                BallY = 230;
                BallX = 390;
                speed = StartSpeed;

            }
            if (Lives1 == 0 || Lives2 == 0) GameMode = 2;

            if (GameMode == 1)
            {

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                if (Keyboard.GetState().IsKeyDown(Keys.W) && Paddle1Y > 0)
                {
                    Paddle1Y = Paddle1Y - SpeedPaddles;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S) && Paddle1Y < 383)
                {
                    Paddle1Y = Paddle1Y + SpeedPaddles;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Up) && Paddle2Y > 0)
                {
                    Paddle2Y = Paddle2Y - SpeedPaddles;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down) && Paddle2Y < 383)
                {
                    Paddle2Y = Paddle2Y + SpeedPaddles;
                }

                //AI CODE (CURRENTLY DEACTIVATED) DON'T REMOVE
                //if (BallY + ball.Height / 2 > Paddle2Y + Paddle2.Height / 2 && Paddle2Y < GraphicsDevice.Viewport.Height - Paddle2.Height) Paddle2Y += 2;
                //if (BallY + ball.Height / 2 < Paddle2Y + Paddle2.Height / 2 && Paddle2Y > 0) Paddle2Y -= 2;
                //DON'T REMOVE

                speed += 0.004f;

                BallX += speed * (float)Math.Cos(BallAngle * Math.PI / 180);
                BallY += speed * (float)Math.Sin(BallAngle * Math.PI / 180);

                //if (BallUp == true) BallY += speed * (float)Math.Sin(BallAngle); else BallY += speed * (float)Math.Sin(BallAngle);
                //if (BallRight == true) BallX += speed * (float)Math.Cos(BallAngle); else BallX += speed * (float)Math.Cos(BallAngle);

                DebugY = Paddle1.Height;
                DebugX = (float)Math.Cos(BallAngle * Math.PI / 180);

                if (BallY < 0 || BallY > GraphicsDevice.Viewport.Height - ball.Height) BallAngle = -BallAngle;

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

                Rectangle P1B = Paddle1.Bounds;
                P1B.Offset(20, Paddle1Y);
                Rectangle P2B = Paddle2.Bounds;
                P2B.Offset(ScreenWidth - Paddle2.Bounds.Width - 20, Paddle2Y);
                Rectangle ballB = ball.Bounds;
                ballB.Offset(BallX, BallY);

                if (ballB.Intersects(P1B)) BallAngle = (BallY + ball.Height / 2) - (Paddle1Y + Paddle1.Height / 2);
                if (ballB.Intersects(P2B)) BallAngle = (Paddle2Y + Paddle2.Height / 2) - (BallY + ball.Height / 2) + 180;

                base.Update(gameTime);

            }

            //Debugger
            if (Keyboard.GetState().IsKeyDown(Keys.P)) Debugger.Break();
            //FullScreen
            if (Keyboard.GetState().IsKeyDown(Keys.F)) graphics.ToggleFullScreen();

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            if (GameMode == 0)
            {
                float PressEnter = Font1.MeasureString("Press Enter to start").X;
                spriteBatch.DrawString(Font1, "Press Enter to start", new Vector2(ScreenWidth / 2 - PressEnter / 2, ScreenHeight / 2), Color.White);
            }

            if (GameMode == 1)
            {
                spriteBatch.Draw(ball, new Vector2(BallX, BallY), Color.White);
                spriteBatch.Draw(Paddle1, new Vector2(20, Paddle1Y), Color.Blue);
                spriteBatch.Draw(Paddle2, new Vector2(ScreenWidth - Paddle2.Bounds.Width - 20, Paddle2Y), Color.Red);
                spriteBatch.Draw(MiddleLine, new Vector2(ScreenWidth / 2 - MiddleLine.Bounds.Width / 2, ScreenHeight / 2 - MiddleLine.Bounds.Height / 2), Color.White);

                if (Lives1 >= 3) spriteBatch.Draw(ball, new Vector2(90, 20), Color.Blue);
                if (Lives1 >= 2) spriteBatch.Draw(ball, new Vector2(70, 20), Color.Blue);
                if (Lives1 >= 1) spriteBatch.Draw(ball, new Vector2(50, 20), Color.Blue);

                if (Lives2 >= 3) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 90, 20), Color.Red);
                if (Lives2 >= 2) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 70, 20), Color.Red);
                if (Lives2 >= 1) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 50, 20), Color.Red);
            }

            if (GameMode == 2)
            {
                float YouWin = Font1.MeasureString("P1 Wins").X;
                if (Lives2 == 0) spriteBatch.DrawString(Font1, "P1 Wins", new Vector2(ScreenWidth / 2 - YouWin / 2, ScreenHeight / 2), Color.White);

                float YouLose = Font1.MeasureString("P2 Wins").X;
                if (Lives1 == 0) spriteBatch.DrawString(Font1, "P2 Wins", new Vector2(ScreenWidth / 2 - YouLose / 2, ScreenHeight / 2), Color.White);

                float PlayAgain = Font1.MeasureString("Press Enter to play again").X;
                spriteBatch.DrawString(Font1, "Press Enter to play again", new Vector2(ScreenWidth / 2 - PlayAgain / 2, ScreenHeight / 2 + 35), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

*/