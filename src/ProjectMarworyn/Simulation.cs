using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Extensions;
using ProjectMarworyn.Core.Managers;
using ProjectMarworyn.Core.Models;
using System;

namespace ProjectMarworyn
{
    public class Simulation : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private TimeSpan _accumulator;
        private AppSettings _appSettings;
        private ISimulationManager _simulationManager;
        private GameState _gameState;

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

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Appsettings.json")
                .Build();

            serviceCollection.Configure<AppSettings>(configuration.GetSection("Configuration"));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            _appSettings = serviceProvider.GetService<IOptions<AppSettings>>().Value;
            _gameState = serviceProvider.GetService<GameState>();
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

            var position = 0;
            foreach(var text in _gameState.Text)
            {
                var vector2 = new Vector2(0, position);

                _spriteBatch.DrawString(_spriteFont,
                    text,
                    vector2,
                    Color.White);

                position += 15;
            }
            _spriteBatch.End();

            //TODO: SamplerState.PointClamp - Used for pixel based fonts

            base.Draw(gameTime);
        }
    }
}