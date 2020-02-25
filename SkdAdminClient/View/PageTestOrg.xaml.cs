using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;

namespace SkdAdminClient.View
{
    /// <summary>
    /// PageTestOrg.xaml 的交互逻辑
    /// </summary>
    public partial class PageTestOrg : Page
    {
        public PageMainNew PageMainNew;
        private DataTable _dt = new DataTable();
        public Frame Frame;
        List<Vender> venders = new List<Vender>();

        public PageTestOrg()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = PageMainNew;
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            bool result = XMessageBox.ShowDialog("导入前建议先同步LMS平台数据，导入格式请参考【模版下载】中的模版，是否现在导入？", "提示", true);
            if (!result) return;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel|*.xlsx|Excel|*.xls";
            ofd.ShowDialog();
            string fileName = ofd.FileName.Trim();
            if (string.IsNullOrEmpty(fileName))
                return;
            TxbPath.Text = fileName;
            DataTable dt = NpoiHelper.ImportExceltoDataTable(fileName);
            int index = 0;
            foreach (DataRow dr in dt.Rows)
            {
                index++;
                try
                {
                    Vender vender = new Vender();
                    vender.Id = index;
                    vender.VenderCode = dr["售后代码"].ToString();
                    vender.VenderName = dr["经销商名称"].ToString();
                    venders.Add(vender);
                }
                catch (Exception err)
                {
                    XMessageBox.Show("第" + index + "行导入失败！ " + err.Message);
                }
            }
            DgvDetail.ItemsSource = venders;

        }


        class Vender
        {
            public int Id { get; set; }
            public string VenderCode { get; set; }
            public string VenderName { get; set; }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DgvDetail.ItemsSource = null;
            venders.Clear();
            TxbPath.Text = "";

        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {

            SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
            bool result = skdServiceSoapClient.UpdateTestOrg(venders.Select(x => x.VenderCode).ToArray());
            if (result)
            {
                XMessageBox.Show("更新成功！");
                BtnCancel_Click(null, null);
            }
            else
            {
                XMessageBox.Show("更新失败！");
            }
        }

        private void BtnTemplate_Click(object sender, RoutedEventArgs e)
        {
            DownLoadTemplate();
           
        }


        public void DownLoadTemplate()
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title = "导出模板";
                dlg.FileName = "Template.xls";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //检查路径正确性
                    if (dlg.FileName.Trim().Length <= 0)
                    {
                        XMessageBox.Show("请选择路径。");
                        return;
                    }
                    //检查文件夹是否存在
                    int n = dlg.FileName.LastIndexOf(@"\") + 1;
                    if (n < 0)
                    {
                        XMessageBox.Show("路径错误。");
                        return;
                    }
                    string PathStr = dlg.FileName.Substring(0, n);
                    if (!Directory.Exists(PathStr))
                    {
                        XMessageBox.Show("路径不存在，请检查。");
                        return;
                    }
                    byte[] template = Properties.Resources.Template; //Excel资源去掉后缀名
                    FileStream stream = new FileStream(dlg.FileName, FileMode.Create);
                    stream.Write(template, 0, template.Length);
                    stream.Close();
                    stream.Dispose();
                }
                XMessageBox.Show("模版下载成功");
            }
            catch (Exception err)
            {
                XMessageBox.Show(err.Message);
            }

           
        }
    }
}
