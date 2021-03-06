﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

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
        public float maxHitpoints=100;
        ParticleEmitter pEmitter;
        public bool Victory = false;
        public bool isDead=false;

        public Vector2 Velocity { get { return velocity; } }

        public Player(Vector2 pos, AnimSprite sprite, float speed):base(pos, sprite, speed)
        {
            input = InputState.GetInput();
            maxSpeed = speed;
            hitpoints=maxHitpoints;

            pEmitter = new ParticleEmitter(Cnt.game.pEngine, Vector2.Zero);
            //pps, pSpeed, 
            //posVar, alphaVel, 
            //minSize,maxSize, sizeVel,
            //ttl
            pEmitter.SetParam(600, 70,
                              10, 400,
                              0.4f, 0.5f, 0.15f,
                              50);
            pEmitter.Color = Color.DarkViolet;
          


        }


        public void Update(GameTime gt, Vector2 R)
        {
            if (!isDead)
            {
                velocity += R * (float)gt.ElapsedGameTime.TotalSeconds;
                velocity.Y = MathHelper.Clamp(velocity.Y, -1000, +1000);
                Update(gt);
                if (landing) { velocity.Y = 0; landing = false; }
                //pos = Vector2.Clamp(pos, Vector2.Zero + Origin/2, new Vector2(Game1.screenWidth, Game1.screenHeight) - Origin/2);
                pos.X = MathHelper.Clamp(pos.X, 0 + Origin.X / 2, Game1.screenWidth - Origin.X / 2);
            }
            pEmitter.Update(gt);
            pEmitter.Pos = pos;
            if (pos.Y > Cnt.game.eternalEvil.Pos - 550) { hitpoints -= 50*(float)gt.ElapsedGameTime.TotalSeconds; }
            if (pos.Y > Cnt.game.eternalEvil.Pos-200) { hitpoints = 0; }
            if (hitpoints <= 0) Die();
        }

        public void Die()
        {
            velocity = Vector2.Zero;
            isDead = true;
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
                Cue cyd = Cnt.game.soundBank.GetCue("Jump");
                cyd.Play();
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
            pos.Y -= 2*maxSpeed*dy;
        }
        public void MoveDown(float dy)
        {
            pos.Y += 2*maxSpeed*dy;
        }
    }
}
