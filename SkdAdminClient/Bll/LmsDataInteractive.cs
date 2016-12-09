using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace SkdAdminClient.Bll
{
    public class LmsDataInteractive
    {
        public string GetPostInfo(string url, string postdata, string type)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            if (type.ToLower() == "post")
            {
                req.ContentType = " application/json; charset=utf-8"; //注重编码格式,utf-8！！
                req.Method = type;
                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    streamWriter.Write(postdata);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            else if (type.ToLower() == "get")
            {
                req.Method = type;
            }
            //获取响应
            HttpWebResponse rp;
            string result = "";
            try
            {
                rp = (HttpWebResponse)req.GetResponse();
                Stream rps = rp.GetResponseStream();
                StreamReader sr = new StreamReader(rps);
                //获得响应字符串
                result = sr.ReadToEnd();
            }
            catch (WebException ex)
            {
                rp = (HttpWebResponse)ex.Response;
            }

            return result;
        }

        public DateTime GetTime()
        {
            string result = new LmsDataInteractive().GetPostInfo(ConfigurationManager.AppSettings["getTime"], "", "get");
            JavaScriptSerializer js = new JavaScriptSerializer();
            ResultTime rt = js.Deserialize<ResultTime>(result);
            return Convert.ToDateTime(rt.time) ;
        }

        /// <summary>
        /// 根据用户名，以及LMS接口获取用户ID，从LMS平台获取失败时，会尝试到数据库获取
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户ID</returns>
        public string GetUserId(string userName)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["getUserCode"];
                JavaScriptSerializer js = new JavaScriptSerializer();
                string userinfo = GetPostInfo(url + userName, "", "GET");
                UserIdResult ui = js.Deserialize<UserIdResult>(userinfo);
                return ui.UserId;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return "";
            }
 
        }

        /// <summary>
        /// 根据课程名，以及LMS接口获取用户ID
        /// </summary>
        /// <param name="courseName">课程名称</param>
        /// <returns>课程ID</returns>
        public string GetCourseId(string courseName)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["getSubjectCode"];
                JavaScriptSerializer js = new JavaScriptSerializer();
                string courseInfo = GetPostInfo(url + courseName, "", "GET");
                CourseIdResult cr = js.Deserialize<CourseIdResult>(courseInfo);
                return cr.courseId;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return "";
            }

        }

        /// <summary>
        /// 保存登录信息到LMS平台
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="xmlInfo"></param>
        /// <returns></returns>
        public ResultInfo SaveLoginXml(string userName,string xmlInfo)
        {
            try
            {
                if (userName == "" || xmlInfo == "") return null;
                #region 更新本次登录后的登录信息XML

                string userId = GetUserId(userName);
                string saveUrl = ConfigurationManager.AppSettings["SaveLoginInfoXml"]; //保存登录信息XML的接口
                string savePostdata = new JavaScriptSerializer().Serialize(new
                {
                    UserId = userId,
                    LoginInfo = xmlInfo
                });
                string result = GetPostInfo(saveUrl, savePostdata, "post");
                JavaScriptSerializer js = new JavaScriptSerializer();
                ResultInfo ri = js.Deserialize<ResultInfo>(result);
                return ri;
                #endregion
            }
            catch (Exception err)
            {
                return null;
            }
        }


    }

    public class ResultTime
    {
        public string resultCode { get; set; }
        public string time { get; set; }
    }


    public class ResultInfo
    {
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
        public string xmlInfo { get; set; }
    }

    /// <summary>
    /// 用来接收从LMS平台获取的UserId信息
    /// </summary>
    public struct UserIdResult
    {
        public string UserId;
        public string ResultCode;
        public string ResultMsg;

    }

    /// <summary>
    /// 用来接收从LMS平台获取的CourseId信息
    /// </summary>
    public struct CourseIdResult
    {
        public string resultCode;
        public string resultMsg;
        public string courseId;
    }

    public class VenderResult
    {
        private string _resultCode = "";
        private string _resultMsg = "";
        private string _venderName = "";
        private string _venderId = "";
        private string _type = "";
        private string _area = "";
        private string _mail = "";
        private string _connectUserName = "";
        private string _connectPhone = "";

        public string VenderName
        {
            get
            {
                return _venderName;
            }

            set
            {
                _venderName = value;
            }
        }

        public string VenderId
        {
            get
            {
                return _venderId;
            }

            set
            {
                _venderId = value;
            }
        }

        public string ResultCode
        {
            get
            {
                return _resultCode;
            }

            set
            {
                _resultCode = value;
            }
        }

        public string ResultMsg
        {
            get
            {
                return _resultMsg;
            }

            set
            {
                _resultMsg = value;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        public string Area
        {
            get
            {
                return _area;
            }

            set
            {
                _area = value;
            }
        }

        public string Mail
        {
            get
            {
                return _mail;
            }

            set
            {
                _mail = value;
            }
        }

        public string ConnectUserName
        {
            get
            {
                return _connectUserName;
            }

            set
            {
                _connectUserName = value;
            }
        }

        public string ConnectPhone
        {
            get
            {
                return _connectPhone;
            }

            set
            {
                _connectPhone = value;
            }
        }
    }

    public class UserResult
    {
        private string _userName = "";
        private string _userId = "";
        private string _userAccount = "";
        private string _vender = "";
        private string _privilege = "";
        private List<string> _courseNameList=new List<string>( );



        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                _userName = value;
            }
        }

        public string UserId
        {
            get
            {
                return _userId;
            }

            set
            {
                _userId = value;
            }
        }

        public string UserAccount
        {
            get
            {
                return _userAccount;
            }

            set
            {
                _userAccount = value;
            }
        }

        public string Vender
        {
            get
            {
                return _vender;
            }

            set
            {
                _vender = value;
            }
        }

        public string Privilege
        {
            get
            {
                return _privilege;
            }

            set
            {
                _privilege = value;
            }
        }
    }
}
