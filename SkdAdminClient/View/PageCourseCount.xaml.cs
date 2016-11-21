using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using SkdAdminClient.Moudle;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;

namespace SkdAdminClient.View
{
    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageCourseCount : Page
    {
        private DataTable _dt = new DataTable();
        List<CourseCount> courseCounts = new List<CourseCount>();
        public Frame Frame ;
        public PageCourseCount( Frame frame)
        {
            InitializeComponent();
            Frame = frame;
            TxtBeginDate.Text = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            string vender = CbxVender.Text.Trim();
            string courseName = CbxCourseName.Text.Trim();
            string finishBeginDate = "";
            string finishEndDate = "";
            if (ChkTime.IsChecked != null && (bool) ChkTime.IsChecked)
            {
                finishBeginDate = Convert.ToDateTime(TxtBeginDate.Text).ToString("yyyy-MM-dd");
                finishEndDate = Convert.ToDateTime(TxtEndDate.Text).ToString("yyyy-MM-dd");
            }

            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();

            _dt = skdServiceSoapClient.GetCourseAddUp(courseName, vender, finishBeginDate, finishEndDate);

            courseCounts.Clear();
            foreach (DataRow row in _dt.Rows)
            {
                CourseCount c = new CourseCount();
                c.CourseName = row["courseName"].ToString();
                c.Vender = row["OrganizationName"].ToString();
                int hour = (int)Convert.ToDouble(row["TotalMinutes"].ToString()) / 60;
                int minute = (int)Convert.ToDouble(row["TotalMinutes"].ToString()) % 60;
                c.TotalMinutes = hour.ToString("00") + ":" + minute.ToString("00") + ":00";
                //c.TotalMinutes = row["TotalMinutes"].ToString();
                c.TotalStamps = Convert.ToDouble(row["TotalStamp"].ToString());
                c.TotalPersons = Convert.ToDouble(row["totalPersons"].ToString());
                c.FinishPersons = Convert.ToDouble(row["finishPersons"].ToString());
                c.Percent = Math.Round(Convert.ToDouble(row["percents"]), 2) + "%";
                courseCounts.Add(c);
            }

 
            ListCollectionView LoginTotals = new ListCollectionView(courseCounts);
            LoginTotals.GroupDescriptions.Add(new PropertyGroupDescription("Vender"));
            DgvLoginTotal.ItemsSource = null;
            DgvLoginTotal.ItemsSource = LoginTotals;
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
                    _dt = ObjectToDataTable.ToDataTable(courseCounts);
                    NpoiHelper.ExportDataTableToExcel(fName, _dt, this.Name, false);
                    XMessageBox.ShowDialog("导出成功！", "提示");
                }
                else
                {
                    return;
                }
            }
            catch (Exception err)
            {
                XMessageBox.ShowDialog("导出失败！ "+err.Message, "提示");
            }
            
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            List<string> courseNames = skdServiceSoapClient.GetCourseName().ToList();
            CbxCourseName.ItemsSource = courseNames;
            List<string> orgList = skdServiceSoapClient.GetOrgList().ToList();
            orgList.Insert(0, "");
            orgList.Insert(1,"All");
            CbxVender.ItemsSource = orgList;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            CbxCourseName.Text = "";
            CbxVender.Text = "";
            DgvLoginTotal.ItemsSource = null;
            ChkTime.IsChecked = false;
        }
    }
}
