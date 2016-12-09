using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using MahApps.Metro;
using SkdAdminClient.Bll;
using SkdAdminClient.SkdWebService;
using SkdAdminModel;

namespace SkdAdminClient.View
{
    public class ProgressBarValue : INotifyPropertyChanged
    {


        private string _textValue = "0%";
        private int _progressValue = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public string TextValue
        {
            get
            {
                return _textValue;
            }
            set
            {
                if (value != _textValue)
                {
                    _textValue = value;
                    NotifyPropertyChanged("TextValue");
                }
            }
        }

        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }

            set
            {
                if (value != _progressValue)
                {
                    _progressValue = value;
                    NotifyPropertyChanged("ProgressValue");
                }
   
            }
        }
    }
    /// <summary>
    /// WindowUpdateData.xaml 的交互逻辑
    /// </summary>
    public partial class WindowUpdateData : Window
    {
        private ProgressBarValue progressBarValue =new  ProgressBarValue();
        private SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
        private System.Windows.Threading.DispatcherTimer dTimer = new DispatcherTimer();
        private DispatcherTimer _updateProgressTimer = new DispatcherTimer();

        public frmLogin MainFrm;
        public WindowUpdateData()
        {
            InitializeComponent();
            Accent accent = ThemeManager.GetAccent("Lime");
            ThemeManager.ChangeAppStyle(Application.Current, accent, ThemeManager.DetectAppStyle(Application.Current).Item1);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = progressBarValue;
           
            bool needUpdateData = new SkdServiceSoapClient().NeedUpdateData();
            if (!needUpdateData)//不需要同步数据
            {
                Hide();
                frmLogin frm = new frmLogin();
                frm.Show();
                //GolableData.MainWindowUpdateData = this;
            }
            else
            {
                dTimer.Tick += new EventHandler(dTimer_Tick);
                dTimer.Interval = new TimeSpan(0, 0, 1);//设置时间：TimeSpan（时, 分， 秒）            
                dTimer.Start();  //启动 DispatcherTimer对象dTime。
            }
          
        }

        private void dTimer_Tick(object sender, EventArgs e)
        {
            dTimer.Stop();
            UpdateData();
            skdServiceSoapClient.FinishUpdateData();
     
        }

        private void UpdateData()
        {
            #region 模拟进度显示
            _updateProgressTimer.Tick += UpdateProgressTimer_Tick;
            _updateProgressTimer.Interval = new TimeSpan(0, 0, 1); //设置时间：TimeSpan（时, 分， 秒）            
            _updateProgressTimer.Start(); //启动 DispatcherTimer对象dTime。

            #endregion 模拟进度显示

            #region 通过接口更新数据，并显示进度

            //List<string> venderIds = GetAllVenderId();
            //int index = 0;
            //foreach (string venderId in venderIds)
            //{
            //    VenderResult vender = GetVenderDetailByVenderId(venderId);
            //    skdServiceSoapClient.InsertNewOrg(vender.VenderId, vender.VenderName, vender.Type, vender.Area,
            //        vender.ConnectUserName, vender.ConnectPhone, vender.Mail, "1");
            //    List<string> userIds = GetUserIdByVenderId(venderId);
            //    foreach (string userId in userIds)
            //    {
            //        UserResult user = GetUserDetailByUserId(userId);
            //        skdServiceSoapClient.InsertNewUser(user.UserId, user.UserName, user.UserAccount, user.Vender,
            //            "123456", user.Privilege, "1");
            //        List<string> courseNameList = GetCourseNameByUserId(userId);
            //        foreach (string courseName in courseNameList)
            //        {
            //            skdServiceSoapClient.InsertNewMap(userId, user.UserAccount, courseName);
            //        }
            //    }
            //    index = index + 1;
            //    int progressValue = (index/venderIds.Count)*100;
            //    progressBarValue.ProgressValue = progressValue;
            //    progressBarValue.TextValue = progressValue + "%";
            //}

            #endregion 通过接口更新数据，并显示进度

          

        }


        void UpdateProgressTimer_Tick(object sender, EventArgs e)
        {
            progressBarValue.ProgressValue = progressBarValue.ProgressValue + 1;
            progressBarValue.TextValue = (Convert.ToInt32(progressBarValue.TextValue.Replace("%","")) + 1)+"%";
            if (progressBarValue.ProgressValue >= 100)
            {
                _updateProgressTimer.Stop();
                Hide();
                frmLogin frm = new frmLogin();
                frm.Show();
                //GolableData.MainWindowUpdateData = this;
            }
        }

        List<string> GetAllVenderId()
        {
            List<string> venderIdList = new List<string>();
            string str = "skoda_2016";
            string token = GetMd5String(str);
            string url = ConfigurationManager.AppSettings["GetActiveVenderId"] + token;
            LmsDataInteractive interactive = new LmsDataInteractive();
            interactive.GetPostInfo(url, "", "get");
            return venderIdList;
        }

        VenderResult GetVenderDetailByVenderId(string venderId)
        {
            VenderResult vender = new VenderResult();
            string str = venderId + "skoda_2016";
            string token = GetMd5String(str);
            string url = ConfigurationManager.AppSettings["GetVenderDetail"] + "Code=" + venderId + "&token=" + token;
            LmsDataInteractive interactive = new LmsDataInteractive();
            interactive.GetPostInfo(url, "", "get");
            return vender;
        }

        List<string> GetUserIdByVenderId(string venderId)
        {
            List<string> userIds = new List<string>();
            string str = venderId+"skoda_2016";
            string token = GetMd5String(str);
            string url = ConfigurationManager.AppSettings["GetVenderUserId"] + "Code=" +venderId + "&token=" + token;
            LmsDataInteractive interactive = new LmsDataInteractive();
            interactive.GetPostInfo(url, "", "get");
            return userIds;
        }

        UserResult GetUserDetailByUserId(string userId)
        {
            UserResult user=new  UserResult();
            string str = userId + "skoda_2016";
            string token = GetMd5String(str);
            string url = ConfigurationManager.AppSettings["GetUserDetail"] + "UserId=" + userId + "&token=" + token;
            LmsDataInteractive interactive = new LmsDataInteractive();
            interactive.GetPostInfo(url, "", "get");
            return user;
        }

        List<string> GetCourseNameByUserId(string userId)
        {
            List<string> courseName = new List<string>();
            string str = userId + "skoda_2016";
            string token = GetMd5String(str);
            string url = ConfigurationManager.AppSettings["GetUserCourse"] + "UserId=" + userId + "&token=" + token;
            LmsDataInteractive interactive = new LmsDataInteractive();
            interactive.GetPostInfo(url, "", "get");
            return courseName;
        }

        public string GetMd5String(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.Unicode.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string md5String = string.Empty;
            foreach (var b in targetData)
                md5String += b.ToString("x2");
            return md5String;
        }

  

    }
}
