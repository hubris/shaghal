using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

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
        protected override void LoadContent()
        {
            effect = Game.Content.Load<Effect>("VolumeRayCast");
        }

        public override void Initialize()
        {
            IGraphicsDeviceService graphicsservice = (IGraphicsDeviceService)Game.Services.GetService(typeof(IGraphicsDeviceService));
            _graphicDevice = graphicsservice.GraphicsDevice;

            _cube = new Cube(_graphicDevice, bbox);

            base.Initialize();
        }
        
        public override void Draw(GameTime gameTime)
        {
            effect.CurrentTechnique = effect.Techniques["Technique1"];
            setupTexGenMatrix();

            Matrix world = Matrix.CreateFromYawPitchRoll(_alpha, _alpha / 4, _alpha / 2);            
            Matrix view = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 5.0f), Vector3.Zero,
                                               Vector3.Up);
            Matrix wvInv = Matrix.Invert(world * view);
            Vector4 camPosTexSpace = new Vector4(wvInv.Translation, 1);
            camPosTexSpace = Vector4.Transform(camPosTexSpace, _texGenMatrix);

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

        private GraphicsDevice _graphicDevice;
        private BoundingBox _bbox;
        private Volume<byte> _volume;

        private Effect effect;

        private Cube _cube;

        private Matrix _texGenMatrix;

        private float _alpha;
    }
}
