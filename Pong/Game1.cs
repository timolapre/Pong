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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.W) && Paddle1Y > 0)
                {
                    Paddle1Y = Paddle1Y-4;
                }
            if (Keyboard.GetState().IsKeyDown(Keys.S) && Paddle1Y < 383)
                {
                    Paddle1Y = Paddle1Y+4;
                }

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && Paddle2Y > 0)
            {
                Paddle2Y = Paddle2Y - 4;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && Paddle2Y < 383)
            {
                Paddle2Y = Paddle2Y + 4;
            }

            if (BallY + ball.Height / 2 > Paddle2Y + Paddle2.Height/2 && Paddle2Y < GraphicsDevice.Viewport.Height - Paddle2.Height) Paddle2Y += 1;
            if (BallY + ball.Height / 2 < Paddle2Y + Paddle2.Height/2 && Paddle2Y > 0) Paddle2Y -= 1;

            speed += 0.001f;

            BallX += speed * (float)Math.Cos(BallAngle * Math.PI / 180);
            BallY += speed * (float)Math.Sin(BallAngle * Math.PI / 180);

            //if (BallUp == true) BallY += speed * (float)Math.Sin(BallAngle); else BallY += speed * (float)Math.Sin(BallAngle);
            //if (BallRight == true) BallX += speed * (float)Math.Cos(BallAngle); else BallX += speed * (float)Math.Cos(BallAngle);

            DebugY = Paddle1.Height;
            DebugX = (float)Math.Cos(BallAngle * Math.PI / 180);

            if (BallY < 0) BallAngle = -BallAngle;
            if (BallY > GraphicsDevice.Viewport.Height-ball.Height) BallAngle = -BallAngle;

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

            Rectangle P1B = Paddle1.Bounds;
            P1B.Offset(50, Paddle1Y);
            Rectangle P2B = Paddle2.Bounds;
            P2B.Offset(ScreenWidth - 50, Paddle2Y);
            Rectangle ballB = ball.Bounds;
            ballB.Offset(BallX, BallY);

            if (ballB.Intersects(P1B)) BallAngle = (BallY+ball.Height/2)-(Paddle1Y+Paddle1.Height/2);
            if (ballB.Intersects(P2B)) BallAngle = (Paddle2Y + Paddle2.Height / 2) - (BallY + ball.Height / 2) + 180;

            base.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.P)) Debugger.Break();

            if (Keyboard.GetState().IsKeyDown(Keys.F)) graphics.ToggleFullScreen();

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            spriteBatch.Begin();
            if (GameMode == 0)
            {
                spriteBatch.DrawString(Font1, "Play", new Vector2(ScreenWidth / 2, ScreenHeight / 2), Color.White);
            }

            if (GameMode == 1)
            {
                spriteBatch.Draw(ball, new Vector2(BallX, BallY), Color.PeachPuff);
                spriteBatch.Draw(Paddle1, new Vector2(50, Paddle1Y), Color.White);
                spriteBatch.Draw(Paddle2, new Vector2(ScreenWidth - 50, Paddle2Y), Color.White);
                if (Lives1 >= 3) spriteBatch.Draw(ball, new Vector2(90, 20), Color.Blue);
                if (Lives1 >= 2) spriteBatch.Draw(ball, new Vector2(70, 20), Color.Blue);
                if (Lives1 >= 1) spriteBatch.Draw(ball, new Vector2(50, 20), Color.Blue);

                if (Lives2 >= 3) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 90, 20), Color.Red);
                if (Lives2 >= 2) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 70, 20), Color.Red);
                if (Lives2 >= 1) spriteBatch.Draw(ball, new Vector2(ScreenWidth - 50, 20), Color.Red);
            }

            spriteBatch.End();

            base.Draw(gameTime); 
        }
    }
}