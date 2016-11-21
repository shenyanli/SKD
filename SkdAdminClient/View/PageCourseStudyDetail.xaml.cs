using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using SkdAdminClient.Moudle;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;

namespace SkdAdminClient.View
{

    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageCourseStudyDetail : Page
    {
        public Frame Frame;
        private DataTable _dt = new DataTable();
        public PageProgressDetail ParentPage;
        public ProgressDetail CurrentProgressDetail;
        private List<CourseStudyDetail> courseStudyDetails = new List<CourseStudyDetail>();
        public PageCourseStudyDetail( Frame frame)
        {
            InitializeComponent();
            Frame = frame;
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            string vender = "";
            string userAccount = TxtUserAccount.Text.Trim().ToLower();
            string userName = TxtUserName.Text.Trim();
            string courseName = CbxCourseName.Text.Trim();
            string loginDateBegin = "";
            string loginDateEnd = "";

            if (ChkTime.IsChecked != null && (bool) ChkTime.IsChecked)
            {
                loginDateBegin = Convert.ToDateTime(TxtBeginDate.Text).ToString("yyyy-MM-dd");
                loginDateEnd = Convert.ToDateTime(TxtEndDate.Text).ToString("yyyy-MM-dd");
            }
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();

            _dt = skdServiceSoapClient.GetCourseStudyDetails(userAccount, userName, courseName, vender,
                loginDateBegin, loginDateEnd);
            courseStudyDetails.Clear();
            foreach (DataRow row in _dt.Rows)
            {
                CourseStudyDetail c = CourseStudyDetail.XmlToCourseStudyDetail(row["xmlInfo"].ToString());
                c.UserName = row["userName"].ToString();
                c.Description = c.Description.Replace(",", "\r\n");
                int hour = (int)Convert.ToDouble(c.Minutes) / 60;
                int minute = (int)Convert.ToDouble(c.Minutes) % 60;
                c.Minutes = hour.ToString("00") + ":" + minute.ToString("00") + ":00";
                courseStudyDetails.Add(c);
            }

            DgvCourseDetail.ItemsSource = courseStudyDetails.OrderByDescending(x=>x.BeginTime);
            XMessageBox.ShowDialog("查询已完成！", "提示");
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtUserName.Text = "";
            CbxCourseName.Text = "";
            TxtUserAccount.Text = "";
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text =DateTime.Now.ToString("yyyy-MM-dd");
            ChkTime.IsChecked = false;
            DgvCourseDetail.ItemsSource = null;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = ParentPage;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentProgressDetail != null)//说明是从学习页面进来的
            {
                TxtUserAccount.Text = CurrentProgressDetail.UserAccount;
                TxtUserAccount.IsEnabled = false;
                TxtUserName.Text = CurrentProgressDetail.UserName;
                TxtUserName.IsEnabled = false;
                //TxtVender.Text = CurrentProgressDetail.Vender;
                //TxtVender.IsEnabled = false;
                CbxCourseName.Text = CurrentProgressDetail.CourseName;
                CbxCourseName.IsEnabled = false;
                BtnBack.Visibility = Visibility.Visible;
                BtnClear.Visibility = Visibility.Hidden;
                BtnQuery_Click(null,null);
                BtnClear.IsEnabled = false;
            }
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            List<string> courseNames = skdServiceSoapClient.GetCourseName().ToList();
            CbxCourseName.ItemsSource = courseNames;
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgvCourseDetail.Items.Count < 1)
                {
                    XMessageBox.ShowDialog("没有数据可供导出！", "提示");
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel|*.xls|Excel|*.xlsx"; //删选、设定文件显示类型
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fName = sfd.FileName;
                    _dt = ObjectToDataTable.ToDataTable(courseStudyDetails);
                    NpoiHelper.ExportDataTableToExcel(fName, _dt, this.Name, false);

                }
                else
                {
                    return;
                }

            }
            catch (Exception err)
            {
                XMessageBox.ShowDialog("导出失败！"+err.Message, "提示");
                return;
                
            }
         
            XMessageBox.ShowDialog("导出成功！", "提示");
        }
    }
}
