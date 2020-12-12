using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpcodeGuide
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*LucaSystemTools.Program.debug = true;

            OpcodeEntity entity = new OpcodeEntity();
            entity.Version = 2;
            entity.Filename = @"C:\Users\29293\Source\Repos\wetor\LucaSystemTools\LucaSystemTools\LucaSystemTools\OPCODE\ISLAND.txt";
            entity.ScriptPath = @"D:\WorkSpace\Test\island";

            entity.LoadScript(0);
            entity.CurrentCodeID++;
            entity.CurrentCodeID--;
            entity.SetOpcodeDict(1, "EQUN", 
                new ProtScript.ParamType(ProtScript.DataType.UInt16, false),
                new ProtScript.ParamType(ProtScript.DataType.UInt16, false));
            entity.ReReadCodeLine();
            entity.CurrentCodeID++;
            entity.CurrentCodeID++;
            entity.CurrentScriptID++;
            entity.CurrentCodeID++;
            entity.CurrentCodeID++;
            entity.CurrentCodeID++;*/
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Guide());
        }
    }
}
