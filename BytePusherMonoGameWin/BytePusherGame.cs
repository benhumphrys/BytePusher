using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

namespace BytePusherMonoGameWin
{
    public class BytePusherGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _ballTexture;
        private Vector2 _ballPosition;
        private float _ballSpeed;

        public BytePusherGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2);
            _ballSpeed = 100f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _ballTexture = Content.Load<Texture2D>("ball");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up))
            {
                _ballPosition.Y -= _ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.Down))
            {
                _ballPosition.Y += _ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.Left))
            {
                _ballPosition.X -= _ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.Right))
            {
                _ballPosition.X += _ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_ballPosition.X > _graphics.PreferredBackBufferWidth - _ballTexture.Width / 2)
            {
                _ballPosition.X = _graphics.PreferredBackBufferWidth - _ballTexture.Width / 2;
            }
            else if (_ballPosition.X < _ballTexture.Width / 2)
            {
                _ballPosition.X = _ballTexture.Width / 2;
            }

            if (_ballPosition.Y > _graphics.PreferredBackBufferHeight - _ballTexture.Height / 2)
            {
                _ballPosition.Y = _graphics.PreferredBackBufferHeight - _ballTexture.Height / 2;
            }
            else if (_ballPosition.Y < _ballTexture.Height / 2)
            {
                _ballPosition.Y = _ballTexture.Height / 2;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(_ballTexture, _ballPosition, null, Color.White, 0f,
                new Vector2(_ballTexture.Width / 2, _ballTexture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}