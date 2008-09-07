using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shaghal
{
    public class VolumeRaycaster : DrawableGameComponent
    {
        public VolumeRaycaster(Game game, Volume<byte> volume) : base(game)
        {
            _bbox = new BoundingBox(new Vector3(-1), new Vector3(1));
            _volume = volume;
            _alpha = 0;

            IGraphicsDeviceService graphicsservice = Game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            _graphicDevice = graphicsservice.GraphicsDevice;
            _volTexture = new Texture3D(_graphicDevice, _volume.Dim.Width, _volume.Dim.Height, _volume.Dim.Depth, 1, TextureUsage.None, SurfaceFormat.Alpha8);
            _tfTexture = new Texture2D(_graphicDevice, 256, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            _tfPreIntTexture = new Texture2D(_graphicDevice, 256, 256, 1, TextureUsage.None, SurfaceFormat.Color);                    
        }

        public Color[] TransferFunction
        {
            set 
            {
                _graphicDevice.Textures[0] = null;
                _graphicDevice.Textures[1] = null;
                _graphicDevice.Textures[2] = null;
                _tfTexture.SetData<Color>(value);
            }
        }

        public TransferFunction TransferFunctionPreInt
        {
            set
            {
                _graphicDevice.Textures[0] = null;
                _graphicDevice.Textures[1] = null;
                _graphicDevice.Textures[2] = null;
                _graphicDevice.Textures[3] = null;
                _tfPreIntTexture.SetData<Color>(value.PreIntTable);
            }
        }

        /// <summary>
        /// Define step size. Default is 1/256.
        /// </summary>
        public float StepSize
        {
            set { _stepSize = value; }
            get { return _stepSize; }
        }

        /// <summary>
        /// If true use preintegration. Default is true.
        /// </summary>
        public bool PreIntegration
        {
            set { _preint = value; }
            get { return _preint; }
        }

        public Camera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }

        public BoundingBox bbox
        {
            get { return _bbox; }
            set { _bbox = value; }
        }

        public Volume<byte> volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        protected override void LoadContent()
        {
            effect = GlobalSystem.Content.Load<Effect>("VolumeRayCast");
            effect.Parameters["VolTexture"].SetValue(_volTexture);
            effect.Parameters["TransferFunction"].SetValue(_tfTexture);
        }

        public override void Initialize()
        {
            _cube = new Cube(_graphicDevice, bbox);
            _volTexture.SetData<byte>(_volume.Data);

            _spriteBatch = new SpriteBatch(_graphicDevice); ;

            base.Initialize();
        }
        
        public override void Draw(GameTime gameTime)
        {
            string technique = _preint?"VolumeRayCastPreIntegration":"VolumeRayCastNoPreIntegration";
            effect.CurrentTechnique = effect.Techniques[technique];

            setupTexGenMatrix();

            Matrix world = Matrix.CreateFromYawPitchRoll(_alpha, 0*MathHelper.Pi / 2.0f, 0);
            //Matrix world = Matrix.CreateFromYawPitchRoll(MathHelper.Pi / 2, MathHelper.Pi / 2, 0);
            //Matrix world = Matrix.Identity;
            Matrix view = _camera.View;
            Matrix wvInv = Matrix.Invert(world * view);
            Vector4 camPosTexSpace = new Vector4(wvInv.Translation, 1);
            camPosTexSpace = Vector4.Transform(camPosTexSpace, _texGenMatrix);

            effect.Parameters["StepSize"].SetValue(_stepSize);
            effect.Parameters["VolTexture"].SetValue(_volTexture);
            effect.Parameters["TransferFunction"].SetValue(_tfTexture);
            effect.Parameters["TransferFunctionPreInt"].SetValue(_tfPreIntTexture);

            effect.Parameters["Alpha"].SetValue(_alpha);
            effect.Parameters["CamPosTexSpace"].SetValue(camPosTexSpace);
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(_camera.View);            
            effect.Parameters["Projection"].SetValue(_camera.Projection);

            _graphicDevice.RenderState.CullMode = CullMode.CullClockwiseFace;            

            _cube.Begin();
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                _cube.Draw();
                pass.End();
            }
            effect.End();
            _cube.End();

            debugTextures();

            base.Draw(gameTime);
        }

        private void debugTextures()
        {
            Rectangle destinationRectangle = new Rectangle(0, 0, 256, 256/6);

            _spriteBatch.Begin();
            Vector2 pos = new Vector2(0, 0);
            _spriteBatch.Draw(_tfTexture, destinationRectangle, Color.White);
            _spriteBatch.Draw(_tfPreIntTexture, new Vector2(0, 256), Color.White);
            _spriteBatch.End();
        }

        private void setupTexGenMatrix()
        {           
            Vector3 size = _bbox.Max - _bbox.Min;
            _texGenMatrix = new Matrix(1.0f / size.X, 0, 0, 0,
                                       0, 1.0f / size.Y, 0, 0,
                                       0, 0, 1.0f / size.Z, 0,
                                       -_bbox.Min.X / size.X, -_bbox.Min.Y / size.Y, -_bbox.Min.Z / size.Z, 1);
            effect.Parameters["TexGenMatrix"].SetValue(_texGenMatrix);
        }

        public override void Update(GameTime gameTime)
        {
            _alpha += 1.0f*(float)gameTime.ElapsedGameTime.TotalSeconds;            
            //float offset = 30*(float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (_alpha >= 256)
            //{
            //    _alpha = 0;
            //}
            //_alpha += offset;
                
            base.Update(gameTime);
        }

        private Camera _camera;

        private GraphicsDevice _graphicDevice;
        private BoundingBox _bbox;
        
        private Volume<byte> _volume;
        private Texture3D _volTexture;
        private Texture2D _tfTexture;
        private Texture2D _tfPreIntTexture;

        private Effect effect;

        private Cube _cube;

        private Matrix _texGenMatrix;

        private float _alpha;

        SpriteBatch _spriteBatch;

        private float _stepSize = 1.0f/256.0f;

        private bool _preint = true;
    }
}

