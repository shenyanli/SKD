using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using Newtonsoft.Json;

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

    [Serializable]
    /// <summary>
    /// 用来接收从LMS平台获取的UserId信息
    /// </summary>
    public struct UserIdResult
    {
        public string UserId;
        public string ResultCode;
        public string ResultMsg;

    }

    [Serializable]
    public struct CourseIdResult
    {
        public string resultCode;
        public string resultMsg;
        public string courseId;
    }

    /// <summary>
    /// 调用LMS平台接口，获取经销商的详细信息
    /// </summary>
    //{"Name":"萍乡长运盛达汽车销售有限公司","Type":"经销商","Area":"大中南区","Code":"74312151","Mail":"skoda12151@163.com","UserName":null,"Phone":null}
    [Serializable]
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
        string _operatingState = "1";//1:在营业 0：不再营业
  
        public string Name
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


        public string Code
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

        public string resultCode
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

        public string resultMsg
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
                if (_area == null)
                    return "";
                return _area;
            }

            set
            {
                if (value == null)
                    _area = "";
                _area = value;
            }
        }

    
        public string Mail
        {
            get
            {
                if (_mail == null)
                    return "";
                return _mail;
            }

            set
            {
                if (value == null)
                    _mail = "";
                _mail = value;
            }
        }

      
        public string UserName
        {
            get
            {
                if (_connectUserName == null)
                    return "";
                 return _connectUserName;
            }

            set
            {
                if (value == null)
                    _connectUserName = "";
                _connectUserName = value;
            }
        }

      
        public string Phone
        {
            get
            {
                if (_connectPhone == null)
                    return "";
                return _connectPhone;
            }

            set
            {
                if (value == null)
                    _connectPhone = "";
                _connectPhone = value;
            }
        }
        public string OperatingState
        {
            get
            {
                return _operatingState;
            }

            set
            {
                _operatingState = value;
            }
        }
    }

    /// <summary>
    /// 调用LMS平台获取学员详细信息接口返回的数据类型
    /// </summary>
    [Serializable]
    public class UserResult
    {
        private string _userName = "";
        private string _userId = "";
        private string _userAccount = "";
        private string _mechanismCode = "";
        private string _userType = "";
        private string _resultCode = "";
        private string _resultMsg = "";
        private string _mechanismName = "";
        bool _enabled = true;
        string _cardId = "";
        string _tel = "";
        string _email = "";


        public string IdCardNumber
        {
            get
            {
                return _cardId;
            }

            set
            {
                _cardId = value;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                _email = value;
            }
        }

        public string Mobile
        {
            get
            {
                return _tel;
            }

            set
            {
                _tel = value;
            }
        }


        public bool Enabled
        {
            get { return _enabled; }
            set { value = _enabled; }
        }

        public string DisplayName
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

        public string Id
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

        public string LoginName
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

        public string MechanismCode
        {
            get
            {
                return _mechanismCode;
            }

            set
            {
                _mechanismCode = value;
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

        public string UserType
        {
            get
            {
                return _userType;
            }

            set
            {
                _userType = value;
            }
        }

        public string MechanismName
        {
            get
            {
                return _mechanismName;
            }

            set
            {
                _mechanismName = value;
            }
        }


    }

    /// <summary>
    /// 调用LMS平台获取全部经销商是售后代码信息接口返回的数据类型
    /// </summary>
    [Serializable]
    public class VenderIdResult
    {
        public string resultCode { get; set; }

        public List<string> Mechanisms { get; set; }

    }

    /// <summary>
    /// 调用LMS平台接口根据经销商售后代码返回该经销商下的用户ID
    /// </summary>
    [Serializable]
    public class UserIdsResult
    {
        public string resultCode { get; set; }
        public List<string> UserId { get; set; }
    }


    /// <summary>
    ///调用LMS平台接口，通过学员ID，获取该学员应学课程数据类型
    /// </summary>
    [Serializable]
    public class CourseMap
    {
        public string resultCode { get; set; }
        public List<string> CourseNames { get; set; }
    }
}
