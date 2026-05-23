using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Extensions;
using System.DirectoryServices.ActiveDirectory;

namespace ProjectMarworyn
{
    public class Simulation : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        public Simulation()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Create service collection
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddCoreServices();

            // Build the provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //var simulationManager = serviceProvider.GetService<SimulationManager>();

            //simulationManager.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _spriteFont = Content.Load<SpriteFont>("SpriteFont");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_spriteFont,
                "Hello world",
                 Vector2.UnitX,
                Color.White);
            _spriteBatch.End();

            // TODO: Add your drawing code here
            //SamplerState.PointClamp - Used for pixel based fonts

            base.Draw(gameTime);
        }
    }
}