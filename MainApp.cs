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
            seg.LeftColor = new Vector4(1, 0, 0, 0.9f);
            seg.RightColor = new Vector4(1, 0, 0, 0.9f);
            seg.ColType = GradientSegment.ColorType.HSV_CW;
            _gradient.AddSegment(seg);

            seg = new GradientSegment();
            seg.Left = 0;
            seg.Middle = 0.2f;
            seg.Right = 0.8f;
            seg.LeftColor = new Vector4(1, 0, 0, 1.0f);
            seg.RightColor = new Vector4(1, 0, 0, 1.0f);
            seg.ColType = GradientSegment.ColorType.HSV_CCW;
            //_gradient.AddSegment(seg);
            //_gradient.Min = 0.5f;

            _gradEditor.Gradient = _gradient;

            Color[] cols = _gradient.GetColors(256);            
            _vrc.TransferFunction = cols;            

            TransferFunction tf = new TransferFunction();
            tf.Colors = cols;
            tf.preIntegrate();
            _vrc.TransferFunctionPreInt = tf;

            base.Initialize();
        }

        private void createSphere()
        {
            DatReader datReader = new DatReader("C:/Users/gandalf/Documents/devel/csharp/VolumeRendering/VolumeRendering/Content/Volumes/head256.dat");
            datReader.Read();
            _volume = new Volume<byte>(datReader.Data, datReader.Dim);
        }

        protected override void LoadContent()
        {
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

                TransferFunction tf = new TransferFunction();
                tf.Colors = cols;
                tf.preIntegrate();
                _vrc.TransferFunctionPreInt = tf;
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
