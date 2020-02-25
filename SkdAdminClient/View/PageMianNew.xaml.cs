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
using MahApps.Metro.Controls;
using Microsoft.Vbe.Interop;
using SkdAdminClient.Control;
using SkdAdminClient.SkdWebService;
using SkdAdminModel;
using Button = System.Windows.Controls.Button;


namespace SkdAdminClient.View
{
    class PagePrivilege
    {
       
        public PagePrivilege(Privilege privilege)
        {
            if (privilege == Privilege.VenderAdmin)
            {
                PageFeedBackHis = Visibility.Collapsed;
                PageTestRightRate = Visibility.Collapsed;
                PageTrainningRigthRate = Visibility.Collapsed;
                PageCourseStudyDetailHis = Visibility.Collapsed;
                PageTrainningRecordHis = Visibility.Collapsed;
                PageLoginHis = Visibility.Collapsed;
            }
        }

        private Visibility _pageLoginTotal;
        private Visibility _pageCoursePassRate;
        private Visibility _pageProgressDetail;
        private Visibility _pageLoginHis;
        private Visibility _pageCourseStudyDetailHis;
        private Visibility _pageFeedBackHis;
        private Visibility _pageTestRightRate;
        private Visibility _pageTrainningRecordHis;
        private Visibility _pageTrainningRigthRate;


        public Visibility PageLoginTotal
        {
            get
            {
                return _pageLoginTotal;
            }

            set
            {
                _pageLoginTotal = value;
            }
        }

        public Visibility PageCoursePassRate
        {
            get
            {
                return _pageCoursePassRate;
            }

            set
            {
                _pageCoursePassRate = value;
            }
        }

        public Visibility PageProgressDetail
        {
            get
            {
                return _pageProgressDetail;
            }

            set
            {
                _pageProgressDetail = value;
            }
        }

        public Visibility PageLoginHis
        {
            get
            {
                return _pageLoginHis;
            }

            set
            {
                _pageLoginHis = value;
            }
        }

        public Visibility PageCourseStudyDetailHis
        {
            get
            {
                return _pageCourseStudyDetailHis;
            }

            set
            {
                _pageCourseStudyDetailHis = value;
            }
        }

        public Visibility PageFeedBackHis
        {
            get
            {
                return _pageFeedBackHis;
            }

            set
            {
                _pageFeedBackHis = value;
            }
        }

        public Visibility PageTestRightRate
        {
            get
            {
                return _pageTestRightRate;
            }

            set
            {
                _pageTestRightRate = value;
            }
        }

        public Visibility PageTrainningRecordHis
        {
            get
            {
                return _pageTrainningRecordHis;
            }

            set
            {
                _pageTrainningRecordHis = value;
            }
        }

        public Visibility PageTrainningRigthRate
        {
            get
            {
                return _pageTrainningRigthRate;
            }

            set
            {
                _pageTrainningRigthRate = value;
            }
        }
    }

    /// <summary>
    /// PageMianNew.xaml 的交互逻辑
    /// </summary>
    public partial class PageMainNew : Page
    {
        private PagePrivilege pagePrivilege = new PagePrivilege(GlobalData.PrivilegeLevel);
        private Frame _frame;
        public WindowMain WindowMain;
        public Button BackButton;
        public PageLoginHis PageLoginDetail = new PageLoginHis();
        public PageLoginTotal PageLoginTotal = new PageLoginTotal();
        public PageProgressDetail PageProgressDetail = new PageProgressDetail();
        public PageTrainningRecordHis PageTrainningRecord = new PageTrainningRecordHis();
        public PageCoursePassRateNew PageCourseCount = new PageCoursePassRateNew();
        public PageCourseStudyDetailHis PageCourseStudyDetail = new PageCourseStudyDetailHis();
        public PageTestRightRate PageTestCount = new PageTestRightRate();
        public PageTrainningRigthRate PageTrainningRecordCount = new PageTrainningRigthRate();
        public PageFeedBackHis PgeFeedBack = new PageFeedBackHis();
        public  PageTestOrg PageTestOrg=new  PageTestOrg();

        public PageMainNew()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = pagePrivilege;
            _frame = WindowMain.AddPage;
            if (GlobalData.PrivilegeLevel < Privilege.VenderAdmin)
            {
                BtnCourseAddUp.IsEnabled = false;
                BtnTestCount.IsEnabled = false;
                BtnTrainningRecordCount.IsEnabled = false;
                // BtnVenderCount.IsEnabled = false;
            }


            SkdServiceSoapClient serviceSoapClient = new SkdServiceSoapClient();

            //2018-11-08 获取课件的发布时间
            DataTable dt = serviceSoapClient.GetCourseReleaseDate();
            foreach (DataRow row in dt.Rows)
            {
                string courseName = row["bigcourseName"].ToString();
                string releaseDate = row["createTime"].ToString();
                if (!GlobalData.CourseReleaseDate.ContainsKey(courseName))
                    GlobalData.CourseReleaseDate.Add(courseName, releaseDate);
            }


            System.Data.DataTable dt1 = serviceSoapClient.GetCourseNameMap();
            foreach (DataRow row in dt1.Rows)
            {
                if(!GlobalData.CourseNameMaps.Keys.Contains(row["coursename"].ToString()))
                GlobalData.CourseNameMaps.Add( row["coursename"].ToString(), row["Bigcoursename"].ToString());
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

        private void BtnTestOrg_Clieck(object sender, RoutedEventArgs e)
        {
            PageTestOrg.PageMainNew = this;
            PageTestOrg.Frame = _frame;
            _frame.Content = PageTestOrg;
        }

        private void BtnUpdateData_Click(object sender, RoutedEventArgs e)
        {
            if (XMessageBox.ShowDialog("同步数据花费的时间较长，确实要同步数据？", "提示",true))
            {
                WindowUpdateData updateData = new WindowUpdateData();
                updateData.CloseFrom = true;
                updateData.Show();
            }
     
        }
    }
}
