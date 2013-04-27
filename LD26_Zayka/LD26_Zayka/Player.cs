using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD26_Zayka
{
    class Player:GameObject
    {
        InputState input;
        public float maxSpeed;
        bool isJumping=false;
        bool landing = false;

        public Vector2 Velocity { get { return velocity; } }

        public Player(Vector2 pos, AnimSprite sprite, float speed):base(pos, sprite, speed)
        {
            input = InputState.GetInput();
            maxSpeed = speed;
        }


        public void Update(GameTime gt, Vector2 R)
        {            
            velocity += R * (float)gt.ElapsedGameTime.TotalSeconds;
            Update(gt);
            if (landing) { velocity.Y = 0; landing = false; }
            //pos = Vector2.Clamp(pos, Vector2.Zero + Origin/2, new Vector2(Game1.screenWidth, Game1.screenHeight) - Origin/2);
            pos.X = MathHelper.Clamp(pos.X, 0 + Origin.X / 2, Game1.screenWidth - Origin.X / 2);
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
                velocity.Y -= 800;
                isJumping = true;
            }
        }

        public void MoveRight()
        {
            velocity.X = 350;            
        }

        public void MoveLeft()
        {
            velocity.X = -350;
        }
        public void Stop()
        {
            velocity.X = 0;
        }
        public void Land()
        {
            isJumping = false;          
        }
    }
}
