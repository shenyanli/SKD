using System.Configuration;
using System.Windows;
using SkdAdminClient.SkdWebService;

namespace SkdAdminClient.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _userName ;
        PageMain pageMain = new PageMain();

        public MainWindow(string userName)
        {
            _userName = userName;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
            pageMain.BackButton = BackButton;
            pageMain.CurrentPage = LblCurrentPage;
            AddPage.Content = pageMain;
            LblUserAccount.Content = _userName+"欢迎您！";
            LblTime.Content = (new SkdServiceSoapClient()).GetTime();
            LblVersion.Content = ConfigurationManager.AppSettings["Version"];
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackButton.Visibility = Visibility.Hidden;
            LblCurrentPage.Visibility = Visibility.Hidden;
            AddPage.Content = pageMain;
        }
    }
}
