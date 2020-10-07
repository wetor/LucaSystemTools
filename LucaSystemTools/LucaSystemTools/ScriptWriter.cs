using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using LucaSystemTools;

namespace ProtScript
{
    public class ScriptWriter
    {
        private FileStream fs;
        private BinaryWriter bw;

        private Dictionary<string, byte> opcodeDict = new Dictionary<string, byte>();

        /// <summary>
        /// 第一遍写入生成dictFlag<flag, pos>
        /// </summary>
        private Dictionary<int, uint> dictFlag = new Dictionary<int, uint>();
        /// <summary>
        /// 写入完后，遍历dictGoto<pos, flag>在pos写入dictFlag[flag]
        /// </summary>
        private Dictionary<int, int> dictGoto = new Dictionary<int, int>();

        private ScriptEntity script = new ScriptEntity();

        public ScriptWriter(FileStream tfs, BinaryWriter tbw, Dictionary<byte, ScriptOpcode> dict)
        {
            fs = tfs;
            bw = tbw;
            foreach (KeyValuePair<byte, ScriptOpcode> kvp in dict)
            {
                opcodeDict.Add(kvp.Value.opcode, kvp.Key);
            }
            //opcodeDict = dict;
        }
        public void WriteScript()
        {
            WriteParamData();
            foreach (var code in script.lines)
            {
                if (code.isFlag)
                {
                    dictFlag.Add(code.flagId, (uint)fs.Position);
                }
                int codeLen = (int)fs.Position;

                if(script.version == 2)
                {
                    bw.Write(opcodeDict[code.opcode]);
                    bw.Write((byte)0x00);//长度填充
                }
                else if(script.version == 3)
                {
                    bw.Write(new byte[2]);//长度填充
                    bw.Write(opcodeDict[code.opcode]);
                    bw.Write(code.info.ToBytes());
                }
                
                foreach (var param in code.paramDatas)
                {
                    if(code.isGoto && param.type == DataType.Position)
                    {
                        dictGoto.Add((int)fs.Position, (int)(uint)param.value);
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
            foreach (KeyValuePair<int, int> gotokv in dictGoto)
            {
                fs.Seek(gotokv.Key, SeekOrigin.Begin);
                bw.Write(BitConverter.GetBytes(dictFlag[gotokv.Value]));
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
        public void WriteParamData()
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
    }
}
