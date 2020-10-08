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


        [Option(Description = "FileType [cz0] [cz1] [cz3] [cz4] [dat] [pak] [psb] [info] [scr]", ShortName = "t")]
        public string FileType { get; set; }

        [Option(Description = "ParserMode [import] or [export]", ShortName = "m")]
        public string ParserMode { get; set; }

        [Option(Description = "FileName or FolderName", ShortName = "f")]
        public string FileName { get; set; }

        [Option(Description = "OutFileName or OutFolderName", ShortName = "o")]
        public string OutFileName { get; set; } = null;

        [Option(Description = "Script opcode ,For [scr]", ShortName = "opcode")]
        public string OpcodePath { get; set; }

        [Option(Description = "Script custom opcode ,For [scr]", ShortName = "c")]
        public string CustomOpcodePath { get; set; }

        [Option(Description = "TBL filename ,For [scr]", ShortName = "tbl")]
        public string TBLFile { get; set; }

        [Option(Description = "Pakfile name coding ,For [pak]", ShortName = "p")]
        public string PakCoding { get; set; }


        [Option("-lua|--format-lua", "Export and import lua format script (Can import) ,For [scr]", CommandOptionType.NoValue)]
        public bool FormatLua { get; set; } = false;
        [Option("-luae|--format-lua-export", "Export lua format script (Without param type, can't import) ,For [scr]", CommandOptionType.NoValue)]
        public bool FormatLuaE { get; set; } = false;
        [Option("-json|--format-json", "Export and import json format script (Import priority json) ,For [scr]", CommandOptionType.NoValue)]
        public bool FormatJson { get; set; } = false;
        [Option("-old|--format-old", "Use old format export and import ,For [scr]", CommandOptionType.NoValue)]
        public bool FormatOld { get; set; } = false;

        [Option("-d|--debug", "Enable debug mode", CommandOptionType.NoValue)]
        public bool Debug { get; set; }

        

        [Option("-l|--game-list", "Show list of supported games", CommandOptionType.NoValue)]
        public bool GameList { get; set; }

        [Option("-oh|--opcode-help", "Show Opcode help", CommandOptionType.NoValue)]
        public bool OpcodeHelp { get; set; }

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
                    case "cz1_4bit":
                        selclass = new CZ1_4bitParser();
                        break;
                    case "cz2":
                        selclass = new CZ2Parser();
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
                        if (!string.IsNullOrEmpty(PakCoding)){
                            selclass = new PAKManager(PakCoding);
                        }
                        else
                        {
                            selclass = new PAKManager();
                        }
                            
                        break;
                    case "psb":
                        selclass = new PsbScript();
                        break;
                    case "info":
                        selclass = new FontInfoParser();
                        break;
                    case "scr":
                        if(!FormatOld && !FormatLua && !FormatLuaE && !FormatJson)
                        {
                            FormatJson = true;
                        }
                        if(FormatLua && FormatLuaE)
                        {
                            FormatLuaE = false; // 优先能导入
                        }
                        if (!string.IsNullOrEmpty(CustomOpcodePath))
                        {
                            selclass = new ScriptParser(GameScript.CUSTOM, CustomOpcodePath, 
                                FormatOld, FormatLua, FormatLuaE,FormatJson);
                        }
                        else if (!string.IsNullOrEmpty(OpcodePath))
                        {
                            if (OpcodePath != "CUSTOM")
                            {
                                selclass = new ScriptParser((GameScript)Enum.Parse(typeof(GameScript), OpcodePath, true),"", 
                                    FormatOld, FormatLua, FormatLuaE,FormatJson);
                            }
                        }
                        else
                        {
                            throw new Exception("Need Input OpcodePath or CustomOpcodePath!");
                        }

                        if (!string.IsNullOrEmpty(TBLFile))
                        {
                            CustomEncoding.LoadTbl(TBLFile);
                        }
                        break;
                    default:
                        break;
                }

                if (ParserMode.ToLower() == "import" || ParserMode.ToLower() == "i")
                {
                    selclass.FileImport(FileName, OutFileName);
                }
                else if (ParserMode.ToLower() == "export" || ParserMode.ToLower() == "e")
                {
                    selclass.FileExport(FileName, OutFileName);
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



 

    }
}
