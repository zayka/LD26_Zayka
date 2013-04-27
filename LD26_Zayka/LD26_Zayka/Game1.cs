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
        public static Random rnd = new Random(111222);
        int lastline=400;
        float G = 2000;
        GamePadState gs;

        public Player player;
        //Platform platform;
        List<Platform> allPlatforms = new List<Platform>();
        List<Bonus> BonusList = new List<Bonus>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;


            IsMouseVisible = true;
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

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

            LoadStartPlatform();
            while (lastline > 0)
            {
                GenerateLine();
            }
            player = new Player(new Vector2(200, 550), new AnimSprite(Content.Load<Texture2D>("player_sprite"), 20, 20, 8, 0.5f),300);
            //Platform platform = new Platform(new Vector2(200, 600), new AnimSprite(Content.Load<Texture2D>("platform1"), 60, 20, 1, 100));            
            //allPlatforms.Add(platform);
           // platform = new Platform(new Vector2(200, 100), new AnimSprite(Content.Load<Texture2D>("platform1"), 60, 20, 1, 100));
            //allPlatforms.Add(platform);
        }

        private void LoadStartPlatform()
        {
            Platform platform;
            for (int i = 0; i < screenWidth; i+=60)
            {
                platform = new Platform(new Vector2(i, 600), new AnimSprite(Content.Load<Texture2D>("platform1"), 60, 20, 1, 100));
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(i+200, 600-100), new AnimSprite(Content.Load<Texture2D>("platform1"), 60, 20, 1, 100));
                allPlatforms.Add(platform);
            }                       
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
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed == 0) return;
            if (input.IsKeyPressed(Keys.Escape)) this.Exit();

            Vector2 R = Vector2.Zero;
            R.Y += G;

            if (input.IsKeyPressed(Keys.D)) player.MoveRight();
            else
                if (input.IsKeyPressed(Keys.A)) player.MoveLeft();
                else player.Stop();            
            if (input.IsNewKeyPressed(Keys.Space)) player.Jump();
            if (input.IsNewKeyPressed(Keys.E)) player.Jump();
            if (input.IsLeftButtonClick()) player.Jump();

            if (input.IsNewKeyPressed(Keys.Q)) allPlatforms.Clear();
            if (input.IsNewKeyPressed(Keys.D1)) 
            {
                Bonus b = new Bonus(new Vector2(100, 200), new AnimSprite(Content.Load<Texture2D>("bonuses"), 20, 20, 1, 100,0), BonusType.Jump);
                BonusList.Add(b);
            }

            if (input.IsNewKeyPressed(Keys.D2))
            {
                Bonus b = new Bonus(new Vector2(200, 200), new AnimSprite(Content.Load<Texture2D>("bonuses"), 20, 20, 1, 100, 20), BonusType.Speed);
                BonusList.Add(b);
            }           

            R=Collision(R*elapsed*elapsed,player.Velocity*elapsed)/elapsed/elapsed;
            
           // player.Pos -= new Vector2(0, -G*elapsed);
            player.Update(gameTime,R);

            for (int i = 0; i < allPlatforms.Count; i++)
            {
                if (allPlatforms[i].isOnScreen) allPlatforms[i].Update(gameTime);
                if (allPlatforms[i].toRemove) allPlatforms.RemoveAt(i--);
            }

            for (int i = 0; i < BonusList.Count; i++)
            {
                if (BonusList[i].isOnScreen) BonusList[i].Update(gameTime);                
            }  

            if (player.Pos.Y-screenHeight/2-1000<lastline) GenerateLine();
            camera.Position = new Vector2(screenWidth/2,player.Pos.Y-100);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            device.Clear(Color.CornflowerBlue);


            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,camera.View);
            player.Draw(spriteBatch);
            foreach (var platform in allPlatforms)
            {
                if (platform.isOnScreen) platform.Draw(spriteBatch);
            }

            for (int i = 0; i < BonusList.Count; i++)
            {
                if (BonusList[i].isOnScreen) BonusList[i].Draw(spriteBatch);
            } 

            spriteBatch.End();




