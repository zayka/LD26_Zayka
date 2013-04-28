using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD26_Zayka
{
    public class Player:GameObject
    {
        InputState input;
        public float maxSpeed;
        public float maxJump = 800;
        bool isJumping=false;
        bool landing = false;
        public float hitpoints;
        public float maxHitpoints;

        public Vector2 Velocity { get { return velocity; } }

        public Player(Vector2 pos, AnimSprite sprite, float speed):base(pos, sprite, speed)
        {
            input = InputState.GetInput();
            maxSpeed = speed;
        }


        public void Update(GameTime gt, Vector2 R)
        {            
            velocity += R * (float)gt.ElapsedGameTime.TotalSeconds;
            velocity.Y = MathHelper.Clamp(velocity.Y, -1000 , +1000);
            Update(gt);
            if (landing) { velocity.Y = 0; landing = false; }
            //pos = Vector2.Clamp(pos, Vector2.Zero + Origin/2, new Vector2(Game1.screenWidth, Game1.screenHeight) - Origin/2);
            pos.X = MathHelper.Clamp(pos.X, 0 + Origin.X / 2, Game1.screenWidth - Origin.X / 2);
           // light.pos = new Vector2(pos.X, pos.Y + Cnt.game.camera.View.Translation.Y);
           // hull.Pos = new Vector2(pos.X, pos.Y + Cnt.game.camera.View.Translation.Y);
        }

        public void HitY()
        {
            velocity.Y = 0;
            landing = true;
        }
        public void HitX()
        {
            velocity.X = 0;
            //landing = true;
        }

        public void Jump()
        {
            if (!isJumping)
            {
                velocity.Y -= maxJump;
                isJumping = true;
            }
        }

        public void MoveRight()
        {
            velocity.X = maxSpeed;            
        }

        public void MoveLeft()
        {
            velocity.X = -maxSpeed;
        }
        public void Stop()
        {
            velocity.X = 0;
        }
        public void Land()
        {
            isJumping = false;          
        }

        public void MoveUp(float dy)
        {
            pos.Y -= maxSpeed*dy;
        }
        public void MoveDown(float dy)
        {
            pos.Y += maxSpeed*dy;
        }
    }
}
