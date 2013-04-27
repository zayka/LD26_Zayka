using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LD26_Zayka   
{
    public class Camera
    {
        Matrix viewMatrix;
        Matrix invViewMatrix;
        public Matrix View { get { return viewMatrix; } set { viewMatrix = value; invViewMatrix = Matrix.Invert(viewMatrix); } }
        public Matrix InvView { get { return invViewMatrix; } }
        float speed = 300;
        public float currentscale = 1;
        public Vector2 Position { get { return new Vector2(-viewMatrix.Translation.X, -viewMatrix.Translation.Y); } }

        

        public Camera()
        {
            viewMatrix = Matrix.Identity;
            Vector2  center = new Vector2(-Game1.screenWidth / 2, -Game1.screenHeight / 2);
            View = Matrix.CreateTranslation(new Vector3(-center, 0));

        }

        public void MoveLeft(GameTime gt)
        {
            float elapsed = (float)gt.ElapsedGameTime.TotalSeconds;
            Vector3 dir = new Vector3(elapsed * speed, 0, 0);
            View *= Matrix.CreateTranslation(dir);          
        }

        public void MoveRight(GameTime gt)
        {
            float elapsed = (float)gt.ElapsedGameTime.TotalSeconds;
            Vector3 dir = new Vector3(-elapsed * speed, 0, 0);
            View *= Matrix.CreateTranslation(dir);
        }

        public void MoveUp(GameTime gt)
        {
            float elapsed = (float)gt.ElapsedGameTime.TotalSeconds;
            Vector3 dir = new Vector3(0, elapsed * speed, 0);
            View *= Matrix.CreateTranslation(dir);            
        }

        public void MoveDown(GameTime gt)
        {
            float elapsed = (float)gt.ElapsedGameTime.TotalSeconds;
            Vector3 dir = new Vector3(0, -elapsed * speed, 0);
            View *= Matrix.CreateTranslation(dir);
        }

        public void MoveToVector(Vector2 dist)
        {
            Vector3 dir = new Vector3(dist, 0);
            View *= Matrix.CreateTranslation(dir);
        }

        public Matrix GetViewMatrix(Vector2 parallax)
        {
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f));
        }

        public void ScaleDown(GameTime gt)
        {

            Vector2 center = new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2);
            float scalechange = 0.1f;
            currentscale *= 1 + scalechange;
            View *= Matrix.CreateTranslation(new Vector3(-center, 0)) * Matrix.CreateScale(new Vector3(1 + scalechange, 1 + scalechange, 1)) * Matrix.CreateTranslation(new Vector3(center, 0));
        }

        public void ScaleUp(GameTime gt)
        {
            Vector2 center = new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2);
            float scalechange = 0.1f;
            currentscale *= 1 / (1 + scalechange);
            View *= Matrix.CreateTranslation(new Vector3(-center, 0)) * Matrix.CreateScale(new Vector3(1 / (1 + scalechange), 1 / (1 + scalechange), 1)) * Matrix.CreateTranslation(new Vector3(center, 0));
        }

    }
}