#if DEBUG
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "velocity"+player.Velocity, new Vector2(10, 0 * 12), Color.LightGreen);
            spriteBatch.DrawString(font, "pos"+player.Pos, new Vector2(10, 1 * 12), Color.LightGreen);
            spriteBatch.DrawString(font, "lastline" + lastline, new Vector2(10, 2 * 12), Color.LightGreen);
            spriteBatch.DrawString(font, "gs" + gs.ToString(), new Vector2(10, 3 * 12), Color.LightGreen);
            spriteBatch.End();
#endif
            base.Draw(gameTime);
        }


        Vector2 Collision(Vector2 Rdxdy, Vector2 Vdxdy)
        {
            Vector2 dxdy = Rdxdy + Vdxdy;
            bool isHitX=false;
            bool isHitY = false;
            Vector2 prevPos = player.Pos;
            float dx = dxdy.X;
            float dy = dxdy.Y;
            // +-5 - player bounds in texture
            foreach (var platform in allPlatforms)
            {
                if (!platform.isOnScreen) continue;
                // h
                player.Pos = player.Pos + new Vector2(dx, 0);
                if (PixelCollision(platform, player))
                {
                    isHitX = true;
                    player.HitX();
                    if (Math.Sign(dx) > 0)
                        dx = (platform.Pos.X - platform.Origin.X) - (prevPos.X + 5);
                    else
                        dx = platform.Pos.X + platform.Origin.X - (prevPos.X - 5);
                }

                player.Pos = prevPos;
                //v
                player.Pos = player.Pos + new Vector2(0, dy);
                if (PixelCollision(platform, player)) 
                {
                    
                    isHitY = true;
                    player.HitY();

                    if (Math.Sign(dy) > 0)
                    { dy = platform.Pos.Y - platform.Origin.Y - (prevPos.Y + 5); player.Land(); platform.Crash(); }
                    else
                    { dy = platform.Pos.Y + platform.Origin.Y - (prevPos.Y - 5); platform.Crash(); }
                }
                
                player.Pos = prevPos;
            }
            player.Pos = prevPos;
            if (isHitX) return new Vector2(dx, Rdxdy.Y);
            if (isHitY) return new Vector2(Rdxdy.X, dy);
            else return Rdxdy;

        }
        
        public static bool PixelCollision(GameObject first, GameObject second)
        {
            Rectangle firstRect = first.rect;
            Rectangle secondRect = second.rect;
            Color[] firstData = new Color[first.Width * first.Height];
            Color[] secondData = new Color[second.Width * second.Height];

            firstData = first.CurrentData;
            secondData = second.CurrentData;

            int top = Math.Max(firstRect.Top, secondRect.Top);
            int bottom = Math.Min(firstRect.Bottom, secondRect.Bottom);
            int left = Math.Max(firstRect.Left, secondRect.Left);
            int right = Math.Min(firstRect.Right, secondRect.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color color1 = firstData[(x - firstRect.Left) + (y - firstRect.Top) * firstRect.Width];
                    Color color2 = secondData[(x - secondRect.Left) + (y - secondRect.Top) * secondRect.Width];
                    int sum1 = color1.R + color1.G + color1.B + color1.A;
                    int sum2 = color2.R + color2.G + color2.B + color2.A;
                    if (sum1 > 300 && sum2 > 300) return true;
                }
            }
            return false;

        }


        public void GenerateLine()
        {
            Platform platform;
            float x = MathHelper.Lerp(0, 50, Math.Abs(lastline+1)/20000.0f);
            for (int i = 30; i < screenWidth; i+=60)
            {
                if (rnd.Next(100) > 50+x)
                {
                    
                    if (rnd.Next(100) > 50) 
                    platform = new Platform(new Vector2(i, lastline), new AnimSprite(Content.Load<Texture2D>("platform1"), 60, 20, 1, 100),false);
                    else
                        platform = new Platform(new Vector2(i, lastline), new AnimSprite(Content.Load<Texture2D>("platform2"), 60, 20, 1, 100), true);
                    allPlatforms.Add(platform);
                }

            }
            lastline-=100;
        }

    }
}
