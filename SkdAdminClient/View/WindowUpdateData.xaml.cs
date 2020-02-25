using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
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
using MahApps.Metro.Controls;
using SkdAdminClient.Bll;
using SkdAdminClient.SkdWebService;
using SkdAdminClient.Tool;
using SkdAdminModel;

namespace SkdAdminClient.View
{


    public class ProgressBarValue : INotifyPropertyChanged
    {


        private string _textValue = "0%";
        private double _progressValue = 0;
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
            get { return _textValue; }
            set
            {
                if (value != _textValue)
                {
                    _textValue = value;
                    NotifyPropertyChanged("TextValue");
                }
            }
        }

        public double ProgressValue
        {
            get { return _progressValue; }

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
        public  delegate  void HideForm();
        BackgroundWorker backgroundWorker1;
        private ProgressBarValue progressBarValue = new ProgressBarValue();
        private SkdServiceSoapClient skdServiceSoapClient = new SkdServiceSoapClient();
        DispatcherTimer dTimer = new DispatcherTimer();
        private DispatcherTimer _updateProgressTimer = new DispatcherTimer();
        private double totalCounts;
        public bool CloseFrom = false;

        public frmLogin MainFrm;


        void HideUpdateDataForm()
        {
            if (!CloseFrom)
            {
                Hide();
                frmLogin frm = new frmLogin();
                frm.Show();
            }
            else
            {
               Close();
            }

        }

        public WindowUpdateData()
        {
            InitializeComponent();
            Accent accent = ThemeManager.GetAccent("Green");
            ThemeManager.ChangeAppStyle(Application.Current, accent,
                ThemeManager.DetectAppStyle(Application.Current).Item1);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            UpdateProgressBar.Maximum = 100;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
           
        }


        void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DoWork(backgroundWorker1);
        }

        private void DoWork(BackgroundWorker bk)
        {
            try
            {
                #region 模拟进度显示

                //_updateProgressTimer.Tick += UpdateProgressTimer_Tick;
                //_updateProgressTimer.Interval = new TimeSpan(0, 0, 1); //设置时间：TimeSpan（时, 分， 秒）            
                //_updateProgressTimer.Start(); //启动 DispatcherTimer对象dTime。

                #endregion 模拟进度显示

                #region 通过接口更新数据，并显示进度

                List<string> venderIds = GetAllVenderId();
                List<string[]> venders = new List<string[]>();
                List<string> userIds = new List<string>();
                List<string[]> users = new List<string[]>();
                List<string[]> courseMaps = new List<string[]>();
                totalCounts = venderIds.Count;
                int onceUpdateNumber = Convert.ToInt32(ConfigurationManager.AppSettings["OnceUpdateNumber"]);
                double index = 0.0;
                int circle = (int)Math.Ceiling(Convert.ToDouble(venderIds.Count) / onceUpdateNumber);

                for (int i = 0; i < circle; i++)
                {
                    //每次取onceUpdateNumber个供应商的信息进行更新，防止数据过大失败
                    List<string> currentCircleVenders = venderIds.Skip(i * onceUpdateNumber).Take(onceUpdateNumber).ToList();
                    foreach (string venderId in currentCircleVenders)
                    {
                        VenderResult venderResult = GetVenderDetailByVenderId(venderId);
                        userIds = GetUserIdByVenderId(venderId);
                        string[] vender =
                        {
                        venderResult.Name, venderResult.Type, venderResult.Area, venderResult.Code,
                        venderResult.UserName,
                        venderResult.Phone, venderResult.Mail,venderResult.OperatingState=="正常营业"?"1":"0"
                    };
                        venders.Add(vender);

                        foreach (string userId in userIds)
                        {
                            #region 根据userId获取用户详细信息

                            UserResult userResult = GetUserDetailByUserId(userId);
                            if (userResult == null) continue;
                            string[] user =
                            {
                            userResult.Id, userResult.LoginName, userResult.DisplayName,
                            userResult.MechanismName,userResult.Enabled?"1":"0",userResult.IdCardNumber,userResult.Mobile,userResult.Email
                        };
                            users.Add(user);
                            List<string> courseNameList = GetCourseNameByUserId(userId);
                            foreach (var courseName in courseNameList)
                            {
                                string[] courseMap = { userResult.LoginName, courseName };
                                courseMaps.Add(courseMap);
                            }

                            #endregion 根据userId获取用户详细信息
                        }
                        index += 1;
                        double value = (double)(index / totalCounts);
                        progressBarValue.ProgressValue = Math.Floor(value * 100);
                        progressBarValue.TextValue = value.ToString("0%");

                        if ((int)index % onceUpdateNumber == 0) //onceUpdateNumber个经销商，往数据库更新一次
                        {
                            if (!UpdateDb(venders.ToArray(), users.ToArray(), courseMaps.ToArray()))
                            {
                                XMessageBox.Show("数据同步异常！");
                                //continue;
                            }
                            venders.Clear(); //清空上次的记录信息
                            userIds.Clear();
                            users.Clear();
                            courseMaps.Clear();
                        }
                        if (progressBarValue.ProgressValue >= 100)
                        {
                            if (UpdateDb(venders.ToArray(), users.ToArray(), courseMaps.ToArray()))
                            {
                                skdServiceSoapClient.FinishUpdateData();
                                XMessageBox.Show("数据同步已完成！");
                            }
                            else
                            {
                                XMessageBox.Show("数据同步异常！");
                            }
                            Dispatcher.Invoke(DispatcherPriority.Normal, new HideForm(HideUpdateDataForm));
                            return;
                        }
                    }
                }

                #endregion 通过接口更新数据，并显示进度
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
           
        }

        bool UpdateDb(string[][] venderResults, string[][] users, string[][] usserCourseMaps)
        {
            try
            {
                string result = skdServiceSoapClient.ExcuteSp(venderResults, users, usserCourseMaps);
                if (string.IsNullOrEmpty(result.Trim()) || result.Split(':').Length < 2)
                {
                    return false;
                }
                string resultCode = result.Split(':')[0];
                string resultMsg = result.Split(':')[1];
                if (resultCode == "0")
                {
             
                    return true;
                }
                XMessageBox.Show(resultMsg);
                return false;
            }
            catch (Exception err)
            {
                XMessageBox.Show(err.Message);
                return false;
            }
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                XMessageBox.Show(e.Error.ToString());
            }
        }

