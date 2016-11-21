using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using SkdAdminClient.Moudle;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;

namespace SkdAdminClient.View
{
    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageLoginDetail : Page
    {
        private DataTable _dt = new DataTable();
        public Frame Frame ;
        List<LoginDetail> loginDetails = new List<LoginDetail>();
        public PageLoginDetail( Frame frame)
        {
            InitializeComponent();
            Frame = frame;
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            string vender = CbxVender.Text.Trim();
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
                LoginDetail l = new LoginDetail();
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
            DgvLoginTotal.ItemsSource = loginDetails;
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
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            List<string> orgList = skdServiceSoapClient.GetOrgList().ToList();
            orgList.Insert(0, "");
            CbxVender.ItemsSource = orgList;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ChkTime.IsChecked = false;
            TxtUserAccount.Text = "";
            TxtUserName.Text = "";
            CbxVender.Text = "";
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
            DgvLoginTotal.ItemsSource = null;
        }
    }
}
