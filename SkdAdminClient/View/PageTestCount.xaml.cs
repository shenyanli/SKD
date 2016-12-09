using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;
using SkdAdminModel;

namespace SkdAdminClient.View
{
    /// <summary>
    /// PageTrainningRecordCount.xaml 的交互逻辑
    /// </summary>
    public partial class PageTestCount : Page
    {
        private DataTable _dt = new DataTable();
        public Frame Frame;
        public PageMainNew PageMainNew;
        public PageProgressDetail ParentPage;
        public BindProgressDetail CurrentProgressDetail;
        List<BindTestCount> _tcs = new List<BindTestCount>();

        public PageTestCount()
        {
            InitializeComponent();
        }




        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            string courseName = CbxCourseName.Text.Trim();
            string vender = CbxVender.Text.Trim();
            string userName = TxtUserName.Text.Trim();
            string userAccount = TxtUserAccount.Text.Trim();
            if (vender.Trim() == "")
                DgvTrainningRecord.Columns[0].Visibility = Visibility.Collapsed;
            if (CurrentProgressDetail != null)
            {
                DgvTrainningRecord.Columns[7].Visibility = Visibility.Collapsed;
                DgvTrainningRecord.Columns[8].Visibility = Visibility.Visible;
            }
      
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
    
            _dt = skdServiceSoapClient.GetTestCount(courseName, vender,userName,userAccount);
            _tcs.Clear();
            foreach (DataRow row in _dt.Rows)
            {
                BindTestCount tc = new BindTestCount();
                tc.CourseName = row["courseName"].ToString();
                if (row["Items"].ToString().Trim() != "")
                    tc.Items = "* " + string.Join("\r\n* ", row["Items"].ToString().Split('|'));
                tc.Vender = vender;
                tc.Percent = row["percents"] + "%";
                tc.Type = row["type"].ToString()=="judge"?"判断题":"选择题";
                tc.RightCount = Convert.ToDouble(row["rightCount"]);
                tc.TotalCount = Convert.ToDouble(row["totalCount"]);
                tc.Title = row["Title"].ToString();
                tc.StandardAnswer = row["StandardAnswer"].ToString();
                _tcs.Add(tc);
            }
            DgvTrainningRecord.ItemsSource = null;
            DgvTrainningRecord.ItemsSource = _tcs.OrderByDescending(x=>x.CourseName);
            XMessageBox.ShowDialog("查询已完成！", "提示");
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgvTrainningRecord.Items.Count < 1)
                {
                    XMessageBox.ShowDialog("没有数据可供导出！", "提示");
                    return;
                }
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = "Excel|*.xls|Excel|*.xlsx"; //设定文件显示类型
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fName = sfd.FileName;
                    _dt = ObjectToDataTable.ToDataTable(_tcs);
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

            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            List<string> courseNames = skdServiceSoapClient.GetCourseName().ToList();
            CbxCourseName.ItemsSource = courseNames;
            List<string> orgList = skdServiceSoapClient.GetOrgList().ToList();
            orgList.Insert(0, "");
            CbxVender.ItemsSource = orgList;

            if (GolableData.PrivilegeLevel < Privilege.VenderAdmin)
            {
                CbxVender.Text = GolableData.Vender;
                CbxVender.IsEnabled = false;
                DgvTrainningRecord.Columns[0].Visibility = Visibility.Collapsed;
                TxtUserName.Text = GolableData.UserName;
                TxtUserName.IsEnabled = false;
                TxtUserAccount.Text = GolableData.UserAccount;
                TxtUserAccount.IsEnabled = false;
            }
            if (GolableData.PrivilegeLevel == Privilege.VenderAdmin)
            {
                CbxVender.Text = GolableData.Vender;
                CbxVender.IsEnabled = false;
                DgvTrainningRecord.Columns[0].Visibility = Visibility.Collapsed;
            }

            if (CurrentProgressDetail != null)//说明是从学习页面进来的
            {
                TxtUserAccount.Text = CurrentProgressDetail.UserAccount;
                TxtUserAccount.IsEnabled = false;
                TxtUserName.Text = CurrentProgressDetail.UserName;
                TxtUserName.IsEnabled = false;
                CbxCourseName.Text = CurrentProgressDetail.CourseName;
                CbxCourseName.IsEnabled = false;
                BtnBack.Visibility = Visibility.Visible;
                BtnClear.Visibility = Visibility.Hidden;
                BtnQuery_Click(null, null);
                BtnClear.IsEnabled = false;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            CbxCourseName.Text = "";
            DgvTrainningRecord.ItemsSource = null;
            if (GolableData.PrivilegeLevel >= Privilege.VenderAdmin)
            {
                CbxVender.Text = "";
                CbxVender.IsEnabled = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = ParentPage;
        }
    }
}
