using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VolumeRendering
{
    public interface IMouseListener
    {
        void onMouseMove(Vector2 newPos, Vector2 oldPos, Vector2 deltaPos);
        void onButtonPressed(MouseManager.MouseButton button);
        void onButtonReleased(MouseManager.MouseButton button);
        void onWheelScroll(int delta);
    }

    public class MouseManager : Notifier<IMouseListener>
    {
        private MouseState _mouseState;
        private MouseState _prevMouseState;

        public enum MouseButton
        {
            RIGHT,
            MIDDLE,
            LEFT
        }

        public MouseManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(MouseManager), this);
            _mouseState = _prevMouseState = Mouse.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            _prevMouseState = _mouseState;
            _mouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        public override void Notify(IMouseListener l)
        {
            //Check move
            Vector2 oldPos = new Vector2(_prevMouseState.X, _prevMouseState.Y);
            Vector2 newPos = new Vector2(_mouseState.X, _mouseState.Y);
            Vector2 deltaPos = newPos - oldPos;

            if (newPos != oldPos)
                l.onMouseMove(newPos, oldPos, deltaPos);

            //Cehck all buttons
            if (_prevMouseState.LeftButton != _mouseState.LeftButton)
            {
                if (_mouseState.LeftButton == ButtonState.Pressed)
                    l.onButtonPressed(MouseButton.LEFT);
                else
                    l.onButtonReleased(MouseButton.LEFT);
            }
            if (_prevMouseState.MiddleButton != _mouseState.MiddleButton)
            {
                if (_mouseState.MiddleButton == ButtonState.Pressed)
                    l.onButtonPressed(MouseButton.MIDDLE);
                else
                    l.onButtonReleased(MouseButton.MIDDLE);
            }
            if (_prevMouseState.RightButton != _mouseState.RightButton)
            {
                if (_mouseState.RightButton == ButtonState.Pressed)
                    l.onButtonPressed(MouseButton.RIGHT);
                else
                    l.onButtonReleased(MouseButton.RIGHT);
            }

            int deltaScroll = _prevMouseState.ScrollWheelValue - _mouseState.ScrollWheelValue;
            if (deltaScroll != 0)
                l.onWheelScroll(deltaScroll);
        }

        public MouseState State
        {
            get { return _mouseState; }
        }

        public Vector2 GetPosition()
        {
            return new Vector2(_mouseState.X, _mouseState.Y);
        }
    }
}
