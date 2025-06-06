using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace BytePusherWinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void LoadBpButton_Click(object sender, EventArgs e)
        {
            //if (OpenBpFileDialog.ShowDialog() != DialogResult.OK) return;

            //TODO: Check file size.
            //var fileData = new Span<byte>(File.ReadAllBytes(OpenBpFileDialog.FileName));

            //const string filePath = "C:\\Users\\BenH\\OneDrive - SCANSATION LTD\\Source\\jsbp\\demos\\palette.bp";
            //const string filePath = "C:\\Users\\BenH\\OneDrive - SCANSATION LTD\\Source\\jsbp\\demos\\sprites.bp";
            //const string filePath = "C:\\Users\\BenH\\OneDrive - SCANSATION LTD\\Source\\jsbp\\demos\\invertloopsine.bp";
            //const string filePath = "C:\\Users\\BenH\\OneDrive - SCANSATION LTD\\Source\\jsbp\\demos\\scroller.bp";
            const string filePath = "C:\\Users\\BenH\\OneDrive - SCANSATION LTD\\Source\\jsbp\\demos\\nyan.bp";
            var fileData = new Span<byte>(File.ReadAllBytes(filePath));

            var rawMemory = new byte[0x1000008];
            var mem = new Span<byte>(rawMemory);
            fileData.CopyTo(mem);

            const int frameTimeMs = 33;
            var palette = GetPalette();

            // Each iteration represents the amount of work that can be done per frame.
            while (true)
            {
                var sw = new Stopwatch();
                sw.Start();

                //TODO: Set keyboard state

                // pc = 3 byte Program Counter. Read from memory at the start of each frame. 
                var pc = mem[2] << 16 | mem[3] << 8 | mem[4];
                var i = 65536;
                do
                {
                    // VM copies a byte from one 3-byte address to another, then sets a new
                    // Program Counter value from a 3-byte address.
                    var src = mem[pc] << 16 | mem[pc + 1] << 8 | mem[pc + 2];
                    var dest = mem[pc + 3] << 16 | mem[pc + 4] << 8 | mem[pc + 5];
                    mem[dest] = mem[src];
                    pc = mem[pc + 6] << 16 | mem[pc + 7] << 8 | mem[pc + 8];
                } while (--i > 0);
                
                // Draw the screen. We draw to an array of byte data which is then blitted to the picture box.
                var bitmap = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
                var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                var pixels = new byte[bitmapData.Stride * bitmapData.Height];

                var pixelDataStart = mem[5] << 16;
                for (var j = 0; j < 0x10000; j++)
                {
                    var pixel = palette[mem[pixelDataStart + j]];
                    var offsetStart = j * 4;
                    pixels[offsetStart++] = (byte)((pixel >> 16) & 0xff);
                    pixels[offsetStart++] = (byte)((pixel >> 8) & 0xff);
                    pixels[offsetStart++] = (byte)(pixel & 0xff);
                    pixels[offsetStart] = (byte)((pixel >> 24) & 0xff);
                }

                Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
                bitmap.UnlockBits(bitmapData);

                var oldBitmap = ScreenPictureBox.Image;
                ScreenPictureBox.Image = bitmap;
                oldBitmap?.Dispose();

                //TODO: Output audio.

                // Wait until the new frame should start.
                Application.DoEvents();
                sw.Stop();
                var msToWait = (int) (frameTimeMs - sw.ElapsedMilliseconds);

                if (msToWait > 0)
                {
                    Thread.Sleep(msToWait);
                }
            }
        }

        private static Span<uint> GetPalette()
        {
            var palette = new Span<uint>(new uint[0x100]);
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

            for (; j <= 0xFF; j++)
            {
                palette[j] = 0;
            }

            return palette;
        }
    }
}