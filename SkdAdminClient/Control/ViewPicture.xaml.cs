using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace SkdAdminClient.Control
{
    /// <summary>
    /// ViewPicture.xaml 的交互逻辑
    /// </summary>
    public partial class ViewPicture : Window
    {

        public BitmapImage ShowImage;
        public ViewPicture()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
     
            Image.Source = ShowImage;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }

   
    }
}
