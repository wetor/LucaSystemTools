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
        public Guide()
        {
            InitializeComponent();
        }

        private void Guide_Load(object sender, EventArgs e)
        {
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
            ColumnHeader columnHeader = new ColumnHeader();
            columnHeader.Width = -1;    //设置列宽度 width
            columnHeader.TextAlign = HorizontalAlignment.Left;   //设置列的对齐方式 
            this.paramsList.Columns.Add(columnHeader);    //将列头添加到ListView控件

            this.paramsList.BeginUpdate();   //与EndUpdate成对使用，挂起UI，绘制控件

            typeList.Items.Clear();
            foreach (var suit in Enum.GetValues(typeof(DataType)))
            {
                if (paramsList.Items.Count >= 12) break;
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = suit.ToString();
                this.paramsList.Items.Add(listViewItem);
                typeList.Items.Add(suit.ToString());
            }

            this.paramsList.EndUpdate();

            paramsList.AllowDrop = true;
            paramsList.ItemDrag += ParamsList_ItemDrag;
            paramsList.DragEnter += ParamsList_DragEnter;
            paramsList.DragOver += ParamsList_DragOver;
            paramsList.DragDrop += ParamsList_DragDrop;
            paramsList.DragLeave += ParamsList_DragLeave;
            paramsList.MouseClick += ParamsList_MouseClick;
            // https://blog.csdn.net/weixin_30369087/article/details/99901570

            List<string> bytes = new List<string>();
            bytes.AddRange(@"
0A 80 81 09 70 EF FF A0 D3 8E 00 00 
0A 80 81 09 70 EF FF A0 D3 8E 00 00 
0A 80 81 09 70 EF FF A0 D3 8E 00 00 
0A 80 81 09 70 EF FF A0 D3 8E 00 00 
0A 80 81 09 70 EF FF A0 D3 8E 00 00".Split(' '));
            currentBytes = new byte[] {
                0x0A,0x80,0x81,0x09,0x70,0xEF,0xFF,0xA0,0xD3,0x8E,0x00,0x00,
                0x0A,0x80,0x81,0x09,0x70,0xEF,0xFF,0xA0,0xD3,0x8E,0x00,0x00,
                0x0A,0x80,0x81,0x09,0x70,0xEF,0xFF,0xA0,0xD3,0x8E,0x00,0x00,
                0x0A,0x80,0x81,0x09,0x70,0xEF,0xFF,0xA0,0xD3,0x8E,0x00,0x00,
                0x0A,0x80,0x81,0x09,0x70,0xEF,0xFF,0xA0,0xD3,0x8E,0x00,0x00
            };
            DataTable dt = new DataTable();
            for (int i = 0; i < bytes.Count; i++)
            {
                dt.Columns.Add(new DataColumn(i.ToString()));
            }
            
            DataRow dr = dt.Rows.Add();
            bytesView.DataSource = dt;

            for (int i = 0; i < bytes.Count; i++)
            {
                if (i < 5)
                {
                    bytesView.Columns[i].DefaultCellStyle.BackColor = Color.Aquamarine;
                }
                bytesView.Columns[i].Width = 25;
                dr[i] = bytes[i];
            }
            
            
            
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
        private void typeList_MouseClick(object sender, MouseEventArgs e)
        {
            if (typeList.SelectedIndex >= 0)
            {
                statusItemSelect.Text = typeList.SelectedItem.ToString();
            }
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
                if (paramsList.Items.Count > 0 && index == paramsList.Items.Count)
                {
                    index--;
                }
                paramsList.SelectedIndices.Add(index);

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

            previewList.Items[0].SubItems[1].Text = (currentBytes[index] > 127 ? 
                currentBytes[index] - 256 : currentBytes[index]).ToString();
            previewList.Items[1].SubItems[1].Text = Convert.ToByte(currentBytes[index]).ToString();

            previewList.Items[2].SubItems[1].Text = BitConverter.ToInt16(currentBytes, index).ToString();
            previewList.Items[3].SubItems[1].Text = BitConverter.ToUInt16(currentBytes, index).ToString();

            previewList.Items[4].SubItems[1].Text = BitConverter.ToInt32(currentBytes, index).ToString();
            previewList.Items[5].SubItems[1].Text = BitConverter.ToUInt32(currentBytes, index).ToString();

            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        
    }
}
