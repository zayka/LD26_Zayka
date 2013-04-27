﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LD26_Zayka
{
    public class AnimSprite
    {
        Texture2D main;
        public int curframe;
        int maxFrame;
        double curframeTime;
        double frameTime;
        int width;
        int height;
        int dimX;
        int dimY;
        public Vector2 Origin { get { return new Vector2(width / 2, height / 2); } }

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public AnimSprite(Texture2D main, int width, int height, int totalframes,  double frameTime)
        {
            this.main = main;
            this.maxFrame = totalframes;
            this.frameTime = frameTime;
            this.width = width;
            this.height = height;
            this.dimX = main.Width / width;
            this.dimY = totalframes / dimX+1;
        }

        public void Update(GameTime gt)
        { 
            curframeTime+=gt.ElapsedGameTime.TotalSeconds;
            if (curframeTime > frameTime)
            {
                curframe = (curframe + 1) % maxFrame;
                curframeTime = 0;
            }
        }


        public void Draw(SpriteBatch sb, Vector2 pos, float angle=0,bool right=false)
        {
            //Vector2 origin = new Vector2(width / 2, height / 2);

            int x = curframe % dimX * height;
            int y = curframe / dimX * width;
            SpriteEffects sp = right ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            sb.Draw(main, pos , new Rectangle(x, y, width, height), Color.White, angle, Origin, 1, sp, 1);            
        }


        
       
        public Color[] GetCurrentData()
        {
            int x = curframe % dimX * height;
            int y = curframe / dimX * width;
            Color[] data = new Color[width * height];
            main.GetData<Color>(0, new Rectangle(x, y, width, height), data, 0, width * height);
            return data;
        }
        
    }
}