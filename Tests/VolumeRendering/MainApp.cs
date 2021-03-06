using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
//using System.Drawing;
//using System.ComponentModel;
//using System.Windows.Forms;
using Shaghal;

namespace VolumeRendering
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainApp : Microsoft.Xna.Framework.Game, IMouseListener, IKeyboardListener
    {        
        GraphicsDeviceManager graphics;
        private Volume<byte> _volume;
        GradientEditor _gradEditor;
        float _oldMax = 1;
        float _oldMin = 0;
        VolumeRaycaster _vrc;
        Gradient _gradient;

        Camera _camera;

        MouseManager _mouseManager;
        KeyboardManager _keyboardManager;

        SpriteBatch _spriteBatch;
        SpriteFont _fontDebug;

        Vector2 curPos;

        public MainApp()
        {
            Shaghal.GlobalSystem.Init(this);            
            graphics = new GraphicsDeviceManager(this);            
            Content.RootDirectory = "Content";
                       
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Draw the scene
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_fontDebug, curPos.ToString(), Vector2.Zero, Color.Black);
            _spriteBatch.End();           
        }

        /// <summary>
        /// Initilize various objects
        /// </summary>
        protected override void Initialize()
        {
            _keyboardManager = new KeyboardManager(this);
            Components.Add(_keyboardManager);
            _keyboardManager.AddListener(this);

            _mouseManager = new MouseManager(this);
            Components.Add(_mouseManager);
            _mouseManager.AddListener(this);

            //_camera = new CameraFirstPerson(this, 45, 0.01f, 100.0f);
            _camera = new CameraTrackBall(this, 45, 0.01f, 100.0f);
            _camera.Position = new Vector3(0, 0, 5);
            Components.Add(_camera);

            createVolumeRaycaster();
            _gradEditor = new GradientEditor();            

            _gradient = new Gradient();
            GradientSegment seg = new GradientSegment();
            seg.Left = 0.0f;
            seg.Middle = 0.5f;
            seg.Right = 1.0f;
            seg.LeftColor = new Vector4(1, 0, 0, 1.0f);
            seg.RightColor = new Vector4(1, 0, 0, 1.0f);
            seg.ColType = GradientSegment.ColorType.HSV_CCW;
            _gradient.AddSegment(seg);

            //seg.LookUpTable = palette.Colors;
            //_gradient.AddSegment(seg);

            //seg = new GradientSegment();
            //seg.Left = 0;
            //seg.Middle = 0.333f/2.0f;
            //seg.Right = 0.333f;
            //seg.LeftColor = ColorHelper.HsvToRgb(0, 1, 0, 1.0f);
            //seg.RightColor = ColorHelper.HsvToRgb(0, 1, 0.8f, 1.0f);
            //seg.ColType = GradientSegment.ColorType.HSV_CW;
            //_gradient.AddSegment(seg);

            //seg = new GradientSegment();
            //seg.Left = 0.333f;
            //seg.Middle = 0.333f+(1.0f-0.333f) / 2.0f;
            //seg.Right = 1.0f;
            //seg.LeftColor = ColorHelper.HsvToRgb(0, 1, 0.8f, 1.0f);
            //seg.RightColor = ColorHelper.HsvToRgb(0.25f * 360.0f, 0, 1.0f, 1);
            //seg.ColType = GradientSegment.ColorType.HSV_CW;
            //_gradient.AddSegment(seg);
  
            _gradEditor.Gradient = _gradient;

            Color[] cols = _gradient.GetColors(256);            
            _vrc.TransferFunction = cols;            

            TransferFunction tf = new TransferFunction();
            tf.Colors = cols;
            tf.preIntegrate();
            _vrc.TransferFunctionPreInt = tf;

            _gradEditor.Show();

            VolumeRaycasterSettings raySettings = new VolumeRaycasterSettings(_vrc);
            raySettings.Show();

            base.Initialize();
        }

        /// <summary>
        /// Initialize volume rendering code
        /// </summary>
        private void createVolumeRaycaster()
        {
            //DatReader datReader = new DatReader("C:/Users/gandalf/Documents/devel/csharp/VolumeRendering/VolumeRendering/Content/Volumes/Head256.dat");
            DatReader datReader = new DatReader("D:/Documents/Devel/shagal/Tests/VolumeRendering/Content/Volumes/Head256.dat");
            _volume = new Volume<byte>(datReader.Data, datReader.Dim);
            //_volume = new Volume<byte>(createSphere(), new Dim3(256, 256, 256));
            _vrc = new VolumeRaycaster(this, _volume);
            _vrc.Camera = _camera;            
            _vrc.StepSize = 1.0f/256.0f;
            _vrc.PreIntegration = false;

            float maxDim = MathHelper.Max(datReader.Dim.Width, datReader.Dim.Height);
            maxDim = MathHelper.Max(maxDim, datReader.Dim.Depth);
            Vector3 normalizedDim = new Vector3(datReader.Dim.Width, datReader.Dim.Height, datReader.Dim.Depth)/maxDim;
            _vrc.bbox = new BoundingBox(-normalizedDim, normalizedDim);
            Components.Add(_vrc);
        }

        private byte[] createSphere()
        {
            int offs = 0;
            byte[] data = new byte[256*256*256];
            Vector3 center = new Vector3(128, 128, 128);
            for(int k = 0; k < 256; k++)
                for(int j = 0; j < 256; j++)
                    for (int i = 0; i < 256; i++)
                    {                        
                        Vector3 pos = new Vector3(i, j, k);
                        Vector3 radius = pos - center;
                        float d = 255.0f*radius.Length() / center.Length();
                        data[offs++] = (byte)d;
                    }
            return data;
        }

        /// <summary>
        /// Load content
        /// </summary>
        protected override void LoadContent()
        {
            _fontDebug = Content.Load<SpriteFont>("SpriteFontDebug");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// Free content
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Update Game state
        /// </summary>
        /// <param name="gameTime"></param>
        
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
            
            //System.Windows.Forms.Control f = System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            //f.Location = new System.Drawing.Point(10, 10);             
        }

        /// <summary>
        /// Handle keys
        /// </summary>
        private void UpdateInput()
        {
            KeyboardManager kbManager = Services.GetService(typeof(KeyboardManager)) as KeyboardManager;
            KeyboardState newState = Keyboard.GetState();

       
        }

        public void onMouseMove(Vector2 newPos, Vector2 oldPos, Vector2 deltaPos)
        {
            curPos = newPos;
        }

        public void onButtonPressed(MouseManager.MouseButton button)
        {
        }

        public void onButtonReleased(MouseManager.MouseButton button)
        {
        }

        public void onWheelScroll(int delta)
        {
        }

        public void onKeyUp(Keys key)
        {
            if (key == Keys.Escape)
                Exit();
        }

        public void onKeyDown(Keys key)
        {
        }

    }
}
