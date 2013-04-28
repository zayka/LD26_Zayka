using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LD26_Zayka
{
    class HUD
    {
        Game1 game;
        Rectangle playerHPRect;
        Rectangle playerHPBackRect;
        Texture2D HUDMain;
        Texture2D HUDBack;
        Texture2D arrowTex;
        Texture2D lineTex;


        public HUD()
        {
            game = Cnt.game;
            HUDMain = game.Content.Load<Texture2D>("hud");
            HUDBack = game.Content.Load<Texture2D>("hudBack");
            arrowTex = game.Content.Load<Texture2D>("arrow");
            playerHPRect=new Rectangle(28,20,240,25);
            playerHPBackRect=new Rectangle(22,54,252,37);

            lineTex = new Texture2D(game.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            lineTex.SetData<Color>(new Color[1] { Color.White }); 

        }


        public void Update()
        {

        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(HUDBack, Vector2.Zero, Color.White);

            float fill1 = game.player.hitpoints / game.player.maxHitpoints;
            
            sb.Draw(HUDMain, new Vector2(28, 20), playerHPRect, Color.White, 0, Vector2.Zero, new Vector2(fill1, 1), SpriteEffects.None, 0);
            sb.Draw(HUDMain, new Vector2(22, 14), playerHPBackRect, Color.White, 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 0);

            sb.Draw(lineTex, new Rectangle(Game1.screenWidth - 20, 50, 5, 650), Color.DarkGray);
            float newpos = 0 - game.player.Pos.Y;
            float playerL = MathHelper.Lerp(680, 50, newpos / game.maxHeight);
            sb.Draw(arrowTex, new Vector2(Game1.screenWidth-40, playerL), Color.Green);

            newpos = 0 - game.eternalEvil.Pos;
            float evilL = MathHelper.Lerp(680, 50, newpos / game.maxHeight);
            sb.Draw(arrowTex, new Vector2(Game1.screenWidth - 40, evilL), Color.Red);


            /*
            if (game.CurrentLevel.LevelBoss != null)
            {
                float fill2 = game.CurrentLevel.LevelBoss.hitpoints / game.CurrentLevel.LevelBoss.maxHitpoints;

                sb.Draw(HUDMain, new Vector2(381 + (1 - fill2) * 621, 20), bossHPRect, Color.White, 0, Vector2.Zero, new Vector2(fill2, 1), SpriteEffects.FlipHorizontally, 0);
                sb.Draw(HUDMain, new Vector2(375, 14), bossHPBackRect, Color.White, 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 0);

            }*/
        }
    }
}
