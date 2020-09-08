using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LucaSystemTools.Utils
{
    public class ImageFillPartten
    {
        public delegate void SetPixelMethod(int x, int y, byte[] curr_line);
        public static void LineDiffPattern(ref Bitmap Picture, ushort Colorblock, int PixelByteCount, byte[] data, SetPixelMethod setPixelMethod=null)
        {
            int Heigth = Picture.Height;
            int Width = Picture.Width;
            ushort Blockh = (ushort)Math.Ceiling((float)Heigth / (float)Colorblock);

            byte[] curr_line = new byte[Width * PixelByteCount];
            byte[] pre_line = new byte[Width * PixelByteCount];
            int i = 0;
            for (int y = 0; y < Heigth; y++)
            {

                Buffer.BlockCopy(data, i, curr_line, 0, Width * PixelByteCount);


                if (y % Blockh != 0)
                    for (int x = 0; x < Width * PixelByteCount; x++)
                        curr_line[x] += pre_line[x];
                for (int x = 0; x < Width; x++)
                {
                    if (setPixelMethod != null)
                    {
                        setPixelMethod.Invoke(x, y, curr_line);
                    }
                    else if (setPixelMethod == null && PixelByteCount == 4)
                    {
                        Picture.SetPixel(x, y, Color.FromArgb(curr_line[x * PixelByteCount + 3], curr_line[x * PixelByteCount + 0], curr_line[x * PixelByteCount + 1], curr_line[x * PixelByteCount + 2]));
                    }
                    else if (setPixelMethod == null && PixelByteCount == 3)
                    {
                        Picture.SetPixel(x, y, Color.FromArgb(curr_line[x * PixelByteCount + 0], curr_line[x * PixelByteCount + 1], curr_line[x * PixelByteCount + 2]));
                    }
                }
                curr_line.CopyTo(pre_line, 0);
                i += Width * PixelByteCount;

            }
        }

    }
}
