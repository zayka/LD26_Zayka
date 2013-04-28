using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD26_Zayka
{
    public enum PlatfotmType
    {
        Safe,
        Unstable,
        Evil,
        RandomAll,
        RandomSU
    }
    class Platform:GameObject
    {

        float hp = 100;
        bool isUnstable=false;
        bool isEvil = false;
        bool isCrashing = false;
        float crashTime = 3.0f;
        public bool toRemove = false;

        public Platform(Vector2 pos, PlatfotmType pType)
            : base(pos, null)
        {
            //AnimSprite sprite;
            if (pType == PlatfotmType.RandomAll) pType = (PlatfotmType)Game1.rnd.Next(3);
            if (pType == PlatfotmType.RandomSU) pType = (PlatfotmType)Game1.rnd.Next(2);
            switch (pType)
            {
                case PlatfotmType.Safe:
                    sprite = new AnimSprite(Cnt.game.Content.Load<Texture2D>("platform0"), 60, 20, 1, 100);
                    break;
                case PlatfotmType.Unstable:
                    sprite = new AnimSprite(Cnt.game.Content.Load<Texture2D>("platform_blue"), 60, 20, 1, 100);
                    isUnstable = true;
                    break;
                case PlatfotmType.Evil:
                    sprite = new AnimSprite(Cnt.game.Content.Load<Texture2D>("platform_red"), 60, 20, 1, 100);
                    isEvil = true;
                    break;               
                default:
                    break;
            }
            
                List<Vector2> list = new List<Vector2>() { new Vector2(-Width / 2, -Height / 2), new Vector2(Width / 2, -Height / 2), new Vector2(Width / 2, Height / 2), new Vector2(-Width / 2, Height / 2) };
                hull = new ZLight.ConvexHull(list, Vector2.Zero);
                hull.Pos = pos;
                light.pos = pos;
                light.radius = 1200;            
        }

        public override void Draw(SpriteBatch sb)
        {       
            Color c = Color.Lerp(Color.Transparent, Color.White, crashTime/3);
            sprite.Draw(sb, Pos, c);                        
        }

        public override void Update(GameTime gt)
        {
            if (isCrashing) crashTime -= (float)gt.ElapsedGameTime.TotalSeconds;
            if (crashTime < 0) toRemove = true;
            base.Update(gt);
        }

        public void Crash()
        {
            if (isUnstable)
                isCrashing = true;
        }

        internal void Damage(Player player)
        {
            if (this.isEvil) player.hitpoints -= 20 * 0.016f;
        }
    }
}
