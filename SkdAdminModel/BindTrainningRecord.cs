using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SkdAdminModel
{

    #region marked

    [XmlRoot(ElementName = "TrainingRecord")]
    public class BindTrainningRecord
    {
        private string _userName = "";
        private string _userAccount = "";
        private string _courseName = "";
        private string _trainningRecordName = "";
        private double _score = 0;
        private string _totalMinutes = "";
        private string _detail = "";
        private string _vender = "";
        private string _id = "";

        [Remark("用户名称")]
        [XmlAttribute(AttributeName = "Name")]
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

        [Remark("用户账户")]
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
        [Remark("课程名称")]
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
        [Remark("虚拟实训名称")]
        public string TrainningRecordName
        {
            get
            {
                return _trainningRecordName;
            }

            set
            {
                _trainningRecordName = value;
            }
        }
        [Remark("得分")]
        public double Score
        {
            get
            {
                return _score;
            }

            set
            {
                _score = value;
            }
        }
        [Remark("操作时长")]
        public string TotalMinutes
        {
            get
            {
                return _totalMinutes;
            }

            set
            {
                _totalMinutes = value;
            }
        }
        [Remark("操作信息")]
        public string Detail
        {
            get
            {
                return _detail;
            }

            set
            {
                _detail = value;
            }
        }

        [Remark("经销商")]
        [XmlIgnore]
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


        [Remark("Record", false)]
        [XmlElement(ElementName = "Record")]
        public List<Record> RecordList = new List<Record>();

        public string TrainingRecordToXml()
        {
            return XmlHelper.ObjectToXml(typeof(BindTrainningRecord), this);
        }

        public static BindTrainningRecord XmlToTrainingRecord(string xmlStr)
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            BindTrainningRecord trainingRecord = XmlHelper.XmlToObject<BindTrainningRecord>(xmlStr, encoding);
            return trainingRecord;
        }
    }

    #endregion


}