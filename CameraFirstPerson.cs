using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VolumeRendering
{
    class CameraFirstPerson : Camera, IKeyboardListener
    {
        KeyboardManager _keyboard;

        public CameraFirstPerson(Game game, float fov, float zNear, float zFar)
            : base(game, fov, zNear, zFar)
        {
            _keyboard = game.Services.GetService(typeof(KeyboardManager)) as KeyboardManager;
            _keyboard.AddListener(this);
        }

        public override void Update(GameTime gameTime)
        {
            float scale = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            scale = 0.001f;
            if(_keyboard.isDown(Keys.Up))
                _position.Z += 10.0f * scale;
            if (_keyboard.isDown(Keys.Down))
                _position.Z -= 10.0f * scale;
            base.Update(gameTime);
        }

        public void onKeyUp(Keys key)
        {

        }

        public void onKeyDown(Keys key)
        {
        }
    }
}
