using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VolumeRendering
{
    class CameraFirstPerson : Camera, IMouseListener
    {
        MouseManager _mouseManager;
        KeyboardManager _keyboard;

        Vector3 _maxYawPitchRoll = new Vector3(360, 360, 360);
        Vector3 _minYawPitchRoll = new Vector3(-360, -360, -360);
        
        public CameraFirstPerson(Game game, float fov, float zNear, float zFar)
            : base(game, fov, zNear, zFar)
        {
            _keyboard = game.Services.GetService(typeof(KeyboardManager)) as KeyboardManager;
            
            _mouseManager = game.Services.GetService(typeof(MouseManager)) as MouseManager;
            _mouseManager.AddListener(this);
        }

        /// <summary>
        /// Set maximum value for yaw, pitch and roll in [-360,360]
        /// </summary>
        public Vector3 MaxYawPitchRoll
        {
            get { return _maxYawPitchRoll; }
            set { _maxYawPitchRoll = value; }
        }

        /// <summary>
        /// Set minimum value for yaw, pitch and roll in [-360,360]
        /// </summary>
        public Vector3 MinYawPitchRoll
        {
            get { return _minYawPitchRoll; }
            set { _minYawPitchRoll = value; }
        }

        public override void Update(GameTime gameTime)
        {
            float scale = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            scale = 0.001f;
            if (_keyboard.isDown(Keys.Up))
                MoveAlongView(10.0f * scale);                
            if (_keyboard.isDown(Keys.Down))
                MoveAlongView(-10.0f * scale);
            if (_keyboard.isDown(Keys.Left))
                MoveAlongSide(10.0f * scale);
            if (_keyboard.isDown(Keys.Right))
                MoveAlongSide(-10.0f * scale);                                
            base.Update(gameTime);
        }

        /// <summary>
        /// Move camera along its zaxis
        /// </summary>
        /// <param name="dist">Amount to move</param>
        private void MoveAlongView(float dist)
        {
            Vector3 forward = Vector3.Transform(Vector3.UnitZ, _orientation);
            _position += -forward*dist;            
        }

        /// <summary>
        /// Move camera along its side
        /// </summary>
        /// <param name="dist">Amount to move</param>
        private void MoveAlongSide(float dist)
        {
            Vector3 right = Vector3.Transform(Vector3.UnitX, _orientation);
            _position += -right * dist;
        }

        public void onMouseMove(Vector2 newPos, Vector2 oldPos, Vector2 deltaPos)
        {
            if (_game.IsMouseVisible)
                return;
            
            UpdateYaw(-deltaPos.X);
            UpdatePitch(-deltaPos.Y);
        }

        public void onButtonPressed(MouseManager.MouseButton button)
        {
        }

        public void onButtonReleased(MouseManager.MouseButton button)
        {
            if (button == MouseManager.MouseButton.RIGHT)
                _game.IsMouseVisible = !_game.IsMouseVisible;            
        }

        public void onWheelScroll(int delta)
        {
        }

        /// <summary>
        /// Add delta to the current yaw
        /// </summary>
        /// <param name="delta">Delta is added to camera's yaw</param>
        private void UpdateYaw(float delta)
        {
            _yaw += delta;
            if (_yaw >= 360.0f)
                _yaw = 360.0f - _yaw;
            if (_yaw <= -360.0f)
                _yaw = 360.0f + _yaw;
            _yaw = MathHelper.Clamp(_yaw, _minYawPitchRoll.X, _maxYawPitchRoll.X);
        }

        /// <summary>
        /// Add delta to the current pitch
        /// </summary>
        /// <param name="delta">Delta is added to camera's pitch</param>
        private void UpdatePitch(float delta)
        {
            _pitch += delta;
            if (_pitch >= 360.0f)
                _pitch = 360.0f - _pitch;
            if (_pitch <= -360.0f)
                _pitch = 360.0f + _pitch;
            _pitch = MathHelper.Clamp(_pitch, _minYawPitchRoll.Y, _maxYawPitchRoll.Y);            
        }
    }
}