        void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            UpdateProgressBar.Value = e.ProgressPercentage;
            TxbProgress.Text = e.UserState.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CloseFrom)
                Title = "数据同步";
            DataContext = progressBarValue;
            try
            {
                bool needUpdateData =  new SkdServiceSoapClient().NeedUpdateData();
                if (!needUpdateData && !CloseFrom) //不需要同步数据
                {
                    Hide();
                    frmLogin frm = new frmLogin();
                    frm.Show();
                    //GolableData.MainWindowUpdateData = this;
                }
                else
                {
                    dTimer.Tick += new EventHandler(dTimer_Tick);
                    dTimer.Interval = new TimeSpan(0, 0, 1); //设置时间：TimeSpan（时, 分， 秒）            
                    dTimer.Start(); //启动 DispatcherTimer对象dTime。
                                    // OnceDataUpdatedHandler += UpdateProgressHandler;
                }
            }
            catch (Exception err)
            {
                XMessageBox.Show(err.Message);
                Close();
            }
           

        }

        private void dTimer_Tick(object sender, EventArgs e)
        {
            dTimer.Stop();
            if (backgroundWorker1.IsBusy)
            {
                return;
            }
            backgroundWorker1.RunWorkerAsync();

        }

        private void UpdateProgressTimer_Tick(object sender, EventArgs e)
        {
            UpdateProgressBar.Value = UpdateProgressBar.Value+1;
            progressBarValue.ProgressValue = progressBarValue.ProgressValue + 1;
            progressBarValue.TextValue = (Convert.ToInt32(progressBarValue.TextValue.Replace("%", "")) + 1) + "%";

            if (UpdateProgressBar.Value >= 100)
            {
                _updateProgressTimer.Stop();
                if (CloseFrom)
                {
                    Close();
                    return;
                }
                Hide();
                frmLogin frm = new frmLogin();
                frm.Show();
            }
        }

        List<string> GetAllVenderId()
        {
            //List<string> venderIdList = new List<string>();
            string str = "skoda_2016";
            string token = GetMd5String(str);
            string url = ConfigurationManager.AppSettings["GetActiveVenderId"] + token;
            LmsDataInteractive interactive = new LmsDataInteractive();
            string result = interactive.GetPostInfo(url, "", "get");
            JavaScriptSerializer js = new JavaScriptSerializer();
            VenderIdResult venderIdResult= js.Deserialize<VenderIdResult>(result);
            return venderIdResult.Mechanisms;
        }

        VenderResult GetVenderDetailByVenderId(string venderId)
        {

            string str = venderId + "skoda_2016";
            string token = GetMd5String(str);
            string url = ConfigurationManager.AppSettings["GetVenderDetail"] + "Code=" + venderId + "&token=" + token;
            LmsDataInteractive interactive = new LmsDataInteractive();
            string result = interactive.GetPostInfo(url, "", "get");
            JavaScriptSerializer js = new JavaScriptSerializer();
            VenderResult vender = js.Deserialize<VenderResult>(result);
            return vender;
        }

        List<string> GetUserIdByVenderId(string venderId)
        {
            string str = venderId+"skoda_2016";
            string token = GetMd5String(str);
            string url = ConfigurationManager.AppSettings["GetVenderUserId"] + "Code=" +venderId + "&token=" + token;
            LmsDataInteractive interactive = new LmsDataInteractive();
           string result= interactive.GetPostInfo(url, "", "get");
            JavaScriptSerializer js=new  JavaScriptSerializer();
            UserIdsResult userIdResult = js.Deserialize<UserIdsResult>(result);
            return userIdResult.UserId;
        }

        UserResult GetUserDetailByUserId(string userId)
        {

            string str = userId.ToUpper() + "skoda_2016";
            string token = GetMd5String(str);
            string url = ConfigurationManager.AppSettings["GetUserDetail"] + "UserId=" + userId.ToUpper() + "&token=" +token;
            LmsDataInteractive interactive = new LmsDataInteractive();
            string result = interactive.GetPostInfo(url, "", "get");
            JavaScriptSerializer js = new JavaScriptSerializer();
            UserResult userResult = js.Deserialize<UserResult>(result);
            if (userResult.ResultCode == "0")
                return userResult;
            else return null;
        }

        List<string> GetCourseNameByUserId(string userId)
        {
            try
            {
                string str = userId.ToUpper() + "skoda_2016";
                string token = GetMd5String(str);
                string url = ConfigurationManager.AppSettings["GetUserCourse"] + "UserId=" + userId.ToUpper() + "&token=" + token;
                Console.WriteLine(url);
                LmsDataInteractive interactive = new LmsDataInteractive();
                string result = interactive.GetPostInfo(url, "", "get");
                Console.WriteLine(result);
                JavaScriptSerializer js = new JavaScriptSerializer();
                CourseMap courseMap = js.Deserialize<CourseMap>(result);
                return courseMap.CourseNames;
            }
            catch (Exception err)
            {
                return new List<string>() {};
            }
           
        }

        public string GetMd5String(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0').ToUpper();
        }



    }
}
