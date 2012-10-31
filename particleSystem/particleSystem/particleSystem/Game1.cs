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

namespace particleSystem
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum gamestate
        {
            unpause, pause
        }
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        cPartSys mipSys;
        BasicEffect particleEffect;
        Model grid;
        gamestate game;

        float petime = 0;
        float pdelay = 0.25f;

        float cameraArc = -5;
        float cameraRotation = 0;
        float cameraDistance = 200;
        float cameraDist = 2.5f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //this.graphics.PreferredBackBufferWidth = 1920;
            //this.graphics.PreferredBackBufferHeight = 1080;
        }

        protected override void Initialize()
        {
            particleEffect = new BasicEffect(graphics.GraphicsDevice);
            mipSys = new cPartSys();
            
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
             spriteBatch = new SpriteBatch(GraphicsDevice);
             grid = Content.Load<Model>("grid");
             mipSys.Initialize(Content);
        }

     
        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            petime += elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) this.Exit();
           
            if (game == gamestate.unpause)
            {
                UpdateCamera(gameTime);
               //mipSys.solver(gameTime);////euler solver
               mipSys.updateParticles(gameTime);////simple solver
                if (petime >= pdelay)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.P))
                    {
                        game = gamestate.pause;
                        petime = 0.0f;
                        cameraRotation += time * 0.05f;
                    }
                }
            }
            else
            {

                if (petime >= pdelay)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.P))
                    {
                        game = gamestate.unpause;
                        petime = 0.0f;
                    }


                }
            }

       

            base.Update(gameTime);
        }

      protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Vector3 cameraPosition = new Vector3(0, 0, 2.5f);

            GraphicsDevice device = graphics.GraphicsDevice;
            float aspectRatio = (float)device.Viewport.Width /
                                  (float)device.Viewport.Height;
          
            float aspect = GraphicsDevice.Viewport.AspectRatio;
           
            Matrix view1 =Matrix.CreateTranslation(0, 0, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                          Matrix.CreateLookAt(new Vector3(0, 0, cameraDist),
                                              new Vector3(0, 0, 0), Vector3.Up);
            Matrix view2 =  Matrix.CreateTranslation(0, -25, 0) *
                            Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                            Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                            Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
                                                new Vector3(0, 0, 0), Vector3.Up);

            Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

            Matrix projection1 = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio, 1, 10000);
             

            Matrix world = Matrix.Identity;
           
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 10);
            DrawGrid(view2, projection1);
            for (int i = 0; i < mipSys.partCount; i++)
            {
                world = Matrix.Identity;

                world *= Matrix.CreateScale(0.04f);
                world *= Matrix.CreateTranslation(new Vector3((float)mipSys.pS0[i].Position.X, 
                                                              (float)mipSys.pS0[i].Position.Y, 
                                                              (float)mipSys.pS0[i].Position.Z));

                particleEffect.World = world;
                particleEffect.View = view1;
                particleEffect.Projection = projection;
                particleEffect.TextureEnabled = true;
                particleEffect.VertexColorEnabled = true;

                
                spriteBatch.Begin(0, null, null, DepthStencilState.None, RasterizerState.CullNone, particleEffect);
                mipSys.Draw(spriteBatch);
                spriteBatch.End();
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(mipSys.Font, "Press P to pause animation", new Vector2(10, 30), Color.Red);
            spriteBatch.DrawString(mipSys.Font, "current effect:", new Vector2(10, 50), Color.White);
            if (mipSys.type == 1) spriteBatch.DrawString(mipSys.Font, "smoke", new Vector2(220, 50), Color.White);
            if (mipSys.type == 2) spriteBatch.DrawString(mipSys.Font, "fire", new Vector2(220, 50), Color.White);
            if (mipSys.type == 3) spriteBatch.DrawString(mipSys.Font, "fountain", new Vector2(220, 50), Color.White);
            if (mipSys.type == 4) spriteBatch.DrawString(mipSys.Font, "rain", new Vector2(220, 50), Color.White);

            spriteBatch.End();
           
            

            base.Draw(gameTime);
        }

      void DrawGrid(Matrix view, Matrix projection)
      {
          GraphicsDevice device = graphics.GraphicsDevice;

          device.DepthStencilState = DepthStencilState.Default;
          device.SamplerStates[0] = SamplerState.LinearWrap;

          grid.Draw(Matrix.Identity, view, projection);
      }

      void UpdateCamera(GameTime gameTime)
      {
          float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
          int a = 5;
          // Check for input to rotate the camera up and down around the model.
          if (Keyboard.GetState().IsKeyDown(Keys.Up) ||
              Keyboard.GetState().IsKeyDown(Keys.W))
          {
              cameraArc += time * 0.025f;
          }

          if (Keyboard.GetState().IsKeyDown(Keys.L))
          {
              a++;
              a = a % 4 + 1;
              mipSys.type = a;
              base.Draw(gameTime);
          }

          if (Keyboard.GetState().IsKeyDown(Keys.Down) ||
              Keyboard.GetState().IsKeyDown(Keys.S))
          {
              cameraArc -= time * 0.025f;
          }

          cameraArc += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * time * 0.05f;

          // Limit the arc movement.
          if (cameraArc > 90.0f)
              cameraArc = 90.0f;
          else if (cameraArc < -90.0f)
              cameraArc = -90.0f;

          // Check for input to rotate the camera around the model.
          if (Keyboard.GetState().IsKeyDown(Keys.Right))
          {
              cameraRotation += time * 0.05f;
          }

          if (Keyboard.GetState().IsKeyDown(Keys.Left)) 
          {
              cameraRotation -= time * 0.05f;
          }

          cameraRotation += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * time * 0.1f;

          // Check for input to zoom camera in and out.
          if (Keyboard.GetState().IsKeyDown(Keys.Z))
              cameraDistance += time * 0.25f;

          if (Keyboard.GetState().IsKeyDown(Keys.X))
              cameraDistance -= time * 0.25f;

          cameraDistance += GamePad.GetState(PlayerIndex.One).Triggers.Left * time * 0.5f;
          cameraDistance -= GamePad.GetState(PlayerIndex.One).Triggers.Right * time * 0.5f;

          // Limit the camera distance.
          if (cameraDistance > 500)
              cameraDistance = 500;
          else if (cameraDistance < 10)
              cameraDistance = 10;

          if (GamePad.GetState(PlayerIndex.One).Buttons.RightStick == ButtonState.Pressed ||
              Keyboard.GetState().IsKeyDown(Keys.R))
          {
              cameraArc = -5;
              cameraRotation = 0;
              cameraDistance = 200;
          }
      }
    }
}
