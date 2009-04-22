using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shaghal
{
    public class CameraTrackBall : Camera, IMouseListener
    {
        private MouseManager _mouseManager;
        private KeyboardManager _keyboard;

        private Vector3 _maxYawPitchRoll = new Vector3(360, 360, 360);
        private Vector3 _minYawPitchRoll = new Vector3(-360, -360, -360);

        private Vector3 _prevPosition = new Vector3();
        private bool _isDragging = false;

        private Quaternion _curOrientation = new Quaternion();
        private float _zoom = 1.0f;

        public CameraTrackBall(Game game, float fov, float zNear, float zFar)
            : base(game, fov, zNear, zFar)
        {
            _keyboard = game.Services.GetService(typeof(KeyboardManager)) as KeyboardManager;
            
            _mouseManager = game.Services.GetService(typeof(MouseManager)) as MouseManager;
            _mouseManager.AddListener(this);
            _curOrientation = _orientation;
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
            float elapsedTime = gameTime.ElapsedGameTime.Milliseconds/1000.0f;

            float speed = 0.1f;

            if (_keyboard.isDown(Keys.LeftShift))
                speed /= 8;

            if (_keyboard.isDown(Keys.Z))
                MoveAlongView(speed);                
            if (_keyboard.isDown(Keys.S))
                MoveAlongView(-speed);
            if (_keyboard.isDown(Keys.Q))
                MoveAlongSide(speed);
            if (_keyboard.isDown(Keys.D))
                MoveAlongSide(-speed);                                
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

        private Vector3 projectOnSphere(Vector2 coord)
        {
            Rectangle rec = _game.Window.ClientBounds;
            Vector3 res = new Vector3(coord.X/_screenCenter.X, 
                                      coord.Y/_screenCenter.Y, 
                                      0);
            res -= new Vector3(1, 1, 0);
            res.Y = -res.Y;
            res.Z = 1 - res.LengthSquared();
            res.Z = res.Z > 0 ? (float)Math.Sqrt(res.Z) : 0;
            res.Normalize();
            
            return res;
        }

        public void onMouseMove(Vector2 newPos, Vector2 oldPos, Vector2 deltaPos)
        {
            if (!_isDragging)
                return;

            Vector3 pos = projectOnSphere(newPos);

            Vector3 axis = Vector3.Cross(_prevPosition, pos);
            
            float tmp = (float)Math.Sqrt(2*(1+Vector3.Dot(_prevPosition, pos)));
            Quaternion delta = new Quaternion(axis/tmp, tmp/2);
            _orientation = delta*_curOrientation;
            _orientation.Normalize();
        }

        protected override void createViewMatrix()
        {            
            _view = Matrix.CreateFromQuaternion(_orientation);
            _view.Translation = -_position;
        }

        protected override void createProjectionMatrix()
        {
            float aspectRatio = (float)_graphics.Viewport.Width / (float)_graphics.Viewport.Height;
            float fov = MathHelper.Clamp(_fov * _zoom, 0, 179);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectRatio,
                                                              _zNear, _zFar);
        }

        private bool isMouseInWindow()
        {
            MouseState mouseState = Mouse.GetState();
            return mouseState.X >= 0 && mouseState.Y >= 0 &&
                   mouseState.X < _graphics.Viewport.Width &&
                   mouseState.Y < _graphics.Viewport.Height;                
        }

        public void onButtonPressed(MouseManager.MouseButton button)
        {
            if (!isMouseInWindow())
                return;

            MouseState mouseState = Mouse.GetState();
            if (button == MouseManager.MouseButton.LEFT)
            {
                _isDragging = true;
                _prevPosition = projectOnSphere(new Vector2(mouseState.X, mouseState.Y));
            }
        }

        public void onButtonReleased(MouseManager.MouseButton button)
        {
            if (button == MouseManager.MouseButton.LEFT)
            {
                _isDragging = false;
                _curOrientation = _orientation;
            }
        }

        public void onWheelScroll(int delta)
        {
            if (delta > 0)
                _zoom *= 1.1f;
            else
                _zoom /= 1.1f;
        }
    }
}
