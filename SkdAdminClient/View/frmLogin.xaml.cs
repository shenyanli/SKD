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
            if (e.Key == Key.Tab || e.Key == Key.Enter)
                BtnLogin.Focus();
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
            GolableData.UserAccount = userAccount;
            GolableData.UserName = info[0];
            GolableData.PrivilegeLevel =(Privilege)Convert.ToInt32(info[1]==""?"5":info[1]);
            if (GolableData.PrivilegeLevel == Privilege.VenderAdmin)//经销商管理员
            {
                GolableData.Venders = skdServiceSoapClient.GetVenders(GolableData.Vender).ToList();
            }
            GolableData.Vender = info[2];
            //View.MainWindow main = new View.MainWindow();
            View.WindowMain main = new View.WindowMain();
            main.Show();
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
  
        }


    }
}
