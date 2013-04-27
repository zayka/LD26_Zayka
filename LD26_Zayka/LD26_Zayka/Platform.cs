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

        //int numBlocks;

        public Platform(Vector2 pos, AnimSprite sprite)
            : base(pos, sprite)
        {
            //this.numBlocks = numBlocks;

        }

        public override void Draw(SpriteBatch sb)
        {
            sprite.Draw(sb, Pos);                     
            base.Draw(sb);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}
