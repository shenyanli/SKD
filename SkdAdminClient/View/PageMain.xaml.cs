using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkdAdminClient.View
{

    /// <summary>
    /// PageMain.xaml 的交互逻辑
    /// </summary>
    public partial class PageMain : Page
    {
        public Button BackButton;
        public Label CurrentPage;
        public View.PageLoginDetail PageLoginDetail = new View.PageLoginDetail(new Frame());
        public View.PageLoginTotal PageLoginTotal = new View.PageLoginTotal(new Frame());
        public PageProgressDetail PageProgressDetail = new PageProgressDetail(new Frame());
        public PageTrainningRecord PageTrainningRecord = new PageTrainningRecord(new Frame());
        public View.PageCourseCount PageCourseCount = new View.PageCourseCount(new Frame() );
        public  View.PageCourseStudyDetail PageCourseStudyDetail=new  View.PageCourseStudyDetail( new Frame() );

        public PageMain()
        {
            InitializeComponent();
      
     
        }

        private void ButtonLoginDetail_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = GetParentObject<Frame>(ButtonProgressDetail, "AddPage");
            ((Frame) parent).Content = PageLoginDetail; //new PageLoginDetail(((Frame)parent));
            BackButton.Visibility = Visibility.Visible;
            CurrentPage.Visibility = Visibility.Visible;
            CurrentPage.Content = PageLoginDetail.Name;
        }

        private void ButtonTrainningRecord_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = GetParentObject<Frame>(ButtonProgressDetail, "AddPage");
            ((Frame) parent).Content = PageTrainningRecord; //new PageTrainningRecord(((Frame)parent));
            BackButton.Visibility = Visibility.Visible;
            CurrentPage.Visibility = Visibility.Visible;
            CurrentPage.Content = PageTrainningRecord.Name;
        }

        private void ButtonProgressDetail_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = GetParentObject<Frame>(ButtonProgressDetail, "AddPage");
            ((Frame)parent).Content = PageProgressDetail; //new PageProgressDetail(((Frame) parent));
            PageProgressDetail.Frame = ((Frame) parent);
            BackButton.Visibility = Visibility.Visible;
            CurrentPage.Visibility = Visibility.Visible;
            CurrentPage.Content = PageProgressDetail.Name;
        
        }

        private void ButtonLoginTotal_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = GetParentObject<Frame>(ButtonProgressDetail, "AddPage");
            ((Frame) parent).Content = PageLoginTotal; //new PageLoginTotal(((Frame)parent));
            BackButton.Visibility = Visibility.Visible;
            CurrentPage.Visibility = Visibility.Visible;
            CurrentPage.Content = PageLoginTotal.Name;
        }

        private void BtnCourseAddUp_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = GetParentObject<Frame>(ButtonProgressDetail, "AddPage");
            ((Frame)parent).Content = PageCourseCount; //new PageLoginTotal(((Frame)parent));
            BackButton.Visibility = Visibility.Visible;
            CurrentPage.Visibility = Visibility.Visible;
            CurrentPage.Content = PageCourseCount.Name;
        }

        private void BtnCourseStudyDetail_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = GetParentObject<Frame>(ButtonProgressDetail, "AddPage");
            ((Frame)parent).Content = PageCourseStudyDetail; //new PageLoginTotal(((Frame)parent));
            BackButton.Visibility = Visibility.Visible;
            CurrentPage.Visibility = Visibility.Visible;
            CurrentPage.Content = PageCourseStudyDetail.Name;

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
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }

        private void BtnTrainningRecordCount_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
