using System;
using System.Collections.Generic;
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
using SkdAdminClient;

namespace SkdAdminClient
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
            
            bool result = XMessageBox.ShowDialog("确定退出？","提示",true);
            if (result )
            {
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
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

        }
    }
}
