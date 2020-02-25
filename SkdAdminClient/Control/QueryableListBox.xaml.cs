using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace SkdAdminClient.Control
{
    public class BindingSourse
    {
        private bool _isSelected;
        private string _textMsg;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
            }
        }

        public string TextMsg
        {
            get
            {
                return _textMsg;
            }

            set
            {
                _textMsg = value;
            }
        }
    }

    /// <summary>
    /// QueryableListBox.xaml 的交互逻辑
    /// </summary>
    public partial class QueryableListBox : UserControl
    {
        public List<BindingSourse> BindingAllSourses=new  List<BindingSourse>();//全部内容
        public QueryableListBox()
        {
            InitializeComponent();
        }

        public void Show(List<string> texts, bool isChecked=false)
        {
            BindingAllSourses.Clear();
            foreach (var text in texts)
            {
                BindingSourse bindingSourse = new BindingSourse();
                bindingSourse.TextMsg = text;
                bindingSourse.IsSelected = isChecked;
                BindingAllSourses.Add(bindingSourse);
            }
            LbxShow.ItemsSource = null;
            LbxShow.ItemsSource = BindingAllSourses;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;
            if (checkBox.IsChecked != null)
            {
                string msg = checkBox.Parent.FindChild<TextBlock>("TxtMsg").Text;
                BindingSourse bindingSourse = BindingAllSourses.FirstOrDefault(x => x.TextMsg == msg);
                if (bindingSourse==null) return;
                if ((bool) checkBox.IsChecked)
                {
                    bindingSourse.IsSelected = true;
                }
                else
                {

                    bindingSourse.IsSelected = false;
                }
           
            }
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            LbxShow.ItemsSource = null;
            LbxShow.ItemsSource = BindingAllSourses.Where(x=>x.IsSelected);
        }

        private void BtnAll_Click(object sender, RoutedEventArgs e)
        {
            LbxShow.ItemsSource = null;
            LbxShow.ItemsSource = BindingAllSourses;
        }

        private void BtnAllCheck_Click(object sender, RoutedEventArgs e)
        {
            if (BtnAllCheck.Content.ToString() == "全选")
            {
                BindingAllSourses.ForEach(x=>x.IsSelected=true);
                LbxShow.ItemsSource = null;
                LbxShow.ItemsSource = BindingAllSourses;
                BtnAllCheck.Content = "取消";
            }
            else if (BtnAllCheck.Content.ToString() == "取消")
            {
                BindingAllSourses.ForEach(x => x.IsSelected = false);
                LbxShow.ItemsSource = null;
                LbxShow.ItemsSource = BindingAllSourses;
                BtnAllCheck.Content = "全选";
            }
           
        }

        private void TxtQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            string queryText = ((TextBox) sender).Text.Trim();
            if (queryText == "")
            {
                LbxShow.ItemsSource = BindingAllSourses;
                return;
            }
            LbxShow.ItemsSource = null;
            LbxShow.ItemsSource = BindingAllSourses.Where(x=>x.TextMsg.Contains(queryText));
        }
    }
}
