using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using LucaSystemTools;
using ProtScript.Entity;

namespace ProtScript
{
    public class ScriptWriter
    {
        private FileStream fs;
        private BinaryWriter bw;

        private Dictionary<string, byte> opcodeDict = new Dictionary<string, byte>();

        /// <summary>
        /// 第一遍写入生成dictLabel<label, pos>
        /// </summary>
        private Dictionary<string, uint> dictLabel = new Dictionary<string, uint>();
        /// <summary>
        /// 写入完后，遍历dictGoto<pos, label>在pos写入dictLabel[label]
        /// </summary>
        private Dictionary<int, string> dictGoto = new Dictionary<int, string>();

        private ScriptEntity script = new ScriptEntity();

        public ScriptWriter(string outpath, Dictionary<string, byte> dict)
        {
            fs = new FileStream(outpath, FileMode.Create);
            bw = new BinaryWriter(fs);
            opcodeDict = dict;
        }
        public void Close()
        {
            bw.Close();
            fs.Close();
        }
        public void WriteScript()
        {
            WriteParamData();
            foreach (var code in script.lines)
            {
                if (Program.debug)
                {
                    Console.WriteLine(fs.Position);
                    Console.WriteLine(code.ToString());
                }
                if (code.isLabel)
                {
                    dictLabel.Add(code.label, (uint)fs.Position);
                }
                int codeLen = (int)fs.Position;

                if(script.version == 2)
                {
                    bw.Write(opcodeDict[code.opcode]);
                    bw.Write((byte)0x00);//长度填充
                    bw.Write(code.info.ToBytes(2));
                }
                else if(script.version == 3)
                {
                    bw.Write(new byte[2]);//长度填充
                    bw.Write(opcodeDict[code.opcode]);
                    if (code.opcode == "END")
                    {
                        code.info.count++;
                    }
                    bw.Write(code.info.ToBytes());
                }
                foreach (var param in code.paramDatas)
                {
                    if (param.bytes == null)
                    {
                        throw new Exception("语句解析错误！" + code.ToString() + "  参数为null！" + param.valueString);
                    }
                    if(code.isPosition && param.type == DataType.Position)
                    {
                        dictGoto.Add((int)fs.Position, param.valueString);
                        bw.Write(param.bytes);
                    }
                    else
                    {
                        bw.Write(param.bytes);
                    }
                }
                codeLen = (int)fs.Position - codeLen;
                if (script.version == 2)
                {
                    fs.Seek(- codeLen + 1, SeekOrigin.Current);
                    bw.Write((byte)Math.Ceiling(codeLen / 2.0));
                }
                else if (script.version == 3)
                {
                    fs.Seek(-codeLen, SeekOrigin.Current);
                    bw.Write(BitConverter.GetBytes((UInt16)codeLen));
                }
                fs.Seek(codeLen - 2, SeekOrigin.Current);
                if (codeLen % 2 != 0)
                {
                    bw.Write((byte)0x00);
                }
            }
            foreach (KeyValuePair<int, string> gotokv in dictGoto)
            {
                fs.Seek(gotokv.Key, SeekOrigin.Begin);
                bw.Write(BitConverter.GetBytes(dictLabel[gotokv.Value]));
            }

        }
        private void WriteParamData()
        {
            for (int line = 0; line < script.lines.Count; line++)
            {
                for (int index = 0; index < script.lines[line].paramDatas.Count; index++)
                {
                    var paramData = script.lines[line].paramDatas[index];
                    script.lines[line].paramDatas[index] = ScriptEntity.ToParamData(paramData.valueString, paramData.type);
                }
            }
        }
        public void LoadJson(string path)
        {
            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.DefaultValueHandling = DefaultValueHandling.Ignore;
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            script = JsonConvert.DeserializeObject<ScriptEntity>(sr.ReadToEnd(), jsetting);
            sr.Close();
            if (script.toolVersion > Program.toolVersion)
            {
                throw new Exception(String.Format("Tool version is {0}, but this file version is {1}!", Program.toolVersion, script.toolVersion));
            }

        }
        public void LoadLua(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            script.lines.Clear();
            script.toolVersion = uint.MaxValue;
            if (sr.BaseStream.Length > 0)
            {
                string[] verstr = sr.ReadLine().Split(':');
                if (verstr.Length == 2)
                {
                    script.toolVersion = Convert.ToUInt32(verstr[1].Trim());
                }
                verstr = sr.ReadLine().Split(':');
                if (verstr.Length == 2)
                {
                    script.version = Convert.ToInt32(verstr[1].Trim());
                }
            }
            if (script.toolVersion > Program.toolVersion)
            {
                throw new Exception(String.Format("Tool version is {0}, but this file version is {1}!", Program.toolVersion, script.toolVersion));
            }
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                script.lines.Add(new CodeLine(line));
            }
            sr.Close();
        }
        
    }
}
