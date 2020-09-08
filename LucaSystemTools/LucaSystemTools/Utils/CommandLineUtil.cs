using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using ProtImage;
using ProtPak;
using ProtScript;
using RealLive;

namespace LucaSystem.Utils
{
    class CommandLineUtil
    {
        public void OnExecute()
        {
            AbstractFileParser selclass=null;
            switch (FileType.ToLower())
            {
                case "cz0":
                    selclass = new CZ0Parser();
                    break;
                case "cz1":
                    selclass = new CZ1Parser();
                    break;
                case "cz2":
                  //  selclass = new CZ2Parser();
                    break;
                case "cz3":
                    selclass = new CZ3Parser();
                    break;
                case "cz4":
                    selclass = new CZ4Parser();
                    break;
                case "dat":
                    selclass = new DatParser();
                    break;
                case "pak":
                    selclass = new PAKManager();
                    break;
                case "psb":
                    selclass = new PsbScript();
                    break;
                case "scr":
                    if (!string.IsNullOrEmpty(OpcodePath))
                    {
                        selclass = new ScriptParser((GameScript)Enum.Parse(typeof(GameScript), OpcodePath, true));
                    }
                    else
                    {
                        throw new Exception("Need Input OpcodePath!");
                    }
                    break;
                default:
                        break;
            }
           
            if (ParserMode.ToLower() == "import" || ParserMode.ToLower() == "i")
            {
                selclass.FileImport(FileName);
            }
            else if (ParserMode.ToLower() == "export" || ParserMode.ToLower() == "e")
            {
                selclass.FileExport(FileName);
            }
            else
            {
                throw new Exception("Need Input ParserMode!");
            }
        }

        [Option(Description = "Script opcode ,For [scr]", ShortName = "o")]
        public string OpcodePath { get; set; }

        [Option(Description = "FileName or FolderName", ShortName = "f")]
        public string FileName { get; set; }

        [Option(Description = "FileType [cz0] [cz1] [cz3] [cz4] [dat] [pak] [psb] [scr]", ShortName = "t")]
        public string FileType { get; set; }

        [Option(Description = "ParserMode [import] or [export]", ShortName = "m")]
        public string ParserMode { get; set; }
    }
}
