using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using LucaSystemTools;

namespace ProtScript
{
    /// <summary>
    /// 指令参数结构
    /// </summary>
    public struct ParamData
    {
        public Type type;
        public object value;
        public string value_string;

        public ParamData(Type _type, object _value, string _string)
        {
            type = _type;
            value = _value;
            value_string = _string;
        }
    }
    /// <summary>
    /// 指令行，信息参数
    /// </summary>
    public struct CodeInfo
    {
        public int count;
        public UInt16[] data;
        public CodeInfo(int count)
        {
            this.count = count;
            data = null;
        }
    }
    /// <summary>
    /// 指令行
    /// </summary>
    public struct CodeLine
    {

        /// <summary>
        /// 行号
        /// </summary>
        public int index;
        /// <summary>
        /// 所在脚本位置
        /// </summary>
        public int position;

        public byte opcode_index;


        public CodeLine(int index, int position)
        {
            this.index = index;
            this.position = position;

            opcode_index = 255;
            opcode = "UNKNOW";
            info = new CodeInfo();
            param_types = null;
            param_datas = null;
            codeLines = null;


        }
        public string opcode;
        public CodeInfo info;
        public ParamType[] param_types;
        public List<ParamData> param_datas;

        /// <summary>
        /// 内部指令块，IF等
        /// </summary>
        public List<CodeLine> codeLines;

    }
    public class ScriptReader
    {
        private FileStream fs;
        private BinaryReader br;
        private int ScriptVersion = 3;

        private Dictionary<byte, ScriptOpcode> opcode_dict = new Dictionary<byte, ScriptOpcode>();

        private int current_line = 0;

        private List<byte[]> code_bytes;
        // 文件位置，行号（1开始）
        private Dictionary<uint, int> code_position;
        public ScriptReader(ref FileStream tfs, ref BinaryReader tbr)
        {
            fs = tfs;
            br = tbr;
        }

        public CodeLine ReadCodeLine()
        {

            CodeLine code = new CodeLine(current_line, (int)fs.Position);
            UInt16 code_len = br.ReadUInt16();
            int code_offset = 0;
            //byte[] bytes = ReadCodeBytes();

            code.opcode_index = br.ReadByte();
            code_offset++;
            if (!opcode_dict.ContainsKey(code.opcode_index))
            {
                throw new Exception("未知的opcode!");
            }
            code.opcode = opcode_dict[code.opcode_index].opcode;

            CodeInfo info = new CodeInfo(br.ReadByte());
            code_offset++;
            info.data = new UInt16[info.count];
            for(int i = 0; i < info.count; i++)
            {
                info.data[i] = br.ReadUInt16();
                code_offset += 2;
            }
            code.info = info;
            // 参数类型列表
            code.param_types = opcode_dict[code.opcode_index].param.ToArray();
            // 参数数据
            code.param_datas = new List<ParamData>(ReadParamData(code.param_types, code_len, ref code_offset));


            
            while (code_offset + 1 < code_len)
            {
                byte[] temp = br.ReadBytes(2);
                code_offset += 2;
                code.param_datas.Add(new ParamData(Type.Byte2, temp, ScriptUtil.Byte2Hex(temp)));
            }
            if (code_offset < code_len)
            {
                byte temp = br.ReadByte();
                code_offset++;
                code.param_datas.Add(new ParamData(Type.Byte, temp, ScriptUtil.Byte2Hex(temp)));
            }


            current_line++;
            return code;
        }

