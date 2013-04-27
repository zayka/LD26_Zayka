using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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


        public Bonus(Vector2 pos, AnimSprite sprite, BonusType bType=BonusType.random):base(pos,sprite)
        {
            if (bType == BonusType.random) bType = (BonusType)Game1.rnd.Next(3);

            switch (bType)
            {
                case BonusType.Jump:
                    getBonus = getJump;
                    amount = 100;
                    getText = "+Jump";
                    break;
                case BonusType.Speed:
                    getBonus = getSpeed;
                    amount = 100;
                    getText = "+speed";
                    break;
                case BonusType.Hp:
                    getBonus = getHP;
                    amount = 25;
                    getText = "+HP";
                    break;
                case BonusType.Jetpack:
                    break;                
                default:
                    break;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!prepareToremove) sprite.Draw(sb, Pos, Color.White);
        }

        public void DrawText(SpriteBatch sb)
        {
            if (prepareToremove) sb.DrawString(Cnt.game.font, getText, Pos - sprite.Origin, Color.White);
        }

        public override void Update(GameTime gt)
        {
            if (prepareToremove) showTextTime -= (float)gt.ElapsedGameTime.TotalSeconds;

            angle += MathHelper.Pi * (float)gt.ElapsedGameTime.TotalSeconds;
            pos.Y = originPosY - 20 * (float)Math.Sin(angle);

            if (showTextTime < 0 && prepareToremove) toRemove = true;
        }


        public void Interact(Player pl)
        {
            if (prepareToremove) return;
            getBonus(pl);
            prepareToremove = true;
        }


        void getHP(Player pl)
        {
            pl.hitpoints += amount;
            pl.hitpoints = MathHelper.Clamp(pl.hitpoints, 0, pl.maxHitpoints);
        }
        void getJump(Player pl)
        {
            pl.maxSpeed += amount;
            //pl.hitpoints = MathHelper.Clamp(pl.hitpoints, 0, pl.maxHitpoints);
        }
        void getSpeed(Player pl)
        {
            pl.maxJump += amount;
            //pl.hitpoints = MathHelper.Clamp(pl.hitpoints, 0, pl.maxHitpoints);
        }
    }
}
