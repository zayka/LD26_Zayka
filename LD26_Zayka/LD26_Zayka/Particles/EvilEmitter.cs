using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LD26_Zayka
{
    class EvilEmitter:ParticleEmitter
    {
        public EvilEmitter(ParticleEngine engine):base(engine,new Vector2(-10000,10000))
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gt)
        {
            float elapsed = (float)gt.ElapsedGameTime.TotalSeconds;
            //param check

            Vector2 dist = prevPos - pos;
            float length = dist.Length();
            float curTime = 0;
            float pTime = 1 / pps;

            while (curTime < elapsed)
            {
                curTime += pTime;
                Vector2 curPos = Vector2.Lerp(prevPos, pos, curTime / elapsed);
                /*
                float angle = (float)Game1.rnd.NextDouble() * MathHelper.TwoPi;
                float x = (float)Math.Cos(angle);
                float y = (float)Math.Sin(angle);
                Vector2 randomDir = new Vector2(x, y);
                */
                Vector2 randomDir = new Vector2((float)Game1.rnd.NextDouble() * 2.0f-1.0f , (float)Game1.rnd.NextDouble() * 0.2f - 1.5f);
                Vector2 randomPos = new Vector2((float)Game1.rnd.NextDouble() * 1.0f - 0.5f, (float)Game1.rnd.NextDouble() * 1.0f - 0.5f);
                engine.Add(posVar * randomPos + curPos, pSpeed * randomDir, 0, 1, new Vector4(color.R, color.G, color.B, color.A), alphaVel, (float)Game1.rnd.NextDouble() * (maxSize - minSize) + minSize, sizeVel, ttl);
            }
            prevPos = pos;
        }
    }
}
