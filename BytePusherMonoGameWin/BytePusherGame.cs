using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using SharpDX.MediaFoundation.DirectX;
using Color = Microsoft.Xna.Framework.Color;

namespace BytePusherMonoGameWin
{
    public class BytePusherGame : Game
    {
        private readonly string _bpFilePath;

        /// <summary>
        /// Sample rate is 256 bytes per frame at 60 frames per second.  512 bytes is required to store
        /// each 256 bytes as 16-bit audio.
        /// </summary>
        private const int SamplesPerFrame = 256;
        private const int FramesPerSecond = 60;
        private const int SampleRate = SamplesPerFrame * FramesPerSecond;
        private const int AudioBufferSizePerFrame = SamplesPerFrame * 2;


        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Vector2 _screenPosition;
        private float _screenSpeed;
        private Texture2D _bytePusherScreen;
        private BytePusherVm _vm;
        private FrameOutput _frameData;
        private DynamicSoundEffectInstance _audio;

        public BytePusherGame(string bpFilePath)
        {
            _bpFilePath = bpFilePath;
            _graphics = new GraphicsDeviceManager(this);
            _audio = new DynamicSoundEffectInstance(SampleRate, AudioChannels.Mono);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _screenPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2);
            _screenSpeed = 100f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _vm = new BytePusherVm();
            _vm.LoadProgram(new Span<byte>(File.ReadAllBytes(_bpFilePath)));

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up))
            {
                _screenPosition.Y -= _screenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.Down))
            {
                _screenPosition.Y += _screenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.Left))
            {
                _screenPosition.X -= _screenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.Right))
            {
                _screenPosition.X += _screenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            int GetBytePusherScreenWidth() => (_bytePusherScreen?.Width ?? 0x100);

            if (_screenPosition.X > _graphics.PreferredBackBufferWidth - GetBytePusherScreenWidth() / 2)
            {
                _screenPosition.X = _graphics.PreferredBackBufferWidth - GetBytePusherScreenWidth() / 2;
            }
            else if (_screenPosition.X < GetBytePusherScreenWidth() / 2)
            {
                _screenPosition.X = GetBytePusherScreenWidth() / 2;
            }

            int GetBytePusherScreenHeight() => (_bytePusherScreen?.Height ?? 0x100);

            if (_screenPosition.Y > _graphics.PreferredBackBufferHeight - GetBytePusherScreenHeight() / 2)
            {
                _screenPosition.Y = _graphics.PreferredBackBufferHeight - GetBytePusherScreenHeight() / 2;
            }
            else if (_screenPosition.Y < GetBytePusherScreenHeight() / 2)
            {
                _screenPosition.Y = GetBytePusherScreenHeight() / 2;
            }

            _frameData = _vm.ExecuteFrame(kstate);
            PlayAudio();

            base.Update(gameTime);
        }

        private void PlayAudio()
        {
            // Convert the 8-bit audio to 16-bit as DynamicSoundEffectInstance requires 16-bit samples.
            var buffer = new byte[AudioBufferSizePerFrame];

            for (var i = 0; i < SamplesPerFrame; i++)
            {
                buffer[i * 2 + 1] = (byte)(_frameData.SoundData[i] & 0xFF);
            }

            // Send the frame's audio to the buffer.
            _audio.SubmitBuffer(buffer);

            if (_audio.State == SoundState.Stopped && _audio.PendingBufferCount > 2)
            {
                // We only play once we have two buffers ready (speculatively guarding against sounds
                // dropping out if the buffer empties and is not filled quickly enough - do we actually
                // need to wait?).
                _audio.Play();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _bytePusherScreen?.Dispose();
            _bytePusherScreen = new Texture2D(_graphics.GraphicsDevice, 0x100, 0x100);
            _bytePusherScreen.SetData(_frameData.PixelData);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_bytePusherScreen, _screenPosition, null, Color.White, 0f,
                new Vector2(_bytePusherScreen.Width / 2, _bytePusherScreen.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            _spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}