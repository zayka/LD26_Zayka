using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD26_Zayka
{
    public class Particle
    {
        Texture2D texture;
        Vector2 pos;
        Vector2 velocityDir;
        float velocity;
        float angle;
        float angleVelocity;
        Vector4 color;
        float alphaVelocity;
        float size;
        float sizeVelocity;        
        float ttl;
        Vector2 origin;

        public bool Active ;

        public bool toRemove;

        /// <summary>
        /// Конструтор частицы
        /// </summary>
        /// <param name="texture">Текстура</param>
        /// <param name="pos">Положение</param>
        /// <param name="speed">Скорость  точк/сек</param>
        /// <param name="angle">угол</param>
        /// <param name="angleVelocity">уголовая скорость рад/сек</param>
        /// <param name="color">цвет</param>
        /// <param name="alphaVelocity">скорость гашения цвета ед в сек.</param>
        /// <param name="size">размер</param>
        /// <param name="sizeVelocity">скорость уменьшения</param>
        /// <param name="ttl">время жизни в сек.</param>
        public Particle(Texture2D texture,
                        Vector2 pos, Vector2 speed, 
                        float angle, float angleVelocity,
                        Vector4 color, float alphaVelocity,
                        float size, float sizeVelocity,
                        float ttl)
        {
            this.pos = pos;
            this.velocity= speed.Length();
            velocityDir = speed;
            if (velocityDir!=Vector2.Zero) velocityDir.Normalize();
            this.angle = angle;
            this.angleVelocity = angleVelocity;
            this.color=color;
            this.color.W /= 255;
            this.alphaVelocity=alphaVelocity/255;
            this.size = size;
            this.sizeVelocity = sizeVelocity;
            this.ttl = ttl;

            this.texture = texture;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Active = false;
            toRemove = false;
        }

        public void Update(GameTime gt)
        {
            float elapsed = (float)gt.ElapsedGameTime.TotalSeconds;

            ttl -= elapsed;
            pos += elapsed * velocity * velocityDir;
            angle += elapsed * angleVelocity;
            color.W -= elapsed * alphaVelocity;
            size = MathHelper.Clamp(size, size - elapsed * sizeVelocity, 0);

            if (ttl < 0 || size <= 0 || color.W < 0) { toRemove = true; Active = false; }

        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, pos, null, new Color(color), angle, origin, size, SpriteEffects.None, 1);                
        }

        public void SetParam(Vector2 pos, Vector2 speed, 
                        float angle, float angleVelocity,
                        Vector4 color, float alphaVelocity,
                        float size, float sizeVelocity,
                        float ttl)
        {
            this.pos = pos;
            this.velocity = speed.Length();
            velocityDir = speed;
            if (velocityDir != Vector2.Zero) velocityDir.Normalize();
            this.angle = angle;
            this.angleVelocity = angleVelocity;
            this.color = color;            
            this.color.W /= 255;
            this.color.X /= 255;
            this.color.Y /= 255;
            this.color.Z /= 255;
            this.alphaVelocity = alphaVelocity / 255;
            this.size = size;
            this.sizeVelocity = sizeVelocity;
            this.ttl = ttl;
            this.toRemove = false;
            this.Active = true;
        }
    }
}
