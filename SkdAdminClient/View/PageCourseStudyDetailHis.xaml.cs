using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;
using SkdAdminModel;

namespace SkdAdminClient.View
{

    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageCourseStudyDetailHis : Page
    {
        public Frame Frame;
        public PageMainNew PageMainNew;
        private DataTable _dt = new DataTable();
        public PageProgressDetail ParentPage;
        public BindProgressDetail CurrentProgressDetail;
        private List<CourseStudyDetail> courseStudyDetails = new List<CourseStudyDetail>();
        private bool _showMsgBox = true;
        SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
        List<string> courseNames=new List<string>();
        public PageCourseStudyDetailHis()
        {
            InitializeComponent();
            TxtBeginDate.Text = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TbTitle.Text = Name;
            if (CurrentProgressDetail != null)//说明是从学习页面进来的
            {
                TxtUserAccount.Text = CurrentProgressDetail.UserAccount;
                TxtUserAccount.IsEnabled = false;
                TxtUserName.Text = CurrentProgressDetail.UserName;
                TxtUserName.IsEnabled = false;
                // LbxCourseName.Show(new List<string>() { CurrentProgressDetail.CourseName }, true); //显示需要查询的课程名称
                LbxCourseName.Show(GlobalData.CourseNameMaps.Where(x => x.Value == CurrentProgressDetail.CourseName).Select(x => x.Key).ToList(), true);
                LbxCourseName.IsEnabled = false;

                BtnBack.IsEnabled = true;
                BtnClear.IsEnabled = false;
                _showMsgBox = false;

                string courseName = string.Join("','",
               GlobalData.CourseNameMaps.Where(x => x.Value == CurrentProgressDetail.CourseName).Select(x => x.Key).ToList());
                Query(CurrentProgressDetail.UserAccount,"", courseName,"", "", "", "");

             //   Query(userAccount, userName, courseName, rbo, vender, loginDateBegin, loginDateEnd);
                //      BtnQuery_Click(null, null);
                _showMsgBox = true;
            }
            else
            {
                courseNames = skdServiceSoapClient.GetCourseName().ToList();
                //     LbxCourseName.Show(courseNames);
                LbxCourseName.Show(GlobalData.CourseNameMaps.Values.Distinct().ToList());
                LbxVenders.Show(GlobalData.Venders);
                LbxRbo.Show(GlobalData.RboList);
            }
  
        }


        void Query(string userAccount,string userName,string courseName,string rbo,string vender,string loginDateBegin,string loginDateEnd)
        {
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();

            _dt = skdServiceSoapClient.GetCourseStudyDetails(userAccount, userName, courseName, rbo, vender,
                loginDateBegin, loginDateEnd);
            courseStudyDetails.Clear();
            foreach (DataRow row in _dt.Rows)
            {
                CourseStudyDetail c = CourseStudyDetail.XmlToCourseStudyDetail(row["xmlInfo"].ToString());

                c.Chapters = c.Chapters.Replace(",", "\r\n");
                c.UserName = row["userName"].ToString();
                c.Description = c.Description.Replace(",", "\r\n");
                c.Vender = row["OrganizationName"].ToString();
                int hour = (int)Convert.ToDouble(c.Minutes) / 60;
                int minute = (int)Convert.ToDouble(c.Minutes) % 60;
                c.Minutes = hour.ToString("00") + ":" + minute.ToString("00") + ":00";
                courseStudyDetails.Add(c);
            }

            DgvCourseDetail.ItemsSource = courseStudyDetails.OrderByDescending(x => x.BeginTime);
            BtnQuery.Content = "检索";
            if (_showMsgBox)
            {
                Expander.IsExpanded = false;
                XMessageBox.ShowDialog("查询到相关数据" + courseStudyDetails.Count + "笔", "提示");
            }
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            BtnQuery.Content = "检索中...";
            string userAccount = TxtUserAccount.Text.Trim().ToLower();
            string userName = TxtUserName.Text.Trim();
            List<string> bigCourseNames =
         LbxCourseName.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg).ToList();
            string courseName = "";//=GlobalData.CourseNameMaps.Where(x=>x.Value)
            foreach (string bigCourseName in bigCourseNames)
            {
                string smallCourseNames = string.Join("','",
                    GlobalData.CourseNameMaps.Where(x => x.Value == bigCourseName).Select(x => x.Key));
                if (courseName == "")
                    courseName = courseName + smallCourseNames;
                else
                    courseName = courseName + "','" + smallCourseNames;
            }
            //string courseName = string.Join("','",
            //    LbxCourseName.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg));
            string vender = string.Join("','",
                LbxVenders.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg.Split('_')[0]));
            string rbo= string.Join("','",
                LbxRbo.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg));
            string loginDateBegin = "";
            string loginDateEnd = "";

            if (ChkTime.IsChecked != null && (bool) ChkTime.IsChecked)
            {
                loginDateBegin = Convert.ToDateTime(TxtBeginDate.Text).ToString("yyyy-MM-dd");
                loginDateEnd = Convert.ToDateTime(TxtEndDate.Text).ToString("yyyy-MM-dd");
            }
            Query(userAccount,userName,courseName,rbo,vender,loginDateBegin,loginDateEnd);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            LbxCourseName.Show(courseNames);
            LbxVenders.Show(GlobalData.Venders);
            LbxRbo.Show(GlobalData.RboList);
            if (GlobalData.PrivilegeLevel > Privilege.Student)
            {
                TxtUserName.Text = "";
                TxtUserName.IsEnabled = true;
                TxtUserAccount.Text = "";
                TxtUserAccount.IsEnabled = true;
            }
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text =DateTime.Now.ToString("yyyy-MM-dd");
            ChkTime.IsChecked = false;
            DgvCourseDetail.ItemsSource = null;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = ParentPage;
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }

        private void DgvCourseDetail_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
            {
                CourseStudyDetail cell = (DgvCourseDetail.CurrentCell.Item) as CourseStudyDetail;
                PropertyInfo[] props = typeof(CourseStudyDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                string str = DgvCourseDetail.CurrentCell.Column.SortMemberPath;

                foreach (PropertyInfo prop in props)
                {
                    string propName = prop.Name;
                    object valueObg = prop.GetValue(cell, null);
                    if (propName == str)
                    {
                        System.Windows.Forms.Clipboard.SetText(valueObg.ToString());
                        break;
                    }
                }
            }
        }
    }
}
