using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private Volume<byte> _sphereVolume;

        public MainApp()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            Gradient grad = new Gradient();
            GradientSegment seg = new GradientSegment();
            seg.Left = 0;
            seg.Middle = 0.5f;
            seg.Right = 1.0f;
            seg.LeftColor = Vector4.UnitX;
            seg.RightColor = Vector4.UnitX;
            seg.ColType = GradientSegment.ColorType.HSV_CW;               
            grad.AddSegment(seg);
            grad.Eval(0.5f);
            Color[] cols = grad.GetColors(256);
        }

        protected override void Initialize()
        {
            createSphere();
            VolumeRaycaster vrc = new VolumeRaycaster(this, _sphereVolume);            
            Components.Add(vrc);           
            base.Initialize();
        }

        private void createSphere()
        {
            Dim3 dim = new Dim3(256, 256, 256);
            Vector3 center = new Vector3(dim.Width / 2, dim.Height / 2, dim.Depth / 2);            
            byte[] data = new byte[dim.Width * dim.Height * dim.Depth];
            for (int k = 0; k < dim.Depth; k++)
                for (int j = 0; j < dim.Height; j++)
                    for (int i = 0; i < dim.Width; i++)
                    {
                        Vector3 pos = new Vector3(i, j, k);
                        byte d = (byte)(pos - center).Length();
                        data[i + (j + k*dim.Height) * dim.Width] = d;
                    }
            _sphereVolume = new Volume<byte>(data, dim);
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
