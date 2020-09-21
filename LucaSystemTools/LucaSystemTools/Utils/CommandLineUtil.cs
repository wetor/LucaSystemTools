using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using ProtFont;
using ProtImage;
using ProtPak;
using ProtScript;
using RealLive;
using LucaSystemTools;

namespace LucaSystem.Utils
{
    [Command(ExtendedHelpText = @"help text")]
    class CommandLineUtil
    {
        public void OnExecute()
        {
            if (Debug)
            {
                Program.debug = true;
            }
            if (!string.IsNullOrEmpty(FileType))
            {
                AbstractFileParser selclass = null;
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
                    case "info":
                        selclass = new FontInfoParser();
                        break;
                    case "scr":
                        if (!string.IsNullOrEmpty(CustomOpcodePath))
                        {
                            selclass = new ScriptParser(GameScript.CUSTOM, CustomOpcodePath);
                        }
                        else if (!string.IsNullOrEmpty(OpcodePath))
                        {
                            if (OpcodePath != "CUSTOM")
                            {
                                selclass = new ScriptParser((GameScript)Enum.Parse(typeof(GameScript), OpcodePath, true));
                            }
                        }
                        else
                        {
                            throw new Exception("Need Input OpcodePath or CustomOpcodePath!");
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
            

            if (GameList)
            {
                Console.WriteLine(@"Supported Games:
Opcode name    Game name & Platform

SP             《Summer Pocket》Nintendo Switch
CL             《Clannad》Nintendo Switch
TAWL           《Tomoyo After Its a Wonderful Life CS Edition》Nintendo Switch
FLOWERS        《Flowers - Shiki》
ISALND         《ISLAND》Psvita

CUSTOM          Read custom Opcode file. Path: OPCODE/{CUSTOM}.txt
");
            }
            if (OpcodeHelp)
            {
                Console.WriteLine(@"
        Byte,
        Byte2,
        Byte3,
        Byte4,
        UInt16,
        UInt32,
        StringUnicode,
        StringSJIS,
        StringUTF8,
        LenStringUnicode,
        LenStringSJIS,
        Opcode,
        Skip
待完善...
");
            }

        }

        [Option(Description = "Script opcode ,For [scr]", ShortName = "o")]
        public string OpcodePath { get; set; }

        [Option(Description = "Script custom opcode ,For [scr]", ShortName = "c")]
        public string CustomOpcodePath { get; set; }

        [Option(Description = "FileName or FolderName", ShortName = "f")]
        public string FileName { get; set; }

        [Option(Description = "FileType [cz0] [cz1] [cz3] [cz4] [dat] [pak] [psb] [info] [scr]", ShortName = "t")]
        public string FileType { get; set; }

        [Option(Description = "ParserMode [import] or [export]", ShortName = "m")]
        public string ParserMode { get; set; }

        [Option("-d|--debug", "Enable debug mode", CommandOptionType.NoValue)]
        public bool Debug { get; set; }

        [Option("-l|--game-list", "Show list of supported games", CommandOptionType.NoValue)]
        public bool GameList { get; set; }
        [Option("-oh|--opcode-help", "Show Opcode help", CommandOptionType.NoValue)]
        public bool OpcodeHelp { get; set; }

    }
}
