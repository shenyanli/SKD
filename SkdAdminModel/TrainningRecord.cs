using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SkdAdminModel
{
    /// <summary>
    /// 对应XML中的TrainingRecord节点，记录一个虚拟实训的一次完整操作信息
    /// </summary>
    [XmlRoot(ElementName = "TrainingRecord")]
    public class TrainingRecord
    {
        //新读取到的虚拟实训记录，0：表示新读取到的，1：之前的历史记录
        private string _sysId = "";

        private bool _newRecord = true;
        /// <summary>
        /// 虚拟实训名称
        /// </summary>
        string _name = "";

        /// <summary>
        /// 虚拟实训类型  考核/练习
        /// </summary>
        string _type = "1";

        /// <summary>
        /// 虚拟实训得分
        /// </summary>
        double _score = 0.0;

        /// <summary>
        /// 本次虚拟实训操作用时
        /// </summary>
        int _time = 0;

        private string operTime = "";

        [XmlElement(ElementName = "Record")]
        public List<Record> RecordList = new List<Record>();

        [XmlAttribute(AttributeName = "SysId")]
        public string SysId
        {
            get { return _sysId; }
            set { _sysId = value; }
        }

        [XmlAttribute(AttributeName = "NewRecord")]
        public bool NewRecord
        {

            get { return _newRecord; }
            set { _newRecord = value; }
        }

        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "OperTime")]
        public string OperTime
        {
            get { return operTime; }
            set { operTime = value; }
        }

        [XmlAttribute(AttributeName = "Score")]
        public double Score
        {
            get { return _score; }
            set { _score = value; }
        }

        [XmlAttribute(AttributeName = "Time")]
        public int Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public string TrainingRecordToXml()
        {
            return XmlHelper.ObjectToXml(typeof(TrainingRecord), this);
        }

        public static TrainingRecord XmlToTrainingRecord(string xmlStr)
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            TrainingRecord trainingRecord = XmlHelper.XmlToObject<TrainingRecord>(xmlStr, encoding);
            return trainingRecord;
        }
    }

    /// <summary>
    /// 对应xml中的record节点,记录虚拟实训的一个操作步骤信息
    /// </summary>
    public class Record
    {
        [XmlElement(ElementName = "No")] public int No = 1;
        [XmlElement(ElementName = "Content")] public string Content = "";
        [XmlElement(ElementName = "Score")] public double Score = 0.0;
        [XmlElement(ElementName = "Remark")] public string Remark = "";
        [XmlElement(ElementName = "Id")] public string Id = "";
    }
}
