using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace ProtScript
{
    public class ScriptWriter
    {
        private FileStream fs;
        private BinaryWriter bw;

        private Dictionary<string, byte> opcodeDict = new Dictionary<string, byte>();

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
            foreach (var code in script.lines)
            {
                Console.WriteLine(code.ToString());
            }
        }
        public void LoadJson(string path)
        {
            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.DefaultValueHandling = DefaultValueHandling.Ignore;
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            script = JsonConvert.DeserializeObject<ScriptEntity>(sr.ReadToEnd(), jsetting);
            sr.Close();
        }
        public void WriteParamData()
        {

        }
    }
}
