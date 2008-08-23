using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VolumeRendering
{
    class VolumeRaycaster : DrawableGameComponent
    {
        public VolumeRaycaster(Game game, Volume<byte> volume) : base(game)
        {
            _bbox = new BoundingBox(new Vector3(-1), new Vector3(1));
            _volume = volume;
            _alpha = 0;
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
            effect = Game.Content.Load<Effect>("VolumeRayCast");
            effect.Parameters["VolTexture"].SetValue(_volTexture);            
        }

        public override void Initialize()
        {
            IGraphicsDeviceService graphicsservice = (IGraphicsDeviceService)Game.Services.GetService(typeof(IGraphicsDeviceService));
            _graphicDevice = graphicsservice.GraphicsDevice;

            _cube = new Cube(_graphicDevice, bbox);
            _volTexture = new Texture3D(_graphicDevice, _volume.Dim.Width, _volume.Dim.Height, _volume.Dim.Depth, 1, TextureUsage.Linear, SurfaceFormat.Alpha8);
            _volTexture.SetData<byte>(_volume.Data);                        
            base.Initialize();
        }
        
        public override void Draw(GameTime gameTime)
        {
            effect.CurrentTechnique = effect.Techniques["VolumeRayCast"];
            setupTexGenMatrix();

            Matrix world = Matrix.CreateFromYawPitchRoll(_alpha, _alpha / 4, _alpha / 2);
            //Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 5.0f), Vector3.Zero,
                                               Vector3.Up);
            Matrix wvInv = Matrix.Invert(world * view);
            Vector4 camPosTexSpace = new Vector4(wvInv.Translation, 1);
            camPosTexSpace = Vector4.Transform(camPosTexSpace, _texGenMatrix);

            effect.Parameters["Alpha"].SetValue(_alpha);
            effect.Parameters["CamPosTexSpace"].SetValue(camPosTexSpace);
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);            
            effect.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),
                (float)_graphicDevice.Viewport.Width / (float)_graphicDevice.Viewport.Height,
                1.0f, 100.0f
                ));

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

            base.Draw(gameTime);
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
            _alpha += 0.1f*(float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        private GraphicsDevice _graphicDevice;
        private BoundingBox _bbox;
        
        private Volume<byte> _volume;
        private Texture3D _volTexture;

        private Effect effect;

        private Cube _cube;

        private Matrix _texGenMatrix;

        private float _alpha;
    }
}

