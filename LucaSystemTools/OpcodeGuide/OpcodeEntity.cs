using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProtScript;
using System.Diagnostics;
using ProtScript.Entity;

namespace OpcodeGuide
{
    public class ScriptInfo
    {
        private string path;
        public string Name { get; set; }
        public string Filename => Path.Combine(path, Name);

        public int Size { get; }
        public int Position { get; set; }

        public int Index { get; set; }

        public ScriptInfo(string filename)
        {
            var info = new FileInfo(filename);
            path = info.DirectoryName;
            Name = info.Name;
            Size = (int)info.Length;
            Position = 0;
            Index = 0;
        }
    }
    /// <summary>
    /// 使用前必须赋值：
    ///     0.Version:脚本版本
    ///     1.Filename:opcode文件名
    ///     2.ScriptPath:脚本目录
    /// 开始使用：
    ///     0.LoadScript(index) or LoadScript(filename)
    ///     
    ///     1.修改CurrentCodeID
    ///     2.读取或修改CurrentCodeLine
    ///         2.0 使用SetOpcodeDict
    ///         2.1 回到CurrentScript.Position位置重新使用ReReadCodeLine重新读取
    ///         2.2 修改CurrentCodeLine
    ///         2.3 回到1
    /// 
    ///     e.Close()
    /// </summary>
    public class OpcodeEntity
    {
        //Opcode扩展名
        private string opcodeExt;
        //文件所在目录
        private string opcodePath;
        //游戏名 Opcode名
        private string opcodeName;
        public string Name { get { return opcodeName; } }

        public bool isOpenOpcode => opcodeName != "" && opcodeName != null;

        public bool isOpenFolder = false;
        public bool isLoadScript => scriptLoaded;
        //文件全路径
        public string Filename {
            get { return Path.Combine(opcodePath, opcodeName + opcodeExt); }
            set
            {
                opcodeName = Path.GetFileNameWithoutExtension(value);
                opcodeExt = Path.GetExtension(value);
                opcodePath = Path.GetDirectoryName(value);
                int tempVersion = LoadOpcode(value);
                if (tempVersion != 0 && tempVersion != scriptVersion) 
                {
                    scriptVersion = tempVersion;
                    Debug.WriteLine("Opcode中的版本与设置不符合，默认采用文件中的版本");
                }
            }
        }
        //游戏脚本版本
        private int scriptVersion;
        public int Version { 
            get { return scriptVersion; }
            set { scriptVersion = value; }
        }
        //脚本目录，发生改变时将重新加载
        public string ScriptPath {
            set {
                Scripts = new List<ScriptInfo>();
                var files = Directory.GetFiles(value);
                foreach (var file in files)
                {
                    Scripts.Add(new ScriptInfo(file));
                }
                isOpenFolder = true;
            }
        }
        //脚本文件名
        public List<ScriptInfo> Scripts;
        private bool scriptLoaded = false;
        //当前脚本编号（id）
        private int index;
        public int CurrentScriptID { 
            get { return index; }
            set {
                if (value >= 0 && value < Scripts.Count)
                {
                    index = value;
                    LoadScript(index);
                }
            } 
        }
        //不同版本的脚本bytes开始位置不同
        public int CurrentBytesOffset
        {
            get
            {
                if (scriptVersion == 2)
                {
                    // 1byte opcode  1byte length  2byte info
                    return 4;
                } 
                else if (scriptVersion == 3)
                { 
                    // 2byte length(uncalc) 1byte opcode  1byte n  n*2byte info
                    return 2 + CurrentCodeLine.info.count * 2;
                }
                else
                {
                    return 0;
                }
                    
            }
        }
        //当前脚本
        public ScriptInfo CurrentScript => Scripts[index];
        //当前脚本的reader
        private ScriptReader reader;
        //当前脚本名
        public string CurrentScriptName => Scripts[index].Name;
        //当前
        public string CurrentScriptFilename => Scripts[index].Filename;
        //脚本全路径
        public string ScriptFilename(int id) { return Scripts[id].Filename; }

        //当前代码编号
        private int codeIndex;
        public int CurrentCodeID
        {
            get { return codeIndex; }
            set {
                if (value >= 0 && value < reader.script.lines.Count)
                {
                    codeIndex = value;
                    Scripts[index].Index = value;
                    Scripts[index].Position = reader.script.lines[value].position;
                }
            }
        }

        //当前代码
        public CodeLine CurrentCodeLine
        {
            get { return reader.script.lines[codeIndex]; }
            set
            {
                reader.script.lines[codeIndex] = value;
            }
        }
        public ScriptOpcode CurrentOpcode => reader.GetOpcodeDict((byte)CurrentOpcodeIndex);

        public int CurrentOpcodeIndex => reader.script.lines[codeIndex].opcodeIndex;
        public int CodeLineCount => reader.script.lines.Count;



        private Dictionary<byte, ScriptOpcode> bytesToOpcodeDict = new Dictionary<byte, ScriptOpcode>();

