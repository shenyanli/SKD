using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SkdAdminClient.SkdWebService;
using SkdAdminModel;

namespace SkdAdminClient.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class frmLogin : Window
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //GolableData.MainWindowUpdateData.Close();
           Application.Current.Shutdown();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TxtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (TxtUserName.Text.Trim()=="") return;
            if (e.Key == Key.Tab || e.Key == Key.Enter)
                TxtPwd.Focus();
        }

        private void TxtPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (TxtPwd.Password.Trim()=="") return;
            if (e.Key == Key.Tab )
                BtnLogin.Focus();
            if (e.Key == Key.Enter)
                BtnLogin_Click(null, null);
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string userAccount =TxtUserName.Text.Trim().ToLower();
            string pwd = TxtPwd.Password.Trim();
            if (userAccount == "" || pwd == "") return;
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            bool result = skdServiceSoapClient.Login(userAccount, pwd);
            if (!result)
            {
                XMessageBox.ShowDialog("帐号或者密码不正确！", "错误");
                return;
            }
            string userPrivelegeInfo = skdServiceSoapClient.GetPrivelegeInfo(userAccount);
            List<string> info = userPrivelegeInfo.Split('|').ToList();
            GlobalData.UserAccount = userAccount;
            GlobalData.UserName = info[0];
            GlobalData.PrivilegeLevel =(Privilege)Convert.ToInt32(info[1]);//权限等级
            if (GlobalData.PrivilegeLevel == Privilege.AreaAdmin)//区域管理员
            {
                //74300001_上海景格科技
                GlobalData.Venders = skdServiceSoapClient.GetVenders(GlobalData.UserAccount).ToList();
                GlobalData.RboList = skdServiceSoapClient.GetRbos(GlobalData.UserAccount).ToList();
            }
            if (GlobalData.PrivilegeLevel == Privilege.SuperAdmin)//超级管理员
            {
                GlobalData.Venders = skdServiceSoapClient.GetOrgIdAndNameList().ToList();
                GlobalData.RboList = skdServiceSoapClient.GetRbos("").ToList();
            }
         
            GlobalData.Area = info[2];//区域
            WindowMain main = new WindowMain();
            main.Show();
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
  
        }


    }
}
