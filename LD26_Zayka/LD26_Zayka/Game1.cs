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
using ZLight;
using System.IO;

namespace LD26_Zayka
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        InputState input;
        public SpriteFont font;
        public static int screenWidth = 1024;
        public static int screenHeight = 768;
        public Camera camera;
        public Camera Camera { get { return camera; } }
        GraphicsDevice device;
        public static Random rnd = new Random(111222);
        int lastline=600;
        public EternalEvil eternalEvil;
        float G = 0;
        ZLights zl;
        Texture2D bg;
        HUD hud;

        public Player player;
        //Platform platform;
        List<Platform> allPlatforms = new List<Platform>();
        List<Bonus> BonusList = new List<Bonus>();
        bool lightsON = true;
        float ambient = 0.2f;
        public float maxHeight = 35000;

        Texture2D startText;
        public ParticleEngine pEngine;
        ParticleEmitter testEmitter;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;


            //IsMouseVisible = true;
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
            zl = new ZLights(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Cnt.game = this;
            device = graphics.GraphicsDevice;
            input = InputState.GetInput();
            font = Content.Load<SpriteFont>("Font1");
            camera = new Camera();
#if DEBUG
            Components.Add(new FPSCounter.FPSCounter(this, font, spriteBatch));     
#endif
            bg = Content.Load<Texture2D>("mainBG");
            startText = Content.Load<Texture2D>("startText");
            hud = new HUD();
            pEngine = new ParticleEngine(this);
            testEmitter = new ParticleEmitter(pEngine, Vector2.Zero);
            //pps, pSpeed, 
            //posVar, alphaVel, 
            //minSize,maxSize, sizeVel,
            //ttl
            testEmitter.SetParam(600, 40,
                              0, 400,
                              0.2f, 0.4f, 0.05f,
                              50);


            eternalEvil = new EternalEvil(pEngine);
            StartingEvent();

            //Ladder1Event();

            /*
            while (lastline > 0)
            {
                GenerateLine();
            }*/
            player = new Player(new Vector2(200, 550), new AnimSprite(Content.Load<Texture2D>("player_sprite"), 20, 20, 8, 0.5f),300);           
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
            camera.Position = new Vector2(screenWidth / 2, player.Pos.Y - 100);
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed == 0) return;

            eternalEvil.Update(gameTime);
            testEmitter.Pos = player.Pos;
            //testEmitter.Update(gameTime);
            pEngine.Update(gameTime);

            if (input.IsKeyPressed(Keys.Escape)) this.Exit();

            Vector2 R = Vector2.Zero;
            R.Y += G;


            if (input.IsKeyPressed(Keys.D1)) G = 2000;
            if (input.IsKeyPressed(Keys.D2)) G = 0;

            if (input.IsKeyPressed(Keys.W)) player.MoveUp(elapsed);
            if (input.IsKeyPressed(Keys.S)) player.MoveDown(elapsed);


            if (input.IsKeyPressed(Keys.D)) player.MoveRight();
            else
                if (input.IsKeyPressed(Keys.A)) player.MoveLeft();
                else player.Stop();
            if (input.IsNewKeyPressed(Keys.Space)) player.Jump();
            if (input.IsLeftButtonClick()) player.Jump();

            if (input.IsNewKeyPressed(Keys.Q)) { lightsON ^= true; };
                

            R=Collision(R*elapsed*elapsed,player.Velocity*elapsed)/elapsed/elapsed;
            
            player.Update(gameTime,R);
          
            for (int i = 0; i < allPlatforms.Count; i++)
            {
                if (allPlatforms[i].isOnScreen) allPlatforms[i].Update(gameTime);
                if (allPlatforms[i].toRemove) allPlatforms.RemoveAt(i--);
            }
            
            for (int i = 0; i < BonusList.Count; i++)
            {
               BonusList[i].Update(gameTime);
               if (BonusList[i].toRemove) BonusList.RemoveAt(i--);
            }  

            if (player.Pos.Y-screenHeight/2-1000<lastline) GenerateLine();

            CollisionBonus();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            ambient = MathHelper.Lerp(0.2f, 0.9f, Math.Abs(player.Pos.Y) / maxHeight);
            ambient = MathHelper.Clamp(ambient, 0.2f, 0.9f);
            if (lightsON)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                List<ConvexHull> chList = new List<ConvexHull>();
                List<Light> LList = new List<Light>();
                foreach (var plt in allPlatforms)
                {
                    if (plt.isOnScreen) chList.Add(plt.hull);
                }
                chList.Add(player.hull);
                LList.Add(player.light);
                Texture2D shadows = zl.RenderShadows(chList, LList, ambient);


                GraphicsDevice.SetRenderTarget(null);

                GraphicsDevice.Clear(new Color(0, 255, 0, 255));
               


                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.View);
                //spriteBatch.Begin();
                spriteBatch.Draw(bg, new Vector2(0,-camera.View.Translation.Y), Color.White);
                spriteBatch.Draw(shadows, new Vector2(0, -camera.View.Translation.Y), Color.White);
                spriteBatch.End();


                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, camera.View);
                pEngine.Draw();
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.View);
                player.Draw(spriteBatch);
                foreach (var platform in allPlatforms)
                {
                    if (platform.isOnScreen) platform.Draw(spriteBatch);
                }

                for (int i = 0; i < BonusList.Count; i++)
                {
                    BonusList[i].Draw(spriteBatch);
                }
                spriteBatch.End();
               
            }
            else
            {
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Gray);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.View);
                spriteBatch.Draw(bg, Vector2.Zero, Color.White);              
                player.Draw(spriteBatch);
                foreach (var platform in allPlatforms)
                {
                    if (platform.isOnScreen) platform.Draw(spriteBatch);
                }

                for (int i = 0; i < BonusList.Count; i++)
                {
                    BonusList[i].Draw(spriteBatch);
                }
                spriteBatch.End();
            }

            //hud
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.View);
            for (int i = 0; i < BonusList.Count; i++)
            {
                BonusList[i].DrawText(spriteBatch);
            }


            spriteBatch.Draw(startText, new Vector2(0,-500), Color.White);
            spriteBatch.End();

            spriteBatch.Begin();

            hud.Draw(spriteBatch);
            spriteBatch.End();




