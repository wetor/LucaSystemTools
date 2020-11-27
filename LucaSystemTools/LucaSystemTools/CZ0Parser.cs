using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using LucaSystem;
using LucaSystemTools;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ProtImage
{
    //作者：marcussacana
    //时间：2018.1
    //https://github.com/marcussacana/LucaSystem
    public class CZ0Parser : CZParserBase

    {

        public override void FileExport(string infile, string outpath = null)
        {
            BinaryReader br = new BinaryReader(File.Open(infile, FileMode.Open));
            Bitmap texture = Export(br.ReadBytes((int)br.BaseStream.Length), infile);
            if (outpath != null)
            {
                texture.Save(outpath, ImageFormat.Png);
            }
            else
            {
                texture.Save(infile + ".png", ImageFormat.Png);
            }
            br.Close();
        }

        public override void FileImport(string path, string outpath = null)
        {
            CZ0HeaderInfo cz0HeaderInfo = null;
            string infopath = path.Replace(".png", ".json");
            if (File.Exists(infopath))
            {
                cz0HeaderInfo = JsonConvert.DeserializeObject<CZ0HeaderInfo>(File.ReadAllText(infopath));
            }
            CZOutputInfo czOutput = new CZOutputInfo();
            
            Bitmap Picture = new Bitmap(new MemoryStream(File.ReadAllBytes(path)));
            StructWriter Writer = new StructWriter(File.Open(path + ".cz0", FileMode.Create));
            CZ0Header header;
            if (cz0HeaderInfo == null)
            {
                header.Signature = "CZ0";
                header.HeaderLength = 0x40;
                header.Width = (ushort)Picture.Width;
                header.Heigth = (ushort)Picture.Height;
                header.Colorbits = 32;
            }
            else
            {
                header = cz0HeaderInfo.cz0Header;
            }
            Writer.WriteStruct(ref header);
            Writer.Seek(header.HeaderLength, SeekOrigin.Begin);

            if (header.Colorbits == 32)
            {
                System.Diagnostics.Debug.WriteLine(32);
                for (int y = 0; y < Picture.Height; y++)
                {
                    for (int x = 0; x < Picture.Width; x++)
                    {
                        Writer.Write(Picture.GetPixel(x, y).R);
                        Writer.Write(Picture.GetPixel(x, y).G);
                        Writer.Write(Picture.GetPixel(x, y).B);
                        Writer.Write(Picture.GetPixel(x, y).A);
                    }
                }
            }
            else if (header.Colorbits == 24)
            {
                System.Diagnostics.Debug.WriteLine(24);
                for (int y = 0; y < Picture.Height; y++)
                {
                    for (int x = 0; x < Picture.Width; x++)
                    {
                        Writer.Write(Picture.GetPixel(x, y).R);
                        Writer.Write(Picture.GetPixel(x, y).G);
                        Writer.Write(Picture.GetPixel(x, y).B);
                    }
                }
            }
            else if (header.Colorbits == 8)
            {
                System.Diagnostics.Debug.WriteLine(8);
                List<Pixel32_BGRA> list = new List<Pixel32_BGRA>();
                for (int y = 0; y < Picture.Height; y++)
                {
                    for (int x = 0; x < Picture.Width; x++)
                    {
                        var color = Picture.GetPixel(x, y);
                        var color2 = new Pixel32_BGRA();
                        color2.R = color.R;
                        color2.G = color.G;
                        color2.B = color.B;
                        color2.A = color.A;
                        if (!list.Contains(color2))
                        {
                            list.Add(color2);
                        }
                    }
                }
               
                //颜色大于256
                if (list.Count > 256)
                {
                    Console.WriteLine("Over 256 Color!!");
                    //调用pngquant png 8bit缩减
                    if (File.Exists("pngquant.exe")|| File.Exists("pngquant"))
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            var psi = new ProcessStartInfo("pngquant.exe") { RedirectStandardOutput = true };
                            psi.Arguments = " 256 "+ path +" --ext tmp ";
                            var proc = Process.Start(psi);
                            if (proc == null)
                            {
                                Console.WriteLine("Can not exec.");
                            }
                            else
                            {
                                using (var sr = proc.StandardOutput)
                                {
                                    while (!sr.EndOfStream)
                                    {
                                        Console.WriteLine(sr.ReadLine());
                                    }

                                    if (!proc.HasExited)
                                    {
                                        proc.Kill();
                                    }
                                }
                            }
                            string pathtmp = path.Replace(".png", "tmp");
                            Picture = new Bitmap(new MemoryStream(File.ReadAllBytes(pathtmp)));
                            File.Delete(pathtmp);
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            //TODO
                        }
                        else
                        {
                            //TODO
                        }

                        list.Clear();
                        for (int y = 0; y < Picture.Height; y++)
                        {
                            for (int x = 0; x < Picture.Width; x++)
                            {
                                var color = Picture.GetPixel(x, y);
                                var color2 = new Pixel32_BGRA();
                                color2.R = color.R;
                                color2.G = color.G;
                                color2.B = color.B;
                                color2.A = color.A;
                                if (!list.Contains(color2))
                                {
                                    list.Add(color2);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Need pngquant !! Download From https://pngquant.org/");
                    }

                }

                var pixel = new Pixel32_BGRA();
                for (int i = 0; i < list.Count; i++)
                {
                    pixel = list[i];
                    Writer.WriteStruct(ref pixel);
                }
                for (int i = 0; i < 256 - list.Count; i++)
                {
                    var pixelempty = new Pixel32_BGRA() { R = 0, G = 0, B = 0, A = 255 };
                    Writer.WriteStruct(ref pixelempty);
                }

                for (int y = 0; y < Picture.Height; y++)
                {
                    for (int x = 0; x < Picture.Width; x++)
                    {
                        var color = Picture.GetPixel(x, y);
                        var color2 = new Pixel32_BGRA();
                        color2.R = color.R;
                        color2.G = color.G;
                        color2.B = color.B;
                        color2.A = color.A;
                        uint index = (uint)list.IndexOf(color2);
                        Writer.Write((byte)index);
                    }
                }
            }
            Writer.Close();
        }

        byte[] Texture;
        public CZ0Parser(byte[] Texture)
        {
            this.Texture = Texture;
        }
        public class CZ0HeaderInfo
        {
            public CZ0Header cz0Header;
            public Pixel32_BGRA[] ColorPanel;
        }
     

        public Bitmap Export(byte[] Texture, string name = "")
        {
            StructReader Reader = new StructReader(new MemoryStream(Texture));
            CZ0Header Header = new CZ0Header();
            Reader.ReadStruct(ref Header);

            if (Header.Signature != "CZ0\x0")
                throw new BadImageFormatException();

            Reader.Seek(Header.HeaderLength, SeekOrigin.Begin);
            CZ0HeaderInfo cz0HeaderInfo = new CZ0HeaderInfo();
            cz0HeaderInfo.cz0Header = Header;
            Bitmap Picture = new Bitmap(Header.Width, Header.Heigth, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            if (Header.Colorbits == 8)
            {
                System.Diagnostics.Debug.WriteLine(8);
                Pixel32_BGRA[] ColorPanel = new Pixel32_BGRA[256];
                Pixel32_BGRA Pixel = new Pixel32_BGRA();
                for (int i = 0; i < ColorPanel.Length; i++)
                {
                    Reader.ReadStruct(ref Pixel);
                    ColorPanel[i] = Pixel;
                }
                cz0HeaderInfo.ColorPanel = ColorPanel;
                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {
                        byte tmp = 0;
                        Reader.ReadStruct(ref tmp);
                        uint index = tmp;
                        Picture.SetPixel(x, y, Color.FromArgb(ColorPanel[index].A, ColorPanel[index].R, ColorPanel[index].G, ColorPanel[index].B));
                    }

            }
            else if (Header.Colorbits == 24)
            {
                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {
                        Pixel24_RGB Pixel = new Pixel24_RGB();
                        Reader.ReadStruct(ref Pixel);
                        Picture.SetPixel(x, y, Color.FromArgb(Pixel.R, Pixel.G, Pixel.B));
                    }
            }
            else if (Header.Colorbits == 32)
            {
                for (int y = 0; y < Header.Heigth; y++)
                    for (int x = 0; x < Header.Width; x++)
                    {
                        Pixel32_RGBA Pixel = new Pixel32_RGBA();
                        Reader.ReadStruct(ref Pixel);
                        Picture.SetPixel(x, y, Color.FromArgb(Pixel.A, Pixel.R, Pixel.G, Pixel.B));
                    }
            }
            Reader.Close();
            string json = JsonConvert.SerializeObject(cz0HeaderInfo, Formatting.Indented);
            File.WriteAllText(name + ".json", json);
            return Picture;
        }


        public CZ0Parser()
        {

        }

    }
    public struct CZ0Header
    {
        [FString(Length = 4)]
        public string Signature;
        public uint HeaderLength;
        public ushort Width;
        public ushort Heigth;
        public byte Colorbits;
        //dynamic length
    }
    public struct Pixel24_RGB
    {
        public byte R, G, B;
    }
    public struct Pixel32_BGRA
    {
        public byte B, G, R, A;
    }
    public struct Pixel32_RGBA
    {
        public byte R, G, B, A;
    }
}
