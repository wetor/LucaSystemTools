using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProtScript;
using ProtPak;
using ProtImage;
namespace PrototypeTools
{
    class Program
    {
        static string work = @"F:\NX\Clannad\Extracted_NCA\romfs\";
        static void Main(string[] args)
        {
#if true //反编译全部脚本
            
            var files = Directory.GetFiles(work + @"SCRIPT.PAK_unpacked", "*");
            string[] black = new string[]
            {
                "_BUILD_COUNT",
                "_VARNUM",
                "_TASK",
                "_SCR_LABEL",
                "_CGMODE",
                "_VOICE_PARAM"
            };
            foreach (var file in files)
            {
                bool flag = false;
                if (Path.GetExtension(file) == ".txt") continue;
                for(int i = 0; i < black.Length; i++)
                {
                    if (Path.GetFileName(file) == black[i])
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag) continue;

                Console.WriteLine(file);
                ScriptParser scr = new ScriptParser(file);
                scr.DeCompress();
                scr.Close();
            }
#endif
#if false //反编译单个脚本
           
            ScriptParser scr = new ScriptParser(work + @"Temp\10_プロローグ0725", true);
            scr.DeCompress();
            scr.Compress();
            scr.Close();
#endif
            //PAKManager.Pack(work + @"OriginFile\SCRIPT.PAK.pakhead", "Shift-Jis");
            //PAKManager.Unpack(work + @"SCRIPT.PAK", "UTF-8");
            //CZ1Parser cz1 = new CZ1Parser();
            //cz1.CZ1ToPng(work + @"OriginFile\SYSCG.PAK_unpacked\CGM_NAME06");
            //cz1.PngToCZ1(work + @"UnpackFile\FONT.PAK_unpacked\ゴシック38.png");
            //CGM_SELECT
            //Console.WriteLine("{0}",(byte)((byte)0x50+(byte)0xff)));
            //CZ3Parser cz3 = new CZ3Parser();
            //cz3.CZ3ToPng(work + @"Temp\CGM_SELECT");
            Console.WriteLine("ok!");
            Console.ReadKey();

        }
    }
}
