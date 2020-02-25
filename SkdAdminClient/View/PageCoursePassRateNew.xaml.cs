using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;
using SkdAdminModel;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace SkdAdminClient.View
{



    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageCoursePassRateNew : Page
    {
        public PageCoursePassRateNew ParrentPage ;
        private DataTable _dt = new DataTable();
        List<BindCourseCount> courseCounts = new List<BindCourseCount>();
        public Frame Frame ;
        public PageMainNew PageMainNew;
        public string CurrentCourseName="";//要查询的课程
        public string CurrentVender="" ;//要查询的经销商
        public string LastCourseName;//父页面条件选择的课程名称
        public string LastVender;//父页面条件选择的经销商名称
        public string LastRbo;//父页面条件选择的RBO
        private bool showMsgBox = true;
        SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
        List<string> courseNameList = new  List<string>();
        public PageCoursePassRateNew( )
        {
            InitializeComponent();
            TxtFinishBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtFinishEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TbTitle.Text = Name;
            if (ParrentPage != null && ChildPage==null) //说明是从另一个页面进来的
            {
                BtnBack.IsEnabled = true;
                BtnClear.IsEnabled = false;
                LbxCourseName.Show(new List<string>() {CurrentCourseName}, true); //显示需要查询的课程名称
                LbxCourseName.IsEnabled = false;
                if (CurrentVender !="")
                {
                    LbxVenderCode.Show(new List<string> { CurrentVender }, true); //显示需要查询的经销商信息
                    LbxVenderCode.IsEnabled = false;
                }
                showMsgBox = false;
                Thread thread = new Thread(Start);
                thread.Start();
                showMsgBox = true;
            }
            else if(ChildPage==null)
            {
                LbxCourseName.Show(GlobalData.CourseNameMaps.Values.Distinct().ToList());
                LbxVenderCode.Show(GlobalData.Venders);
                LbxRboName.Show(GlobalData.RboList);
            }
        
        }

        void Start()
        {
            BtnQuery_Click(null, null);
        }

        void PartVenderCoursePassRate(string rbos,string venderIds,string courseNames, string studyBeginDate, string studyEndDate, string finishBeginDate, string finishEndDate,int status)
        {
            SkdServiceSoapClient serviceSoapClient = new SkdServiceSoapClient();
            _dt = serviceSoapClient.PartVenderCoursePassRate(rbos,venderIds,courseNames, finishBeginDate, finishEndDate,
                studyBeginDate, studyEndDate,status);
            courseCounts.Clear();
            int venderTotalCounts = LbxVenderCode.BindingAllSourses.Count;//总经销商数
            foreach (DataRow row in _dt.Rows)
            {
                BindCourseCount c = new BindCourseCount();
                c.Vender = row["VenderName"].ToString();
                c.VenderCode = row["VenderId"].ToString();
                c.Rbo = row["Rbo"].ToString();
                c.CourseName = row["bigcourseName"].ToString();
                int hour = (int)Convert.ToDouble(row["TotalMinutes"].ToString()) / 60;
                int minute = (int)Convert.ToDouble(row["TotalMinutes"].ToString()) % 60;
                double totalMinutes = hour + Math.Round(minute / 60.00, 2);
                c.TotalMinutes = totalMinutes.ToString("f2");
                c.TotalStamps = Convert.ToDouble(row["TotalStamps"].ToString());
                c.TotalPersons = Convert.ToDouble(row["BeginPersonCounts"].ToString());
                c.FinishPersons = Convert.ToDouble(row["FinishPersonCounts"].ToString());
                c.VenderTotalCounts = venderTotalCounts;
                c.VenderBeginCounts = string.IsNullOrEmpty(row["BeginVenderCounts"].ToString()) ? 0 : Convert.ToInt32(row["BeginVenderCounts"].ToString());
                c.ReleaseDate = GlobalData.CourseReleaseDate.ContainsKey(c.CourseName) ? "\r\n" + GlobalData.CourseReleaseDate[c.CourseName].Replace("|","\r\n") : "";//2018-11-08 
                courseCounts.Add(c);
            }
            DgvCoursePassRate.ItemsSource = courseCounts.OrderByDescending(x => x.CourseName);
            if (showMsgBox)
            {
           //     Expander.IsExpanded = false;
                XMessageBox.ShowDialog("查询到相关数据" + courseCounts.Count + "笔", "提示");
            }
        }

        void AllVenderCoursePassRate(string courseNames,string studyBeginDate,string studyEndDate, string finishBeginDate, string finishEndDate)
        {
          
            SkdServiceSoapClient serviceSoapClient = new SkdServiceSoapClient();
            _dt = serviceSoapClient.AllVenderCoursePassRate(courseNames, finishBeginDate, finishEndDate,
                studyBeginDate, studyEndDate);
            courseCounts.Clear();
            int venderTotalCounts = LbxVenderCode.BindingAllSourses.Count;//总经销商数
            foreach (DataRow row in _dt.Rows)
            {
                BindCourseCount c = new BindCourseCount();
                c.CourseName = row["bigcourseName"].ToString();
                c.Vender = row["VenderName"].ToString();
                c.VenderCode = row["VenderId"].ToString();
                c.Rbo = row["Rbo"].ToString();
                int hour = (int)Convert.ToDouble(row["TotalMinutes"].ToString()) / 60;
                int minute = (int)Convert.ToDouble(row["TotalMinutes"].ToString()) % 60;
                double totalMinutes = hour + Math.Round(minute / 60.00, 2);
                c.TotalMinutes = totalMinutes.ToString("f2");
                c.TotalStamps = Convert.ToDouble(row["TotalStamps"].ToString());
                c.TotalPersons = Convert.ToDouble(row["BeginPersonCounts"].ToString());
                c.FinishPersons = Convert.ToDouble(row["FinishPersonCounts"].ToString());
                c.VenderTotalCounts = venderTotalCounts;
                c.VenderBeginCounts = string.IsNullOrEmpty(row["BeginVenderCounts"].ToString()) ? 0 : Convert.ToInt32(row["BeginVenderCounts"].ToString());
                c.ReleaseDate = GlobalData.CourseReleaseDate.ContainsKey(c.CourseName) ? "\r\n"+GlobalData.CourseReleaseDate[c.CourseName].Replace("|", "\r\n") : "";//2018-11-08 
                courseCounts.Add(c);
            }
            DgvCoursePassRate.ItemsSource = courseCounts.OrderByDescending(x => x.CourseName);
            if (showMsgBox)
            {
             //   Expander.IsExpanded = false;
                XMessageBox.ShowDialog("查询到相关数据" + courseCounts.Count + "笔", "提示");
            }
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string courseNames = string.Join(",",
LbxCourseName.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg));
                string rbos = string.Join(",",
                    LbxRboName.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg));
                string venderIds = "";
                if (LbxVenderCode.BindingAllSourses.Count > 0 &&
                    LbxVenderCode.BindingAllSourses.All(x => x.IsSelected) &&
                    GlobalData.PrivilegeLevel == Privilege.SuperAdmin)
                {
                    venderIds = "all";
                    rbos = "";
                }
                else
                {
                    venderIds = string.Join(",",
                        LbxVenderCode.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg.Split('_')[0]));
                }
                if (courseNames == "")
                {
                    XMessageBox.Show("请选择要查询的课程!");
                    return;
                }
                if (LbxVenderCode.BindingAllSourses.Count>0 && venderIds == "" && rbos == "")
                {
                    XMessageBox.Show("请选择要查询的经销商或者经销商区域!");
                    return;
                }
                string finishBeginDate = "";
                string finishEndDate = "";
                string studyBeginDate = "";
                string studyEndDate = "";
                if (ChkFinishTime.IsChecked != null && (bool)ChkFinishTime.IsChecked)
                {
                    finishBeginDate = Convert.ToDateTime(TxtFinishBeginDate.Text).ToString("yyyy-MM-dd");
                    finishEndDate = Convert.ToDateTime(TxtFinishEndDate.Text).ToString("yyyy-MM-dd");
                }
                if (ChkBeginTime.IsChecked != null && (bool)ChkBeginTime.IsChecked)
                {
                    studyBeginDate = Convert.ToDateTime(TxtBeginDate.Text).ToString("yyyy-MM-dd");
                    studyEndDate = Convert.ToDateTime(TxtEndDate.Text).ToString("yyyy-MM-dd");
                }
                var colums = DgvCoursePassRate.Columns.FirstOrDefault(x => x.Header.ToString() == "已学习经销商数量");

                if (venderIds == "all")
                {
                    if (colums != null)
                        colums.Visibility = Visibility.Visible;

                    AllVenderCoursePassRate(courseNames, studyBeginDate, studyEndDate, finishBeginDate, finishEndDate);
                }

                else
                {

                    if (colums != null)
                        colums.Visibility = Visibility.Collapsed;
                    PartVenderCoursePassRate(rbos, venderIds, courseNames, studyBeginDate, studyEndDate, finishBeginDate, finishEndDate,CbxStatus.SelectedIndex);
                }
            }));
       

 
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgvCoursePassRate.Items.Count < 1)
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
            }
            catch (Exception err)
            {
                XMessageBox.ShowDialog("导出失败！ "+err.Message, "提示");
            }
            
        }




        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            LbxCourseName.Show(courseNameList);
            LbxVenderCode.Show(GlobalData.Venders);
            LbxRboName.Show(GlobalData.RboList);
            ChkBeginTime.IsChecked = false;
            ChkFinishTime.IsChecked = false;
            DgvCoursePassRate.ItemsSource = null;
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = ParrentPage;
        }

  

        private void DgvCoursePassRate_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Detail();
        }

        private void DgvCoursePassRate_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
            {
                BindCourseCount cell =( DgvCoursePassRate.CurrentCell.Item )as BindCourseCount;
                PropertyInfo[] props = typeof(BindCourseCount).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                string str = DgvCoursePassRate.CurrentCell.Column.SortMemberPath;
                if (str == "")
                {
                    TextBlock textBlock=sender as TextBlock;
                    if (textBlock != null) str = textBlock.Text;
                }
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

        private void ButtonProgress_Click(object sender, RoutedEventArgs e)
        {
            Detail();
        }

        public Page ChildPage;

        void Detail()
        {
            BindCourseCount courseCount = DgvCoursePassRate.SelectedItem as BindCourseCount;
            if (courseCount == null) return;
            if (courseCount.Vender.Trim() == "全部经销商") //如果当前选中的行是全部经销商的信息，则双击进入每个经销商的统计信息
            {
                PageCoursePassRateNew childPage = new PageCoursePassRateNew();
                childPage.Frame = Frame;
                childPage.CurrentCourseName = courseCount.CourseName;
                if (childPage.CurrentCourseName.Trim() == "") return;
                childPage.ParrentPage = this;
                ChildPage = childPage;
                childPage.PageMainNew = PageMainNew;
                Frame.Content = childPage;
            }
            else//如果不是全部经销商的信息，则说明是某一个具体经销商的信息，此时双击硬挨进入当前经销商下的学员信息页面
            {
                PageProgressDetail childPage = new PageProgressDetail();
                childPage.Frame = Frame;
                childPage.CurrentCourseName = courseCount.CourseName;
                childPage.CurrentVenderCode = courseCount.VenderCode;
                childPage.ParentPage = this;
                ChildPage = childPage;
                childPage.PageMainNew = PageMainNew;
                Frame.Content = childPage;
            }
        }
    }
}
