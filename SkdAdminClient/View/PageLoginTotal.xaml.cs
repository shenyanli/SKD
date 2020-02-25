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
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace SkdAdminClient.View
{

    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageLoginTotal : Page
    {
        public bool DetailBackToTotal = false;
        public Frame Frame;
        public PageMainNew PageMainNew;
        private DataTable _dt = new DataTable();
        List<BindLoginTotal> loginTotals = new List<BindLoginTotal>();
        private bool _showMsgBox = true;
        public PageLoginTotal()
        {
            InitializeComponent();
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TbTitle.Text = Name;
            if (!DetailBackToTotal)
            {
                LbxVenders.Show(GlobalData.Venders);
                LbxRbo.Show(GlobalData.RboList);
            }
            _showMsgBox = false;
            BtnQuery_Click(null, null);
            _showMsgBox = true;
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            BtnQuery.Content = "检索中...";
            string vender = string.Join("','",LbxVenders.BindingAllSourses.Where(x=>x.IsSelected).Select(x=>x.TextMsg.Split('_')[0]));
            string rbo = string.Join("','",LbxRbo.BindingAllSourses.Where(x => x.IsSelected).Select(x=>x.TextMsg));
            string userAccount = TxtUserAccount.Text.Trim().ToLower();
            string userName = TxtUserName.Text.Trim();
            string loginDateBegin = "";
            string loginDateEnd = "";

            if (ChkTime.IsChecked != null && (bool)ChkTime.IsChecked)
            {
                loginDateBegin = Convert.ToDateTime(TxtBeginDate.Text).ToString("yyyy-MM-dd");
                loginDateEnd = Convert.ToDateTime(TxtEndDate.Text).ToString("yyyy-MM-dd");
            }
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();

             _dt = skdServiceSoapClient.GetLoginTotalTable(rbo,vender, userName, userAccount, loginDateBegin,loginDateEnd);

            loginTotals.Clear();
            foreach (DataRow row in _dt.Rows)
            {
                BindLoginTotal l = new BindLoginTotal();
                l.Vender = row["OrganizationName"].ToString();
                l.UserAccount= row["userAccount"].ToString();
                l.UserName= row["userName"].ToString();

                double stayTime = row["TotalMinutes"].ToString().Trim() == ""
                    ? 0
                    : Convert.ToDouble(row["TotalMinutes"].ToString());
                int hours = (int)(stayTime / 60);
                int minutes = (int)(stayTime % 60);
                l.TotalMinutes = hours.ToString("00") + ":" + minutes.ToString("00") + ":00";
                l.LoginCounts = row["LoginCounts"].ToString().Trim()==""?0:Convert.ToDouble(row["LoginCounts"].ToString());
                l.Status = row["Status"].ToString()=="0"?"不在线":"在线";
                l.LastLoginDate = row["loginDate"].ToString();
                l.LastLogoutDate = row["LastLogoutDate"].ToString();
                loginTotals.Add(l);
            }
            DgvLoginTotal.ItemsSource = null;
            DgvLoginTotal.ItemsSource = loginTotals.OrderByDescending(x=>x.LastLoginDate);
            BtnQuery.Content = "检索";
            if (_showMsgBox)
            {
                Expander.IsExpanded = false;
                XMessageBox.ShowDialog("查询到相关数据" + loginTotals.Count + "笔", "提示");
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {

            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text =DateTime.Now.ToString("yyyy-MM-dd");
            ChkTime.IsChecked = false;
            DgvLoginTotal.ItemsSource = null;
            LbxVenders.Show(GlobalData.Venders);
            LbxRbo.Show(GlobalData.RboList);
            if (GlobalData.PrivilegeLevel > Privilege.Student)
            {
                TxtUserName.Text = "";
                TxtUserName.IsEnabled = true;
                TxtUserAccount.Text = "";
                TxtUserAccount.IsEnabled = true;
            }
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
                    _dt = ObjectToDataTable.ToDataTable(loginTotals);
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }

        private void ButtonHistory_Click(object sender, RoutedEventArgs e)
        {
            PageLoginHis childPage = new PageLoginHis();
            childPage.Frame = Frame;
            childPage.ParentPage = this;
            childPage.PageMainNew = PageMainNew;
            BindLoginTotal blt = DgvLoginTotal.SelectedItem as BindLoginTotal;
            if (blt != null)
            {
                childPage.LoginTotal = blt;
            }
            Frame.Content = childPage;
        }

        private void DgvLoginTotal_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageLoginHis childPage = new PageLoginHis();
            childPage.Frame = Frame;
            childPage.ParentPage = this;
            childPage.PageMainNew = PageMainNew;
            BindLoginTotal blt = DgvLoginTotal.SelectedItem as BindLoginTotal;
            if (blt != null)
            {
                childPage.LoginTotal = blt;
            }
            Frame.Content = childPage;
        }

 

        private void DgvLoginTotal_KeyUp(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
            {
                BindLoginTotal cell = (DgvLoginTotal.CurrentCell.Item) as BindLoginTotal;
                PropertyInfo[] props = typeof(BindLoginTotal).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                string str = DgvLoginTotal.CurrentCell.Column.SortMemberPath;
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
