using ProtScript;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace TestWFCore
{
    public partial class Guide : Form
    {
        private string opcodeFile = "";
        private bool isOpenOpcode => opcodeFile != ""; // 是否打开了OPCODE

        private List<string> scriptFile = new List<string>();
        private bool isOpenScript => scriptFile.Count > 0; // 是否打开了脚本

        private Dictionary<byte, ScriptOpcode> bytesToOpcodeDict = new Dictionary<byte, ScriptOpcode>();
        private int scriptVersion = 3;


        private byte[] currentBytes = null;

        private Dictionary<DataType, int> typeSize = new Dictionary<DataType, int>();

        private Dictionary<DataType, string> typeTip = new Dictionary<DataType, string>();
        public Guide()
        {
            InitializeComponent();
        }

        private void Guide_Load(object sender, EventArgs e)
        {
            typeSize.Add(DataType.Null, 0);
            typeTip.Add(DataType.Null, "占位用\n长度：0");
            typeSize.Add(DataType.Byte, 1);
            typeTip.Add(DataType.Byte, "单字节类型\n长度：1");
            typeSize.Add(DataType.Byte2, 2);
            typeTip.Add(DataType.Byte2, "双字节类型\n长度：2");
            typeSize.Add(DataType.Byte3, 3);
            typeTip.Add(DataType.Byte3, "三字节类型\n长度：3");
            typeSize.Add(DataType.Byte4, 4);
            typeTip.Add(DataType.Byte4, "四字节类型\n长度：4");

            typeSize.Add(DataType.Int16, 2);
            typeTip.Add(DataType.Int16, "有符号短整数类型\n长度：2\n值域：[-32768, 32767]");

            typeSize.Add(DataType.UInt16, 2);
            typeTip.Add(DataType.UInt16, "无符号短整数类型\n长度：2\n值域：[0, 65535]");

            typeSize.Add(DataType.Int32, 4);
            typeTip.Add(DataType.Int32, "有符号整数类型\n长度：4\n值域：[-2^31, 2^31-1]");

            typeSize.Add(DataType.UInt32, 4);
            typeTip.Add(DataType.UInt32, "无符号整数类型\n长度：4\n值域：[0, 2^32-1]");

            typeSize.Add(DataType.Position, 4);
            typeTip.Add(DataType.Position, "跳转地址，无符号整数类型\n长度：4\n值域：[0, {fileSize}]");

            typeTip.Add(DataType.StringUnicode, "Unicode编码的字符串\n长度：以[0x0000]结尾");
            typeTip.Add(DataType.StringSJIS, "日文Shift-JIS编码的字符串\n长度：以[0x00]结尾");
            typeTip.Add(DataType.StringUTF8, "UTF-8编码的字符串\n长度：以[0x00]结尾");
            typeTip.Add(DataType.StringCustom, "自定义编码的字符串\n长度：以[0x00]结尾\n说明：通常为替换的Shift-JIS编码");
            
            typeTip.Add(DataType.LenStringUnicode, "有长度Unicode编码的字符串\n长度：字符串前UInt16数据");
            typeTip.Add(DataType.LenStringSJIS, "有长度日文Shift-JIS编码的字符串\n长度：字符串前UInt16数据");
            typeTip.Add(DataType.LenStringUTF8, "[未定义]有长度UTF-8编码的字符串\n长度：未定义");
            typeTip.Add(DataType.LenStringCustom, "有长度自定义编码的字符串\n长度：字符串前UInt16数据\n说明：通常为替换的Shift-JIS编码");


            statusToolVersion.Text = "20201212";
            foreach(ToolStripMenuItem menu in mainMenu.Items)
            {
                for (int i = 0; i < menu.DropDownItems.Count; i++)
                {
                    switch (menu.DropDownItems[i].Name)
                    {
                        case "menuNew":
                            menu.DropDownItems[i].Click += MenuNew_Click;
                            break;
                        case "menuOpen":
                            menu.DropDownItems[i].Click += MenuOpen_Click;
                            break;
                        case "menuOpenScript":
                            menu.DropDownItems[i].Click += MenuOpenScript_Click;
                            break;
                        case "menuClose":
                            menu.DropDownItems[i].Click += MenuClose_Click;
                            break;
                        case "menuCloseScript":
                            menu.DropDownItems[i].Click += MenuCloseScript_Click;
                            break;
                        case "menuExit":
                            menu.DropDownItems[i].Click += MenuExit_Click;
                            break;
                        default:
                            break;
                    }
                }
            }



            typeList.Items.Clear();
            foreach (var t in Enum.GetValues(typeof(DataType)))
            {
                typeList.Items.Add(t.ToString());
            }

            paramsList.AllowDrop = true;
            paramsList.ItemDrag += ParamsList_ItemDrag;
            paramsList.DragEnter += ParamsList_DragEnter;
            paramsList.DragOver += ParamsList_DragOver;
            paramsList.DragDrop += ParamsList_DragDrop;
            paramsList.DragLeave += ParamsList_DragLeave;
            paramsList.MouseClick += ParamsList_MouseClick;
            // https://blog.csdn.net/weixin_30369087/article/details/99901570

            List<string> bytes = new List<string>();
            string str = @"18 1B D0 82 3C 00 13 00 60 83 4B 83 4C 83 4C 82 62 40 81 75 93 87 82 CC 90 6C 82 A9 82 C8 81 48 81 40 82 BB 81 41 8A 4F 82 CC 90 6C 8A D4 82 A9 82 C8 81 48 81 76 00 05";
            bytes.AddRange(str.Split(' '));

            currentBytes = ScriptUtil.Hex2Byte(str);
            LoadBytes(currentBytes);


        }

        private void LoadBytes(byte[] bytes)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < bytes.Length; i++)
            {
                dt.Columns.Add(new DataColumn(i.ToString()));
            }
            DataRow dr = dt.Rows.Add();
            bytesView.DataSource = dt;
            Graphics currentGraphics = Graphics.FromHwnd(this.Handle);
            double dpixRatio = currentGraphics.DpiX / 96;
            int w = (int)Math.Ceiling(25 * dpixRatio);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytesView.Columns[i].Width = w;
                dr[i] = bytes[i].ToString("X2");
            }
            currentBytes = bytes;
        }

        #region 菜单栏 File
        /// <summary>
        /// 新建OPCODE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuNew_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("选择游戏引擎版本\n【是：3】\t【否：2】",
                    "新建OPCODE", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                scriptVersion = 3;
            }
            else if (result == DialogResult.No)
            {
                scriptVersion = 2;
            }
            else
            {
                return;
            }
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                opcodeFile = saveFile.FileName;
                this.Text = "OpcodeGuide  FileName: " + opcodeFile + " *"; // 未保存标记
                LoadOpcode();
                statusScriptVersion.Text = scriptVersion.ToString();
            }
        }
        /// <summary>
        /// 打开OPCODE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuOpen_Click(object sender, EventArgs e)
        {
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                opcodeFile = openFile.FileName;
                this.Text = "OpcodeGuide  FileName: " + opcodeFile;
                scriptVersion = LoadOpcode(opcodeFile);
                statusScriptVersion.Text = scriptVersion.ToString();
            }
        }
        /// <summary>
        /// 载入多个脚本文件用于测试OPCODE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuOpenScript_Click(object sender, EventArgs e)
        {
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                scriptFile.AddRange(openFile.FileNames);
                // 更新GUI
            }
        }
        /// <summary>
        /// 初始化程序界面
        /// </summary>
        private void InitGuide()
        {

        }
        /// <summary>
        /// 关闭全部脚本文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuCloseScript_Click(object sender, EventArgs e)
        {
            scriptFile.Clear();
            InitGuide();
        }
        /// <summary>
        /// 关闭此OPCODE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuClose_Click(object sender, EventArgs e)
        {
            OnClose();
            InitGuide();
        }


        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuExit_Click(object sender, EventArgs e)
        {
            OnClose();
            Application.Exit();
            Environment.Exit(0);
        }


        #endregion
        /// <summary>
        /// 关闭前执行，确认是否需要保存
        /// </summary>
        /// <returns>True:可以关闭；False:不可以关闭</returns>
        private bool OnClose()
        {
            if (!isOpenOpcode)
            {
                return true;
            }
            // 提示是否保存
            var result = MessageBox.Show("数据未保存，是否保存？",
                    "未保存！", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                OnSave();
            }
            else if (result == DialogResult.Cancel)
            {
                return false;
            }

            this.Text = "OpcodeGuide";
            opcodeFile = "";
            scriptFile.Clear();
            return true;
        }
        /// <summary>
        /// 保存或另存为函数
        /// </summary>
        /// <param name="file">另存为的文件名</param>
        private void OnSave(string file = null)
        {
            if (!isOpenOpcode)
            {
                return;
            }

            if (file != null)
            {
                // SaveAs 另存为
            }
            else
            {
                // Save 保存
            }
        }

        /// <summary>
        /// 创建或载入OPCODE
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        private int LoadOpcode(string opcode = "")
        {
            int version = 3;
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
                if (bytesToOpcodeDict.Count == 0)
                {
                    notNull = false;
                }
                else
                {
                    Debug.WriteLine("已加载opcode文件：{0}", Path.GetFileName(opcode));
                }

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
        /// <summary>
        /// 窗口即将关闭时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Guide_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !OnClose(); // Cancel = true 取消关闭（窗口不会关闭）
        }
        private void ParamsList_MouseClick(object sender, EventArgs e)
        {
            if(paramsList.SelectedItems.Count > 0)
            {
                statusItemSelect.Text = paramsList.SelectedItems[0].Text;
            }
        }
        private void typeList_DoubleClick(object sender, EventArgs e)
        {
            btnInsertDown_Click(sender, e);
        }

        #region ParamsList 拖拽排序
        private void ParamsList_DragLeave(object sender, EventArgs e)
        {
            paramsList.InsertionMark.Index = -1;
        }

        private void ParamsList_DragDrop(object sender, DragEventArgs e)
        {
            int targetIndex = paramsList.InsertionMark.Index;
            if (targetIndex == -1)
                return;
            if (paramsList.InsertionMark.AppearsAfterItem)
                targetIndex++;

            ListViewItem draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            paramsList.BeginUpdate();
            paramsList.Items.Insert(targetIndex, (ListViewItem)draggedItem.Clone());
            paramsList.Items.Remove(draggedItem);
            paramsList.EndUpdate();
        }

        private void ParamsList_DragOver(object sender, DragEventArgs e)
        {
            Point ptScreen = new Point(e.X, e.Y);
            Point pt = paramsList.PointToClient(ptScreen);
            ListViewItem item = paramsList.GetItemAt(pt.X, pt.Y);

            int targetIndex = paramsList.InsertionMark.NearestIndex(pt);
            if (targetIndex > -1)
            {
                Rectangle itemBounds = paramsList.GetItemRect(targetIndex);
                paramsList.InsertionMark.AppearsAfterItem = pt.X > itemBounds.Left + (itemBounds.Width / 2);
                    //|| pt.Y > itemBounds.Top + (itemBounds.Height / 2);
            }
            paramsList.InsertionMark.Index = targetIndex;
        }

        private void ParamsList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void ParamsList_ItemDrag(object sender, ItemDragEventArgs e)
        {
            paramsList.DoDragDrop(e.Item, DragDropEffects.Move);
        }
        #endregion 

        private void btnInsertDown_Click(object sender, EventArgs e)
        {
            string selectStr = (string)typeList.SelectedItem;
            if (selectStr == null)
                return;
            if (paramsList.Items.Count >= 12)
            {
                MessageBox.Show("最多12个数据！");
                return;
            }
            if (checkNullable.Checked)
            {
                selectStr = "!" + selectStr;
            }
            if (checkExport.Checked)
            {
                selectStr = "@" + selectStr;
            }
            if (paramsList.SelectedIndices.Count > 0)
            {
                int index = paramsList.SelectedIndices[0] + 1;
                paramsList.Items.Insert(index, selectStr);
                paramsList.SelectedIndices.Clear();
                paramsList.SelectedIndices.Add(index);
            }
            else
            {
                paramsList.Items.Add(selectStr);
                paramsList.SelectedIndices.Add(paramsList.Items.Count - 1);
            }
        }

        private void btnInsertUp_Click(object sender, EventArgs e)
        {
            string selectStr = (string)typeList.SelectedItem;
            if (selectStr == null)
                return;
            if (paramsList.Items.Count >= 12)
            {
                MessageBox.Show("最多12个数据！");
                return;
            }
            if (checkNullable.Checked)
            {
                selectStr = "!" + selectStr;
            }
            if (checkExport.Checked)
            {
                selectStr = "@" + selectStr;
            }
            if (paramsList.SelectedIndices.Count > 0)
            {
                int index = paramsList.SelectedIndices[0];
                paramsList.Items.Insert(index, selectStr);
                paramsList.SelectedIndices.Clear();
                paramsList.SelectedIndices.Add(index);
            }
            else
            {
                paramsList.Items.Add(selectStr);
                paramsList.SelectedIndices.Add(paramsList.Items.Count - 1);
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (paramsList.SelectedIndices.Count > 0 && (index = paramsList.SelectedIndices[0]) > 0)
            {
                ListViewItem item = (ListViewItem)paramsList.Items[index].Clone();
                paramsList.Items.RemoveAt(index);
                paramsList.Items.Insert(index - 1, item);
                paramsList.SelectedIndices.Clear();
                paramsList.SelectedIndices.Add(index - 1);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (paramsList.SelectedIndices.Count > 0 &&
                (index = paramsList.SelectedIndices[0]) < paramsList.Items.Count - 1) 
            {
                ListViewItem item = (ListViewItem)paramsList.Items[index].Clone();
                paramsList.Items.RemoveAt(index);
                paramsList.Items.Insert(index + 1, item);
                paramsList.SelectedIndices.Clear();
                paramsList.SelectedIndices.Add(index + 1);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (paramsList.SelectedIndices.Count > 0)
            {
                index = paramsList.SelectedIndices[0];
                paramsList.Items.RemoveAt(index);
                paramsList.SelectedIndices.Clear();
                if (paramsList.Items.Count > 0 )
                {
                    if(index == paramsList.Items.Count)
                    {
                        index--;
                    }
                    paramsList.SelectedIndices.Add(index);
                }
                

            }
        }

        private int lastSelectIndex = -1;
        private int lastSelectCount = 0;
        private void bytesView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                bytesView.ClearSelection();
                lastSelectIndex = -1;
                lastSelectCount = 0;
            }
            else if (e.Button == MouseButtons.Left 
                && !(e.ColumnIndex == lastSelectIndex && bytesView.SelectedCells.Count == lastSelectCount)) 
            {

                int[] ids = new int[bytesView.SelectedCells.Count];
                for (int i = 0; i < bytesView.SelectedCells.Count; i++) 
                {
                    ids[i] = bytesView.SelectedCells[i].ColumnIndex;
                }
                Array.Sort(ids);
                if(ids[0] + ids.Length - 1 != ids[ids.Length - 1]) // 不是连续选中
                {
                    bytesView.ClearSelection();
                    lastSelectIndex = -1;
                    lastSelectCount = 0;
                    return;
                }
                //Debug.WriteLine("{0} {1} {2}", e.RowIndex, e.ColumnIndex, bytesView.SelectedCells.Count);
                UpdatePreviewList(ids[0], ids.Length);
                statusBytesSelect.Text = ids[0] + "-" + ids[ids.Length - 1] + ", " + ids.Length;
                lastSelectIndex = e.ColumnIndex;
                lastSelectCount = bytesView.SelectedCells.Count;
            }
            
            
        }
        private void UpdatePreviewList(int index, int count)
        {
            string[] content = new string[previewList.Items.Count];
            content[0] = (currentBytes[index] > 127 ? 
                currentBytes[index] - 256 : currentBytes[index]).ToString();
            content[1] = Convert.ToByte(currentBytes[index]).ToString();
            if(index< currentBytes.Length - 2)
            {
                content[2] = BitConverter.ToInt16(currentBytes, index).ToString();
                content[3] = BitConverter.ToUInt16(currentBytes, index).ToString();
                if (index < currentBytes.Length - 4)
                {
                    content[4] = BitConverter.ToInt32(currentBytes, index).ToString();
                    content[5] = BitConverter.ToUInt32(currentBytes, index).ToString();
                }
            }
            if (count >= 2)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                content[6] = Encoding.GetEncoding("Shift-Jis").GetString(currentBytes, index, count);
                content[7] = Encoding.UTF8.GetString(currentBytes, index, count);
                content[8] = Encoding.Unicode.GetString(currentBytes, index, count);
            }

            for(int i = 0; i < content.Length; i++)
            {
                previewList.Items[i].SubItems[1].Text = content[i];
            }
        }

        private void previewList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string strText = previewList.GetItemAt(e.X, e.Y).SubItems[1].Text;
            Clipboard.SetDataObject(strText);
        }

        private void previewList_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = previewList.GetItemAt(e.X, e.Y);
            if (item.Index >= 6)
            {
                textView.Text = item.SubItems[1].Text;
            }
            
        }

        private int lastTypeStart = 0;
        private int lastTypeEnd = 0;
        private void paramsList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            
            int index = e.ItemIndex;
            int start = 0;
            int end = 0;
            for (int i = 0; i <= index; i++)
            {
                string str = paramsList.Items[i].Text;
                while(str[0] == '!' || str[0] == '@')
                {
                    str = str.Remove(0, 1);
                }
                DataType t = (DataType)Enum.Parse(typeof(DataType), str, true);
                if ((int)t >= 10) // 字符串
                {
                    end = start = 0;
                    break;
                }
                if (i == index)
                {
                    end = start + typeSize[t];
                }
                else
                {
                    start += typeSize[t];
                }

            }
            for (int i = 0; i < bytesView.Columns.Count; i++)
            {
                if (i >= start && i < end)
                {
                    bytesView.Columns[i].DefaultCellStyle.BackColor = Color.Aquamarine; // 海蓝色
                }
                else if(i >= lastTypeStart && i < lastTypeEnd) // 上次改变颜色的变回来
                {
                    bytesView.Columns[i].DefaultCellStyle.BackColor = Color.White;
                }
            }
            lastTypeStart = start;
            lastTypeEnd = end;
        }

        private void btnTextToList_Click(object sender, EventArgs e)
        {
            try
            {
                var code = new ScriptOpcode((byte)0, textOpcode.Text + "(" + textParams.Text + ")");
                if (code.param.Count > 12)
                {
                    throw new Exception();
                }
                paramsList.Clear();
                foreach (ParamType t in code.param)
                {
                    paramsList.Items.Add(t.type.ToString());
                }
            }
            catch (Exception)
            {
                MessageBox.Show("格式错误！\n参数类型不区分大小写，使用英文','分隔，最多12个参数。");
                return;
            }
            
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            var code = new ScriptOpcode((byte)0);

        }

        private void typeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            DataType t = (DataType)Enum.Parse(typeof(DataType), typeList.SelectedItem.ToString(), true);
            if ((int)t < 10) // 非字符串
            {
                checkExport.Checked = false;
                checkExport.Enabled = false;
            }
            else
            {
                checkExport.Enabled = true;
            }
            textView.Text = typeTip[t];
        }
    }
}
