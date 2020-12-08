using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProtScript;
using System.Diagnostics;

namespace OpcodeGuide
{
    public class ScriptInfo
    {
        private string path;
        public string Name { get; set; }
        public string Filename => Path.Combine(path, Name);

        public int Size { get; }
        public int Position { get; set; }

        public ScriptInfo(string filename)
        {
            var info = new FileInfo(filename);
            path = info.DirectoryName;
            Name = info.Name;
            Size = (int)info.Length;
            Position = 0;
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
    /// 
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
                Scripts.Clear();
                var files = Directory.GetFiles(value);
                foreach (var file in files)
                {
                    Scripts.Add(new ScriptInfo(file));
                }
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
                index = value;
                LoadScript(index);
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





        private Dictionary<byte, ScriptOpcode> bytesToOpcodeDict = new Dictionary<byte, ScriptOpcode>();
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
            reader = new ScriptReader(ScriptFilename(index), bytesToOpcodeDict, scriptVersion);
            reader.ReadScript_Clear();
            // 载入脚本
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
