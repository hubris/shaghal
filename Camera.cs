using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VolumeRendering
{
    class Camera : GameComponent
    {        
        private Matrix _view;
        private Matrix _projection;
        private GraphicsDevice _graphics;

        private float _zNear;
        private float _zFar;
        private float _fov;

        protected Vector3 _position = new Vector3(0.0f, 0.0f, 1.0f);
        protected Vector3 _target = Vector3.Zero;
        protected Vector3 _up = Vector3.Up;

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
        }

        public override void Initialize()
        {
            base.Initialize();
            createMatrix();
        }

        private void createMatrix()
        {
            float aspectRatio = (float)_graphics.Viewport.Width / (float)_graphics.Viewport.Height;
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), aspectRatio,
                                                              _zNear, _zFar);
            _view = Matrix.CreateLookAt(_position, _target, _up);
        }

        public override void Update(GameTime gameTime)
        {
            createMatrix();
            base.Update(gameTime);
        }
    }
}
