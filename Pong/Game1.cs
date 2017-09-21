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

        //object vars
        float Paddle1Y;
        float Paddle2Y;
        float BallY;
        float BallX;
        //bool BallUp = true;
        //bool BallRight = false;

        //variable vars
        int Lives1 = 3, Lives2 = 3;
        float speed = 3f;
        
        //general vars
        int GameMode = 0;
        int StartSide;
        float BallAngle;
        float ScreenWidth;
        float ScreenHeight;
        
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
            Font1 = Content.Load <SpriteFont>("Magnum");
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
                //start vars
                GameMode = 1;
                Lives1 = 3;
                Lives2 = 3;
                BallY = 230;
                BallX = 390;
                speed = 3;

            }
            if (Lives1 == 0 || Lives2 == 0) GameMode = 2;

            if (GameMode == 1)
            {
                ///controls
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                //controls player1
                if (Keyboard.GetState().IsKeyDown(Keys.W) && Paddle1Y > 0)
                {
                    Paddle1Y = Paddle1Y - 4;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S) && Paddle1Y < 383)
                {
                    Paddle1Y = Paddle1Y + 4;
                }
                //controls player2
                if (Keyboard.GetState().IsKeyDown(Keys.Up) && Paddle2Y > 0)
                {
                    Paddle2Y = Paddle2Y - 4;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down) && Paddle2Y < 383)
                {
                    Paddle2Y = Paddle2Y + 4;
                }

                //ball movement
                if (BallY + ball.Height / 2 > Paddle2Y + Paddle2.Height / 2 && Paddle2Y < GraphicsDevice.Viewport.Height - Paddle2.Height) Paddle2Y += 1.95f;
                if (BallY + ball.Height / 2 < Paddle2Y + Paddle2.Height / 2 && Paddle2Y > 0) Paddle2Y -= 1.95f;

                speed += 0.001f;

                BallX += speed * (float)Math.Cos(BallAngle * Math.PI / 180);
                BallY += speed * (float)Math.Sin(BallAngle * Math.PI / 180);

                //if (BallUp == true) BallY += speed * (float)Math.Sin(BallAngle); else BallY += speed * (float)Math.Sin(BallAngle);
                //if (BallRight == true) BallX += speed * (float)Math.Cos(BallAngle); else BallX += speed * (float)Math.Cos(BallAngle);

                //paddle debug
                DebugY = Paddle1.Height;
                DebugX = (float)Math.Cos(BallAngle * Math.PI / 180);

                if (BallY < 0) BallAngle = -BallAngle;
                if (BallY > GraphicsDevice.Viewport.Height - ball.Height) BallAngle = -BallAngle;

                //lives
                if (BallX < 0)
                {
                    BallY = 230;
                    BallX = 390;
                    speed = 3;
                    Lives1--;
                }
                if (BallX > 800)
                {
                    BallY = 230;
                    BallX = 390;
                    speed = 3;
                    Lives2--;
                }

                ///collsions
                //paddle collisions
                //hitbox
                Rectangle P1B = Paddle1.Bounds;
                P1B.Offset(50, Paddle1Y);
                Rectangle P2B = Paddle2.Bounds;
                P2B.Offset(ScreenWidth - 50, Paddle2Y);
                Rectangle ballB = ball.Bounds;
                ballB.Offset(BallX, BallY);
                //collsion
                if (ballB.Intersects(P1B)) BallAngle = (BallY + ball.Height / 2) - (Paddle1Y + Paddle1.Height / 2);
                if (ballB.Intersects(P2B)) BallAngle = (Paddle2Y + Paddle2.Height / 2) - (BallY + ball.Height / 2) + 180;
                
                base.Update(gameTime);

                //screenoptions
                if (Keyboard.GetState().IsKeyDown(Keys.F)) graphics.ToggleFullScreen();

            }
            if (Keyboard.GetState().IsKeyDown(Keys.P)) Debugger.Break();

        }
        ///gamedraw loop
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);
            //draw menu
            spriteBatch.Begin();
            if (GameMode == 0)
            {
                float PressEnter = Font1.MeasureString("Press Enter to start").X;
                spriteBatch.DrawString(Font1, "Press Enter to start", new Vector2(ScreenWidth / 2 - PressEnter / 2, ScreenHeight / 2), Color.White);
            }
            //draw pong gamemode 1
            if (GameMode == 1)
            {
                //draw objects
                spriteBatch.Draw(ball, new Vector2(BallX, BallY), Color.White);
                spriteBatch.Draw(Paddle1, new Vector2(50, Paddle1Y), Color.White);
                spriteBatch.Draw(Paddle2, new Vector2(ScreenWidth - 50, Paddle2Y), Color.White);
                ///draw lives
                //draw lives player1
                if (Lives1 >= 3) spriteBatch.Draw(ball, new Vector2(90, 20), Color.Blue);
                if (Lives1 >= 2) spriteBatch.Draw(ball, new Vector2(70, 20), Color.Blue);
                if (Lives1 >= 1) spriteBatch.Draw(ball, new Vector2(50, 20), Color.Blue);
                //draw lives player2
                if (Lives2 >= 3) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 90, 20), Color.Red);
                if (Lives2 >= 2) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 70, 20), Color.Red);
                if (Lives2 >= 1) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 50, 20), Color.Red);
            }
            //draw game end
            if(GameMode == 2)
            {
                //draw game end
                float YouWin = Font1.MeasureString("You Win").X;
                if (Lives2 == 0) spriteBatch.DrawString(Font1, "You Win", new Vector2(ScreenWidth / 2 - YouWin / 2, ScreenHeight / 2), Color.White);

                float YouLose = Font1.MeasureString("You Lose").X;
                if (Lives1 == 0) spriteBatch.DrawString(Font1, "You Lose", new Vector2(ScreenWidth / 2 - YouLose / 2, ScreenHeight / 2), Color.White);
                //draw game restart
                float PlayAgain = Font1.MeasureString("Press Enter to play again").X;
                spriteBatch.DrawString(Font1, "Press Enter to play again", new Vector2(ScreenWidth / 2 - PlayAgain / 2, ScreenHeight / 2 + 35), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime); 
        }
    }
}