using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using SkdAdminClient.Moudle;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;

namespace SkdAdminClient.View
{
    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageTrainningRecord : Page
    {
        private DataTable _dt = new DataTable();
        public Frame Frame;
        public View.PageProgressDetail ParentPage;
        public ProgressDetail CurrentProgressDetail;
        List<TrainningRecord> trs = new List<TrainningRecord>();
        public PageTrainningRecord(Frame frame)
        {
            InitializeComponent();
            Frame = frame;
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            string courseName = CbxCourseName.Text.Trim();
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

            _dt = skdServiceSoapClient.GetTrainningRecord( userName, userAccount,courseName, loginDateBegin,loginDateEnd);
            trs.Clear();

            foreach (DataRow row in _dt.Rows)
            {
                TrainningRecord tr = new TrainningRecord();
                tr.UserName = row["userName"].ToString();
                tr.UserAccount= row["userAccount"].ToString();
                tr.CourseName= row["courseName"].ToString();
                tr.TrainningRecordName= row["recordName"].ToString();
                tr.Score = Convert.ToDouble(row["score"].ToString());
                string xml= row["message"].ToString();
                XmlDocument xmlDocument = new XmlDataDocument();
                xmlDocument.LoadXml(xml);
                XmlNodeList nodeList = xmlDocument.SelectNodes("TrainingRecord/Record");
                if (nodeList != null)
                {
                    List<XmlNode> nodes = nodeList.Cast<XmlNode>().ToList();
                    if (nodes.Count>0)
                    {
                        XmlNode node = xmlDocument.SelectSingleNode("TrainingRecord/Record/Content");
                        string message = node.InnerText;
                        List<string> spilt = new List<string>();
                        spilt.Add("分]");
                        tr.Detail = message.Replace("分]", "分]\r\n");
                    }
                }
                double totalMinutes = Convert.ToDouble(row["totalMinutes"].ToString());
                int hour =(int) (totalMinutes/60);
                int minutes= (int)(totalMinutes % 60);
                tr.TotalMinutes = hour.ToString("00") + ":" + minutes.ToString("00") + ":00";
                trs.Add(tr);
            }
            //ListCollectionView LoginTotals = new ListCollectionView(loginTotals);
            //LoginTotals.GroupDescriptions.Add(new PropertyGroupDescription(vender));
            DgvTrainningRecord.ItemsSource = null;
            DgvTrainningRecord.ItemsSource = trs;
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
                    _dt = ObjectToDataTable.ToDataTable(trs);
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
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            List<string> courseNames = skdServiceSoapClient.GetCourseName().ToList();
            CbxCourseName.ItemsSource = courseNames;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtUserName.Text = "";
            TxtUserAccount.Text = "";
            CbxCourseName.Text = "";
            ChkTime.IsChecked = false;
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DgvTrainningRecord.ItemsSource = null;
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = ParentPage;
        }
    }
}
