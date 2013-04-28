using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Concurrent;

namespace LD26_Zayka
{
    public class ParticleEngine
    {
        List<Particle> mainList;
        Game1 game;
        SpriteBatch sb;
        public int Count { get { return mainList.Count; } }
        ConcurrentStack<Particle> pool;
        Particle pollen;
        Texture2D p4;
        List<int> freeblocks = new List<int>();
        int POOLSIZE = 1000;

        public ParticleEngine(Game1 game)
        {
            mainList = new List<Particle>();
            this.game = game;
            //sb = new SpriteBatch(game.GraphicsDevice);
            sb = Game1.spriteBatch;
            pool = new ConcurrentStack<Particle>();
            p4 = Cnt.game.Content.Load<Texture2D>("p4");

            for (int i = 0; i < POOLSIZE; i++)
            {
                pollen = new Particle(p4, Vector2.Zero, Vector2.Zero, 00, 0, new Vector4(0, 0, 0, 0), 0, 0, 0, 0);
                //pool.Push(pollen);
                mainList.Add(pollen);
            }
            freeblocks.Add(0);

        }


        public void Draw()
        {
            //sb.Draw(game.red, new Vector2(100, 100), Color.White);
            foreach (Particle p in mainList)
            {
                if (p.Active) p.Draw(sb);
            }
        }

        public void Clear()
        {
            foreach (var p in mainList)
            {
                p.Active = false;
            }
            freeblocks.Clear();
            freeblocks.Add(0);
        }

        public void Add(Vector2 pos, Vector2 speed,
                        float angle, float angleVelocity,
                        Vector4 color, float alphaVelocity,
                        float size, float sizeVelocity,
                        float ttl)
        {  /*
            pollen = new Particle(p4, pos, speed, angle, angleVelocity, color, alphaVelocity, size, sizeVelocity, ttl);
            mainList.Add(pollen);
           */
            pollen = mainList[freeblocks[0]];
            pollen.SetParam(pos, speed, angle, angleVelocity, color, alphaVelocity, size, sizeVelocity, ttl);

            if (freeblocks[0] < POOLSIZE - 1)
            {
                if (mainList[freeblocks[0] + 1].Active) freeblocks.Remove(freeblocks[0]);
                else freeblocks[0]++;
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    pollen = new Particle(p4, Vector2.Zero, Vector2.Zero, 00, 0, new Vector4(0, 0, 0, 0), 0, 0, 0, 0);
                    //pool.Push(pollen);
                    mainList.Add(pollen);
                }
                freeblocks.Clear();
                freeblocks.Add(POOLSIZE);
                POOLSIZE += 100;
            }
        }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < mainList.Count; i++)
            {
                if (mainList[i].Active) mainList[i].Update(gt);
                if (mainList[i].toRemove) Remove(mainList[i], i);
            }

            freeblocks.Sort();




            bool b = false;
            if (b)
            {
                int z = mainList.FindAll(p => p.Active).Count;
            }


        }

        void Remove(Particle p, int place)
        {
            p.Active = false;
            p.toRemove = false;
            if (place != 0 && !mainList[place - 1].Active)
                if (place != POOLSIZE - 1 && mainList[place + 1].Active) { return; }//-O+
                else
                {
                    freeblocks.Remove(place + 1);//-O-                                     
                }
            else
            {
                if (place != POOLSIZE - 1 && mainList[place + 1].Active) { freeblocks.Add(place); }//+O+
                else
                {
                    int index = freeblocks.FindIndex(block => block == place + 1);//+O-
                    freeblocks[index]--;
                }
            }

        }


        
    }
}
