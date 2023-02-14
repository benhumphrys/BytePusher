using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BytePusherMonoGameWin
{
    internal sealed class BytePusherVm
    {
        private byte[] _rawMemory;
        private static readonly uint[] Palette;

        static BytePusherVm()
        {
            // Generate the palette lookup table.
            Palette = GetPalette();
        }

        public void LoadProgram(Span<byte> program)
        {
            _rawMemory = new byte[0x1000008];
            var mem = new Span<byte>(_rawMemory);
            program.CopyTo(mem);
        }

        public FrameOutput ExecuteFrame(KeyboardState keyState)
        {
            var mem = new Span<byte>(_rawMemory);

            // Process keyboard state ready for the frame.
            var keyboardMem = GetKeyboardMemory(keyState);
            mem[0] = keyboardMem[0];
            mem[1] = keyboardMem[1];

            // pc = 3 byte Program Counter. Read from memory at the start of each frame. 
            var pc = mem[2] << 16 | mem[3] << 8 | mem[4];
            var i = 65536;
            do
            {
                // VM copies a byte from one 24-bit/3-byte address to another, then sets a new
                // Program Counter value from a 3-byte address.
                var src = mem[pc] << 16 | mem[pc + 1] << 8 | mem[pc + 2];
                var dest = mem[pc + 3] << 16 | mem[pc + 4] << 8 | mem[pc + 5];
                mem[dest] = mem[src];
                pc = mem[pc + 6] << 16 | mem[pc + 7] << 8 | mem[pc + 8];
            } while (--i > 0);

            // Draw the screen. We draw to an array of byte data.
            var pixelDataStart = mem[5] << 16;
            var colorPixelData = new Color[0x10000];
            for (var j = 0; j < 0x10000; j++)
            {
                var pixel = Palette[mem[pixelDataStart + j]];
                colorPixelData[j] = new Color(pixel);
            }

            // Get the sound data.
            var soundDataStart = mem[6] << 16 | mem[7] << 8;
            var soundData = mem.Slice(soundDataStart, 0x100).ToArray();
            return new FrameOutput(colorPixelData, soundData);
        }

        private static byte[] GetKeyboardMemory(KeyboardState keyState)
        {
            var keyValues = new[]
            {
                keyState.IsKeyDown(Keys.D0),
                keyState.IsKeyDown(Keys.D1),
                keyState.IsKeyDown(Keys.D2),
                keyState.IsKeyDown(Keys.D3),
                keyState.IsKeyDown(Keys.D4),
                keyState.IsKeyDown(Keys.D5),
                keyState.IsKeyDown(Keys.D6),
                keyState.IsKeyDown(Keys.D7),
                keyState.IsKeyDown(Keys.D8),
                keyState.IsKeyDown(Keys.D9),
                keyState.IsKeyDown(Keys.A),
                keyState.IsKeyDown(Keys.B),
                keyState.IsKeyDown(Keys.C),
                keyState.IsKeyDown(Keys.D),
                keyState.IsKeyDown(Keys.E),
                keyState.IsKeyDown(Keys.F),
            };

            var bitArray = new BitArray(keyValues);
            var keyboardMem = new byte[2];
            bitArray.CopyTo(keyboardMem, 0);

            // Swap the order of the bytes when we assign to RAM as we are little-endian.
            return new[] { keyboardMem[1], keyboardMem[0] };
        }

        private static uint[] GetPalette()
        {
            var palette = new uint[0x100];
            var j = 0;
            for (var r = 0; r <= 0xFF; r += 0x33)
            {
                for (var g = 0; g <= 0xFF; g += 0x33)
                {
                    for (var b = 0; b <= 0xFF; b += 0x33)
                    {
                        var rgbValue = 0xFF000000 | (uint)(b << 16) | (uint)(g << 8) | (uint)r;
                        palette[j++] = rgbValue;
                    }
                }
            }

            // There are only 216 colors; the rest are black.
            for (; j <= 0xFF; j++)
            {
                palette[j] = 0;
            }

            return palette;
        }
    }

    /// <summary>
    /// Holds the output data for each frame.
    /// </summary>
    internal struct FrameOutput
    {
        public Color[] PixelData { get; }

        public byte[] SoundData { get; }

        public FrameOutput(Color[] pixelData, byte[] soundData)
        {
            PixelData = pixelData;
            SoundData = soundData;
        }
    }
}
