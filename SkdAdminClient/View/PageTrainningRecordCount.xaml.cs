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
    public partial class PageTrainningRecordCount : Page
    {
        private DataTable _dt = new DataTable();
        public Frame Frame;
        public PageMainNew PageMainNew;
        public PageProgressDetail ParentPage;
        public BindProgressDetail CurrentProgressDetail;
        List<BindTrainningBaseInfo> tbs = new List<BindTrainningBaseInfo>();

        public PageTrainningRecordCount()
        {
            InitializeComponent();
        }




        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            string courseName = CbxCourseName.Text.Trim();
            string recordName = CbxRecordName.Text.Trim();
            string vender = CbxVender.Text.Trim();
            if (courseName == "" || recordName == "")
            {
                XMessageBox.ShowDialog("请选择要分析的课程及虚拟实训名称！","提示");
               return;
            }
            if (vender.Trim()=="")
                DgvTrainningRecord.Columns[0].Visibility=Visibility.Collapsed;
            else
                DgvTrainningRecord.Columns[0].Visibility = Visibility.Visible;
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();

            DataTable dt = skdServiceSoapClient.GetMaxScoreTrainning(vender, courseName,recordName);
            List<string> messages =
                (from DataRow row in dt.Rows
                    select row["message"].ToString()).ToList();
            List<TrainingRecord> trsList = messages.Select(x => TrainingRecord.XmlToTrainingRecord(x)).ToList();
           // List<TrainingRecord> trsList = (from DataRow row in dt.Rows select row["message"].ToString() into message select TrainingRecord.XmlToTrainingRecord(message)).ToList();

            _dt = skdServiceSoapClient.GetTrainningBaseInfo( courseName, recordName);
            tbs.Clear();

            foreach (DataRow row in _dt.Rows)
            {

                BindTrainningBaseInfo tb = new BindTrainningBaseInfo();
                tb.Vender = vender;
                tb.Content = row["content"].ToString();
                tb.CourseName = row["courseName"].ToString();
                tb.RecordName = row["recordName"].ToString();
                tb.Id = row["id"].ToString();
                tb.RightPersons =
                    trsList.Count(tr => tr.RecordList.Any(x => x.Id == tb.Id && !x.Remark.Contains("操作错误")));
                tb.TotalPersons = trsList.Count;
                double percent = Math.Round(tb.RightPersons/tb.TotalPersons, 2);
                if (trsList.Count == 0)
                    tb.Percent = "尚未有人做过该虚拟实训！";
                else
                    tb.Percent = percent*100 + "%";
                tbs.Add(tb);
            }
            DgvTrainningRecord.ItemsSource = null;
            DgvTrainningRecord.ItemsSource = tbs.OrderByDescending(x=>x.CourseName);
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
                    _dt = ObjectToDataTable.ToDataTable(tbs);
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
            if (GolableData.PrivilegeLevel <= Privilege.VenderAdmin)
            {
                CbxVender.Text = GolableData.Vender;
                CbxVender.IsEnabled = false;
            }

        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (GolableData.PrivilegeLevel > Privilege.VenderAdmin)
            {
                CbxVender.Text = "";
                CbxVender.IsEnabled = true;
            }
            CbxCourseName.Text = "";
            DgvTrainningRecord.ItemsSource = null;
        }


        private void CbxCourseName_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbxCourseName.SelectedIndex >= 0)
            {
                string courseName = CbxCourseName.SelectedItem.ToString();
                SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
                string[] recordNames = skdServiceSoapClient.GetDistinctRecoordName(courseName);
                if (recordNames.Length < 1)
                {
                    XMessageBox.ShowDialog("课程["+courseName+"]暂时没有虚拟实训部分！","提示");
                    return;
                    
                }
                CbxRecordName.ItemsSource = recordNames;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }
    }
}
