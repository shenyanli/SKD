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
using SkdAdminClient.Control;
using SkdAdminModel;


namespace SkdAdminClient.View
{
    /// <summary>
    /// PageMianNew.xaml 的交互逻辑
    /// </summary>
    public partial class PageMainNew : Page
    {
        private Frame _frame;
        public WindowMain WindowMain;
        public Button BackButton;
        public PageLoginDetail PageLoginDetail = new PageLoginDetail();
        public PageLoginTotal PageLoginTotal = new PageLoginTotal();
        public PageProgressDetail PageProgressDetail = new PageProgressDetail();
        public PageTrainningRecord PageTrainningRecord = new PageTrainningRecord();
        public PageCourseCount PageCourseCount = new PageCourseCount();
        public PageCourseStudyDetail PageCourseStudyDetail = new PageCourseStudyDetail();
        public PageTestCount PageTestCount = new PageTestCount();
        public PageTrainningRecordCount PageTrainningRecordCount = new PageTrainningRecordCount();
        public PageFeedBack PgeFeedBack = new PageFeedBack();

        public PageMainNew()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _frame = WindowMain.AddPage;
            if (GolableData.PrivilegeLevel < Privilege.VenderAdmin)
            {
                BtnCourseAddUp.IsEnabled = false;
                BtnTestCount.IsEnabled = false;
                BtnTrainningRecordCount.IsEnabled = false;
                BtnVenderCount.IsEnabled = false;
            }
        }


        public T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name || string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }


        public T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                grandChild = GetChildObject<T>(child, name);
                if (grandChild != null)
                    return grandChild;
            }
            return null;
        }

        private void ButtonLoginTotal_Click(object sender, RoutedEventArgs e)
        {
            PageLoginTotal.PageMainNew = this;
            PageLoginTotal.Frame = _frame;
            _frame.Content = PageLoginTotal;
        }

        private void ButtonProgressDetail_Click(object sender, RoutedEventArgs e)
        {
            PageProgressDetail.PageMainNew = this;
            PageProgressDetail.Frame = _frame;
            _frame.Content = PageProgressDetail;
        }

        private void BtnTestCount_Click(object sender, RoutedEventArgs e)
        {

            PageTestCount.PageMainNew = this;
            PageTestCount.Frame = _frame;
            _frame.Content = PageTestCount;
        }

        private void BtnTrainningRecordCount_Click(object sender, RoutedEventArgs e)
        {
            PageTrainningRecordCount.PageMainNew = this;
            PageTrainningRecordCount.Frame = _frame;
            _frame.Content = PageTrainningRecordCount;
        }

        private void BtnCourseAddUp_Click(object sender, RoutedEventArgs e)
        {

            PageCourseCount.PageMainNew = this;
            PageCourseCount.Frame = _frame;
            _frame.Content = PageCourseCount;
        }

        private void BtnCourseStudyDetail_Click(object sender, RoutedEventArgs e)
        {

            PageCourseStudyDetail.PageMainNew = this;
            PageCourseStudyDetail.Frame = _frame;
            _frame.Content = PageCourseStudyDetail;
        }

        private void ButtonTrainningRecord_Click(object sender, RoutedEventArgs e)
        {
            PageTrainningRecord.PageMainNew = this;
            PageTrainningRecord.Frame = _frame;
            _frame.Content = PageTrainningRecord;
        }

        private void ButtonLoginDetail_Click(object sender, RoutedEventArgs e)
        {
            PageLoginDetail.PageMainNew = this;
            PageLoginDetail.Frame = _frame;
            _frame.Content = PageLoginDetail;
        }

        private void BtnFeedBack_Click(object sender, RoutedEventArgs e)
        {
            PgeFeedBack.PageMainNew = this;
            PgeFeedBack.Frame = _frame;
            _frame.Content = PgeFeedBack;
        }
    }
}
