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
            _alpha = 1.0f;

            IGraphicsDeviceService graphicsservice = Game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            _graphicDevice = graphicsservice.GraphicsDevice;
            _volTexture = new Texture3D(_graphicDevice, _volume.Dim.Width, _volume.Dim.Height, _volume.Dim.Depth, 1, TextureUsage.None, SurfaceFormat.Alpha8);
            _tfTexture = new Texture2D(_graphicDevice, 256, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            _tfPreIntTexture = new Texture2D(_graphicDevice, 256, 256, 1, TextureUsage.None, SurfaceFormat.Color);

            Viewport viewport = _graphicDevice.Viewport;
            _entryPointRt = new RenderTarget2D(_graphicDevice, viewport.Width, viewport.Height, 1, SurfaceFormat.Vector4);
            _exitPointRt = new RenderTarget2D(_graphicDevice, viewport.Width, viewport.Height, 1, SurfaceFormat.Vector4);
            _tempRt = new RenderTarget2D(_graphicDevice, viewport.Width, viewport.Height, 1, SurfaceFormat.Vector4);
        }

        public Color[] TransferFunction
        {
            set 
            {
                _graphicDevice.Textures[0] = null;
                _graphicDevice.Textures[1] = null;
                _graphicDevice.Textures[2] = null;
                _graphicDevice.Textures[3] = null;
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
                _graphicDevice.Textures[4] = null;
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
            _effectGenRay = GlobalSystem.Content.Load<Effect>("EntryExitPointGen");
            _effectClip = GlobalSystem.Content.Load<Effect>("EntryPointClip");
            _effect = GlobalSystem.Content.Load<Effect>("VolumeRayCast");
            _effect.Parameters["VolTexture"].SetValue(_volTexture);
            _effect.Parameters["TransferFunction"].SetValue(_tfTexture);
        }

        public override void Initialize()
        {
            _cube = new Cube(_graphicDevice, bbox);
            _volTexture.SetData<byte>(_volume.Data);

            _spriteBatch = new SpriteBatch(_graphicDevice);            

            base.Initialize();
        }

        private void setupShaderParameters()
        {
            //Matrix world = Matrix.CreateFromYawPitchRoll(_alpha, 0 * MathHelper.Pi / 2.0f, 0);
            Matrix world = Matrix.CreateFromYawPitchRoll(MathHelper.Pi / 2, MathHelper.Pi / 2, 0);
            //Matrix world = Matrix.Identity;
            Matrix view = _camera.View;
            Matrix wvInv = Matrix.Invert(world * view);
            Vector4 camPosTexSpace = new Vector4(wvInv.Translation, 1);
            camPosTexSpace = Vector4.Transform(camPosTexSpace, _texGenMatrix);
            Matrix mvp = world * _camera.View * _camera.Projection;
            Matrix mvpi = Matrix.Invert(mvp);

            _effect.Parameters["StepSize"].SetValue(_stepSize);
            _effect.Parameters["VolTexture"].SetValue(_volTexture);
            _effect.Parameters["TransferFunction"].SetValue(_tfTexture);
            _effect.Parameters["TransferFunctionPreInt"].SetValue(_tfPreIntTexture);

            _effect.Parameters["Alpha"].SetValue(_alpha);
            _effect.Parameters["World"].SetValue(world);
            _effect.Parameters["View"].SetValue(_camera.View);
            _effect.Parameters["Projection"].SetValue(_camera.Projection);
            _effect.Parameters["TexGenMatrix"].SetValue(_texGenMatrix);

            Vector4 deltaTex = new Vector4(1.0f/_volume.Dim.Width, 1.0f/_volume.Dim.Height, 1.0f/_volume.Dim.Depth, 0);
            _effect.Parameters["DeltaTex"].SetValue(deltaTex);

            _effect.Parameters["LightDir"].SetValue(new Vector3(0, 0, 1));

            _effectClip.Parameters["MVPI"].SetValue(mvpi);
            _effectClip.Parameters["TexGenMatrix"].SetValue(_texGenMatrix);

            _effectGenRay.Parameters["MVP"].SetValue(mvp);
            _effectGenRay.Parameters["TexGenMatrix"].SetValue(_texGenMatrix);

            Vector4 a = new Vector4(0, 0, 0, 1);
            a = Vector4.Transform(a, mvpi);
            a.X /= a.W;
            a.Y /= a.W;
            a.Z /= a.W;


            float density = _volume.Dim.Depth / (_bbox.Max.Z - _bbox.Min.Z);
            float samplingRate = _stepSize * _volume.Dim.Depth;
            _effect.Parameters["SamplingRate"].SetValue(samplingRate);
        }

        public void GenerateEntryPoint()
        {
            _effectGenRay.CurrentTechnique = _effectGenRay.Techniques["VolumeRayCastGenerateRay"];

            setupTexGenMatrix();
            setupShaderParameters();

            RenderTarget oldTarget = _graphicDevice.GetRenderTarget(0);
            
            for (int i = 0; i < 2; i++)
            {
                _graphicDevice.SetRenderTarget(0, (i % 2 == 0) ? _tempRt : _exitPointRt);                
                _graphicDevice.Clear(ClearOptions.Target, new Color(0, 0, 0, 0), 0, 0);
                
                _cube.Begin();
                _effectGenRay.Begin();
                _effectGenRay.CurrentTechnique.Passes[i].Begin();
                _cube.Draw();
                _effectGenRay.CurrentTechnique.Passes[i].End();
                _effectGenRay.End();
                _cube.End();
            }
            
            _graphicDevice.SetRenderTarget(0, _entryPointRt);

            _graphicDevice.Clear(ClearOptions.Target, new Color(0, 0, 0, 0), 0, 0);
            _graphicDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            Texture2D exitTex = _exitPointRt.GetTexture();
            _effectClip.Parameters["ExitPointTexture"].SetValue(exitTex);

            //Fill holes generated by near plane clipping            
            ApplyFullScreenEffect(_effectClip, _tempRt.GetTexture());
            
            _graphicDevice.SetRenderTarget(0, oldTarget as RenderTarget2D);
        }

        public void ApplyFullScreenEffect(Effect effect, Texture2D tex)
        {
            Viewport viewport = _graphicDevice.Viewport;
            effect.Begin();            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                _spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                pass.Begin();
                Rectangle destinationRectangle = new Rectangle(0, 0, viewport.Width, viewport.Height);
                _spriteBatch.Draw(tex, destinationRectangle, Color.White);
                _spriteBatch.End();
                pass.End();
            }
            effect.End();  
        }

        public override void Draw(GameTime gameTime)
        {
            GenerateEntryPoint();

            string technique = _preint ? "VolumeRayCastPreIntegration":"VolumeRayCastNoPreIntegration";
            _effect.CurrentTechnique = _effect.Techniques[technique];

            Texture2D entryPointTexture = _entryPointRt.GetTexture();
            _effect.Parameters["ExitPointTexture"].SetValue(_exitPointRt.GetTexture());

            setupTexGenMatrix();
            setupShaderParameters();

             ApplyFullScreenEffect(_effect, entryPointTexture);
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
        }

        public override void Update(GameTime gameTime)
        {
           // _alpha += 1.0f*(float)gameTime.ElapsedGameTime.TotalSeconds;            
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

        private RenderTarget2D _entryPointRt;
        private RenderTarget2D _exitPointRt;
        private RenderTarget2D _tempRt;

        private Effect _effect;

        private Effect _effectClip;
        private Effect _effectGenRay;

        private Cube _cube;

        private Matrix _texGenMatrix;

        private float _alpha;

        SpriteBatch _spriteBatch;

        private float _stepSize = 1.0f/200.0f;

        private bool _preint = true;
    }
}

