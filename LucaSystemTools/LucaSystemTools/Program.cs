using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProtScript;
using ProtPak;
using ProtImage;
using RealLive;
namespace LucaSystemTools
{
    /**
     * 此项目未完工，大概率已坑
     * 
     * 
     */
    class Program
    {
        static string work = @"C:\Users\29293\Desktop\Prototype\";
        static void Main(string[] args)
        {
#if false //反编译全部脚本
            
            var files = Directory.GetFiles(work + @"SCRIPT.PAK_unpacked", "*");
            string[] black_list = new string[] // 跳过的脚本文件
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
                for(int i = 0; i < black_list.Length; i++)
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
            //
            ScriptParser scr = new ScriptParser(work + @"island\SCRIPT.PAK_unpacked\KAR01", true);
            scr.DeCompress();
            //scr.Compress();
            scr.Close();
#endif
            /*PsbScript psb = new PsbScript(work + @"prot_tblpak_v11\psvscr\04_3A6E269.psb", true);
            psb.DeCompress();
            psb.Close();*/

#if false //反编译单个脚本
            var files = Directory.GetFiles(work + @"prot_tblpak_v11\imgscr\", "*.psb");
            string[] black_list = new string[] // 跳过的脚本文件
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
                for (int i = 0; i < black_list.Length; i++)
                {
                    if (Path.GetFileName(file) == black_list[i])
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag) continue;

                Console.WriteLine(file);
                PsbScript psb = new PsbScript(file);
                psb.DeCompress();
                psb.Close();
            }
#endif
            //DatParser dat = new DatParser();
            //dat.DatToPng(work + @"prot_tblpak_v11\0884_85630CE\0884_85630CE.dat");
            //PAKManager.Pack(work + @"OriginFile\SCRIPT.PAK.pakhead", "Shift-Jis");
            //PAKManager.Unpack(work + @"SCRIPT.PAK", "Shift-Jis");
            //CZ1Parser cz1 = new CZ1Parser();
            //cz1.CZ1ToPng(work + @"OriginFile\SYSCG.PAK_unpacked\CGM_NAME06");
            //cz1.PngToCZ1(work + @"UnpackFile\FONT.PAK_unpacked\ゴシック38.png");
            //CZ3Parser cz3 = new CZ3Parser();
            //cz3.CZ3ToPng(work + @"Temp\CGM_SELECT");
            Console.WriteLine("ok!");
            Console.ReadKey();

        }
    }
}
