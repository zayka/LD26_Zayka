using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD26_Zayka
{
    enum PlatfotmType
    {
        Horisontal,
        Vertical
    }
    class Platform:GameObject
    {

        float hp = 100;
        bool isUnstable;
        bool isCrashing = false;
        float crashTime = 3.0f;
        public bool toRemove = false;

        public Platform(Vector2 pos, AnimSprite sprite, bool unstable=false)
            : base(pos, sprite)
        {

            isUnstable = unstable;
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
    }
}
