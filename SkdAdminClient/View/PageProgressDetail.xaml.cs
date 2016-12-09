using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;
using SkdAdminModel;
using static System.StringSplitOptions;

//using NPOI.Extension;

namespace SkdAdminClient.View
{
    public enum LearnStatus
    {
        All,
        Finished,
        UnFinish
    }

    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageProgressDetail : Page
    {
        private DataTable _dt = new DataTable();
        private LearnStatus _learnStatus = 0;
        public Frame Frame;
        public PageMainNew PageMainNew;
        List<BindProgressDetail> progressDetails = new List<BindProgressDetail>();
        public string CurrentCourseName;
        public PageCourseCount ParentPage;

        public PageProgressDetail( )
        {
            InitializeComponent();
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
            TxtBeginFinishDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndFinishDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string vender = CbxVender.Text.Trim();
                string userAccount = TxtUserAccount.Text.Trim().ToLower();
                string userName = TxtUserName.Text.Trim();
                string courseName = CboCourseName.Text.Trim();
                string finishBeginTime = "";
                string finishEndTime = "";
                string dateBegin = "";
                string dateEnd = "";
                if (ChkTime.IsChecked != null && (bool)ChkTime.IsChecked)
                {
                    dateBegin = Convert.ToDateTime(TxtBeginDate.Text).ToString("yyyy-MM-dd");
                    dateEnd = Convert.ToDateTime(TxtEndDate.Text).ToString("yyyy-MM-dd");
                }
                if (ChkFinishTime.IsChecked != null && (bool)ChkFinishTime.IsChecked)
                {
                    finishBeginTime = Convert.ToDateTime(TxtBeginFinishDate.Text).ToString("yyyy-MM-dd");
                    finishEndTime = Convert.ToDateTime(TxtEndFinishDate.Text).ToString("yyyy-MM-dd");
                }
                SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();

                _dt = skdServiceSoapClient.GetProgressDetailTable(vender, userName, userAccount, courseName, dateBegin, dateEnd, finishBeginTime, finishEndTime, (int)_learnStatus);

                progressDetails.Clear();
                foreach (DataRow row in _dt.Rows)
                {
                    BindProgressDetail l = new BindProgressDetail();
                    l.Vender = row["OrganizationName"].ToString();
                    l.UserAccount = row["userAccount"].ToString();
                    l.UserName = row["userName"].ToString();
                    l.CourseName = row["courseName"].ToString();
                    l.BeginDate = row["beginTime"].ToString();
                    l.EndDate = row["EndTime"].ToString();
                    l.Scheduel = row["progress"].ToString() + "%";
                    l.Score = Convert.ToDouble(row["Score"].ToString());
                    l.Rbo = row["OrganizationLocation"].ToString();
                    double totalMinutes = Convert.ToDouble(row["totalMinutes"].ToString());
                    int hours = (int)(totalMinutes / 60);
                    int minutes = (int)(totalMinutes % 60);
                    l.TotalMinutes = hours.ToString("00") + ":" + minutes.ToString("00") + ":00";
                    l.TotalStamps = Convert.ToDouble(row["totalStamp"].ToString());             
                    if (row["TrainningRecord"].ToString().Trim() != "")
                    {
                        l.HaveTrainningRecord = true;
                        l.TrainningRecord = "\r\n";
                        foreach (string str in row["TrainningRecord"].ToString().Split(new [] {'|'}, RemoveEmptyEntries)) 
                        {
                            l.TrainningRecord += (str.Split(':')[1]+"分").PadRight(5) + "["+str.Split(':')[0]+"]\r\n";
                        }
                        l.SysIdList = row["TrainningSysId"].ToString().Split(new[] {'|'},RemoveEmptyEntries).ToList();
                    }
                    
                    l.Status = l.Scheduel == "100%" ? "已完成" : "未完成";
                    progressDetails.Add(l);
                }
                DgvProgressDetail.ItemsSource = null;
       
                DgvProgressDetail.ItemsSource = progressDetails.OrderByDescending(x=>x.BeginDate);
                XMessageBox.ShowDialog("查询已完成！", "提示");
            }
            catch (Exception err)
            {

                XMessageBox.ShowDialog(err.Message, "提示");
            }
           
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (DgvProgressDetail.Items.Count < 1)
            {
                XMessageBox.ShowDialog("没有数据可供导出！", "提示");
                return;
            }

            try
            {
                if (DgvProgressDetail.Items.Count < 1)
                {
                    XMessageBox.ShowDialog("没有数据可供导出！", "提示");
                    return;
                }
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = "Excel|*.xls|Excel|*.xlsx"; //设定文件显示类型
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fName = sfd.FileName;
                    _dt = ObjectToDataTable.ToDataTable(progressDetails);
                    NpoiHelper.ExportDataTableToExcel(fName, _dt, Name, false);
                }
                else
                {
                    return;
                }
            }
            catch (Exception err)
            {
                XMessageBox.ShowDialog("导出失败！" + err.Message, "提示");
                return;
            }
            XMessageBox.ShowDialog("导出成功！", "提示");

           
        }
   
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TbTitle.Text = Name;
            Status.ItemsSource = new List<string> {"全部", "已完成", "未完成"};
            Status.SelectedIndex = 0;
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            List<string> courseNames = skdServiceSoapClient.GetCourseName().ToList();
            CboCourseName.ItemsSource = courseNames;
            List<string> orgList = skdServiceSoapClient.GetOrgList().ToList();
            orgList.Insert(0, "");
            CbxVender.ItemsSource = orgList;

            if (GolableData.PrivilegeLevel <=Privilege.VenderAdmin)
            {
                CbxVender.Text = GolableData.Vender;
                CbxVender.IsEnabled = false;
            }
            if (GolableData.PrivilegeLevel < Privilege.VenderAdmin)
            {
                TxtUserName.Text = GolableData.UserName;
                TxtUserName.IsEnabled = false;
                TxtUserAccount.Text = GolableData.UserAccount;
                TxtUserAccount.IsEnabled = false;
            }
            if (ParentPage != null)
            {
                BtnBack.Visibility = Visibility.Visible;
                CboCourseName.Text = CurrentCourseName;
                BtnQuery_Click(null,null);
            }
        }


        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (GolableData.PrivilegeLevel >Privilege.VenderAdmin)
            {
                CbxVender.Text = "";
                CbxVender.IsEnabled = true;
            }
            if (GolableData.PrivilegeLevel > Privilege.Student)
            {
                TxtUserName.Text = "";
                TxtUserName.IsEnabled = true;
                TxtUserAccount.Text = "";
                TxtUserAccount.IsEnabled = true;
            }

            CboCourseName.Text = "";
            CboCourseName.SelectedIndex = -1;
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DgvProgressDetail.ItemsSource = null;
            _learnStatus = 0;
            ChkTime.IsChecked = false;
        }


        private void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Status.SelectedIndex >0)
            {
                _learnStatus =(LearnStatus)Status.SelectedIndex;
            }
            else
            {
                _learnStatus = 0;
            }
        }

        private void ButtonCourseStudyDetail_Click(object sender, RoutedEventArgs e)
        {
            View.PageCourseStudyDetail childPage = new View.PageCourseStudyDetail();
            childPage.Frame = Frame;
            childPage.ParentPage = this;
            childPage.PageMainNew = PageMainNew;
            BindProgressDetail p = DgvProgressDetail.SelectedItem as BindProgressDetail;
            if (p != null)
            {
                childPage.CurrentProgressDetail = p;
            }
            Frame.Content = childPage;
        }

        private void ButtonTrainningRecord_Click(object sender, RoutedEventArgs e)
        {
            PageTrainningRecord childPage = new PageTrainningRecord();
            childPage.Frame = Frame;
            childPage.ParentPage = this;
            childPage.PageMainNew = PageMainNew;
            BindProgressDetail p = DgvProgressDetail.SelectedItem as BindProgressDetail;
            if (p != null)
            {
                childPage.CurrentProgressDetail = p;

            }
            Frame.Content = childPage;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            PageTestCount childPage = new PageTestCount();
            childPage.Frame = Frame;
            childPage.ParentPage = this;
            childPage.PageMainNew = PageMainNew;
            BindProgressDetail p = DgvProgressDetail.SelectedItem as BindProgressDetail;
            if (p != null)
            {
                childPage.CurrentProgressDetail = p;
            }
            Frame.Content = childPage;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = ParentPage;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }
    }
}
