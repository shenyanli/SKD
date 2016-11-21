using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using KnightsWarriorAutoupdater;
using Application = System.Windows.Application;

using System.Diagnostics;
using System.IO;
using System.Text;


namespace SkdAdminClient
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        App()
        {

            #region  如果还有.old文件则删除

            string path = Environment.CurrentDirectory;
            DirectoryInfo theFolder = new DirectoryInfo(path);
            foreach (FileInfo file in theFolder.GetFiles())
            {
                if (file.Extension.Contains("old"))
                {
                    string fpath = file.DirectoryName + "\\" + file.Name;
                    File.Delete(fpath);
                }
            }

            #endregion


            #region 检测更新

            IAutoUpdater autoUpdater = new KnightsWarriorAutoupdater.AutoUpdater();
            try
            {
                autoUpdater.Update();
            }
            catch (WebException exp)
            {
                MessageBox.Show(String.Format("无法找到指定资源\n\n{0}", exp.Message), "自动升级", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (XmlException exp)
            {
                MessageBox.Show(String.Format("下载的升级文件有错误\n\n{0}", exp.Message), "自动升级", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (NotSupportedException exp)
            {
                MessageBox.Show(String.Format("升级地址配置错误\n\n{0}", exp.Message), "自动升级", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException exp)
            {
                MessageBox.Show(String.Format("下载的升级文件有错误\n\n{0}", exp.Message), "自动升级", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception exp)
            {
                MessageBox.Show(String.Format("升级过程中发生错误\n\n{0}", exp.Message), "自动升级", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
            InitializeComponent();
        }

        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            SkdAdminClient.App app = new SkdAdminClient.App();
            app.InitializeComponent();

            try
            {
                app.Run();
            }
            catch (Exception err)
            {
            }
   
        }
    }
}
