using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
//using NPOI.Extension;

namespace SkdAdminClient.Moudle
{
    public class ProgressDetail
    {
    
        private string _vender;
        private string _userName;
        private string _userAccount;
        private string _courseName;
        private string _scheduel;
        private double _score;
        private string _trainningRecord;
        private string _totalMinutes;
        private double _totalStamps;
        private string _beginDate;
        private string _endDate;
        private string _status;
        private string _rbo;
        private bool _haveTrainningRecord;

        private List<CourseStudyDetail> _details = new List<CourseStudyDetail>();

        //[Column(Index = 0, Title = "RBO", AllowMerge = true)]
        public string Rbo
        {
            get
            {
                return _rbo;
            }

            set
            {
                _rbo = value;
            }
        }
       

        //[Column(Index = 1, Title = "经销商", AllowMerge = true)]
        public string Vender
        {
            get { return _vender; }

            set { _vender = value; }
        }
        //[Column(Index = 2, Title = "用户名称", AllowMerge = true)]
        public string UserName
        {
            get { return _userName; }

            set { _userName = value; }
        }

        //[Column(Index =3, Title = "用户帐号")]
        public string UserAccount
        {
            get { return _userAccount; }

            set { _userAccount = value; }
        }

        //[Column(Index = 4, Title = "上次学习结束时间")]
        public string CourseName
        {
            get
            {
                return _courseName;
            }

            set
            {
                _courseName = value;
            }
        }

       // [Column(Index = 5, Title = "进度")]
        public string Scheduel
        {
            get { return _scheduel; }

            set { _scheduel = value; }
        }

       // [Column(Index = 6, Title = "测试得分")]
        public double Score
        {
            get { return _score; }

            set { _score = value; }
        }

        //[Column(Index = 7, Title = "虚拟实训名称及得分")]
        public string TrainningRecord
        {
            get { return _trainningRecord; }

            set { _trainningRecord = value; }
        }

        //[Column(Index = 8, Title = "学习总时长")]
        public string TotalMinutes
        {
            get { return _totalMinutes; }

            set { _totalMinutes = value; }
        }

        //[Column(Index = 9, Title = "学习总次数")]
        public double TotalStamps
        {
            get { return _totalStamps; }

            set { _totalStamps = value; }
        }

        //[Column(Index = 10, Title = "上次学习开始时间")]
        public string BeginDate
        {
            get { return _beginDate; }

            set { _beginDate = value; }
        }

        //[Column(Index = 11, Title = "上次学习结束时间")]
        public string EndDate
        {
            get { return _endDate; }

            set { _endDate = value; }
        }

        //[Column(Index = 12, Title = "完成状态")]
        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
            }
        }

        public bool HaveTrainningRecord
        {
            get
            {
                return _haveTrainningRecord;
            }

            set
            {
                _haveTrainningRecord = value;
            }
        }
    }

    /// <summary>
    /// 单次学习情况
    /// </summary>
    [XmlRoot(ElementName = "coursedetial")]
    public class CourseStudyDetail
    {
        //private string _sysId = "";

        //[XmlElement(ElementName = "sysid")]
        //public string SysId
        //{
        //    get { return _sysId; }
        //    set { _sysId = value; }
        //}

        //private bool _newRecord = true;
        //[XmlElement(ElementName = "newrecord")]
        //public bool NewRecord
        //{
        //    get { return _newRecord; }
        //    set { _newRecord = value; }
        //}

        private string _courseName = "";

        [XmlElement(ElementName = "coursename")]
        public string CourseName
        {
            get { return _courseName; }
            set { _courseName = value; }
        }


        private string _userName = "";
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


        private string _userAccount = "";

        [XmlElement(ElementName = "username")]
        public string UserAccount
        {
            get { return _userAccount; }
            set { _userAccount = value; }
        }

        private string _beginTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");

        [XmlElement(ElementName = "begintime")]
        public string BeginTime
        {
            get { return _beginTime; }
            set { _beginTime = value; }
        }

        private string _endTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");

        [XmlElement(ElementName = "endtime")]
        public string EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        private string _minutes = "0";

        [XmlElement(ElementName = "minutes")]
        public string Minutes
        {
            get { return _minutes; }
            set { _minutes = value; }
        }

        private string _chapters = "";

        [XmlElement(ElementName = "percent")]
        public string Chapters //单次学习的百分节点
        {
            get { return _chapters; }
            set { _chapters = value; }
        }

        private string _description = "";

        [XmlElement(ElementName = "description")]
        public string Description //单次学习的百分节点对应的章节信息
        {
            get { return _description; }
            set { _description = value; }
        }



        public static CourseStudyDetail XmlToCourseStudyDetail(string xmlStr)
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            CourseStudyDetail saveInfo = XmlHelper.XmlToObject<CourseStudyDetail>(xmlStr, encoding);
            return saveInfo;
        }

        /// <summary>
        /// 把SaveData类序列化成xml
        /// </summary>
        /// <returns></returns>
        public string CourseStudyDetailToXml()
        {
            return XmlHelper.ObjectToXml(typeof(CourseStudyDetail), this);
        }

    }
    public class XmlHelper
    {
        /// <summary>
        /// 将xml序列化成类对象
        /// </summary>
        /// <typeparam name="T">需要序列化成的类名</typeparam>
        /// <param name="source">xml源文件内容</param>
        /// <param name="encoding">编码</param>
        /// <returns>序列化后的类对象</returns>
        public static T XmlToObject<T>(string source, Encoding encoding)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream(encoding.GetBytes(source)))
            {
                return (T)mySerializer.Deserialize(stream);
            }
        }


        #region 对象序列化成xml
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string ObjectToXml(Type type, object obj)
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            XmlTextWriter textWriter = new XmlTextWriter(stream, Encoding.GetEncoding("utf-8"));//定义输出的编码格式
            xmlSerializerNamespaces.Add("", "");
            try
            {
                //序列化对象
                xml.Serialize(textWriter, obj, xmlSerializerNamespaces);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            string str = sr.ReadToEnd();

            sr.Dispose();
            stream.Dispose();
            return str;
        }


        #endregion

        public static string RemoveXmlHeader(string xmlInfo)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlInfo);
            if (xmlDocument.DocumentElement != null)
            {
                string xmlNoHeader = xmlDocument.DocumentElement.OuterXml;
                return xmlNoHeader;
            }
            return "";
        }

        public static XmlAttribute CreateAttribute(XmlNode node, string attributeName, string value)
        {
            try
            {
                XmlDocument doc = node.OwnerDocument;
                XmlAttribute attr = null;
                attr = doc.CreateAttribute(attributeName);
                attr.Value = value;
                node.Attributes.SetNamedItem(attr);
                return attr;
            }
            catch (Exception err)
            {
                return null;
            }
        }


    }

}