#if DEBUG
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "velocity" + player.Velocity, new Vector2(10, 0 * 12), Color.LightGreen);
            spriteBatch.DrawString(font, "pos" + player.Pos, new Vector2(10, 1 * 12), Color.LightGreen);
            spriteBatch.DrawString(font, "lastline" + lastline, new Vector2(10, 2 * 12), Color.LightGreen);
            spriteBatch.DrawString(font, "enernalEvil= " + eternalEvil.Pos, new Vector2(10, 3 * 12), Color.LightGreen);     
            spriteBatch.DrawString(font, "hp= " + player.hitpoints, new Vector2(10, 4 * 12), Color.LightGreen);
            spriteBatch.DrawString(font, "p= " + pEngine.Count, new Vector2(10, 5 * 12), Color.LightGreen); 
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
                    { dy = platform.Pos.Y - platform.Origin.Y - (prevPos.Y + 5); player.Land(); platform.Crash(); platform.Damage(player); }
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

        void CollisionBonus()
        {
            foreach (var bonus in BonusList)
            {
                //if (bonus.isOnScreen)
                if (bonus.rect.Intersects(player.rect))
                {
                    if (PixelCollision(bonus, player))
                    {
                        bonus.Interact(player);
                    }
                }
            }
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
                    if (sum1 > 100 && sum2 > 100) return true;
                }
            }
            return false;

        }


        public void GenerateLine()
        {
            if (rnd.NextDouble()*100 > 95)
            {
                int eventN = rnd.Next(4);
                if (eventN == 0) { Ladder1Event(); }
                if (eventN == 1) { Ladder2Event(); }
                if (eventN == 2) { Nors1Event(); }
                if (eventN == 3) { Nors2Event(); }
                GenerateRandomLine();
                GenerateRandomLine();
            }
            else
                GenerateRandomLine();

        }

        public void GenerateRandomLine()
        {
            Platform platform;
            float x = MathHelper.Lerp(0, 30, Math.Abs(lastline) / maxHeight);
            x = MathHelper.Clamp(x, 0, 35);
            for (int i = 30; i < screenWidth; i += 60)
            {
                if (rnd.Next(100) > 50 + x)
                {
                    platform = new Platform(new Vector2(i, lastline), PlatfotmType.RandomAll);
                    allPlatforms.Add(platform);
                    if (rnd.Next(100) > 70)
                    {
                        Bonus b = new Bonus(new Vector2(rnd.Next(screenWidth), lastline - 30), new AnimSprite(Content.Load<Texture2D>("bonuses"), 40, 40, 1, 100, 20));
                        BonusList.Add(b);
                    }
                }
            }
            lastline -= 100;
        }
        void TextureToFile(Texture2D tex, string filename)
        {
            Stream stream = new MemoryStream();
            Color[] colors = new Color[tex.Width * tex.Height];
            tex.GetData<Color>(colors);

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(tex.Width, tex.Height);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.PixelFormat pxf = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, pxf);
            IntPtr ptr = bmpData.Scan0;
            int numBytes = bmp.Width * bmp.Height * 3;
            byte[] rgbValues = new byte[numBytes];

            int j = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                rgbValues[j++] = colors[i].B;
                rgbValues[j++] = colors[i].G;
                rgbValues[j++] = colors[i].R;
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, numBytes);
            bmp.UnlockBits(bmpData);
            bmp.Save(filename);
        }

        void DrawEternalEvil()
        {

        }



        private void StartingEvent()
        {
            Platform platform;
            for (int i = 0; i < screenWidth; i += 60)
            {
                platform = new Platform(new Vector2(i, 600), PlatfotmType.RandomAll);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(i, 620), PlatfotmType.Safe);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(i, 640), PlatfotmType.Safe);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(i, 660), PlatfotmType.Safe);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(i, 680), PlatfotmType.Safe);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(i, 700), PlatfotmType.Safe);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(i, 720), PlatfotmType.Safe);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(i, 740), PlatfotmType.Safe);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(i, 760), PlatfotmType.Safe);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(i, 780), PlatfotmType.Safe);
                allPlatforms.Add(platform);
            }

            platform = new Platform(new Vector2(screenWidth / 2, 500), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 60, 400), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 120, 300), PlatfotmType.Evil );
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 180, 200), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 240, 100), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 180, 0), PlatfotmType.Safe);
            allPlatforms.Add(platform);

            platform = new Platform(new Vector2(screenWidth / 2 + 120, -100), PlatfotmType.Unstable);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 60, -100), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 60, -200), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 60, -300), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 60, -400), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 60, -500), PlatfotmType.Safe);
            allPlatforms.Add(platform);

            
             for (int i = 0; i < 60*10; i += 60)
             {
                 platform = new Platform(new Vector2(screenWidth / 2 - i, -500), PlatfotmType.Safe);
                 allPlatforms.Add(platform);
             }
            lastline = -600;
        }

        void Ladder1Event()
        {
            
         Platform platform;

         platform = new Platform(new Vector2(screenWidth / 2 - 120, lastline), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 180, lastline), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 240, lastline), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 120, lastline), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 180, lastline), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 240, lastline), PlatfotmType.Safe);
            allPlatforms.Add(platform);

            platform = new Platform(new Vector2(screenWidth / 2 - 120, lastline-100), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 180, lastline - 200), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 240, lastline - 300), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 300, lastline - 400), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 360, lastline - 500), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 440, lastline - 600), PlatfotmType.Safe);
            allPlatforms.Add(platform);

            platform = new Platform(new Vector2(screenWidth / 2 - 440, lastline - 700), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 360, lastline - 800), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 300, lastline - 900), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 240, lastline - 1000), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 180, lastline - 1100), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 - 120, lastline - 1200), PlatfotmType.Safe);
            allPlatforms.Add(platform);



            platform = new Platform(new Vector2(screenWidth / 2 + 120, lastline - 100), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 180, lastline - 200), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 240, lastline - 300), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 300, lastline - 400), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 360, lastline - 500), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 440, lastline - 600), PlatfotmType.Safe);
            allPlatforms.Add(platform);

            platform = new Platform(new Vector2(screenWidth / 2 + 440, lastline - 700), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 360, lastline - 800), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 300, lastline - 900), PlatfotmType.Safe);
            allPlatforms.Add(platform);            
            platform = new Platform(new Vector2(screenWidth / 2 + 240, lastline - 1000), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 180, lastline - 1100), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            platform = new Platform(new Vector2(screenWidth / 2 + 120, lastline - 1200), PlatfotmType.Safe);
            allPlatforms.Add(platform);
            
            lastline -= 1300;
        }


        void Ladder2Event()
        {
            Platform platform;
            /*
            for (int i = 0; i < screenWidth; i += 60)
            {
                platform = new Platform(new Vector2(i, lastline+100), new AnimSprite(Content.Load<Texture2D>("platform1"), 60, 20, 1, 100));
                allPlatforms.Add(platform);               
            }//*/

            for (int i = 0; i < 301; i += 100)
            {

                platform = new Platform(new Vector2(screenWidth / 2 + 60 * 5, lastline - i), PlatfotmType.RandomSU);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(screenWidth / 2 - 60 * 5, lastline - i), PlatfotmType.Safe);
                allPlatforms.Add(platform);
            }
            for (int i = 400; i < 701; i += 100)
            {
                platform = new Platform(new Vector2(screenWidth / 2 + 60 * 4, lastline - i), PlatfotmType.RandomSU);
                allPlatforms.Add(platform);
                platform = new Platform(new Vector2(screenWidth / 2 - 60 * 4, lastline - i), PlatfotmType.RandomSU);
                allPlatforms.Add(platform);
            }                      

            lastline -= 800;
        }


        void Nors1Event()
        {

            Platform platform;
            for (int i= 0; i < screenWidth; i += 120)
            {
                platform = new Platform(new Vector2(i, lastline), PlatfotmType.RandomAll);
                allPlatforms.Add(platform);
            }
            lastline -= 100;
            for (int j = 0; j < 501; j += 200)
            {

                platform = new Platform(new Vector2(30, lastline - j), PlatfotmType.Safe);
                allPlatforms.Add(platform);

                for (int i = 2 * 60 + 30; i < screenWidth; i += 60)
                {
                    platform = new Platform(new Vector2(i, lastline - j), PlatfotmType.Safe);
                    allPlatforms.Add(platform);
                }

                platform = new Platform(new Vector2(screenWidth - 30, lastline - j - 100), PlatfotmType.Safe);
                allPlatforms.Add(platform);

                for (int i = 30; i < screenWidth - 30-2*60; i += 60)
                {
                    platform = new Platform(new Vector2(i, lastline - j - 100), PlatfotmType.Safe);
                    allPlatforms.Add(platform);
                }
            }
          
            lastline -= 600;
        }

        void Nors2Event()
        {

            Platform platform;
            for (int i = 0; i < screenWidth; i += 120)
            {
                platform = new Platform(new Vector2(i, lastline), PlatfotmType.RandomAll);
                allPlatforms.Add(platform);
            }
            lastline -= 100;

            for (int j = 0; j < 501; j += 200)
            {

                platform = new Platform(new Vector2(30, lastline - j), PlatfotmType.Safe);
                allPlatforms.Add(platform);

                for (int i = 2 * 60 + 30; i < screenWidth; i += 60)
                {                   
                    platform = new Platform(new Vector2(i, lastline - j), PlatfotmType.RandomSU);                   
                    allPlatforms.Add(platform);                
                }

                platform = new Platform(new Vector2(screenWidth - 30, lastline - j - 100), PlatfotmType.Safe);
                allPlatforms.Add(platform);

                for (int i = 30; i < screenWidth - 30 - 2 * 60; i += 60)
                {
                    platform = new Platform(new Vector2(i, lastline - j - 100), PlatfotmType.RandomSU);
                    allPlatforms.Add(platform);
                }
            }

            lastline -= 600;
        }


    }
}
