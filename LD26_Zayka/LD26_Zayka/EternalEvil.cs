using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD26_Zayka
{
    public class EternalEvil
    {
        public float Pos;
        float speed = 100;

        List<EvilEmitter> pEmitters=new List<EvilEmitter>();

        public EternalEvil(ParticleEngine engine)
        {
            Pos = 2000;           
            //pps, pSpeed, 
            //posVar, alphaVel, 
            //minSize,maxSize, sizeVel,
            //ttl
            for (int i = 0; i < 21; i++)
            {
                EvilEmitter em = new EvilEmitter(engine);
                em.SetParam(50, 200,
                             10, 50,
                             1.0f, 1.2f, 0.02f,
                             50);
                em.Color = new Color(Game1.rnd.Next(50)+205, Game1.rnd.Next(50)+50, Game1.rnd.Next(50)+50, 255);
                pEmitters.Add(em);

            }
           
           
        }

        public void Update(GameTime gt)
        {
            speed = MathHelper.Lerp(70, 150, -Pos / Cnt.game.maxHeight);
            float elapsed =(float)gt.ElapsedGameTime.TotalSeconds;
            Pos -= speed * elapsed;
            int i = 0;
            foreach (var em in pEmitters)
            {
                em.Update(gt);
                em.Pos = new Vector2(i*50, Pos);
                i++;
            }
        }

        public void Draw(SpriteBatch sb)
        {
          
        }
    }
}
