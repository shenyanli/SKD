﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkdAdminClient.Control
{
    /// <summary>
    /// DataGridPage.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridPage : UserControl
    {
        public DataGridPage()
        {
            InitializeComponent();
            this.Loaded += delegate
            {
                //首页  
                this.btnFirst.MouseLeftButtonUp += new MouseButtonEventHandler(btnFirst_Click);
                this.btnFirst.MouseLeftButtonDown += new MouseButtonEventHandler(btnFirst_MouseLeftButtonDown);
                //上一页  
                this.btnPrev.MouseLeftButtonUp += new MouseButtonEventHandler(btnPrev_Click);
                this.btnPrev.MouseLeftButtonDown += new MouseButtonEventHandler(btnPrev_MouseLeftButtonDown);
                //下一页  
                this.btnNext.MouseLeftButtonUp += new MouseButtonEventHandler(btnNext_Click);
                this.btnNext.MouseLeftButtonDown += new MouseButtonEventHandler(btnNext_MouseLeftButtonDown);
                //末页  
                this.btnLast.MouseLeftButtonUp += new MouseButtonEventHandler(btnLast_Click);
                this.btnLast.MouseLeftButtonDown += new MouseButtonEventHandler(btnLast_MouseLeftButtonDown);
                this.btnGo.Click += new RoutedEventHandler(btnGo_Click);
            };
        }

        private DataTable _dt = new DataTable();
        //每页显示多少条  
        private int pageNum =Convert.ToInt32(ConfigurationManager.AppSettings["PageNum"]);
        //当前是第几页  
        private int pIndex = 1;
        //对象  
        private DataGrid grdList;
        //最大页数  
        private int MaxIndex = 1;
        //一共多少条  
        private int allNum = 0;

        #region 初始化数据  

        /// <summary>  
        /// 初始化数据  
        /// </summary>  
        /// <param name="grd"></param>  
        /// <param name="dtt"></param>  
        /// <param name="Num"></param>  
        public void ShowPages(DataGrid grd, DataTable dt, int Num)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;

            this._dt = dt.Clone();
            this.grdList = grd;
            this.pageNum = Num;
            this.pIndex = 1;
            foreach (DataRow r in dt.Rows)
                this._dt.ImportRow(r);
            SetMaxIndex();
            ReadDataTable();
            if (this.MaxIndex > 1)
            {
                this.pageGo.IsReadOnly = false;
                this.btnGo.IsEnabled = true;
            }
        }

        #endregion

        #region 画数据  

        /// <summary>  
        /// 画数据  
        /// </summary>  
        private void ReadDataTable()
        {
            try
            {
                DataTable tmpTable = new DataTable();
                tmpTable = this._dt.Clone();
                int first = this.pageNum * (this.pIndex - 1);
                first = (first > 0) ? first : 0;
                //如果总数量大于每页显示数量  
                if (this._dt.Rows.Count >= this.pageNum * this.pIndex)
                {
                    for (int i = first; i < pageNum * this.pIndex; i++)
                        tmpTable.ImportRow(this._dt.Rows[i]);
                }
                else
                {
                    for (int i = first; i < this._dt.Rows.Count; i++)
                        tmpTable.ImportRow(this._dt.Rows[i]);
                }
                this.grdList.ItemsSource = tmpTable.DefaultView;
                tmpTable.Dispose();
            }
            catch
            {
                MessageBox.Show("错误");
            }
            finally
            {
                DisplayPagingInfo();
            }
        }

        #endregion

        #region 画每页显示等数据  

        /// <summary>  
        /// 画每页显示等数据  
        /// </summary>  
        private void DisplayPagingInfo()
        {
            if (this.pIndex == 1)
            {
                this.btnPrev.IsEnabled = false;
                this.btnFirst.IsEnabled = false;
            }
            else
            {
                this.btnPrev.IsEnabled = true;
                this.btnFirst.IsEnabled = true;
            }
            if (this.pIndex == this.MaxIndex)
            {
                this.btnNext.IsEnabled = false;
                this.btnLast.IsEnabled = false;
            }
            else
            {
                this.btnNext.IsEnabled = true;
                this.btnLast.IsEnabled = true;
            }
            this.tbkRecords.Text = string.Format("每页{0}条/共{1}条", this.pageNum, this.allNum);
            int first = (this.pIndex - 4) > 0 ? (this.pIndex - 4) : 1;
            int last = (first + 9) > this.MaxIndex ? this.MaxIndex : (first + 9);
            this.grid.Children.Clear();
            for (int i = first; i <= last; i++)
            {
                ColumnDefinition cdf = new ColumnDefinition();
                this.grid.ColumnDefinitions.Add(cdf);
                TextBlock tbl = new TextBlock();
                tbl.Text = i.ToString();
                tbl.Style = FindResource("PageTextBlock3") as Style;
                tbl.MouseLeftButtonUp += new MouseButtonEventHandler(tbl_MouseLeftButtonUp);
                tbl.MouseLeftButtonDown += new MouseButtonEventHandler(tbl_MouseLeftButtonDown);
                if (i == this.pIndex)
                    tbl.IsEnabled = false;
                Grid.SetColumn(tbl, this.grid.ColumnDefinitions.Count - 1);
                Grid.SetRow(tbl, 0);
                this.grid.Children.Add(tbl);
            }
        }

        #endregion

        #region 首页  

        /// <summary>  
        /// 首页  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void btnFirst_Click(object sender, System.EventArgs e)
        {
            this.pIndex = 1;
            ReadDataTable();
        }

        /// <summary>  
        /// 首页  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void btnFirst_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region 上一页  
        /// <summary>  
        /// 上一页  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void btnPrev_Click(object sender, System.EventArgs e)
        {
            if (this.pIndex <= 1)
                return;
            this.pIndex--;
            ReadDataTable();
        }

        /// <summary>  
        /// 上一页  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void btnPrev_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region 下一页  

        /// <summary>  
        /// 下一页  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void btnNext_Click(object sender, System.EventArgs e)
        {
            if (this.pIndex >= this.MaxIndex)
                return;
            this.pIndex++;
            ReadDataTable();
        }

        /// <summary>  
        /// 下一页  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void btnNext_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region 未页  

        /// <summary>  
        /// 未页  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void btnLast_Click(object sender, System.EventArgs e)
        {
            this.pIndex = this.MaxIndex;
            ReadDataTable();
        }

        /// <summary>  
        /// 未页  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void btnLast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region 设置最多大页面  

        /// <summary>  
        /// 设置最多大页面  
        /// </summary>  
        private void SetMaxIndex()
        {
            //多少页  
            int Pages = this._dt.Rows.Count / pageNum;
            if (this._dt.Rows.Count != (Pages * pageNum))
            {
                if (_dt.Rows.Count < (Pages * pageNum))
                    Pages--;
                else
                    Pages++;
            }
            this.MaxIndex = Pages;
            this.allNum = this._dt.Rows.Count;
        }

        #endregion

        #region 跳转到多少页  

        /// <summary>  
        /// 跳转到多少页  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumber(this.pageGo.Text))
            {
                int pageNum = int.Parse(this.pageGo.Text);
                if (pageNum > 0 && pageNum <= this.MaxIndex)
                {
                    this.pIndex = pageNum;
                    ReadDataTable();
                }
                else if (pageNum > this.MaxIndex)
                {
                    this.pIndex = this.MaxIndex;
                    ReadDataTable();
                }
            }
            this.pageGo.Text = "";
        }

        #endregion

        #region 分页数字的点击触发事件  

        private void tbl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock tbl = sender as TextBlock;
            if (tbl == null)
                return;
            int index = int.Parse(tbl.Text.ToString());
            this.pIndex = index;
            if (index > this.MaxIndex)
                this.pIndex = this.MaxIndex;
            if (index < 1)
                this.pIndex = 1;
            ReadDataTable();
        }

        void tbl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        private static Regex RegNumber = new Regex("^[0-9]+$");

        #region 判断是否是数字  
        /// <summary>  
        /// 判断是否是数字  
        /// </summary>  
        /// <param name="valString"></param>  
        /// <returns></returns>  
        public static bool IsNumber(string valString)
        {
            Match m = RegNumber.Match(valString);
            return m.Success;
        }
        #endregion
    }
}

