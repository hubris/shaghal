using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shaghal
{
    public class Camera : GameComponent
    {
        protected Game _game;
        protected Matrix _view = new Matrix();
        protected Matrix _projection = new Matrix();
        protected GraphicsDevice _graphics;

        protected float _zNear;
        protected float _zFar;
        protected float _fov;

        protected Vector3 _position = new Vector3(0.0f, 0.0f, 1.0f);
        protected Vector3 _target = Vector3.Zero;
        protected Vector3 _up = Vector3.Up;

        protected float _yaw = 0;
        protected float _pitch = 0;
        protected float _roll = 0;

        protected Vector2 _screenCenter;

        protected Quaternion _orientation = new Quaternion(0, 0, 0, 1);

        public Matrix Projection
        {
            get { return _projection; }
        }

        public Matrix View
        {
            get { return _view; }
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector3 Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public Camera(Game game, float fov, float zNear, float zFar)
            : base(game)
        {
            IGraphicsDeviceService graphicsservice = Game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            _graphics = graphicsservice.GraphicsDevice;
            _zNear = zNear;
            _zFar = zFar;
            _fov = fov;
            _game = game;
        }

        public override void Initialize()
        {
            base.Initialize();
            createMatrix();
            Rectangle rec = _game.Window.ClientBounds;
            _screenCenter = new Vector2(rec.Width / 2, rec.Height / 2);
        }

        protected virtual void createProjectionMatrix()
        {
            float aspectRatio = (float)_graphics.Viewport.Width / (float)_graphics.Viewport.Height;
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), aspectRatio,
                                                              _zNear, _zFar);
        }

        protected virtual void createViewMatrix()
        {
            Quaternion qPitch = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(_pitch));
            Quaternion qYaw = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(_yaw));
            _orientation = qYaw*qPitch;

            _orientation.Normalize();
            _view = Matrix.CreateFromQuaternion(_orientation);
            _view.Translation = _position;
            Matrix.Invert(ref _view, out _view);
        }

        private void createMatrix()
        {
            createProjectionMatrix();
            createViewMatrix();
        }

        public override void Update(GameTime gameTime)
        {
            createMatrix();
            base.Update(gameTime);
        }
    }
}
