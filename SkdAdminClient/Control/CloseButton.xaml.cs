using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkdAdminClient.Control
{
    /// <summary>
    /// CloseButton.xaml 的交互逻辑
    /// </summary>
    public partial class CloseButton : UserControl
    {
        public CloseButton()
        {
            InitializeComponent();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            bool result = SkdAdminClient.XMessageBox.ShowDialog("确定退出？", "提示", true);
            if (result)
            {
                Application.Current.Shutdown();
            }
            //else
            //{
            //    e.Cancel = true;
            //}
            //ToggleButton button = sender as ToggleButton;
            //Window targetWindow = Window.GetWindow(button);
            //targetWindow.Close();
        }
    }
}
