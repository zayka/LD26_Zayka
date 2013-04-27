using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace LD26_Zayka
{
    class InputState
    {
        KeyboardState kstate;
        MouseState mstate;
        KeyboardState oldkstate;
        MouseState oldmstate;


        static InputState instance;

        private InputState()
        {
            oldkstate = kstate = Keyboard.GetState();
            oldmstate = mstate = Mouse.GetState();            
        }


        public static InputState GetInput()
        {
            if (instance==null) instance = new InputState();
            return instance;
        }

        public void Update()
        {
            oldkstate = kstate;
            oldmstate = mstate;
            kstate = Keyboard.GetState();
            mstate = Mouse.GetState();
        }

        public bool IsKeyPressed(Keys key)
        {
            return kstate.IsKeyDown(key);
        }

        public bool IsLeftButtonClick()
        {
            return (mstate.LeftButton == ButtonState.Pressed) && (oldmstate.LeftButton != ButtonState.Pressed);
        }

        public bool IsRightButtonClick()
        {
            return (mstate.RightButton == ButtonState.Pressed) && (oldmstate.RightButton != ButtonState.Pressed);
        }
        public bool IsLeftButtonPress()
        {
            return mstate.LeftButton == ButtonState.Pressed;
        }

        public bool IsLeftButtonRelease()
        {
            return (mstate.LeftButton == ButtonState.Released) && (oldmstate.LeftButton != ButtonState.Released);
        }
        public bool IsRightButtonPress()
        {
            return mstate.RightButton == ButtonState.Pressed;
        }

        public bool IsNewKeyPressed(Keys key)
        {
            return (kstate.IsKeyDown(key) && !oldkstate.IsKeyDown(key));
        }

        public bool isHover(Rectangle rect)
        {
            Vector2 currentMouse =MouseVector();
            return currentMouse == Vector2.Clamp(currentMouse, new Vector2(rect.Left, rect.Top), new Vector2(rect.Right,rect.Bottom));
        }
        
        public Vector2 MouseVector()
        {           
            return Vector2.Transform(new Vector2(mstate.X, mstate.Y), Cnt.game.Camera.InvView);
        }

        public bool isMouseScrollUp()
        {            
            return mstate.ScrollWheelValue - oldmstate.ScrollWheelValue < 0;            
        }

        public bool isMouseScrollDown()
        {
            return mstate.ScrollWheelValue - oldmstate.ScrollWheelValue > 0;
        }

        public Vector2 MouseLastDist()
        {
            return new Vector2(mstate.X, mstate.Y)-new Vector2(oldmstate.X, oldmstate.Y);
        }


        public bool IsKeyReleased(Keys key)
        {
            return (kstate.IsKeyUp(key) && !oldkstate.IsKeyUp(key));
        }

    }
}
