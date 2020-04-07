using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public string ImagePath = "";
        public ViewPicture()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowImage = new BitmapImage();
            int BytesToRead = 100;

            WebRequest request = WebRequest.Create(new Uri(ImagePath, UriKind.Absolute));
            request.Timeout = -1;
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            BinaryReader reader = new BinaryReader(responseStream);
            MemoryStream memoryStream = new MemoryStream();

            byte[] bytebuffer = new byte[BytesToRead];
            int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

            while (bytesRead > 0)
            {
                memoryStream.Write(bytebuffer, 0, bytesRead);
                bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
            }

            ShowImage.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);

            ShowImage.StreamSource = memoryStream;
            ShowImage.EndInit();

            Image.Source = ShowImage;// ShowImage;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }

   
    }
}
