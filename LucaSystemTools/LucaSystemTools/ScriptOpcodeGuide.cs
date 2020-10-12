using LucaSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtScript.Entity;

namespace ProtScript
{
    public class ScriptOpcodeGuide : AbstractFileParser
    {

        public Dictionary<byte, ScriptOpcode> bytesToOpcodeDict = new Dictionary<byte, ScriptOpcode>();

        public bool hasOpcode = false;

        public int scriptVersion = 3;

        public ScriptOpcodeGuide(string opcode = "", int version = 3)
        {
            if (opcode == "")
            {
                scriptVersion = version;
                for (int i = 0; i < 128; i++)
                {
                    bytesToOpcodeDict.Add((byte)i, new ScriptOpcode((byte)i, "OPCODE_" + ScriptUtil.Byte2Hex((byte)i)));
                }
                Console.WriteLine("无opcode文件，创建默认opcode文件");
            }
            else if(File.Exists(opcode))
            {
                string[] dic;
                dic = File.ReadAllLines(opcode);
                bytesToOpcodeDict.Clear();
                if (dic.Length > 0)
                {
                    if (dic[0].Trim() == ";Ver3")
                    {
                        scriptVersion = 3;
                    }
                    else if (dic[0].Trim() == ";Ver2")
                    {
                        scriptVersion = 2;
                    }
                }
                int i = 0;
                foreach (string line in dic)
                {
                    if (line.TrimStart()[0] == ';')
                    {
                        continue;
                    }
                    bytesToOpcodeDict.Add((byte)i, new ScriptOpcode((byte)i, line.Replace("\r", "")));
                    i++;
                }
                if (bytesToOpcodeDict.Count > 0)
                {
                    hasOpcode = true;
                }
                Console.WriteLine("已加载opcode文件：{0}", Path.GetFileName(opcode));
            }
        }
        public override void FileExport(string path, string outpath = null)
        {
            ScriptReader reader = new ScriptReader(path, bytesToOpcodeDict, scriptVersion);
            Console.WriteLine("已加载脚本文件：{0}，版本：{1}", Path.GetFileName(path), scriptVersion);
            Console.WriteLine("开始读取脚本...");
            reader.ReadScript_Clear();
            int position;
            int length;
            int i = 0;
            while (reader.ReadScript_CanStepRead())
            {
                Console.WriteLine("== {0:0000} ===================================", i);
                CodeLine code = reader.ReadScript_StepRead(out position, out length);
                Console.WriteLine("文件中位置：{0}，长度：{1}", position, length);
                Console.WriteLine("{0}", code.ToStringAll());
                Console.WriteLine("按[E]编辑此Opcode参数列表，其他按键读取下一条");
                if (Console.ReadKey().Key == ConsoleKey.E)
                {
                    Console.WriteLine();
                    Console.WriteLine("按下指定[数字键]修改对应内容：");
                    string tipParamKey = "";
                    if (!hasOpcode)
                    {
                        tipParamKey += "[0]:opcode名，";
                    }
                    
                    for(int j = 0; j < code.paramDatas.Count; j++)
                    {
                        tipParamKey += String.Format("[{0}]:参数{1}({2})，", j + 1, j + 1, code.paramDatas[j].valueString);
                    }
                    tipParamKey += "[Q]:返回";
                    Console.WriteLine(tipParamKey);
                    Console.Write("输入：");
                    ConsoleKey key = Console.ReadKey().Key;
                    Console.WriteLine();
                    List<DataType> types = new List<DataType>();
                    string setOpcode = code.opcode;
                    while (key!= ConsoleKey.Q)
                    {
                        if (key == ConsoleKey.D0 || key == ConsoleKey.NumPad0) {
                            Console.Write("设置opcode为：");
                            setOpcode = Console.ReadLine();
                            Console.WriteLine();
                        }
                        for (int j = 0; j < code.paramDatas.Count; j++)
                        {
                            if(key == ConsoleKey.D1 + j || key == ConsoleKey.NumPad1 + j)
                            {
                                Console.WriteLine("正在设置参数{0}，当前类型：{1}，值：{2}，", 
                                    j + 1, code.paramDatas[j].type.ToString(), code.paramDatas[j].valueString);
                                Console.Write("设置为的类型：");
                                string type = Console.ReadLine();
                                types.Add((DataType)Enum.Parse(typeof(DataType), type, true));
                                Console.WriteLine();
                            }
                        }
                        Console.WriteLine(tipParamKey);
                        Console.Write("输入：");
                        key = Console.ReadKey().Key;
                        Console.WriteLine();
                    }

                    string temp = setOpcode + "(";
                    for(int k = 0; k < types.Count; k++)
                    {
                        temp += types[k].ToString();
                        if (k < types.Count - 1)
                        {
                            temp += ", ";
                        }
                    }
                    temp += ")";
                    Console.WriteLine();
                    Console.WriteLine("修改结果为：{0}，即将重新读取此条...",temp);
                    bytesToOpcodeDict[code.opcodeIndex] = new ScriptOpcode(code.opcodeIndex, temp);
                    reader.ReadScript_Seek(-length);
                    i--;
                    

                }

                i++;
                Console.WriteLine();
            }


        }

        public override void FileImport(string path, string outpath = null)
        {
            throw new NotImplementedException();
        }
    }
}
