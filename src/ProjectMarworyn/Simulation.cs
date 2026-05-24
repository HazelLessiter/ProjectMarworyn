using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Extensions;
using ProjectMarworyn.Core.Managers;
using System;

namespace ProjectMarworyn
{
    public class Simulation : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private TimeSpan _accumulator;
        private TimeSpan _dayDuration;
        private AppSettings _appSettings;
        private ISimulationManager _simulationManager;

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
            serviceCollection.ConfigureOptions(new AppSettings());

            var serviceProvider = serviceCollection.BuildServiceProvider();

            _appSettings = serviceProvider.GetService<AppSettings>();
            _simulationManager = serviceProvider.GetService<ISimulationManager>();

            base.Initialize();

            _simulationManager.Start();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _spriteFont = Content.Load<SpriteFont>("SpriteFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _accumulator += gameTime.ElapsedGameTime;
            if (_accumulator >= _appSettings.DayDuration)
            {
                _accumulator -= _appSettings.DayDuration;
                _simulationManager.ProgressDay();
            }

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

            //TODO: SamplerState.PointClamp - Used for pixel based fonts

            base.Draw(gameTime);
        }
    }
}