using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using SkdAdminClient.Control;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;
using SkdAdminModel;

namespace SkdAdminClient.View
{



    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageFeedBack : Page
    {
        private DataTable _dt = new DataTable();
        List<BindFeedBack> _feedBacks = new List<BindFeedBack>();
        public Frame Frame ;
        public PageMainNew PageMainNew;
        public PageFeedBack( )
        {
            InitializeComponent();
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            string userAccount = TxtUserAccout.Text.Trim();
            string userName = TxtUserName.Text.Trim();
            string courseName = CbxCourseName.Text.Trim();
            string vender = CbxVender.Text.Trim();
            if (GolableData.PrivilegeLevel == Privilege.AreaAdmin)//区域管理员
            {
                vender = string.Join("','", GolableData.Venders);
            }
            string feedBackBeginDate = "";
            string feedBackBEndDate = "";
            if (ChkTime.IsChecked != null && (bool)ChkTime.IsChecked)
            {
                feedBackBeginDate = Convert.ToDateTime(TxtBeginDate.Text).ToString("yyyy-MM-dd");
                feedBackBEndDate = Convert.ToDateTime(TxtEndDate.Text).ToString("yyyy-MM-dd");
            }

            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();

            List<byte[]> feedBacksByteses =
                skdServiceSoapClient.GetFeedBackInfo(courseName, userAccount, userName, vender, feedBackBeginDate,
                    feedBackBEndDate).ToList();

            _feedBacks.Clear();
            foreach (byte[] bytes in feedBacksByteses)
            {
                BindFeedBack feedBack = new BindFeedBack();
                feedBack = (BindFeedBack) feedBack.DeserializeObject(bytes);
                _feedBacks.Add(feedBack);
            }

            if (GolableData.PrivilegeLevel > Privilege.VenderAdmin)
            {
                ListCollectionView feedBackListCollectionView = new ListCollectionView(_feedBacks);
                feedBackListCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("CourseName"));

                DgvLoginTotal.ItemsSource = null;
                DgvLoginTotal.ItemsSource = feedBackListCollectionView;
            }
            else
            {
                DgvLoginTotal.Columns[1].Visibility = Visibility.Visible;
                DgvLoginTotal.ItemsSource = _feedBacks.OrderByDescending(x => x.CourseName);
            }

            XMessageBox.ShowDialog("查询已完成！", "提示");

        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgvLoginTotal.Items.Count < 1)
                {
                    XMessageBox.ShowDialog("没有数据可供导出！", "提示");
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel|*.xls|Excel|*.xlsx"; //删选、设定文件显示类型
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fName = sfd.FileName;
                    _dt = ObjectToDataTable.ToDataTable(_feedBacks);
                    NpoiHelper.ExportDataTableToExcel(fName, _dt, this.Name, false);
                    XMessageBox.ShowDialog("导出成功！", "提示");
                }
            }
            catch (Exception err)
            {
                XMessageBox.ShowDialog("导出失败！ "+err.Message, "提示");
            }
            
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TbTitle.Text = Name;
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            List<string> courseNames = skdServiceSoapClient.GetCourseName().ToList();
            CbxCourseName.ItemsSource = courseNames;
            List<string> orgList = skdServiceSoapClient.GetOrgList().ToList();
            orgList.Insert(0, "");
            CbxVender.ItemsSource = orgList;
            if (GolableData.PrivilegeLevel == Privilege.VenderAdmin)//经销商管理员
            {
                CbxVender.Text = GolableData.Vender;
                CbxVender.IsEnabled = false;
            }


        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            CbxCourseName.Text = "";
            if (GolableData.PrivilegeLevel > Privilege.VenderAdmin)
            {
                CbxVender.Text = "";
                CbxVender.IsEnabled = true;
            }
            DgvLoginTotal.ItemsSource = null;
            ChkTime.IsChecked = false;
        }

  

   

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }

        private void BtnImage_Click(object sender, RoutedEventArgs e)
        {
            BindFeedBack p = DgvLoginTotal.SelectedItem as BindFeedBack;
            if (p==null) return;
            System.Drawing.Image img = p.Image;
            Bitmap map = new Bitmap(img);
            ViewPicture viewPicture = new ViewPicture();
            viewPicture.ShowImage = BitmapToBitmapImage2(map);
            viewPicture.Show();
        }

        private BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, bitmap.RawFormat);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        public BitmapImage BitmapToBitmapImage2(Bitmap bitmap)
        {
            Bitmap bitmapSource = new Bitmap(bitmap.Width, bitmap.Height);
            int i, j;
            for (i = 0; i < bitmap.Width; i++)
                for (j = 0; j < bitmap.Height; j++)
                {
                    Color pixelColor = bitmap.GetPixel(i, j);
                    Color newColor = Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B);
                    bitmapSource.SetPixel(i, j, newColor);
                }
            MemoryStream ms = new MemoryStream();
            bitmapSource.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
            bitmapImage.EndInit();

            return bitmapImage;
        }

    }
}
