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

namespace LD26_Zayka
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputState input;
        public SpriteFont font;
        public static int screenWidth = 1024;
        public static int screenHeight = 768;
        public Camera camera;
        public Camera Camera { get { return camera; } }
        GraphicsDevice device;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;


            IsMouseVisible = true;
            //IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Cnt.game = this;
            device = graphics.GraphicsDevice;
            input = InputState.GetInput();
            font = Content.Load<SpriteFont>("Font1");
            camera = new Camera();
#if DEBUG
            Components.Add(new FPSCounter.FPSCounter(this, font, spriteBatch));     
#endif
            
        }
   
        protected override void UnloadContent() { }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            input.Update();
            if (input.IsKeyPressed(Keys.Escape)) this.Exit();
          
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            device.Clear(Color.DarkBlue);

#if DEBUG
            spriteBatch.Begin();
            //spriteBatch.DrawString(font, "123123123", new Vector2(10, 0 * 12), Color.LightGreen);
            //spriteBatch.DrawString(font, "123123123", new Vector2(10, 1 * 12), Color.LightGreen);
            spriteBatch.End();
#endif
            base.Draw(gameTime);
        }
    }
}
