using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;
using SkdAdminModel;

namespace SkdAdminClient.View
{
    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageTrainningRecordHis : Page
    {
        private DataTable _dt = new DataTable();
        public Frame Frame;
        public PageMainNew PageMainNew;
        public PageProgressDetail ParentPage;
        public BindProgressDetail CurrentProgressDetail;
        List<BindTrainningRecord> trs = new List<BindTrainningRecord>();
        private List<string> _sysIdList=new List<string>( );
        private bool _showMsgBox = true;
        SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
        List<string> courseNames=new List<string>();
        public PageTrainningRecordHis()
        {
            InitializeComponent();
            TxtBeginDate.Text = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd");
            TxtEndDate.Text= DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            BtnQuery.Content = "检索中...";
            //string courseName = string.Join("','",
            //    LbxCourseName.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg));

            List<string> bigCourseNames =
                LbxCourseName.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg).ToList();

            string courseName="";//=GlobalData.CourseNameMaps.Where(x=>x.Value)
            foreach (string bigCourseName in bigCourseNames)
            {
                string smallCourseNames = string.Join("','",
                    GlobalData.CourseNameMaps.Where(x => x.Value == bigCourseName).Select(x=>x.Key));
                if (courseName == "")
                    courseName = courseName + smallCourseNames;
                else
                    courseName = courseName + "','" + smallCourseNames;
            }

            string venderIds = string.Join("','",
                LbxVenders.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg.Split('_')[0]));
            string rbo = string.Join("','", LbxRbo.BindingAllSourses.Where(x => x.IsSelected).Select(x => x.TextMsg));
            string userAccount = TxtUserAccount.Text.Trim().ToLower();
            string userName = TxtUserName.Text.Trim();
            string loginDateBegin ="";
            string loginDateEnd ="";
       
            if (GlobalData.PrivilegeLevel == Privilege.AreaAdmin)
            {
                if (venderIds.Trim() == "")
                    venderIds = string.Join("','", GlobalData.Venders);
            }

            if (ChkTime.IsChecked != null && (bool) ChkTime.IsChecked)
            {
                loginDateBegin = Convert.ToDateTime(TxtBeginDate.Text).ToString("yyyy-MM-dd");
                loginDateEnd = Convert.ToDateTime(TxtEndDate.Text).ToString("yyyy-MM-dd");
            }

