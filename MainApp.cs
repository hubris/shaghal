using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace VolumeRendering
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainApp : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState _oldState;
        private Volume<byte> _volume;
        GradientEditor _gradEditor;
        float _oldMax = 1;
        float _oldMin = 0;
        VolumeRaycaster _vrc;
        Gradient _gradient;

        public MainApp()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            createSphere();
            _vrc = new VolumeRaycaster(this, _volume);            
            Components.Add(_vrc);
            _gradEditor = new GradientEditor();
            _gradEditor.Show();


            _gradient = new Gradient();
            GradientSegment seg = new GradientSegment();
            seg.Left = 0.0f;
            seg.Middle = 0.5f;
            seg.Right = 1.0f;
            seg.LeftColor = new Vector4(1, 1, 1, 1.0f);
            seg.RightColor = new Vector4(0, 0, 0, 1.0f);
            //seg.ColType = GradientSegment.ColorType.HSV_CW;
            _gradient.AddSegment(seg);

            seg = new GradientSegment();
            seg.Left = 0;
            seg.Middle = 0.2f;
            seg.Right = 0.8f;
            seg.LeftColor = new Vector4(1, 0, 0, 1.0f);
            seg.RightColor = new Vector4(1, 0, 0, 1.0f);
            seg.ColType = GradientSegment.ColorType.HSV_CCW;
            //_gradient.AddSegment(seg);
            _gradient.Min = 0.5f;

            _gradEditor.Gradient = _gradient;

            Color[] cols = _gradient.GetColors(256);            
            _vrc.TransferFunction = cols;            

            base.Initialize();
        }

        private void createSphere()
        {
            //Dim3 dim = new Dim3(256, 256, 256);
            //Vector3 center = new Vector3(dim.Width / 2, dim.Height / 2, dim.Depth / 2);            
            //byte[] data = new byte[dim.Width * dim.Height * dim.Depth];
            //float maxLen = center.Length();
            //for (int k = 0; k < dim.Depth; k++)
            //    for (int j = 0; j < dim.Height; j++)
            //        for (int i = 0; i < dim.Width; i++)
            //        {
            //            Vector3 pos = new Vector3(i, j, k);
            //            Vector3 radius = (pos - center);
            //            float d = radius.Length()/maxLen;
            //            data[i + (j + k*dim.Height) * dim.Width] = (byte)(d*255);
            //        }
            //_volume = new Volume<byte>(data, dim);

            DatReader datReader = new DatReader("C:/Users/gandalf/Documents/devel/csharp/VolumeRendering/VolumeRendering/Content/Volumes/bonsai.dat");
            datReader.Read();
            _volume = new Volume<byte>(datReader.Data, datReader.Dim);
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdateInput();

            if (_oldMax != _gradient.Max || _oldMin != _gradient.Min)
            {
                Color[] cols = _gradient.GetColors(256);
                _vrc.TransferFunction = cols;
                _oldMax = _gradient.Max;
                _oldMin = _gradient.Min;
            }

            base.Update(gameTime);
        }

        private void UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyUp(Keys.Escape) && _oldState.IsKeyDown(Keys.Escape) )
            {
                this.Exit();
            }            
            _oldState = newState;
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
