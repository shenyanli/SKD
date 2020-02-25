using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
    public partial class PageTestRightRate : Page
    {
        private DataTable _dt = new DataTable();
        public Frame Frame;
        public PageMainNew PageMainNew;
        public PageProgressDetail ParentPage;
        public BindProgressDetail CurrentProgressDetail;
        List<BindTestCount> _tcs = new List<BindTestCount>();
        private bool _showMsgBox = true;
        SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
        List<string> courseNames = new List<string>();

        public PageTestRightRate()
        {
            InitializeComponent();
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            BtnQuery.Content = "检索中...";
            string courseName = string.Join("','",
                LbxCourseNames.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg));
            string vender = string.Join("','",
                LbxVenders.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg.Split('_')[0]));
            string rbo = string.Join("','",
                LbxRbo.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg));
            string userName = TxtUserName.Text.Trim();
            string userAccount = TxtUserAccount.Text.Trim();
            if (vender.Trim() == "")
                DgvTrainningRecord.Columns[0].Visibility = Visibility.Collapsed;
            if (CurrentProgressDetail != null)
            {
                DgvTrainningRecord.Columns[7].Visibility = Visibility.Collapsed;
                DgvTrainningRecord.Columns[8].Visibility = Visibility.Visible;
            }
            _dt = skdServiceSoapClient.GetTestCount(courseName, rbo, vender, userName, userAccount);
            _tcs.Clear();
            foreach (DataRow row in _dt.Rows)
            {
                BindTestCount tc = new BindTestCount();
                tc.CourseName = row["courseName"].ToString();
                if (row["Items"].ToString().Trim() != "")
                    tc.Items = "* " + string.Join("\r\n* ", row["Items"].ToString().Split('|'));
                tc.Vender = LbxVenders.BindingAllSourses.All(x=>x.IsSelected)?"全部经销商": vender;
                tc.Percent = row["percents"] + "%";
                tc.Type = row["type"].ToString() == "judge" ? "判断题" : "选择题";
                tc.RightCount = Convert.ToDouble(row["rightCount"]);
                tc.TotalCount = Convert.ToDouble(row["totalCount"]);
                tc.Title = row["Title"].ToString();
                tc.StandardAnswer = row["StandardAnswer"].ToString();
                _tcs.Add(tc);
            }
            DgvTrainningRecord.ItemsSource = null;
            DgvTrainningRecord.ItemsSource = _tcs.OrderByDescending(x => x.CourseName);
            BtnQuery.Content = "检索";
            if (_showMsgBox)
            {
                Expander.IsExpanded = false;
                XMessageBox.ShowDialog("查询到相关数据" + _tcs.Count + "笔", "提示");
            }
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


            if (CurrentProgressDetail != null) //说明是从学习页面进来的
            {
                TxtUserAccount.Text = CurrentProgressDetail.UserAccount;
                TxtUserAccount.IsEnabled = false;
                TxtUserName.Text = CurrentProgressDetail.UserName;
                TxtUserName.IsEnabled = false;
                LbxCourseNames.Show(new List<string> {CurrentProgressDetail.CourseName},true);
                LbxCourseNames.IsEnabled = false;
                BtnBack.IsEnabled =true;
                BtnClear.IsEnabled = false;
                _showMsgBox = false;
                BtnQuery_Click(null, null);
                _showMsgBox = true;
            }
            else
            {
                courseNames = skdServiceSoapClient.GetCourseName().ToList();
                LbxCourseNames.Show(courseNames);
                LbxVenders.Show(GlobalData.Venders);
                LbxRbo.Show(GlobalData.RboList);
            }

        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            LbxCourseNames.Show(courseNames);
            LbxVenders.Show(GlobalData.Venders);
            LbxRbo.Show(GlobalData.RboList);
            DgvTrainningRecord.ItemsSource = null;
            if (GlobalData.PrivilegeLevel > Privilege.Student)
            {
                TxtUserName.Text = "";
                TxtUserName.IsEnabled = true;
                TxtUserAccount.Text = "";
                TxtUserAccount.IsEnabled = true;
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

        private void DgvTrainningRecord_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
            {
                BindTestCount cell = (DgvTrainningRecord.CurrentCell.Item) as BindTestCount;
                PropertyInfo[] props = typeof (BindTestCount).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                string str = DgvTrainningRecord.CurrentCell.Column.SortMemberPath;
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

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (Expander.IsExpanded)
                Expander.Header = "检索信息";
            else
                Expander.Header = "检索信息（点击展开）";
        }
    }
}
