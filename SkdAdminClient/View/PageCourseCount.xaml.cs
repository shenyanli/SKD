using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;
using SkdAdminModel;

namespace SkdAdminClient.View
{



    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageCourseCount : Page
    {
        private DataTable _dt = new DataTable();
        List<BindCourseCount> courseCounts = new List<BindCourseCount>();
        public Frame Frame ;
        public PageMainNew PageMainNew;
        public PageCourseCount( )
        {
            InitializeComponent();
            TxtBeginDate.Text = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
       
            string vender = CbxVender.Text.Trim()=="全部经销商"?"All": CbxVender.Text.Trim();
            if (GolableData.PrivilegeLevel ==Privilege.AreaAdmin)//区域管理员
            {
                vender = string.Join("','", GolableData.Venders);
            }
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
                BindCourseCount c = new BindCourseCount();
                c.CourseName = row["courseName"].ToString();
                c.Vender = row["OrganizationName"].ToString();
                int hour = (int)Convert.ToDouble(row["TotalMinutes"].ToString()) / 60;
                int minute = (int)Convert.ToDouble(row["TotalMinutes"].ToString()) % 60;
                c.TotalMinutes = hour.ToString("00") + ":" + minute.ToString("00") + ":00";
                c.TotalStamps = Convert.ToDouble(row["TotalStamp"].ToString());
                c.PlanPersons = Convert.ToDouble(row["planPersons"]);
                c.TotalPersons = Convert.ToDouble(row["totalPersons"].ToString());
                c.FinishPersons = Convert.ToDouble(row["finishPersons"].ToString());
                //c.Percent = Math.Round(Convert.ToDouble(row["percents"]), 2) + "%";
                if (c.PlanPersons > 0)
                    c.Percent = Math.Round(c.FinishPersons/c.PlanPersons, 2)*100 + "%";
                else
                {
                    c.Percent = "应学人数维护错误！";
                }
                courseCounts.Add(c);
            }

            if (GolableData.PrivilegeLevel > Privilege.VenderAdmin)
            {
                ListCollectionView LoginTotals = new ListCollectionView(courseCounts);
                LoginTotals.GroupDescriptions.Add(new PropertyGroupDescription("CourseName"));
       
                DgvLoginTotal.ItemsSource = null;
                DgvLoginTotal.ItemsSource = LoginTotals;
            }
            else
            {
                DgvLoginTotal.Columns[1].Visibility = Visibility.Visible;
                DgvLoginTotal.ItemsSource = courseCounts.OrderByDescending(x=>x.CourseName);
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


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TbTitle.Text = Name;
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            List<string> courseNames = skdServiceSoapClient.GetCourseName().ToList();
            CbxCourseName.ItemsSource = courseNames;
            List<string> orgList = skdServiceSoapClient.GetOrgList().ToList();
            orgList.Insert(0, "");
            orgList.Insert(1,"全部经销商");
            CbxVender.ItemsSource = orgList;
            if (GolableData.PrivilegeLevel ==Privilege.VenderAdmin)//经销商管理员
            {
                CbxVender.Text = GolableData.Vender;
                CbxVender.IsEnabled = false;
                DgvLoginTotal.Columns[0].Visibility = Visibility.Collapsed;
                DgvLoginTotal.Columns[8].Visibility = Visibility.Visible;

            }//经销商管理员以上级别
            else
            {
                DgvLoginTotal.Columns[0].Visibility = Visibility.Visible;
                DgvLoginTotal.Columns[8].Visibility = Visibility.Collapsed;
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

  

        private void ButtonProgress_Click(object sender, RoutedEventArgs e)
        {
            PageProgressDetail childPage = new PageProgressDetail();
            childPage.Frame = Frame;
            //childPage.ParentPage = this;
            BindCourseCount courseCount = DgvLoginTotal.SelectedItem as BindCourseCount;
            if (courseCount != null)
            {
                childPage.CurrentCourseName=courseCount.CourseName ;
               
            }
            Frame.Content = childPage;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }
    }
}
