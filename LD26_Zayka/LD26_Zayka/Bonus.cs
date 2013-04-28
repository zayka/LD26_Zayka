using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace LD26_Zayka
{
    public enum BonusType
    {
        Jump,
        Speed,
        Hp,
        Jetpack,
        random
    }
    class Bonus:GameObject
    {
        delegate void BonusDelegate(Player pl);
        BonusDelegate getBonus;
        float amount = 0;
        public bool toRemove;
        public bool prepareToremove;
        float showTextTime = 2.0f;
        Rectangle rectTex;
        float angle = 0;
        float originPosY;
        string getText = string.Empty;
        //public Rectangle rectB { get { return new Rectangle((int)Math.Round(Pos.X - origin.X), (int)Math.Round(Pos.Y - origin.Y), Width, Height); } }
       // ParticleEmitter pEmitter;

        public Bonus(Vector2 pos, AnimSprite sprite, BonusType bType=BonusType.random):base(pos,sprite)
        {
            if (bType == BonusType.random) bType = (BonusType)Game1.rnd.Next(3);
            /*
            pEmitter = new ParticleEmitter(Cnt.game.pEngine, Vector2.Zero);
            //pps, pSpeed, 
            //posVar, alphaVel, 
            //minSize,maxSize, sizeVel,
            //ttl
            pEmitter.SetParam(10, 30,
                              0, 200,
                              0.2f, 0.3f, 0.05f,

                              50);*/
            switch (bType)
            {
                case BonusType.Jump:
                    getBonus = getJump;
                    amount = 12;
                    getText = "+Jump";
                    sprite.startY = 0;
                   // pEmitter.Color = Color.Fuchsia;
                    break;
                case BonusType.Speed:
                    getBonus = getSpeed;
                    amount = 10;
                    getText = "+speed";
                    sprite.startY = 40;
                   // pEmitter.Color = Color.DeepSkyBlue;
                    break;
                case BonusType.Hp:
                    getBonus = getHP;
                    amount = 20;
                    getText = "+HP";
                    sprite.startY = 80;
                  //  pEmitter.Color = Color.Red;
                    break;
                case BonusType.Jetpack:
                    break;                
                default:
                    break;
            }
            //this.pos = pos;
            originPosY = pos.Y;
            angle = (float)Game1.rnd.NextDouble() * MathHelper.TwoPi;
          
           
        }
        
        public override void Draw(SpriteBatch sb)
        {
            if (!prepareToremove) { sprite.Draw(sb, Pos, Color.White); }
        }

        public void DrawText(SpriteBatch sb)
        {
            if (prepareToremove) sb.DrawString(Cnt.game.font, getText, Pos - sprite.Origin, Color.White);
        }

        public override void Update(GameTime gt)
        {
            if (prepareToremove) showTextTime -= (float)gt.ElapsedGameTime.TotalSeconds;

            angle += MathHelper.Pi * (float)gt.ElapsedGameTime.TotalSeconds;
            pos.Y = originPosY - 10 * (float)Math.Sin(angle);
           // if (!prepareToremove)  pEmitter.Update(gt);
          //  pEmitter.Pos = pos;
            if (showTextTime < 0 && prepareToremove) toRemove = true;
        }


        public void Interact(Player pl)
        {
            if (prepareToremove) return;
            getBonus(pl);
            prepareToremove = true;
            Cue cyd = Cnt.game.soundBank.GetCue("bonus");
            cyd.Play();
        }


        void getHP(Player pl)
        {
            pl.hitpoints += amount;
            pl.hitpoints = MathHelper.Clamp(pl.hitpoints, 0, pl.maxHitpoints);
        }
        void getJump(Player pl)
        {
            pl.maxJump += amount;
            //pl.hitpoints = MathHelper.Clamp(pl.hitpoints, 0, pl.maxHitpoints);
        }
        void getSpeed(Player pl)
        {
            pl.maxSpeed += amount;
            //pl.hitpoints = MathHelper.Clamp(pl.hitpoints, 0, pl.maxHitpoints);
        }
    }
}
