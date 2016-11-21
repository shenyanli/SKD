using System.Windows;
using System.Windows.Input;
using SkdAdminClient.SkdWebService;

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

            string userName = TxtUserName.Text.Trim().ToLower();
            string pwd = TxtPwd.Password.Trim();
            if (userName == "" || pwd == "") return;
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            bool result = skdServiceSoapClient.Login(userName, pwd);
            if (!result)
            {
                XMessageBox.ShowDialog("帐号或者密码不正确！", "错误");
                return;
            }
            View.MainWindow main = new View.MainWindow(userName);
            main.Show();
            this.Hide();
        }
    }
}
