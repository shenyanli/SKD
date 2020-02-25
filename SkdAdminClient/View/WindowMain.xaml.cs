using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KnightsWarriorAutoupdater;
using SkdAdminClient.SkdWebService;
using MahApps.Metro;
using SkdAdminModel;
using Path = System.IO.Path;

namespace SkdAdminClient.View
{
    /// <summary>
    /// PageMainNew.xaml 的交互逻辑
    /// </summary>
    public partial class WindowMain 
    {
        private PageMainNew pageMainNew = new PageMainNew();
        public WindowMain()
        {
            InitializeComponent();
            Accent accent = ThemeManager.GetAccent("Green");
            ThemeManager.ChangeAppStyle(Application.Current, accent, ThemeManager.DetectAppStyle(Application.Current).Item1);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            pageMainNew.WindowMain = this;
            AddPage.Content = pageMainNew;
            LblUserAccount.Content = GlobalData.UserName + " 欢迎您！ ";
            string privilege;
            switch (GlobalData.PrivilegeLevel)
            {
                case Privilege.SuperAdmin:
                    privilege = "超级管理员";
                    break;
                case Privilege.AreaAdmin:
                    privilege = "区域管理员";
                    break;
                default:
                    privilege = "学员";
                    break;
            }
            LblPrivelege.Content ="权限："+ privilege;
            LblArea.Content = "区域："+GlobalData.Area;
            LblTime.Content =(new SkdServiceSoapClient()).GetTime();
            Config config = Config.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.FILENAME));
            LblVersion.Content = config.UpdateFileList[0].LastVer;
        }


        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool result = XMessageBox.ShowDialog("确定退出？", "提示", true);
            if (result)
            {
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
