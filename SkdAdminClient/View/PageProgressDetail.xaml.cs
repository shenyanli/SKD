using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;
using SkdAdminModel;


//using NPOI.Extension;

namespace SkdAdminClient.View
{
    public enum LearnStatus
    {
        All,
        Finished,
        UnFinish,
        UnStart
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
        public string CurrentCourseName="";
        public string CurrentVenderCode = "";
        public string ParentPageVender="";
        public string ParentPageCourseName = "";
        public PageCoursePassRateNew ParentPage;
        private bool _showMsgBox = true;
        SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
        List<string> courseNames = new List<string>();

        public PageProgressDetail( )
        {
            InitializeComponent();
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
            TxtBeginFinishDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndFinishDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TbTitle.Text = Name;
            Status.ItemsSource = new List<string> { "全部",  "未完成", "未开始", "已完成" };

            if (ParentPage != null)
            {
                BtnBack.IsEnabled = true;
                BtnClear.IsEnabled = false;
                Status.SelectedIndex = 0; //全部状态的信息;
                LbxCourseNames.Show(new List<string>() { CurrentCourseName }, true); //显示需要查询的课程名称
                LbxCourseNames.IsEnabled = false;
                LbxVenders.Show(new List<string>() {CurrentVenderCode},true);
                LbxVenders.IsEnabled = false;
                _showMsgBox = false;
                BtnQuery_Click(null, null);
                _showMsgBox = true;
            }
            else
            {
                courseNames = skdServiceSoapClient.GetCourseName().ToList();
                LbxVenders.Show(GlobalData.Venders);
                LbxRbo.Show(GlobalData.RboList);
                //LbxCourseNames.Show(courseNames);
                LbxCourseNames.Show(GlobalData.CourseNameMaps.Values.Distinct().ToList());
                Status.SelectedIndex = 0; //未完成状态的信息
            }
         
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnQuery.Content = "检索中...";
                string venders = string.Join(",",
                    LbxVenders.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg.Split('_')[0]));
                string courseName = string.Join(",",
                    LbxCourseNames.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg));
                string rbos = string.Join(",",
                    LbxRbo.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg));
                string userAccount = TxtUserAccount.Text.Trim().ToLower();
                string userName = TxtUserName.Text.Trim();


                string finishBeginTime = "";
                string finishEndTime = "";
                string dateBegin = "";
                string dateEnd = "";
                if (ChkTime.IsChecked != null && (bool) ChkTime.IsChecked)
                {
                    dateBegin = Convert.ToDateTime(TxtBeginDate.Text).ToString("yyyy-MM-dd");
                    dateEnd = Convert.ToDateTime(TxtEndDate.Text).ToString("yyyy-MM-dd");
                }
                if (ChkFinishTime.IsChecked != null && (bool) ChkFinishTime.IsChecked)
                {
                    finishBeginTime = Convert.ToDateTime(TxtBeginFinishDate.Text).ToString("yyyy-MM-dd");
                    finishEndTime = Convert.ToDateTime(TxtEndFinishDate.Text).ToString("yyyy-MM-dd");
                }
                SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();

                //_dt = skdServiceSoapClient.GetProgressDetailTable(rbos, venders, userName, userAccount, courseName,
                //    dateBegin,
                //    dateEnd, finishBeginTime, finishEndTime, (int) _learnStatus);
                _dt = skdServiceSoapClient.ProgressDetailTable(rbos, venders, userName, userAccount, courseName,
                    dateBegin, dateEnd, finishBeginTime, finishEndTime, Status.SelectedIndex);
                progressDetails.Clear();
                foreach (DataRow row in _dt.Rows)
                {
                    BindProgressDetail l = new BindProgressDetail();
                 
                    l.VenderId = row["OrganizationId"].ToString(); //GlobalData.Venders.Where(x => x.Split('_')[1] == l.Vender).Select(x => x.Split('_')[0]).FirstOrDefault();
                    l.Vender = row["OrganizationName"].ToString();
                    l.Rbo = row["OrganizationLocation"].ToString();
                    l.UserAccount = row["userAccount"].ToString();
                    l.UserName = row["userName"].ToString();
                    l.CourseName = row["bigcourseName"].ToString();
                    l.Score= Convert.ToDouble( row["Score"]);
                    //  l.BeginDate = row["beginTime"].ToString();
                    //  l.EndDate = row["EndTime"].ToString();
                    l.CreateDate = row["CreateDate"].ToString();
                    
                    if (row["FinishDate"].ToString().Trim() != "")
                    {
                        l.FinishDate = (Convert.ToDateTime(row["FinishDate"].ToString().Trim()) ==
                                        Convert.ToDateTime("1900/1/1 0:00:00"))
                            ? ""
                            : row["FinishDate"].ToString().Trim();
                    }
                    l.Scheduel = row["progress"].ToString().Trim() == "" ? "0%" : row["progress"] + "%";
                    //if ((l.FinishDate==null ||l.FinishDate.Trim() == "") && l.Scheduel == "100%")//如果进度已经达到100%但是finishTime尚未有时间显示，则默认显示上次学习完成时间
                    //{
                    //    l.FinishDate = l.EndDate;
                    //}
                 //   l.Score = Convert.ToDouble(row["Score"].ToString());
                 
                    double totalMinutes = Convert.ToDouble(row["totalMinutes"].ToString());
                    int hours = (int) (totalMinutes/60);
                    int minutes = (int) (totalMinutes%60);
                    l.TotalMinutes = hours.ToString("00") + ":" + minutes.ToString("00") + ":00";
                    l.TotalStamps = Convert.ToDouble(row["totalStamps"].ToString());
                    if (row["TrainningRecord"].ToString().Trim() != "")
                    {
                        l.HaveTrainningRecord = true;
                        l.TrainningRecord = "\r\n";
                        foreach (string str in row["TrainningRecord"].ToString().Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries))
                        {
                            l.TrainningRecord += (str.Split(':')[1] + "分").PadRight(5) + "[" + str.Split(':')[0] +
                                                 "]\r\n";
                        }
                     //   l.SysIdList = row["TrainningSysId"].ToString().Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                    if (row["progress"].ToString().Trim() == "100")
                    {
                        l.Status = "已完成";
                    }
                    else if (row["progress"].ToString().Trim() == "" ||Convert.ToInt32(row["progress"])==0)//2017-04-27 去除进度为0的
                    {
                        l.Status = "未开始";
                    }
                    else
                    {
                        l.Status = "未完成";
                    }
                    progressDetails.Add(l);
                }
                DgvProgressDetail.ItemsSource = null;

                DgvProgressDetail.ItemsSource = progressDetails;
                BtnQuery.Content = "检索";
                if (_showMsgBox)
                {
                    Expander.IsExpanded = false;
                    XMessageBox.ShowDialog("查询到相关数据" + progressDetails.Count + "笔!", "提示");
                }

            }
            catch (Exception err)
            {
                BtnQuery.Content = "检索";
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
   



        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalData.PrivilegeLevel > Privilege.Student)
            {
                TxtUserName.Text = "";
                TxtUserName.IsEnabled = true;
                TxtUserAccount.Text = "";
                TxtUserAccount.IsEnabled = true;
            }
            DgvProgressDetail.ItemsSource = null;
            LbxVenders.Show(GlobalData.Venders);
            LbxRbo.Show(GlobalData.RboList);
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            TxtBeginFinishDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndFinishDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            _learnStatus = 0;
            ChkTime.IsChecked = false;
            ChkFinishTime.IsChecked = false;
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
            View.PageCourseStudyDetailHis childPage = new View.PageCourseStudyDetailHis();
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
            PageTrainningRecordHis childPage = new PageTrainningRecordHis();
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
            PageTestRightRate childPage = new PageTestRightRate();
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
            ParentPage.LastCourseName = ParentPageCourseName;
            ParentPage.LastVender = ParentPageVender;
            Frame.Content = ParentPage;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }

        private void DgvProgressDetail_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
            {
                BindProgressDetail cell = (DgvProgressDetail.CurrentCell.Item) as BindProgressDetail;
                PropertyInfo[] props = typeof(BindProgressDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                string str = DgvProgressDetail.CurrentCell.Column.SortMemberPath;
                if (str == "") return;
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
