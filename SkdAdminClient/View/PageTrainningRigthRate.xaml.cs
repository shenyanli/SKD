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
    public partial class PageTrainningRigthRate : Page
    {
        private DataTable _dt = new DataTable();
        public Frame Frame;
        public PageMainNew PageMainNew;
        public PageProgressDetail ParentPage;
        public BindProgressDetail CurrentProgressDetail;
        List<BindTrainningBaseInfo> tbs = new List<BindTrainningBaseInfo>();
        SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
        List<string> courseNames =new List<string>();

        public PageTrainningRigthRate()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TbTitle.Text = Name;
            courseNames = skdServiceSoapClient.GetCourseName().ToList();
              CbxCourseName.ItemsSource = GlobalData.CourseNameMaps.Values.Distinct().ToList();
         
            if (ParentPage != null)
            {
                CbxCourseName.Text = CurrentProgressDetail.CourseName;
            }
            else
            {
                LbxVenders.Show(GlobalData.Venders);
                LbxRbo.Show(GlobalData.RboList);
             
            }
  
        }


        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            BtnQuery.Content = "检索中...";
            string courseName = CbxCourseName.Text.Trim();
            string recordName = string.Join("','", LbxRecordName.BindingAllSourses.Where(x => x.IsSelected).Select(x=>x.TextMsg));
            string vender = string.Join("','", LbxVenders.BindingAllSourses.Where(x => x.IsSelected).Select(x=>x.TextMsg.Split('_')[1]));
            string rbo = string.Join("','", LbxRbo.BindingAllSourses.Where(x => x.IsSelected).Select(x=>x.TextMsg));
            if (vender.Trim() == "")
                DgvTrainningRecord.Columns[0].Visibility = Visibility.Collapsed;
            else
                DgvTrainningRecord.Columns[0].Visibility = Visibility.Visible;
 
            if (courseName == "" || recordName == "")
            {
                XMessageBox.ShowDialog("请选择要分析的课程及虚拟实训名称！","提示");
               return;
            }

            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();

            DataTable dt = skdServiceSoapClient.GetMaxScoreTrainning(vender, courseName,recordName);
            List<string> messages =
                (from DataRow row in dt.Rows
                    select row["message"].ToString()).ToList();
            List<TrainingRecord> trsList = messages.Select(x => TrainingRecord.XmlToTrainingRecord(x)).ToList();
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
            _dt = skdServiceSoapClient.GetCourseAddUp(courseName, "", "", "", "", "",
      "", "");
            tbs.ForEach(x => x.PlanPersons = _dt.Rows.Cast<DataRow>().Sum(r => Convert.ToDouble(r["planPersons"])));
            DgvTrainningRecord.ItemsSource = null;
            DgvTrainningRecord.ItemsSource = tbs.OrderByDescending(x=>x.CourseName);
            BtnQuery.Content = "检索";
            {
                Expander.IsExpanded = false;
                XMessageBox.ShowDialog("查询到相关数据" + tbs.Count + "笔", "提示");
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

    

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            LbxVenders.Show(GlobalData.Venders);
            LbxRbo.Show(GlobalData.RboList);
            CbxCourseName.Text = "";
            DgvTrainningRecord.ItemsSource = null;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }

        private void CbxCourseName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbxCourseName.SelectedIndex >= 0)
            {
                string courseName = CbxCourseName.SelectedItem.ToString();
                SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
                string[] recordNames = skdServiceSoapClient.GetDistinctRecoordName(courseName);
                if (recordNames.Length < 1)
                {
                    XMessageBox.ShowDialog("课程[" + courseName + "]暂时没有虚拟实训部分！", "提示");
                    LbxRecordName.Show(new List<string>() {});
                    return;
                }
                LbxRecordName.Show(recordNames.ToList());
            }
        }

        private void DgvTrainningRecord_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
            {
                BindTrainningBaseInfo cell = (DgvTrainningRecord.CurrentCell.Item) as BindTrainningBaseInfo;
                PropertyInfo[] props = typeof(BindTrainningBaseInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                string str = DgvTrainningRecord.CurrentCell.Column.SortMemberPath;
                if (str=="")return;
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
