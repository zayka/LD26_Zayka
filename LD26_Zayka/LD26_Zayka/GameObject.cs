using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZLight;

namespace LD26_Zayka
{
    public class GameObject
    {
        protected Vector2 pos;
        protected Vector2 prevPos;
        public Vector2 Dir;
        protected AnimSprite sprite;
        protected Vector2 velocity;

        public int Height { get { return sprite.Height; } }
        public int Width { get { return sprite.Width; } }
        public Rectangle rect { get { return new Rectangle((int)Math.Round(Pos.X - Origin.X), (int)Math.Round(Pos.Y - Origin.Y), sprite.Width, sprite.Height); } }
        protected float speed;

        public Vector2 Pos { get { return pos; } set { pos = value; } }
        public Vector2 PrevPos { get { return prevPos; } set { prevPos = value; } }
        public Vector2 Origin { get { return sprite.Origin; } }
        public float Speed { get { return speed; } set { speed = value; } }
        public bool isOnScreen 
        { 
            get 
            {
                float cameraOffset = Cnt.game.Camera.View.Translation.Y;
                return Pos == Vector2.Clamp(Pos, new Vector2(0, -cameraOffset), new Vector2(Game1.screenWidth, Game1.screenHeight - cameraOffset)); 
            } 
        }
        public Color[] CurrentData { get { return sprite.GetCurrentData(); } }
        public ZLight.ConvexHull hull;
        public Light light;

        public GameObject(Vector2 pos, AnimSprite sprite,float speed = 0)
        {
            this.prevPos = pos;
            this.pos = pos;
            this.sprite = sprite;            
            this.Pos = pos;
            this.Dir = Vector2.Zero;
            this.speed = speed;

            if (sprite != null)
            {
                List<Vector2> list = new List<Vector2>() { new Vector2(-Width / 2, -Height / 2), new Vector2(Width / 2, -Height / 2), new Vector2(Width / 2, Height / 2), new Vector2(-Width / 2, Height / 2) };
                hull = new ConvexHull(list, Vector2.Zero);
                hull.Pos = pos;
                light.pos = pos;
                light.radius = 1200;
            }
        }

        public virtual void Update(GameTime gt)
        {
            light.pos = new Vector2(pos.X, pos.Y + Cnt.game.camera.View.Translation.Y);
            hull.Pos = new Vector2(pos.X, pos.Y + Cnt.game.camera.View.Translation.Y);
            PrevPos = Pos;
            sprite.Update(gt);
            //Pos += Dir * speed * (float)gt.ElapsedGameTime.TotalSeconds;
            Pos += velocity * (float)gt.ElapsedGameTime.TotalSeconds;
           
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sprite.Draw(sb, Pos, Color.White);
        }

    }
}
