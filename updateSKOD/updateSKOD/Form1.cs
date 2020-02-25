using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace updateSKOD
{
    public partial class frmMain : Form
    {
        //private string path="";

        public frmMain()
        {
            InitializeComponent();
        }

        public frmMain(string[] args)
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                //关闭主程序exe
                Process[] client = Process.GetProcessesByName("SkdAdminClient");
                foreach (Process p in client)
                {
                    if (p.ProcessName == Process.GetCurrentProcess().ProcessName)
                    {
                        p.Kill();
                    }
                }
                //关闭更新exe,重启主程序exe
                Process.Start(Application.StartupPath + "\\SkdAdminClient.exe");
            
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            Environment.Exit(0);
        }


    }
}
