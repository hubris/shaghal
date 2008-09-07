using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shaghal
{
    public interface IKeyboardListener
    {
        void onKeyUp(Keys key);
        void onKeyDown(Keys key);
    }

    public class KeyboardManager : Notifier<IKeyboardListener>
    {
        private KeyboardState _keyboardState;
        private KeyboardState _prevKeyboardState;

        public KeyboardManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(KeyboardManager), this);
            _prevKeyboardState = Keyboard.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            _prevKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// Check if a button has just been pressed
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>True if button has just been pressed</returns>
        public bool pressed(Keys key)
        {
            return !_prevKeyboardState.IsKeyDown(key) && _keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Check if a button has just been released
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>True if button has just been released</returns>
        public bool released(Keys key)
        {
            return _prevKeyboardState.IsKeyDown(key) && !_keyboardState.IsKeyDown(key);
        }
        
        /// <summary>
        /// Check if key is up
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>True if key is up</returns>
        public bool isUp(Keys key)
        {
            return _keyboardState.IsKeyUp(key); ;
        }

        /// <summary>
        /// Check if key is down
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>True if key is down</returns>
        public bool isDown(Keys key)
        {
            return _keyboardState.IsKeyDown(key); ;
        }

        public override void Notify(IKeyboardListener l)
        {
            Keys[] keys = _keyboardState.GetPressedKeys();
            Keys[] prevKeys = _prevKeyboardState.GetPressedKeys();
            foreach (Keys k in keys)
            {
                int i = Array.FindIndex<Keys>(prevKeys, n => n == k);
                if (i == -1)
                    l.onKeyDown(k);                
            }

            foreach (Keys k in prevKeys)
            {
                int i = Array.FindIndex<Keys>(keys, n => n == k);
                if (i == -1)
                    l.onKeyUp(k);
            }
        }
    }
}