        public ParamData[] ReadParamData(ParamType[] param, int code_len, ref int code_offset)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<string> retn = new List<string>();
            List<ParamData> datas = new List<ParamData>();
            int count = 0;
            bool nullable_skip = false;
            string data_str;
            foreach (var value in param)
            {
                Type type = value.type;
                count++;
                var temp_pos = fs.Position;
                switch (type)
                {
                    case Type.Byte:
                    case Type.Byte2:
                    case Type.Byte3:
                    case Type.Byte4:
                        {
                            var data_bytes = br.ReadBytes((int)type + 1);
                            data_str = ScriptUtil.Byte2Hex(data_bytes);
                            datas.Add(new ParamData(type, data_bytes, data_str));
                            break;
                        }
                    case Type.UInt16:
                        if (value.nullable && code_offset + 2 > code_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_uint16 = br.ReadUInt16();
                        data_str = data_uint16.ToString();
                        datas.Add(new ParamData(Type.UInt16, data_uint16, data_str));
                        break;
                    case Type.Int16:
                        if (value.nullable && code_offset + 2 > code_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_int16 = br.ReadInt16();
                        data_str = data_int16.ToString();
                        datas.Add(new ParamData(Type.Int16, data_int16, data_str));
                        break;
                    case Type.UInt32:
                        if (value.nullable && code_offset + 4 > code_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_uint32 = br.ReadUInt32();
                        data_str = data_uint32.ToString();
                        datas.Add(new ParamData(Type.UInt32, data_uint32, data_str));
                        break;
                    case Type.Int32:
                        if (value.nullable && code_offset + 4 > code_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        var data_int32 = br.ReadInt32();
                        data_str = data_int32.ToString();
                        datas.Add(new ParamData(Type.Int32, data_int32, data_str));
                        break;
                    case Type.StringUnicode:
                    case Type.StringSJIS:
                    case Type.StringUTF8:
                        {
                            if (type == Type.StringUnicode)
                            {
                                data_str = Encoding.Unicode.GetString(ReadStringDoubleEnd());
                                datas.Add(new ParamData(Type.StringUnicode, data_str, data_str));
                            }
                            else if (type == Type.StringSJIS)
                            {
                                data_str = Encoding.GetEncoding("Shift-Jis").GetString(ReadStringSingleEnd());
                                datas.Add(new ParamData(Type.StringSJIS, data_str, data_str));
                            }
                            else if (type == Type.StringUTF8)
                            {
                                data_str = Encoding.UTF8.GetString(ReadStringSingleEnd());
                                datas.Add(new ParamData(Type.StringUTF8, data_str, data_str));
                            }
                            break;
                        }
                    case Type.LenStringUnicode:
                    case Type.LenStringSJIS:
                        {
                            if (value.nullable && code_offset + 2 > code_len)
                            {
                                nullable_skip = true;
                                break;
                            }
                            int len = br.ReadUInt16();
                            if (type == Type.LenStringUnicode)
                            {
                                data_str = Encoding.Unicode.GetString(br.ReadBytes(len * 2));
                                datas.Add(new ParamData(Type.LenStringUnicode, data_str, data_str));
                            }
                            else if (type == Type.LenStringSJIS)
                            {
                                data_str = Encoding.GetEncoding("Shift-Jis").GetString(br.ReadBytes(len * 2));
                                datas.Add(new ParamData(Type.LenStringSJIS, data_str, data_str));
                            }
                            break;
                        }
                    case Type.Position:
                        if (value.nullable && code_offset + 4 > code_len)
                        {
                            nullable_skip = true;
                            break;
                        }
                        uint pos = br.ReadUInt32();
                        if (code_position.ContainsKey(pos))
                        {
                            datas.Add(new ParamData(Type.Position, pos, code_position[pos].ToString()));
                        }
                        else
                        {
                            throw new Exception("错误的跳转位置: " + pos + "！");
                        }
                        break;
                    default:
                        break;
                }
                code_offset += (int)(fs.Position - temp_pos);

            }
            return datas.ToArray();
        }
        private byte[] ReadCodeBytes()
        {
            //[0x00 / null] [len:UInt16] [bytes]
            //              |<-     len      ->|
            if (!CanRead()) return new byte[0];
            List<byte> datas = new List<byte>();
            int len = 0;
            if (ScriptVersion == 2)
            {
                byte opcode = br.ReadByte();
                byte data = br.ReadByte();
                len = data * 2;
                fs.Seek(-2, SeekOrigin.Current);
            }
            else if (ScriptVersion == 3)
            {
                len = br.ReadUInt16() - 2;
            }

            if (Program.debug)
                Console.WriteLine("{0}  {1}", fs.Position, len);

            datas.AddRange(br.ReadBytes(len));
            if (len % 2 != 0)
            {
                //长度非偶数，最后会有补位
                br.ReadByte();
            }
            return datas.ToArray();
        }
        private bool CanRead()
        {
            return fs.CanRead && fs.Position < fs.Length;
        }
        private byte[] ReadStringDoubleEnd()
        {
            List<byte> buff = new List<byte>();
            byte[] btmp = br.ReadBytes(2);
            while (!(btmp[0] == 0x00 && btmp[1] == 0x00))
            {
                buff.AddRange(btmp);
                btmp = br.ReadBytes(2);
            }
            return buff.ToArray();
        }
        private byte[] ReadStringSingleEnd()
        {
            List<byte> buff = new List<byte>();
            byte btmp = br.ReadByte();
            while (btmp != 0x00)
            {
                buff.Add(btmp);
                btmp = br.ReadByte();
            }
            return buff.ToArray();
        }

    }
}
