namespace TestWFCore
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
            this.tools = new System.Windows.Forms.ToolStripMenuItem();
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.help = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.paramsList = new System.Windows.Forms.ListView();
            this.textView = new System.Windows.Forms.RichTextBox();
            this.typeList = new System.Windows.Forms.ListBox();
            this.btnInsertUp = new System.Windows.Forms.Button();
            this.btnInsertDown = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.bytesView = new System.Windows.Forms.DataGridView();
            this.previewList = new System.Windows.Forms.ListView();
            this.type = new System.Windows.Forms.ColumnHeader();
            this.value = new System.Windows.Forms.ColumnHeader();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusToolVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusScriptVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusItemSelect = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBytesSelect = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bytesView)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.file,
            this.tools,
            this.help});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(908, 25);
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
            // tools
            // 
            this.tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customizeToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(52, 21);
            this.tools.Text = "&Tools";
            // 
            // customizeToolStripMenuItem
            // 
            this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            this.customizeToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.customizeToolStripMenuItem.Text = "&Customize";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // help
            // 
            this.help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.help.Name = "help";
            this.help.Size = new System.Drawing.Size(47, 21);
            this.help.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(124, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
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
            // paramsList
            // 
            this.paramsList.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.paramsList.FullRowSelect = true;
            this.paramsList.GridLines = true;
            this.paramsList.HideSelection = false;
            this.paramsList.Location = new System.Drawing.Point(12, 142);
            this.paramsList.MultiSelect = false;
            this.paramsList.Name = "paramsList";
            this.paramsList.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.paramsList.Scrollable = false;
            this.paramsList.Size = new System.Drawing.Size(138, 242);
            this.paramsList.TabIndex = 1;
            this.paramsList.TileSize = new System.Drawing.Size(60, 40);
            this.paramsList.UseCompatibleStateImageBehavior = false;
            this.paramsList.View = System.Windows.Forms.View.List;
            // 
            // textView
            // 
            this.textView.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textView.Location = new System.Drawing.Point(12, 441);
            this.textView.Name = "textView";
            this.textView.Size = new System.Drawing.Size(709, 85);
            this.textView.TabIndex = 2;
            this.textView.Text = "<div style=\"background:#000\">text</div>\nasdad\n这是中文";
            // 
            // typeList
            // 
            this.typeList.FormattingEnabled = true;
            this.typeList.ItemHeight = 17;
            this.typeList.Location = new System.Drawing.Point(258, 142);
            this.typeList.MultiColumn = true;
            this.typeList.Name = "typeList";
            this.typeList.Size = new System.Drawing.Size(127, 242);
            this.typeList.TabIndex = 3;
            this.typeList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.typeList_MouseClick);
            // 
            // btnInsertUp
            // 
            this.btnInsertUp.Location = new System.Drawing.Point(177, 332);
            this.btnInsertUp.Name = "btnInsertUp";
            this.btnInsertUp.Size = new System.Drawing.Size(75, 23);
            this.btnInsertUp.TabIndex = 4;
            this.btnInsertUp.Text = "<向上插入";
            this.btnInsertUp.UseVisualStyleBackColor = true;
            this.btnInsertUp.Click += new System.EventHandler(this.btnInsertUp_Click);
            // 
            // btnInsertDown
            // 
            this.btnInsertDown.Location = new System.Drawing.Point(177, 361);
            this.btnInsertDown.Name = "btnInsertDown";
            this.btnInsertDown.Size = new System.Drawing.Size(75, 23);
            this.btnInsertDown.TabIndex = 5;
            this.btnInsertDown.Text = "<向下插入";
            this.btnInsertDown.UseVisualStyleBackColor = true;
            this.btnInsertDown.Click += new System.EventHandler(this.btnInsertDown_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(156, 142);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(156, 200);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(75, 23);
            this.btnMoveDown.TabIndex = 7;
            this.btnMoveDown.Text = "↓向下移动";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(156, 171);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(75, 23);
            this.btnMoveUp.TabIndex = 8;
            this.btnMoveUp.Text = "↑向上移动";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // bytesView
            // 
            this.bytesView.AllowUserToAddRows = false;
            this.bytesView.AllowUserToDeleteRows = false;
            this.bytesView.AllowUserToResizeColumns = false;
            this.bytesView.AllowUserToResizeRows = false;
            this.bytesView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.bytesView.ColumnHeadersVisible = false;
            this.bytesView.Location = new System.Drawing.Point(12, 390);
            this.bytesView.Name = "bytesView";
            this.bytesView.ReadOnly = true;
            this.bytesView.RowHeadersVisible = false;
            this.bytesView.RowTemplate.Height = 25;
            this.bytesView.ShowCellToolTips = false;
            this.bytesView.Size = new System.Drawing.Size(709, 45);
            this.bytesView.TabIndex = 9;
            this.bytesView.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.bytesView_CellMouseUp);
            // 
            // previewList
            // 
            this.previewList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.type,
            this.value});
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
            this.previewList.Location = new System.Drawing.Point(403, 142);
            this.previewList.MultiSelect = false;
            this.previewList.Name = "previewList";
            this.previewList.Size = new System.Drawing.Size(318, 242);
            this.previewList.TabIndex = 10;
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 597);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(908, 26);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(57, 21);
            this.toolStripStatusLabel1.Text = "ToolVer:";
            // 
            // statusToolVersion
            // 
            this.statusToolVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusToolVersion.Name = "statusToolVersion";
            this.statusToolVersion.Size = new System.Drawing.Size(68, 21);
            this.statusToolVersion.Text = "20201212";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(64, 21);
            this.toolStripStatusLabel2.Text = "ScriptVer:";
            // 
            // statusScriptVersion
            // 
            this.statusScriptVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusScriptVersion.Name = "statusScriptVersion";
            this.statusScriptVersion.Size = new System.Drawing.Size(19, 21);
            this.statusScriptVersion.Text = "3";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(71, 21);
            this.toolStripStatusLabel4.Text = "ItemSelect:";
            // 
            // statusItemSelect
            // 
            this.statusItemSelect.AutoSize = false;
            this.statusItemSelect.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusItemSelect.Name = "statusItemSelect";
            this.statusItemSelect.Size = new System.Drawing.Size(128, 21);
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(76, 21);
            this.toolStripStatusLabel3.Text = "BytesSelect:";
            // 
            // statusBytesSelect
            // 
            this.statusBytesSelect.AutoSize = false;
            this.statusBytesSelect.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusBytesSelect.Name = "statusBytesSelect";
            this.statusBytesSelect.Size = new System.Drawing.Size(85, 21);
            this.statusBytesSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Guide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 623);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.previewList);
            this.Controls.Add(this.bytesView);
            this.Controls.Add(this.btnMoveUp);
            this.Controls.Add(this.btnMoveDown);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnInsertDown);
            this.Controls.Add(this.btnInsertUp);
            this.Controls.Add(this.typeList);
            this.Controls.Add(this.textView);
            this.Controls.Add(this.paramsList);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "Guide";
            this.Text = "OpcodeGuide";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Guide_FormClosing);
            this.Load += new System.EventHandler(this.Guide_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bytesView)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem tools;
        private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem help;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private System.Windows.Forms.ToolStripMenuItem menuClose;
        private System.Windows.Forms.ToolStripMenuItem menuOpenScript;
        private System.Windows.Forms.ListView paramsList;
        private System.Windows.Forms.RichTextBox textView;
        private System.Windows.Forms.ListBox typeList;
        private System.Windows.Forms.Button btnInsertUp;
        private System.Windows.Forms.Button btnInsertDown;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.DataGridView bytesView;
        private System.Windows.Forms.ListView previewList;
        private System.Windows.Forms.ColumnHeader type;
        private System.Windows.Forms.ColumnHeader value;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel statusToolVersion;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel statusScriptVersion;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel statusBytesSelect;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel statusItemSelect;
    }
}

