using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ZLight
{
    public struct Light
    {
        public Vector2 pos;
        public float radius;
    }

    public struct ConvexHull
    {
        public List<Vector2> vertices;
        public Vector2 Pos;
        public ConvexHull(List<Vector2> list, Vector2 pos)
        {

            this.Pos = pos;
            this.vertices = list;
        }
    }

    public class ZLights
    {
        Effect effectLight;
        Effect effectStrip;
        Queue<RenderTarget2D> qrt;
        RenderTarget2D shadowMap;
        Game game;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        VertexPositionColor[] vertices;
        int screenWidth;
        int screenHeight;

        public ZLights(Game game)
        {
            this.game = game;
            device = game.GraphicsDevice;
            
            screenWidth = device.Viewport.TitleSafeArea.Width;
            screenHeight = device.Viewport.TitleSafeArea.Height;
            qrt = new Queue<RenderTarget2D>();
            shadowMap = new RenderTarget2D(device, screenWidth, screenHeight, false, device.DisplayMode.Format, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

            effectLight = game.Content.Load<Effect>("lightEffect");
            effectStrip = game.Content.Load<Effect>("effects");

            RenderTarget2D rt1 = new RenderTarget2D(device, screenWidth, screenHeight, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            RenderTarget2D rt2 = new RenderTarget2D(device, screenWidth, screenHeight, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);           
            qrt = new Queue<RenderTarget2D>();
            qrt.Enqueue(rt1);
            qrt.Enqueue(rt2);
            spriteBatch = new SpriteBatch(device);

        }



        public Texture2D RenderShadows(List<ConvexHull> hulls, List<Light> lights, float ambient = 0.0f)
        {
            RenderTarget2D currentRT;
            RenderTarget2D nextRT;

            currentRT = qrt.Dequeue();
            nextRT = qrt.Dequeue();
            device.SetRenderTarget(currentRT);
            device.Clear(new Color(0, 0, 0, 255));
            device.SetRenderTarget(nextRT);
            device.Clear(new Color(0, 0, 0, 255));

            effectLight.CurrentTechnique = effectLight.Techniques["Lights"];
            foreach (Light l in lights)
            {
                device.SetRenderTarget(shadowMap);
                device.Clear(new Color(0, 0, 0, 0));

                foreach (ConvexHull hull in hulls)
                {
                    int shadowStart = -1;
                    int shadowEnd = -1;
                    float prevDot = 0;
                    for (int i = 0; i < hull.vertices.Count; i++)
                    {
                        int n = i == hull.vertices.Count - 1 ? 0 : i + 1;
                        Vector2 v1 = hull.vertices[i] + hull.Pos;
                        Vector2 v2 = hull.vertices[n] + hull.Pos;
                        Vector2 normal = new Vector2(v2.Y - v1.Y, v1.X - v2.X);
                        Vector2 lightVector = hull.vertices[i] + hull.Pos - l.pos;
                        Vector2 lightVector2 = hull.vertices[n] + hull.Pos - l.pos;
                        float dot = Vector2.Dot(normal, lightVector);
                        float dot2 = Vector2.Dot(normal, lightVector2);
                        //>0 - back
                        //<0 - front
                        if (i == 0) prevDot = dot;
                        else
                        {
                            if (Math.Sign(dot) != Math.Sign(prevDot))
                            {
                                if (Math.Sign(dot) == 1) shadowStart = i;
                                else shadowEnd = i;
                            }
                            prevDot = dot;
                        }
                    }
                    List<Vector2> shadow = new List<Vector2>();
                    shadowStart = shadowStart == -1 ? 0 : shadowStart;
                    shadowEnd = shadowEnd == -1 ? 0 : shadowEnd;
                    int index = shadowStart;
                    Vector2 length = Vector2.Zero;
                    shadow.Add(hull.vertices[shadowStart] + hull.Pos);
                    while (index != shadowEnd)
                    {
                        int n = index == hull.vertices.Count - 1 ? 0 : index + 1;
                        length = hull.vertices[index] + hull.Pos - l.pos;
                        shadow.Add(hull.vertices[index] + hull.Pos + 70 * length);
                        shadow.Add(hull.vertices[n] + hull.Pos);

                        index = (index + 1) % hull.vertices.Count;
                    }
                    length = hull.vertices[shadowEnd] + hull.Pos - l.pos;
                    shadow.Add(hull.vertices[shadowEnd] + hull.Pos + 70 * length);

                    effectStrip.CurrentTechnique = effectStrip.Techniques["Pretransformed"];
                    device.Textures[1] = nextRT;
                    foreach (EffectPass pass in effectStrip.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        if (shadow.Count != 2) DrawStrip(shadow);
                    }
                }
                // shadowMap end

                device.SetRenderTarget(currentRT);
                device.Textures[1] = shadowMap;

                effectLight.Parameters["lightRadius"].SetValue(l.radius);
                effectLight.Parameters["lightpos"].SetValue(l.pos);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effectLight);
                spriteBatch.Draw(nextRT, Vector2.Zero, Color.White);
                spriteBatch.End();


                qrt.Enqueue(nextRT);
                qrt.Enqueue(currentRT);
                currentRT = qrt.Dequeue();
                nextRT = qrt.Dequeue();
            }
            
            device.SetRenderTarget(currentRT);
            effectLight.CurrentTechnique = effectLight.Techniques["Ambient"];
            effectLight.Parameters["ambient"].SetValue(ambient);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effectLight);
            spriteBatch.Draw(nextRT, Vector2.Zero, Color.White);
            spriteBatch.End();
            

            qrt.Enqueue(currentRT);
            qrt.Enqueue(nextRT);

            return (Texture2D)currentRT;

        }


        void DrawStrip(List<Vector2> list)
        {
            vertices = new VertexPositionColor[list.Count];

            int i = 0;
            foreach (var v in list)
            {
                vertices[i].Position = new Vector3(v.X / screenWidth * 2 - 1, -(v.Y / screenHeight * 2 - 1), 0);//-1..1
                vertices[i].Color = Color.Black;
                i++;
            }
            device.SamplerStates[1] = SamplerState.LinearClamp;
            //device.SamplerStates[0] = SamplerState.LinearClamp;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, (list.Count / 2 - 1) * 2, VertexPositionColor.VertexDeclaration);
        }
    }
}
