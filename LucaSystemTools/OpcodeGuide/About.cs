using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OpcodeGuide
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            Process.Start("explorer", e.Link.LinkData.ToString());
        }

        private void About_Load(object sender, EventArgs e)
        {
            linkLabel1.Links[0].LinkData = "http://wetorx.cn";
            linkLabel2.Links[0].LinkData = "https://github.com/wetor/LucaSystemTools";
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer",e.Link.LinkData.ToString());
        }
    }
}
