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
    public partial class PageLoginHis : Page
    {
        public BindLoginTotal LoginTotal;
        public PageLoginTotal ParentPage;
        private DataTable _dt = new DataTable();
        public Frame Frame ;
        public PageMainNew PageMainNew;
        List<BindLoginDetail> loginDetails = new List<BindLoginDetail>();// new List<LoginDetail>();
        private bool _showMsgBox = true;
        public PageLoginHis( )
        {
            InitializeComponent();
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            BtnQuery.Content = "检索中...";
            string vender = CbxVender.Text.Trim() ;
            if (GlobalData.PrivilegeLevel == Privilege.AreaAdmin)
            {
                if (vender.Trim() == "")
                    vender = string.Join("','", GlobalData.Venders);
            }
            string userAccount = TxtUserAccount.Text.Trim().ToLower();
            string userName = TxtUserName.Text.Trim();
            string loginDateBegin ="";
            string loginDateEnd ="";
            if (ChkTime.IsChecked != null && (bool) ChkTime.IsChecked)
            {
                loginDateBegin = Convert.ToDateTime(TxtBeginDate.Text).ToString("yyyy-MM-dd");
                loginDateEnd = Convert.ToDateTime(TxtEndDate.Text).ToString("yyyy-MM-dd");
            }

            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();

            _dt = skdServiceSoapClient.GetLoginDetailTable(vender, userName, userAccount, loginDateBegin,loginDateEnd);

            loginDetails.Clear();
            foreach (DataRow row in _dt.Rows)
            {
                BindLoginDetail l = new BindLoginDetail();
                l.Vender = row["OrganizationName"].ToString();
                l.UserAccount= row["userAccount"].ToString();
                l.UserName= row["userName"].ToString();
                l.LoginDate = row["loginDate"].ToString();
                l.LogoutDate= row["LogoutDate"].ToString();
                double stayTime = (Convert.ToDateTime(l.LogoutDate) - Convert.ToDateTime(l.LoginDate)).TotalMinutes;
                int hours = (int) (stayTime/60);
                int minutes = (int) (stayTime%60);
                l.StayTime = hours.ToString("00") +":"+minutes.ToString("00") + ":00";
                loginDetails.Add(l);
            }
            DgvLoginTotal.ItemsSource = null;
            DgvLoginTotal.ItemsSource = loginDetails.OrderByDescending(x=>x.LoginDate);
            BtnQuery.Content = "检索";
            if (_showMsgBox)
            {
                Expander.IsExpanded = false;
                XMessageBox.ShowDialog("查询到相关数据" + loginDetails.Count + "笔", "提示");
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
                    _dt = ObjectToDataTable.ToDataTable(loginDetails);
                    NpoiHelper.ExportDataTableToExcel(fName, _dt, Name, false);
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


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TbTitle.Text = Name;
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            switch (GlobalData.PrivilegeLevel)
            {
                case Privilege.SuperAdmin:
                    List<string> orgList = new List<string>();//2017-01-04skdServiceSoapClient.GetOrgList().ToList();
                    CbxVender.ItemsSource = orgList;
                    break;
                case Privilege.AreaAdmin:
                    CbxVender.ItemsSource = GlobalData.Venders;
                    break;
                case Privilege.VenderAdmin:
                    CbxVender.Text = GlobalData.Vender;
                    CbxVender.IsEnabled = false;
                    break;
                case Privilege.Student:
                    CbxVender.Text = GlobalData.Vender;
                    CbxVender.IsEnabled = false;
                    TxtUserName.Text = GlobalData.UserName;
                    TxtUserName.IsEnabled = false;
                    TxtUserAccount.Text = GlobalData.UserAccount;
                    TxtUserAccount.IsEnabled = false;
                    break;
            }
            if (LoginTotal != null)
            {
                TxtUserName.Text = LoginTotal.UserName;
                TxtUserAccount.Text = LoginTotal.UserAccount;
                TxtUserName.IsEnabled = false;
                TxtUserAccount.IsEnabled = false;
                BtnBack.IsEnabled = true;
                BtnClear.IsEnabled = false;
            }
            _showMsgBox = false;
            BtnQuery_Click(null, null);
            _showMsgBox = true;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ChkTime.IsChecked = false;

            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
            DgvLoginTotal.ItemsSource = null;
            if (GlobalData.PrivilegeLevel > Privilege.VenderAdmin)
            {
                CbxVender.Text = "";
                CbxVender.IsEnabled = true;
            }
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
            ParentPage.DetailBackToTotal = true;
            Frame.Content = ParentPage;
        }

        private void DgvLoginTotal_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
            {
                BindLoginDetail cell = (DgvLoginTotal.CurrentCell.Item) as BindLoginDetail;
                PropertyInfo[] props = typeof(BindLoginDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance);
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
