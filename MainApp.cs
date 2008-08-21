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
            int dim = 256;
            Vector3 center = new Vector3(dim / 2, dim / 2, dim / 2);
            byte[, ,] data = new byte[dim, dim, dim];
            for (int k = 0; k < dim; k++)
                for (int j = 0; j < dim; j++)
                    for (int i = 0; i < dim; i++)
                    {
                        Vector3 pos = new Vector3(i, j, k);
                        byte d = (byte)(pos - center).Length();
                        data[i, j, k] = d;
                    }
            _sphereVolume = new Volume<byte>(data);
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
