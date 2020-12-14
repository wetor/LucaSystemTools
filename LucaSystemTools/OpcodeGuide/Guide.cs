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

namespace OpcodeGuide
{
    public partial class Guide : Form
    {

        private OpcodeEntity opcodeEntity = new OpcodeEntity();


        private byte[] currentBytes = null;

        private int scriptFileIndex = -1;

        private bool needSave = false;

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


            statusToolVersion.Text = "20201214 Alpha 1";
            foreach (ToolStripMenuItem menu in mainMenu.Items)
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
                        case "menuSave":
                            menu.DropDownItems[i].Click += MenuSave_Click;
                            break;
                        case "menuSaveAs":
                            menu.DropDownItems[i].Click += MenuSaveAs_Click;
                            break;
                        case "menuAbout":
                            menu.DropDownItems[i].Click += MenuAbout_Click;
                            break;
                        case "menuHelp":
                            menu.DropDownItems[i].Click += MenuHelp_Click;
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

            typeList.DoubleClick += typeList_DoubleClick;

            btnInsertDown.Click += btnInsertDown_Click;
            btnInsertUp.Click += btnInsertUp_Click;
            //btnMoveDown.Click += btnMoveDown_Click;
            //btnMoveUp.Click += btnMoveUp_Click;
            btnDelete.Click += btnDelete_Click;

            bytesView.CellMouseUp += bytesView_CellMouseUp;
            previewList.MouseDoubleClick += previewList_MouseDoubleClick;
            previewList.MouseClick += previewList_MouseClick;
            paramsList.ItemSelectionChanged += paramsList_ItemSelectionChanged;

            btnTextToList.Click += btnTextToList_Click;
            btnApply.Click += btnApply_Click;
            btnPreview.Click += btnPreview_Click;
            btnRestore.Click += btnRestore_Click;
            typeList.SelectedIndexChanged += typeList_SelectedIndexChanged;

            scriptList.MouseClick += scriptList_MouseClick;
            btnLoadScript.Click += btnLoadScript_Click;

            btnLoadNext.Click += btnLoadNext_Click;
            btnLoadPrev.Click += btnLoadPrev_Click;
            btnScriptJump.Click += btnScriptJump_Click;
            opcodeList.MouseClick += opcodeList_MouseClick;
            paramsList.Enter += paramsList_Enter;