        public void SetOpcodeDict(byte opcode_byte, string opcode, params ParamType[] values)
        {
            bytesToOpcodeDict[opcode_byte] = new ScriptOpcode(opcode_byte, opcode, values);
        }
        public void SetOpcodeDict(byte opcode_byte, string text)
        {
            bytesToOpcodeDict[opcode_byte] = new ScriptOpcode(opcode_byte, text);
        }
        public void SetOpcodeDict(ScriptOpcode opcode)
        {
            bytesToOpcodeDict[opcode.opcode_byte] = opcode;
        }
        public void SaveOpcodeDict(string filename = null)
        {
            if(filename == null)
            {
                filename = Filename;
            }
            StreamWriter sw = new StreamWriter(filename);
            sw.WriteLine(";Ver" + scriptVersion);
            foreach(var opcode in bytesToOpcodeDict)
            {
                sw.WriteLine(opcode.Value.ToString());
            }
            sw.Close();
        }
        public string[] OpcodeArray
        {
            get
            {
                string[] array = new string[bytesToOpcodeDict.Count];
                byte i = 0;
                while (i < array.Length) 
                {
                    array[i] = bytesToOpcodeDict[i].opcode;
                    i++;
                }
                return array;
            }
        }
        public ScriptOpcode getOpcodeDict(int index)
        {
            if (bytesToOpcodeDict.ContainsKey((byte)index))
            {
                return bytesToOpcodeDict[(byte)index];
            }
            return null;
        }
        /// <summary>
        /// 尝试跳转到指定位置
        /// </summary>
        /// <returns>返回差值，为正</returns>
        public int TryJumpPosition(int position)
        {
            if(!(position>=0 && position < Scripts[index].Size))
            {
                return 0;
            }
            int offset = 0;
            int start = 0, end = 0; // 查找的范围
            
            if (position < reader.script.lines[codeIndex].position)
            {
                end = CodeLineCount;
            }
            else
            {
                start = codeIndex;
                end = CodeLineCount;
            }
            for (int i = start; i < end; i++)
            {
                if(position == reader.script.lines[i].position)
                {
                    CurrentCodeID = i;
                    offset = 0;
                    break;
                }
                else if(position < reader.script.lines[i].position)
                {
                    CurrentCodeID = i - 1;
                    offset = position - reader.script.lines[i - 1].position;
                    break;
                }
                if (i == end - 1)
                {
                    CurrentCodeID = i;
                    offset = position - reader.script.lines[i].position;
                }
            }
            return offset;
        }
        /// <summary>
        /// 获取当前语句的字节数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return reader.script.lines[codeIndex].bytes;
        }

        /// <summary>
        /// 载入不在列表中的脚本，并加入到列表
        /// </summary>
        /// <param name="filename"></param>
        public void LoadScript(string filename)
        {
            Scripts.Add(new ScriptInfo(filename));
            LoadScript(Scripts.Count - 1);
        }

        /// <summary>
        /// 02.载入脚本
        /// </summary>
        /// <param name="id">编号</param>
        public void LoadScript(int id)
        {
            if (scriptLoaded)
            {
                reader.Close();
            }
            scriptLoaded = true;
            index = id;
            var opcodes = (Dictionary<byte, ScriptOpcode>)ScriptUtil.Clone(bytesToOpcodeDict);
            reader = new ScriptReader(ScriptFilename(index), opcodes, scriptVersion);
            reader.ReadScript(); // 读入整个脚本
            CurrentCodeID = 0;
        }
        /// <summary>
        /// 还原reader的opcode dict
        /// </summary>
        public void OpcodeDictRestore()
        {
            var opcodes = (Dictionary<byte, ScriptOpcode>)ScriptUtil.Clone(bytesToOpcodeDict);
            reader.SetOpcodeDict(opcodes);
            ReReadCodeLine();

        }
        /// <summary>
        /// 重新读取当前codeline 临时
        /// </summary>
        public void ReReadCodeLine(ScriptOpcode opcode)
        {
            reader.ReadScript_Seek(CurrentScript.Position, SeekOrigin.Begin);
            CurrentCodeLine = reader.ReadScript_StepReadByOpcode(opcode);
        }
        /// <summary>
        /// 重新读取当前codeline
        /// </summary>
        public void ReReadCodeLine()
        {
            int position, length;
            reader.ReadScript_Seek(CurrentScript.Position, SeekOrigin.Begin);
            CurrentCodeLine = reader.ReadScript_StepRead(out position,out length);
        }
        public void Close()
        {
            if (scriptLoaded)
            {
                reader.Close();
            }
        }
        /// <summary>
        /// 01.创建或载入OPCODE，初始化bytesToOpcodeDict
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        private int LoadOpcode(string opcode = "")
        {
            int version = 0;
            bool notNull = true;
            if (opcode == "")
            {
                notNull = false;
            }
            else if (File.Exists(opcode))
            {
                string[] dic;
                dic = File.ReadAllLines(opcode);
                bytesToOpcodeDict.Clear();
                if (dic.Length > 0)
                {
                    if (dic[0].Trim() == ";Ver3")
                    {
                        version = 3;
                    }
                    else if (dic[0].Trim() == ";Ver2")
                    {
                        version = 2;
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
                if (bytesToOpcodeDict.Count == 0) // 文件为空
                {
                    notNull = false;
                }
                else
                {
                    Debug.WriteLine("已加载opcode文件：{0}", Path.GetFileName(opcode));
                }

            }
            else // 文件不存在
            {
                notNull = false;
            }

            if (!notNull)
            {
                for (int i = 0; i < 128; i++)
                {
                    bytesToOpcodeDict.Add((byte)i, new ScriptOpcode((byte)i, "OP_" + ScriptUtil.Byte2Hex((byte)i)));
                }
                Debug.WriteLine("无opcode文件或无opcode，创建默认opcode");
            }
            return version;
        }
    }
}