            Query(rbo,venderIds,userName,userAccount,courseName,loginDateBegin,loginDateEnd);
        }


        void Query(string rbo,string venderIds,string userName,string userAccount,string courseNames,string loginDateBegin,string loginDateEnd)
        {
            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            _dt = skdServiceSoapClient.GetTrainningRecord(new string [] {}, rbo,"", venderIds, userName, userAccount, courseNames, loginDateBegin, loginDateEnd);
            trs.Clear();

            foreach (DataRow row in _dt.Rows)
            {
                BindTrainningRecord tr = new BindTrainningRecord();
                tr.UserName = row["userName"].ToString();
                tr.UserAccount = row["userAccount"].ToString();
                tr.CourseName = row["courseName"].ToString();
                tr.TrainningRecordName = row["recordName"].ToString();
                tr.Score = Convert.ToDouble(row["score"].ToString());
                tr.Vender = row["OrganizationName"].ToString();
                tr.OperTime = row["OperTime"].ToString();
                string xml = row["message"].ToString();
                XmlDocument xmlDocument = new XmlDataDocument();
                xmlDocument.LoadXml(xml);
                XmlNodeList nodeList = xmlDocument.SelectNodes("TrainingRecord/Record");
                if (nodeList != null)
                {
                    List<XmlNode> nodes = nodeList.Cast<XmlNode>().ToList();
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode noNode = node.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "No");
                        XmlNode contentNode = node.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "Content");//xmlDocument.SelectSingleNode("TrainingRecord/Record/Content"); //操作内容
                        XmlNode remarkNode = node.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "Remark");// xmlDocument.SelectSingleNode("TrainingRecord/Record/Remark");//是否正确
                        XmlNode scoreNode = node.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "Score");// xmlDocument.SelectSingleNode("TrainingRecord/Record/Score"); //得分
                        string no = "1";
                        string remark = "";
                        string score = "0";
                        string message = "";


                        if (noNode != null)
                            no = noNode.InnerText;
                        if (contentNode != null)
                            message = contentNode.InnerText;

                        if (scoreNode != null)
                            score = scoreNode.InnerText.Trim() == "" ? "0" : scoreNode.InnerText.Trim();

                        if (remarkNode != null)
                        {
                            remark = remarkNode.InnerText;
                            if (remark.Trim() == "")
                            {
                                remark = Convert.ToDouble(score) >= 0 ? "操作正确" : "操作错误";
                            }
                        }

                        tr.Detail += no + "." + message + "   [" + remark + "]" + score + "分\r\n";
                    }

                }
                double totalMinutes = Convert.ToDouble(row["totalMinutes"].ToString());
                int hour = (int)(totalMinutes / 60);
                int minutes = (int)(totalMinutes % 60);
                tr.TotalMinutes = hour.ToString("00") + ":" + minutes.ToString("00") + ":00";
                trs.Add(tr);
            }
            //ListCollectionView LoginTotals = new ListCollectionView(loginTotals);
            //LoginTotals.GroupDescriptions.Add(new PropertyGroupDescription(vender));
            DgvTrainningRecord.ItemsSource = null;
            DgvTrainningRecord.ItemsSource = trs.OrderByDescending(x => x.CourseName);
            BtnQuery.Content = "检索";
            if (_showMsgBox)
            {
                Expander.IsExpanded = false;
                XMessageBox.ShowDialog("查询到相关数据" + trs.Count + "笔", "提示");
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
            TbTitle.Text = Name;
            
            if (CurrentProgressDetail != null) //说明是从学习页面进来的
            {
                TxtUserAccount.Text = CurrentProgressDetail.UserAccount;
                TxtUserAccount.IsEnabled = false;
                TxtUserName.Text = CurrentProgressDetail.UserName;
                TxtUserName.IsEnabled = false;
                LbxCourseName.Show( GlobalData.CourseNameMaps.Where(x => x.Value == CurrentProgressDetail.CourseName).Select(x=>x.Key).ToList(), true);
                LbxCourseName.IsEnabled = false;
                BtnBack.IsEnabled = true;
                BtnClear.IsEnabled = false;
            //    _sysIdList = CurrentProgressDetail.SysIdList.Select(x => x.Split(':')[1]).ToList();
                _showMsgBox = false;
                //      BtnQuery_Click(null, null);
                string courseName = string.Join("','",
                    GlobalData.CourseNameMaps.Where(x => x.Value == CurrentProgressDetail.CourseName).Select(x => x.Key).ToList());
                Query("","","",CurrentProgressDetail.UserAccount, courseName,"","");
                      _showMsgBox = true;
            }
            else
            {
               // courseNames = skdServiceSoapClient.GetCourseName().ToList();
                //LbxCourseName.Show(courseNames);
                LbxCourseName.Show(GlobalData.CourseNameMaps.Values.Distinct().ToList());
                LbxVenders.Show(GlobalData.Venders);
                LbxRbo.Show(GlobalData.RboList);
            }
  
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtUserName.Text = "";
            TxtUserAccount.Text = "";
            LbxCourseName.Show(courseNames);
            LbxVenders.Show(GlobalData.Venders);
            LbxRbo.Show(GlobalData.RboList);
            ChkTime.IsChecked = false;
            TxtBeginDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DgvTrainningRecord.ItemsSource = null;
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = ParentPage;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }

        private void DgvTrainningRecord_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
            {
                BindTrainningRecord cell = (DgvTrainningRecord.CurrentCell.Item) as BindTrainningRecord;
                PropertyInfo[] props = typeof(BindTrainningRecord).GetProperties(BindingFlags.Public | BindingFlags.Instance);
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
    }
}
