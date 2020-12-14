namespace OpcodeGuide
{
    partial class Guide
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Guide));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Signed Byte",
            "0"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Unsigned Byte",
            "0"}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "Signed Int16",
            "0"}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "Unsigned Int16",
            "0"}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "Signed Int32",
            "0"}, -1);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "Unsigned Int32",
            "0"}, -1);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "String SJIS",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
            "String UTF8",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
            "String Unicode",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] {
            "String Custom",
            ""}, -1);
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.file = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenScript = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.help = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusToolVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusScriptVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusItemSelect = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBytesSelect = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel_script_ctl = new System.Windows.Forms.Panel();
            this.btnScriptJump = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.textJumpPosition = new System.Windows.Forms.TextBox();
            this.textJumpIndex = new System.Windows.Forms.TextBox();
            this.btnLoadNext = new System.Windows.Forms.Button();
            this.btnLoadPrev = new System.Windows.Forms.Button();
            this.radioJumpPosition = new System.Windows.Forms.RadioButton();
            this.radioJumpIndex = new System.Windows.Forms.RadioButton();
            this.labelScriptCodeNum = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.labelScriptCodeLen = new System.Windows.Forms.Label();
            this.labelScriptCodeID = new System.Windows.Forms.Label();
            this.labelScriptPos = new System.Windows.Forms.Label();
            this.btnLoadScript = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelScriptSize = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textFilename = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.scriptList = new System.Windows.Forms.ListBox();
            this.opcodeList = new System.Windows.Forms.ListBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel_paramList_btn = new System.Windows.Forms.Panel();
            this.panel_paramList = new System.Windows.Forms.Panel();
            this.paramsList = new System.Windows.Forms.ListView();
            this.panel_paramList_right = new System.Windows.Forms.Panel();
            this.panel_typeList_btn = new System.Windows.Forms.Panel();
            this.btnInsertUp = new System.Windows.Forms.Button();
            this.checkNullable = new System.Windows.Forms.CheckBox();
            this.checkExport = new System.Windows.Forms.CheckBox();
            this.btnInsertDown = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.panel_typeList_label = new System.Windows.Forms.Panel();
            this.panel_typeList = new System.Windows.Forms.Panel();
            this.typeList = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel_previewList_label = new System.Windows.Forms.Panel();
            this.panel_previewList = new System.Windows.Forms.Panel();
            this.previewList = new System.Windows.Forms.ListView();
            this.type = new System.Windows.Forms.ColumnHeader();
            this.value = new System.Windows.Forms.ColumnHeader();
            this.label4 = new System.Windows.Forms.Label();
            this.panel_opcode_textbox_label = new System.Windows.Forms.Panel();
            this.panel_opcode_textbox = new System.Windows.Forms.Panel();
            this.panel_opcode_textbox_param = new System.Windows.Forms.Panel();
            this.textParams = new System.Windows.Forms.TextBox();
            this.panel_opcode_textbox_name = new System.Windows.Forms.Panel();
            this.textOpcode = new System.Windows.Forms.TextBox();
            this.btnTextToList = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.bytesView = new System.Windows.Forms.DataGridView();
            this.textView = new System.Windows.Forms.RichTextBox();
            this.openFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.panel_window_left = new System.Windows.Forms.Panel();
            this.panel_window_left_bottom = new System.Windows.Forms.Panel();
            this.panel_window_left_top = new System.Windows.Forms.Panel();
            this.panel_window_right = new System.Windows.Forms.Panel();
            this.panel_window_right_bottom = new System.Windows.Forms.Panel();
            this.panel_window_right_top = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel_window = new System.Windows.Forms.Panel();
            this.mainMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel_script_ctl.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel_paramList_btn.SuspendLayout();
            this.panel_paramList.SuspendLayout();
            this.panel_paramList_right.SuspendLayout();
            this.panel_typeList_btn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.panel_typeList_label.SuspendLayout();
            this.panel_typeList.SuspendLayout();
            this.panel_previewList_label.SuspendLayout();
            this.panel_previewList.SuspendLayout();
            this.panel_opcode_textbox_label.SuspendLayout();
            this.panel_opcode_textbox.SuspendLayout();
            this.panel_opcode_textbox_param.SuspendLayout();
            this.panel_opcode_textbox_name.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bytesView)).BeginInit();
            this.panel_window_left.SuspendLayout();
            this.panel_window_left_bottom.SuspendLayout();
            this.panel_window_left_top.SuspendLayout();
            this.panel_window_right.SuspendLayout();
            this.panel_window_right_bottom.SuspendLayout();
            this.panel_window_right_top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel_window.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.file,
            this.help});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(988, 25);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // file
            // 
            this.file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNew,
            this.menuOpen,
            this.menuOpenScript,
            this.toolStripSeparator,
            this.menuSave,
            this.menuSaveAs,
            this.toolStripSeparator1,
            this.menuClose,
            this.menuExit});
            this.file.Name = "file";
            this.file.Size = new System.Drawing.Size(39, 21);
            this.file.Text = "&File";
            // 
            // menuNew
            // 
            this.menuNew.Image = ((System.Drawing.Image)(resources.GetObject("menuNew.Image")));
            this.menuNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuNew.Name = "menuNew";
            this.menuNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuNew.Size = new System.Drawing.Size(206, 22);
            this.menuNew.Text = "&New Opcode";
            // 
            // menuOpen
            // 
            this.menuOpen.Image = ((System.Drawing.Image)(resources.GetObject("menuOpen.Image")));
            this.menuOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOpen.Size = new System.Drawing.Size(206, 22);
            this.menuOpen.Text = "&Open Opcode";
            // 
            // menuOpenScript
            // 
            this.menuOpenScript.Name = "menuOpenScript";
            this.menuOpenScript.Size = new System.Drawing.Size(206, 22);
            this.menuOpenScript.Text = "Open Script";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(203, 6);
            // 
            // menuSave
            // 
            this.menuSave.Image = ((System.Drawing.Image)(resources.GetObject("menuSave.Image")));
            this.menuSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuSave.Name = "menuSave";
            this.menuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSave.Size = new System.Drawing.Size(206, 22);
            this.menuSave.Text = "&Save";
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Name = "menuSaveAs";
            this.menuSaveAs.Size = new System.Drawing.Size(206, 22);
            this.menuSaveAs.Text = "Save &As";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(203, 6);
            // 
            // menuClose
            // 
            this.menuClose.Name = "menuClose";
            this.menuClose.Size = new System.Drawing.Size(206, 22);
            this.menuClose.Text = "Close Opcode";
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(206, 22);
            this.menuExit.Text = "E&xit";
            // 
            // help
            // 
            this.help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelp,
            this.toolStripSeparator5,
            this.menuAbout});
            this.help.Name = "help";
            this.help.Size = new System.Drawing.Size(47, 21);
            this.help.Text = "&Help";
            // 
            // menuHelp
            // 
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(127, 22);
            this.menuHelp.Text = "&Contents";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(124, 6);
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(127, 22);
            this.menuAbout.Text = "&About...";
            // 
            // openFile
            // 
            this.openFile.DefaultExt = "txt";
            this.openFile.FileName = "openFileDialog1";
            // 
            // saveFile
            // 
            this.saveFile.DefaultExt = "txt";
            this.saveFile.FileName = "OPCODE";
            this.saveFile.Filter = "文本文件(*.txt)|*.txt|所有文件|*.*";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.statusToolVersion,
            this.toolStripStatusLabel2,
            this.statusScriptVersion,
            this.toolStripStatusLabel4,
            this.statusItemSelect,
            this.toolStripStatusLabel3,
            this.statusBytesSelect});
            this.statusStrip1.Location = new System.Drawing.Point(0, 607);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(988, 27);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(57, 22);
            this.toolStripStatusLabel1.Text = "ToolVer:";
            // 
            // statusToolVersion
            // 
            this.statusToolVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusToolVersion.Name = "statusToolVersion";
            this.statusToolVersion.Size = new System.Drawing.Size(68, 22);
            this.statusToolVersion.Text = "20201212";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(64, 22);
            this.toolStripStatusLabel2.Text = "ScriptVer:";
            // 
            // statusScriptVersion
            // 
            this.statusScriptVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusScriptVersion.Name = "statusScriptVersion";
            this.statusScriptVersion.Size = new System.Drawing.Size(19, 22);
            this.statusScriptVersion.Text = "3";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(71, 22);
            this.toolStripStatusLabel4.Text = "ItemSelect:";
            // 
            // statusItemSelect
            // 
            this.statusItemSelect.AutoSize = false;
            this.statusItemSelect.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusItemSelect.Name = "statusItemSelect";
            this.statusItemSelect.Size = new System.Drawing.Size(128, 22);
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(76, 22);
            this.toolStripStatusLabel3.Text = "BytesSelect:";
            // 
            // statusBytesSelect
            // 
            this.statusBytesSelect.AutoSize = false;
            this.statusBytesSelect.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusBytesSelect.Name = "statusBytesSelect";
            this.statusBytesSelect.Size = new System.Drawing.Size(95, 22);
            this.statusBytesSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel_script_ctl);
            this.groupBox1.Controls.Add(this.labelScriptCodeNum);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.labelScriptCodeLen);
            this.groupBox1.Controls.Add(this.labelScriptCodeID);
            this.groupBox1.Controls.Add(this.labelScriptPos);
            this.groupBox1.Controls.Add(this.btnLoadScript);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.labelScriptSize);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textFilename);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(799, 120);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "脚本控制";
            // 
            // panel_script_ctl
            // 
            this.panel_script_ctl.Controls.Add(this.btnScriptJump);
            this.panel_script_ctl.Controls.Add(this.btnRestore);
            this.panel_script_ctl.Controls.Add(this.textJumpPosition);
            this.panel_script_ctl.Controls.Add(this.textJumpIndex);
            this.panel_script_ctl.Controls.Add(this.btnLoadNext);
            this.panel_script_ctl.Controls.Add(this.btnLoadPrev);
            this.panel_script_ctl.Controls.Add(this.radioJumpPosition);
            this.panel_script_ctl.Controls.Add(this.radioJumpIndex);
            this.panel_script_ctl.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_script_ctl.Location = new System.Drawing.Point(548, 19);
            this.panel_script_ctl.Name = "panel_script_ctl";
            this.panel_script_ctl.Size = new System.Drawing.Size(248, 98);
            this.panel_script_ctl.TabIndex = 48;
            // 
            // btnScriptJump
            // 
            this.btnScriptJump.Location = new System.Drawing.Point(0, 4);
            this.btnScriptJump.Name = "btnScriptJump";
            this.btnScriptJump.Size = new System.Drawing.Size(59, 52);
            this.btnScriptJump.TabIndex = 10;
            this.btnScriptJump.Text = "跳转到";
            this.btnScriptJump.UseVisualStyleBackColor = true;
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(91, 62);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(58, 31);
            this.btnRestore.TabIndex = 47;
            this.btnRestore.Text = "重载";
            this.btnRestore.UseVisualStyleBackColor = true;
            // 
            // textJumpPosition
            // 
            this.textJumpPosition.Location = new System.Drawing.Point(160, 5);
            this.textJumpPosition.Name = "textJumpPosition";
            this.textJumpPosition.Size = new System.Drawing.Size(80, 23);
            this.textJumpPosition.TabIndex = 8;
            this.textJumpPosition.Text = "0";
            // 
            // textJumpIndex
            // 
            this.textJumpIndex.Location = new System.Drawing.Point(160, 33);
            this.textJumpIndex.Name = "textJumpIndex";
            this.textJumpIndex.Size = new System.Drawing.Size(80, 23);
            this.textJumpIndex.TabIndex = 9;
            this.textJumpIndex.Text = "0";
            // 
            // btnLoadNext
            // 
            this.btnLoadNext.Location = new System.Drawing.Point(155, 62);
            this.btnLoadNext.Name = "btnLoadNext";
            this.btnLoadNext.Size = new System.Drawing.Size(85, 31);
            this.btnLoadNext.TabIndex = 30;
            this.btnLoadNext.Text = "解析下一句>";
            this.btnLoadNext.UseVisualStyleBackColor = true;
            // 
            // btnLoadPrev
            // 
            this.btnLoadPrev.Location = new System.Drawing.Point(0, 62);
            this.btnLoadPrev.Name = "btnLoadPrev";
            this.btnLoadPrev.Size = new System.Drawing.Size(85, 31);
            this.btnLoadPrev.TabIndex = 31;
            this.btnLoadPrev.Text = "<解析上一句";
            this.btnLoadPrev.UseVisualStyleBackColor = true;
            // 
            // radioJumpPosition
            // 
            this.radioJumpPosition.AutoSize = true;
            this.radioJumpPosition.Checked = true;
            this.radioJumpPosition.Location = new System.Drawing.Point(68, 6);
            this.radioJumpPosition.Name = "radioJumpPosition";
            this.radioJumpPosition.Size = new System.Drawing.Size(86, 21);
            this.radioJumpPosition.TabIndex = 26;
            this.radioJumpPosition.TabStop = true;
            this.radioJumpPosition.Text = "语句位置：";
            this.radioJumpPosition.UseVisualStyleBackColor = true;
            // 
            // radioJumpIndex
            // 
            this.radioJumpIndex.AutoSize = true;
            this.radioJumpIndex.Location = new System.Drawing.Point(68, 35);
            this.radioJumpIndex.Name = "radioJumpIndex";
            this.radioJumpIndex.Size = new System.Drawing.Size(86, 21);
            this.radioJumpIndex.TabIndex = 27;
            this.radioJumpIndex.Text = "语句序号：";
            this.radioJumpIndex.UseVisualStyleBackColor = true;
            // 
            // labelScriptCodeNum
            // 
            this.labelScriptCodeNum.AutoSize = true;
            this.labelScriptCodeNum.Location = new System.Drawing.Point(206, 92);
            this.labelScriptCodeNum.Name = "labelScriptCodeNum";
            this.labelScriptCodeNum.Size = new System.Drawing.Size(15, 17);
            this.labelScriptCodeNum.TabIndex = 36;
            this.labelScriptCodeNum.Text = "0";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(144, 92);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(68, 17);
            this.label14.TabIndex = 35;
            this.label14.Text = "语句数量：";
            // 
            // labelScriptCodeLen
            // 
            this.labelScriptCodeLen.AutoSize = true;
            this.labelScriptCodeLen.Location = new System.Drawing.Point(69, 92);
            this.labelScriptCodeLen.Name = "labelScriptCodeLen";
            this.labelScriptCodeLen.Size = new System.Drawing.Size(15, 17);
            this.labelScriptCodeLen.TabIndex = 3;
            this.labelScriptCodeLen.Text = "0";
            // 
            // labelScriptCodeID
            // 
            this.labelScriptCodeID.AutoSize = true;
            this.labelScriptCodeID.Location = new System.Drawing.Point(206, 69);
            this.labelScriptCodeID.Name = "labelScriptCodeID";
            this.labelScriptCodeID.Size = new System.Drawing.Size(15, 17);
            this.labelScriptCodeID.TabIndex = 34;
            this.labelScriptCodeID.Text = "0";
            // 
            // labelScriptPos
            // 
            this.labelScriptPos.AutoSize = true;
            this.labelScriptPos.Location = new System.Drawing.Point(69, 69);
            this.labelScriptPos.Name = "labelScriptPos";
            this.labelScriptPos.Size = new System.Drawing.Size(15, 17);
            this.labelScriptPos.TabIndex = 33;
            this.labelScriptPos.Text = "0";
            // 
            // btnLoadScript
            // 
            this.btnLoadScript.Location = new System.Drawing.Point(239, 20);
            this.btnLoadScript.Name = "btnLoadScript";
            this.btnLoadScript.Size = new System.Drawing.Size(78, 23);
            this.btnLoadScript.TabIndex = 32;
            this.btnLoadScript.Text = "载入脚本";
            this.btnLoadScript.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(144, 69);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 17);
            this.label10.TabIndex = 29;
            this.label10.Text = "当前序号：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 92);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 17);
            this.label9.TabIndex = 28;
            this.label9.Text = "语句长度：";
            // 
            // labelScriptSize
            // 
            this.labelScriptSize.AutoSize = true;
            this.labelScriptSize.Location = new System.Drawing.Point(69, 46);
            this.labelScriptSize.Name = "labelScriptSize";
            this.labelScriptSize.Size = new System.Drawing.Size(15, 17);
            this.labelScriptSize.TabIndex = 5;
            this.labelScriptSize.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 17);
            this.label8.TabIndex = 4;
            this.label8.Text = "脚本大小：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "当前位置：";
            // 
            // textFilename
            // 
            this.textFilename.Location = new System.Drawing.Point(69, 20);
            this.textFilename.Name = "textFilename";
            this.textFilename.ReadOnly = true;
            this.textFilename.Size = new System.Drawing.Size(155, 23);
            this.textFilename.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 17);
            this.label5.TabIndex = 1;
            this.label5.Text = "文 件 名 ：";
            // 
            // scriptList
            // 
            this.scriptList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptList.FormattingEnabled = true;
            this.scriptList.ItemHeight = 17;
            this.scriptList.Location = new System.Drawing.Point(0, 17);
            this.scriptList.Name = "scriptList";
            this.scriptList.Size = new System.Drawing.Size(185, 221);
            this.scriptList.TabIndex = 26;
            // 
            // opcodeList
            // 
            this.opcodeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opcodeList.FormattingEnabled = true;
            this.opcodeList.ItemHeight = 17;
            this.opcodeList.Location = new System.Drawing.Point(0, 17);
            this.opcodeList.Name = "opcodeList";
            this.opcodeList.Size = new System.Drawing.Size(185, 327);
            this.opcodeList.TabIndex = 27;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Top;
            this.label11.Location = new System.Drawing.Point(0, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(92, 17);
            this.label11.TabIndex = 28;
            this.label11.Text = "脚本文件列表：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Dock = System.Windows.Forms.DockStyle.Top;
            this.label12.Location = new System.Drawing.Point(0, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(95, 17);
            this.label12.TabIndex = 29;
            this.label12.Text = "OPCODE列表：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.splitContainer2);
            this.groupBox2.Controls.Add(this.panel_opcode_textbox_label);
            this.groupBox2.Controls.Add(this.bytesView);
            this.groupBox2.Controls.Add(this.textView);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(799, 462);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "OPCODE控制";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 65);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel_paramList_btn);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(793, 264);
            this.splitContainer2.SplitterDistance = 280;
            this.splitContainer2.TabIndex = 52;
            // 
            // panel_paramList_btn
            // 
            this.panel_paramList_btn.Controls.Add(this.panel_paramList);
            this.panel_paramList_btn.Controls.Add(this.panel_paramList_right);
            this.panel_paramList_btn.Controls.Add(this.label2);
            this.panel_paramList_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_paramList_btn.Location = new System.Drawing.Point(0, 0);
            this.panel_paramList_btn.Name = "panel_paramList_btn";
            this.panel_paramList_btn.Size = new System.Drawing.Size(280, 264);
            this.panel_paramList_btn.TabIndex = 48;
            // 
            // panel_paramList
            // 
            this.panel_paramList.Controls.Add(this.paramsList);
            this.panel_paramList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_paramList.Location = new System.Drawing.Point(0, 17);
            this.panel_paramList.Name = "panel_paramList";
            this.panel_paramList.Size = new System.Drawing.Size(172, 247);
            this.panel_paramList.TabIndex = 50;
            // 
            // paramsList
            // 
            this.paramsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paramsList.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.paramsList.FullRowSelect = true;
            this.paramsList.GridLines = true;
            this.paramsList.HideSelection = false;
            this.paramsList.Location = new System.Drawing.Point(0, 0);
            this.paramsList.MultiSelect = false;
            this.paramsList.Name = "paramsList";
            this.paramsList.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.paramsList.Scrollable = false;
            this.paramsList.Size = new System.Drawing.Size(172, 247);
            this.paramsList.TabIndex = 25;
            this.paramsList.TileSize = new System.Drawing.Size(60, 40);
            this.paramsList.UseCompatibleStateImageBehavior = false;
            this.paramsList.View = System.Windows.Forms.View.List;
            // 
            // panel_paramList_right
            // 
            this.panel_paramList_right.Controls.Add(this.panel_typeList_btn);
            this.panel_paramList_right.Controls.Add(this.btnPreview);
            this.panel_paramList_right.Controls.Add(this.btnDelete);
            this.panel_paramList_right.Controls.Add(this.btnApply);
            this.panel_paramList_right.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_paramList_right.Location = new System.Drawing.Point(172, 17);
            this.panel_paramList_right.Name = "panel_paramList_right";
            this.panel_paramList_right.Size = new System.Drawing.Size(108, 247);
            this.panel_paramList_right.TabIndex = 49;
            // 
            // panel_typeList_btn
            // 
            this.panel_typeList_btn.Controls.Add(this.btnInsertUp);
            this.panel_typeList_btn.Controls.Add(this.checkNullable);
            this.panel_typeList_btn.Controls.Add(this.checkExport);
            this.panel_typeList_btn.Controls.Add(this.btnInsertDown);
            this.panel_typeList_btn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_typeList_btn.Location = new System.Drawing.Point(0, 141);
            this.panel_typeList_btn.Name = "panel_typeList_btn";
            this.panel_typeList_btn.Size = new System.Drawing.Size(108, 106);
            this.panel_typeList_btn.TabIndex = 47;
            // 
            // btnInsertUp
            // 
            this.btnInsertUp.Location = new System.Drawing.Point(33, 48);
            this.btnInsertUp.Name = "btnInsertUp";
            this.btnInsertUp.Size = new System.Drawing.Size(75, 23);
            this.btnInsertUp.TabIndex = 28;
            this.btnInsertUp.Text = "<向上插入";
            this.btnInsertUp.UseVisualStyleBackColor = true;
            // 
            // checkNullable
            // 
            this.checkNullable.AutoSize = true;
            this.checkNullable.Location = new System.Drawing.Point(57, 26);
            this.checkNullable.Name = "checkNullable";
            this.checkNullable.Size = new System.Drawing.Size(51, 21);
            this.checkNullable.TabIndex = 44;
            this.checkNullable.Text = "可空";
            this.checkNullable.UseVisualStyleBackColor = true;
            // 
            // checkExport
            // 
            this.checkExport.AutoSize = true;
            this.checkExport.Location = new System.Drawing.Point(57, 7);
            this.checkExport.Name = "checkExport";
            this.checkExport.Size = new System.Drawing.Size(51, 21);
            this.checkExport.TabIndex = 45;
            this.checkExport.Text = "导出";
            this.checkExport.UseVisualStyleBackColor = true;
            // 
            // btnInsertDown
            // 
            this.btnInsertDown.Location = new System.Drawing.Point(33, 77);
            this.btnInsertDown.Name = "btnInsertDown";
            this.btnInsertDown.Size = new System.Drawing.Size(75, 23);
            this.btnInsertDown.TabIndex = 29;
            this.btnInsertDown.Text = "<向下插入";
            this.btnInsertDown.UseVisualStyleBackColor = true;
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(6, 72);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(92, 30);
            this.btnPreview.TabIndex = 46;
            this.btnPreview.Text = "应用此条";
            this.btnPreview.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(6, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 30;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnApply.Location = new System.Drawing.Point(5, 108);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(93, 35);
            this.btnApply.TabIndex = 43;
            this.btnApply.Text = "应用全局";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 17);
            this.label2.TabIndex = 37;
            this.label2.Text = "参数类型列表（可拖动）：";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.panel_typeList_label);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.panel_previewList_label);
            this.splitContainer3.Size = new System.Drawing.Size(509, 264);
            this.splitContainer3.SplitterDistance = 185;
            this.splitContainer3.TabIndex = 52;
            // 
            // panel_typeList_label
            // 
            this.panel_typeList_label.Controls.Add(this.panel_typeList);
            this.panel_typeList_label.Controls.Add(this.label3);
            this.panel_typeList_label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_typeList_label.Location = new System.Drawing.Point(0, 0);
            this.panel_typeList_label.Name = "panel_typeList_label";
            this.panel_typeList_label.Size = new System.Drawing.Size(185, 264);
            this.panel_typeList_label.TabIndex = 51;
            // 
            // panel_typeList
            // 
            this.panel_typeList.Controls.Add(this.typeList);
            this.panel_typeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_typeList.Location = new System.Drawing.Point(0, 17);
            this.panel_typeList.Name = "panel_typeList";
            this.panel_typeList.Size = new System.Drawing.Size(185, 247);
            this.panel_typeList.TabIndex = 50;
            // 
            // typeList
            // 
            this.typeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeList.FormattingEnabled = true;
            this.typeList.ItemHeight = 17;
            this.typeList.Location = new System.Drawing.Point(0, 0);
            this.typeList.Name = "typeList";
            this.typeList.Size = new System.Drawing.Size(185, 247);
            this.typeList.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 17);
            this.label3.TabIndex = 38;
            this.label3.Text = "可选类型（双击插入）：";
            // 
            // panel_previewList_label
            // 
            this.panel_previewList_label.Controls.Add(this.panel_previewList);
            this.panel_previewList_label.Controls.Add(this.label4);
            this.panel_previewList_label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_previewList_label.Location = new System.Drawing.Point(0, 0);
            this.panel_previewList_label.Name = "panel_previewList_label";
            this.panel_previewList_label.Size = new System.Drawing.Size(320, 264);
            this.panel_previewList_label.TabIndex = 49;
            // 
            // panel_previewList
            // 
            this.panel_previewList.Controls.Add(this.previewList);
            this.panel_previewList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_previewList.Location = new System.Drawing.Point(0, 17);
            this.panel_previewList.Name = "panel_previewList";
            this.panel_previewList.Size = new System.Drawing.Size(320, 247);
            this.panel_previewList.TabIndex = 50;
            // 
            // previewList
            // 
            this.previewList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.type,
            this.value});
            this.previewList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewList.FullRowSelect = true;
            this.previewList.GridLines = true;
            this.previewList.HideSelection = false;
            this.previewList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10});
            this.previewList.Location = new System.Drawing.Point(0, 0);
            this.previewList.MultiSelect = false;
            this.previewList.Name = "previewList";
            this.previewList.Size = new System.Drawing.Size(320, 247);
            this.previewList.TabIndex = 34;
            this.previewList.UseCompatibleStateImageBehavior = false;
            this.previewList.View = System.Windows.Forms.View.Details;
            // 
            // type
            // 
            this.type.Name = "type";
            this.type.Text = "Type";
            this.type.Width = 100;
            // 
            // value
            // 
            this.value.Name = "value";
            this.value.Text = "Value";
            this.value.Width = 210;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(188, 17);
            this.label4.TabIndex = 39;
            this.label4.Text = "数据预览（双击复制到剪辑板）：";
            // 
            // panel_opcode_textbox_label
            // 
            this.panel_opcode_textbox_label.Controls.Add(this.panel_opcode_textbox);
            this.panel_opcode_textbox_label.Controls.Add(this.label1);
            this.panel_opcode_textbox_label.Controls.Add(this.label6);
            this.panel_opcode_textbox_label.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_opcode_textbox_label.Location = new System.Drawing.Point(3, 19);
            this.panel_opcode_textbox_label.Name = "panel_opcode_textbox_label";
            this.panel_opcode_textbox_label.Size = new System.Drawing.Size(793, 46);
            this.panel_opcode_textbox_label.TabIndex = 47;
            // 
            // panel_opcode_textbox
            // 
            this.panel_opcode_textbox.Controls.Add(this.panel_opcode_textbox_param);
            this.panel_opcode_textbox.Controls.Add(this.panel_opcode_textbox_name);
            this.panel_opcode_textbox.Controls.Add(this.btnTextToList);
            this.panel_opcode_textbox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_opcode_textbox.Location = new System.Drawing.Point(0, 21);
            this.panel_opcode_textbox.Name = "panel_opcode_textbox";
            this.panel_opcode_textbox.Size = new System.Drawing.Size(793, 25);
            this.panel_opcode_textbox.TabIndex = 43;
            // 
            // panel_opcode_textbox_param
            // 
            this.panel_opcode_textbox_param.Controls.Add(this.textParams);
            this.panel_opcode_textbox_param.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_opcode_textbox_param.Location = new System.Drawing.Point(194, 0);
            this.panel_opcode_textbox_param.Name = "panel_opcode_textbox_param";
            this.panel_opcode_textbox_param.Size = new System.Drawing.Size(523, 25);
            this.panel_opcode_textbox_param.TabIndex = 48;
            // 
            // textParams
            // 
            this.textParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textParams.Location = new System.Drawing.Point(0, 0);
            this.textParams.Name = "textParams";
            this.textParams.Size = new System.Drawing.Size(523, 23);
            this.textParams.TabIndex = 40;
            // 
            // panel_opcode_textbox_name
            // 
            this.panel_opcode_textbox_name.Controls.Add(this.textOpcode);
            this.panel_opcode_textbox_name.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_opcode_textbox_name.Location = new System.Drawing.Point(0, 0);
            this.panel_opcode_textbox_name.Name = "panel_opcode_textbox_name";
            this.panel_opcode_textbox_name.Size = new System.Drawing.Size(194, 25);
            this.panel_opcode_textbox_name.TabIndex = 48;
            // 
            // textOpcode
            // 
            this.textOpcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textOpcode.Location = new System.Drawing.Point(0, 0);
            this.textOpcode.Name = "textOpcode";
            this.textOpcode.Size = new System.Drawing.Size(194, 23);
            this.textOpcode.TabIndex = 35;
            // 
            // btnTextToList
            // 
            this.btnTextToList.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnTextToList.Location = new System.Drawing.Point(717, 0);
            this.btnTextToList.Name = "btnTextToList";
            this.btnTextToList.Size = new System.Drawing.Size(76, 25);
            this.btnTextToList.TabIndex = 42;
            this.btnTextToList.Text = "解析到列表";
            this.btnTextToList.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 17);
            this.label1.TabIndex = 36;
            this.label1.Text = "OPCODE(指令名)：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(194, 1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 17);
            this.label6.TabIndex = 41;
            this.label6.Text = "参数类型序列：";
            // 
            // bytesView
            // 
            this.bytesView.AllowUserToAddRows = false;
            this.bytesView.AllowUserToDeleteRows = false;
            this.bytesView.AllowUserToResizeColumns = false;
            this.bytesView.AllowUserToResizeRows = false;
            this.bytesView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.bytesView.ColumnHeadersVisible = false;
            this.bytesView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bytesView.Location = new System.Drawing.Point(3, 329);
            this.bytesView.Name = "bytesView";
            this.bytesView.ReadOnly = true;
            this.bytesView.RowHeadersVisible = false;
            this.bytesView.RowTemplate.Height = 25;
            this.bytesView.ShowCellToolTips = false;
            this.bytesView.Size = new System.Drawing.Size(793, 45);
            this.bytesView.TabIndex = 33;
            // 
            // textView
            // 
            this.textView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textView.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textView.Location = new System.Drawing.Point(3, 374);
            this.textView.Name = "textView";
            this.textView.Size = new System.Drawing.Size(793, 85);
            this.textView.TabIndex = 26;
            this.textView.Text = "";
            // 
            // openFolder
            // 
            this.openFolder.Description = "选择脚本所在文件夹";
            this.openFolder.UseDescriptionForTitle = true;
            // 
            // panel_window_left
            // 
            this.panel_window_left.Controls.Add(this.panel_window_left_bottom);
            this.panel_window_left.Controls.Add(this.panel_window_left_top);
            this.panel_window_left.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_window_left.Location = new System.Drawing.Point(0, 0);
            this.panel_window_left.Name = "panel_window_left";
            this.panel_window_left.Size = new System.Drawing.Size(185, 582);
            this.panel_window_left.TabIndex = 32;
            // 
            // panel_window_left_bottom
            // 
            this.panel_window_left_bottom.Controls.Add(this.opcodeList);
            this.panel_window_left_bottom.Controls.Add(this.label12);
            this.panel_window_left_bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_window_left_bottom.Location = new System.Drawing.Point(0, 238);
            this.panel_window_left_bottom.Name = "panel_window_left_bottom";
            this.panel_window_left_bottom.Size = new System.Drawing.Size(185, 344);
            this.panel_window_left_bottom.TabIndex = 35;
            // 
            // panel_window_left_top
            // 
            this.panel_window_left_top.Controls.Add(this.scriptList);
            this.panel_window_left_top.Controls.Add(this.label11);
            this.panel_window_left_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_window_left_top.Location = new System.Drawing.Point(0, 0);
            this.panel_window_left_top.Name = "panel_window_left_top";
            this.panel_window_left_top.Size = new System.Drawing.Size(185, 238);
            this.panel_window_left_top.TabIndex = 34;
            // 
            // panel_window_right
            // 
            this.panel_window_right.Controls.Add(this.panel_window_right_bottom);
            this.panel_window_right.Controls.Add(this.panel_window_right_top);
            this.panel_window_right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_window_right.Location = new System.Drawing.Point(0, 0);
            this.panel_window_right.Name = "panel_window_right";
            this.panel_window_right.Size = new System.Drawing.Size(799, 582);
            this.panel_window_right.TabIndex = 33;
            // 
            // panel_window_right_bottom
            // 
            this.panel_window_right_bottom.Controls.Add(this.groupBox2);
            this.panel_window_right_bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_window_right_bottom.Location = new System.Drawing.Point(0, 120);
            this.panel_window_right_bottom.Name = "panel_window_right_bottom";
            this.panel_window_right_bottom.Size = new System.Drawing.Size(799, 462);
            this.panel_window_right_bottom.TabIndex = 32;
            // 
            // panel_window_right_top
            // 
            this.panel_window_right_top.Controls.Add(this.groupBox1);
            this.panel_window_right_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_window_right_top.Location = new System.Drawing.Point(0, 0);
            this.panel_window_right_top.Name = "panel_window_right_top";
            this.panel_window_right_top.Size = new System.Drawing.Size(799, 120);
            this.panel_window_right_top.TabIndex = 31;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel_window_left);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel_window_right);
            this.splitContainer1.Size = new System.Drawing.Size(988, 582);
            this.splitContainer1.SplitterDistance = 185;
            this.splitContainer1.TabIndex = 49;
            // 
            // panel_window
            // 
            this.panel_window.Controls.Add(this.splitContainer1);
            this.panel_window.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_window.Location = new System.Drawing.Point(0, 25);
            this.panel_window.Name = "panel_window";
            this.panel_window.Size = new System.Drawing.Size(988, 582);
            this.panel_window.TabIndex = 52;
            // 
            // Guide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 634);
            this.Controls.Add(this.panel_window);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "Guide";
            this.Text = "OpcodeGuide";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Guide_FormClosing);
            this.Load += new System.EventHandler(this.Guide_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel_script_ctl.ResumeLayout(false);
            this.panel_script_ctl.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel_paramList_btn.ResumeLayout(false);
            this.panel_paramList_btn.PerformLayout();
            this.panel_paramList.ResumeLayout(false);
            this.panel_paramList_right.ResumeLayout(false);
            this.panel_typeList_btn.ResumeLayout(false);
            this.panel_typeList_btn.PerformLayout();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.panel_typeList_label.ResumeLayout(false);
            this.panel_typeList_label.PerformLayout();
            this.panel_typeList.ResumeLayout(false);
            this.panel_previewList_label.ResumeLayout(false);
            this.panel_previewList_label.PerformLayout();
            this.panel_previewList.ResumeLayout(false);
            this.panel_opcode_textbox_label.ResumeLayout(false);
            this.panel_opcode_textbox_label.PerformLayout();
            this.panel_opcode_textbox.ResumeLayout(false);
            this.panel_opcode_textbox_param.ResumeLayout(false);
            this.panel_opcode_textbox_param.PerformLayout();
            this.panel_opcode_textbox_name.ResumeLayout(false);
            this.panel_opcode_textbox_name.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bytesView)).EndInit();
            this.panel_window_left.ResumeLayout(false);
            this.panel_window_left_bottom.ResumeLayout(false);
            this.panel_window_left_bottom.PerformLayout();
            this.panel_window_left_top.ResumeLayout(false);
            this.panel_window_left_top.PerformLayout();
            this.panel_window_right.ResumeLayout(false);
            this.panel_window_right_bottom.ResumeLayout(false);
            this.panel_window_right_top.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel_window.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem file;
        private System.Windows.Forms.ToolStripMenuItem menuNew;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.ToolStripMenuItem menuSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem help;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private System.Windows.Forms.ToolStripMenuItem menuClose;
        private System.Windows.Forms.ToolStripMenuItem menuOpenScript;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel statusToolVersion;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel statusScriptVersion;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel statusBytesSelect;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel statusItemSelect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textJumpIndex;
        private System.Windows.Forms.TextBox textJumpPosition;
        private System.Windows.Forms.Label labelScriptSize;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelScriptCodeLen;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textFilename;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnScriptJump;
        private System.Windows.Forms.RadioButton radioJumpPosition;
        private System.Windows.Forms.RadioButton radioJumpIndex;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnLoadPrev;
        private System.Windows.Forms.Button btnLoadNext;
        private System.Windows.Forms.ListBox scriptList;
        private System.Windows.Forms.ListBox opcodeList;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkExport;
        private System.Windows.Forms.CheckBox checkNullable;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnTextToList;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textParams;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textOpcode;
        private System.Windows.Forms.ListView previewList;
        private System.Windows.Forms.ColumnHeader type;
        private System.Windows.Forms.ColumnHeader value;
        private System.Windows.Forms.DataGridView bytesView;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnInsertDown;
        private System.Windows.Forms.Button btnInsertUp;
        private System.Windows.Forms.ListBox typeList;
        private System.Windows.Forms.RichTextBox textView;
        private System.Windows.Forms.ListView paramsList;
        private System.Windows.Forms.FolderBrowserDialog openFolder;
        private System.Windows.Forms.Button btnLoadScript;
        private System.Windows.Forms.Label labelScriptCodeID;
        private System.Windows.Forms.Label labelScriptPos;
        private System.Windows.Forms.Label labelScriptCodeNum;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Panel panel_window_left;
        private System.Windows.Forms.Panel panel_window_right;
        private System.Windows.Forms.Panel panel_window_left_bottom;
        private System.Windows.Forms.Panel panel_window_left_top;
        private System.Windows.Forms.Panel panel_window_right_bottom;
        private System.Windows.Forms.Panel panel_window_right_top;
        private System.Windows.Forms.Panel panel_opcode_textbox_label;
        private System.Windows.Forms.Panel panel_paramList_btn;
        private System.Windows.Forms.Panel panel_paramList;
        private System.Windows.Forms.Panel panel_paramList_right;
        private System.Windows.Forms.Panel panel_typeList_label;
        private System.Windows.Forms.Panel panel_typeList;
        private System.Windows.Forms.Panel panel_previewList_label;
        private System.Windows.Forms.Panel panel_previewList;
        private System.Windows.Forms.Panel panel_opcode_textbox;
        private System.Windows.Forms.Panel panel_opcode_textbox_name;
        private System.Windows.Forms.Panel panel_opcode_textbox_param;
        private System.Windows.Forms.Panel panel_typeList_btn;
        private System.Windows.Forms.Panel panel_script_ctl;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel_window;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
    }
}