            /*

            List<string> bytes = new List<string>();
            string str = @"18 1B D0 82 3C 00 13 00 60 83 4B 83 4C 83 4C 82 62 40 81 75 93 87 82 CC 90 6C 82 A9 82 C8 81 48 81 40 82 BB 81 41 8A 4F 82 CC 90 6C 8A D4 82 A9 82 C8 81 48 81 76 00 05";
            bytes.AddRange(str.Split(' '));

            currentBytes = ScriptUtil.Hex2Byte(str);
            LoadBytes(currentBytes);*/


        }
        /// <summary>
        /// 将字节数组显示到dataGridView上
        /// </summary>
        /// <param name="bytes"></param>
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
        private void MenuHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("请查看README");
        }
        private void MenuAbout_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }

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
                opcodeEntity.Version = 3;
            }
            else if (result == DialogResult.No)
            {
                opcodeEntity.Version = 2;
            }
            else
            {
                return;
            }
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                OpenOpcode(saveFile.FileName, true);
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
                OpenOpcode(openFile.FileName);
            }
        }
        private void OpenOpcode(string filename, bool isNew = false)
        {
            opcodeEntity.Filename = filename;
            this.Text = "OpcodeGuide  FileName: " + opcodeEntity.Name + (isNew ? " *" : "");
            statusScriptVersion.Text = opcodeEntity.Version.ToString();
            opcodeList.Items.Clear();
            opcodeList.Items.AddRange(opcodeEntity.OpcodeArray);
        }
        /// <summary>
        /// 打开脚本文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuOpenScript_Click(object sender, EventArgs e)
        {
            if (openFolder.ShowDialog() == DialogResult.OK)
            {
                opcodeEntity.ScriptPath = openFolder.SelectedPath;
                // 更新GUI
                scriptList.Items.Clear();
                foreach (var item in opcodeEntity.Scripts)
                {
                    scriptList.Items.Add(item.Name);
                }
            }
        }
        private void MenuSave_Click(object sender, EventArgs e)
        {
            OnSave();
        }
        private void MenuSaveAs_Click(object sender, EventArgs e)
        {
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                OnSave(saveFile.FileName);
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
            opcodeEntity.ScriptPath = "";
            scriptFileIndex = -1;
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
            if (!opcodeEntity.isOpenOpcode)
            {
                return true;
            }
            if (needSave)
            {
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
            }
            this.Text = "OpcodeGuide";
            opcodeEntity.Close();
            return true;
        }
        /// <summary>
        /// 保存或另存为函数
        /// </summary>
        /// <param name="file">另存为的文件名</param>
        private void OnSave(string file = null)
        {
            if (!opcodeEntity.isOpenOpcode)
            {
                return;
            }
            opcodeEntity.SaveOpcodeDict(file);
            if (file == null)
            {
                // 保存 后不需要保存，另存为仍需要保存
                needSave = false;
                this.Text = "OpcodeGuide  FileName: " + opcodeEntity.Name;
            }

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
            if (paramsList.SelectedItems.Count > 0)
            {
                statusItemSelect.Text = paramsList.SelectedItems[0].Text;
            }
        }
        private void typeList_DoubleClick(object sender, EventArgs e)
        {
            btnInsertDown_Click(null, null);
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

        /*private void btnMoveUp_Click(object sender, EventArgs e)
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
        }*/

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (paramsList.SelectedIndices.Count > 0)
            {
                index = paramsList.SelectedIndices[0];
                paramsList.Items.RemoveAt(index);
                paramsList.SelectedIndices.Clear();
                if (paramsList.Items.Count > 0)
                {
                    if (index == paramsList.Items.Count)
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
                if (ids[0] + ids.Length - 1 != ids[ids.Length - 1]) // 不是连续选中
                {
                    bytesView.ClearSelection();
                    lastSelectIndex = -1;
                    lastSelectCount = 0;
                    return;
                }
                //Debug.WriteLine("{0} {1} {2}", e.RowIndex, e.ColumnIndex, bytesView.SelectedCells.Count);
                UpdatePreviewList(ids[0], ids.Length);

                statusBytesSelect.Text = (int.Parse(labelScriptPos.Text) + ids[0]) + ", " + ids.Length;
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
            if (index < currentBytes.Length - 2)
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

            for (int i = 0; i < content.Length; i++)
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
            int start = opcodeEntity.CurrentBytesOffset;
            int end = 0;
            for (int i = 0; i <= index; i++)
            {
                string str = paramsList.Items[i].Text;
                while (str[0] == '!' || str[0] == '@')
                {
                    str = str.Remove(0, 1);
                }
                DataType t = (DataType)Enum.Parse(typeof(DataType), str, true);
                if ((int)t >= 10) // 字符串
                {
                    end = start = opcodeEntity.CurrentBytesOffset;
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
            bytesView.CurrentCell = bytesView.Rows[0].Cells[start];
            bytesView_CellMouseUp(null, new DataGridViewCellMouseEventArgs(start, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
            for (int i = 0; i < bytesView.Columns.Count; i++)
            {
                if (i >= start && i < end)
                {
                    bytesView.Columns[i].DefaultCellStyle.BackColor = Color.Aquamarine; // 海蓝色
                }
                else if (i >= lastTypeStart && i < lastTypeEnd) // 上次改变颜色的变回来
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
                    paramsList.Items.Add((t.export ? "@" : "") + (t.nullable ? "!" : "") + t.type);
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
            if (!(opcodeEntity.isOpenOpcode && opcodeEntity.isLoadScript))
            {
                return;
            }
            needSave = true;
            this.Text = "OpcodeGuide  FileName: " + opcodeEntity.Name + " *";
            opcodeEntity.SetOpcodeDict(listToOpcode());
            opcodeEntity.OpcodeDictRestore();
            opcodeList.Items.Clear();
            opcodeList.Items.AddRange(opcodeEntity.OpcodeArray);
            int index = opcodeEntity.CurrentCodeID;
            try
            {
                opcodeEntity.LoadScript(scriptFileIndex);
            }
            catch (Exception)
            {
                MessageBox.Show("解析脚本发生错误，清检查脚本是否可解析！");
            }

            opcodeEntity.CurrentCodeID = index;
            UpdateScriptInfo();
        }
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (!(opcodeEntity.isOpenOpcode && opcodeEntity.isLoadScript))
            {
                return;
            }

            opcodeEntity.ReReadCodeLine(listToOpcode());
            UpdateScriptInfo();
        }
        private ScriptOpcode listToOpcode()
        {
            var opcode = new ScriptOpcode((byte)opcodeEntity.CurrentOpcodeIndex);
            opcode.opcode = textOpcode.Text;
            foreach (ListViewItem item in paramsList.Items)
            {
                string str = item.Text;
                bool nullable = false, export = false;
                if (str[0] == '@')
                {
                    str = str.Remove(0, 1);
                    export = true;
                }
                if (str[0] == '!')
                {
                    str = str.Remove(0, 1);
                    nullable = true;
                }
                DataType t = (DataType)Enum.Parse(typeof(DataType), str, true);
                opcode.param.Add(new ParamType(t, nullable, export));
            }
            return opcode;
        }
        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (!(opcodeEntity.isOpenOpcode && opcodeEntity.isLoadScript))
            {
                return;
            }
            opcodeEntity.OpcodeDictRestore();
            UpdateScriptInfo();
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

        private void scriptList_MouseClick(object sender, MouseEventArgs e)
        {
            if (!opcodeEntity.isOpenFolder)
            {
                return;
            }
            scriptFileIndex = scriptList.SelectedIndex;
            UpdateScriptInfo();
        }
        private void UpdateScriptInfo()
        {
            var script = opcodeEntity.Scripts[scriptFileIndex];
            textFilename.Text = script.Name;
            labelScriptSize.Text = script.Size.ToString();
            labelScriptPos.Text = script.Position.ToString();
            labelScriptCodeID.Text = script.Index.ToString();
            if (!(opcodeEntity.isOpenOpcode && opcodeEntity.isLoadScript))
            {
                return;
            }
            byte[] bytes = opcodeEntity.GetBytes();
            LoadBytes(bytes);
            labelScriptCodeLen.Text = bytes.Length.ToString();
            labelScriptCodeNum.Text = opcodeEntity.CodeLineCount.ToString();
            textJumpIndex.Text = labelScriptCodeID.Text;


            opcodeList.SelectedIndex = opcodeEntity.CurrentCodeLine.opcodeIndex;
            var opcode = opcodeEntity.CurrentOpcode;
            textOpcode.Text = opcode.opcode;
            textParams.Text = opcode.ToStringParam();
            textView.Text = opcodeEntity.CurrentCodeLine.ToString();
            paramsList.Items.Clear();
            foreach (ParamType t in opcode.param)
            {
                paramsList.Items.Add((t.export ? "@" : "") + (t.nullable ? "!" : "") + t.type);
            }

        }
        private void btnLoadScript_Click(object sender, EventArgs e)
        {
            if (!(opcodeEntity.isOpenOpcode && scriptFileIndex >= 0))
            {
                return;
            }
            try
            {
                opcodeEntity.LoadScript(scriptFileIndex);
            }
            catch (Exception)
            {
                MessageBox.Show("解析脚本发生错误，清检查脚本是否可解析！");
            }
            UpdateScriptInfo();
        }
        private void btnLoadNext_Click(object sender, EventArgs e)
        {
            if (!(opcodeEntity.isOpenOpcode && opcodeEntity.isLoadScript))
            {
                return;
            }
            opcodeEntity.CurrentCodeID++;
            UpdateScriptInfo();
        }
        private void btnLoadPrev_Click(object sender, EventArgs e)
        {
            if (!(opcodeEntity.isOpenOpcode && opcodeEntity.isLoadScript))
            {
                return;
            }
            opcodeEntity.CurrentCodeID--;
            UpdateScriptInfo();
        }
        private void btnScriptJump_Click(object sender, EventArgs e)
        {
            if (!(opcodeEntity.isOpenOpcode && opcodeEntity.isLoadScript))
            {
                return;
            }
            int offset = opcodeEntity.CurrentBytesOffset;
            if (radioJumpIndex.Checked)
            {
                int num = int.Parse(textJumpIndex.Text);
                opcodeEntity.CurrentCodeID = num;
            }
            else if (radioJumpPosition.Checked)
            {
                int num = int.Parse(textJumpPosition.Text);
                offset = opcodeEntity.TryJumpPosition(num);
            }
            UpdateScriptInfo();
            if (radioJumpPosition.Checked)
            {
                bytesView.CurrentCell = bytesView.Rows[0].Cells[offset];
                bytesView_CellMouseUp(null, new DataGridViewCellMouseEventArgs(offset, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
            }
        }

        private void opcodeList_MouseClick(object sender, MouseEventArgs e)
        {
            if (!opcodeEntity.isOpenOpcode)
            {
                return;
            }
            textView.Text = opcodeEntity.getOpcodeDict(opcodeList.SelectedIndex).ToString();
        }


        private void paramsList_Enter(object sender, EventArgs e)
        {
            if (!(opcodeEntity.isOpenOpcode && opcodeEntity.isLoadScript))
            {
                return;
            }
            textView.Text = opcodeEntity.CurrentCodeLine.ToString();
        }
    }

}
