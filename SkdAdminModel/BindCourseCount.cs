using System;

namespace SkdAdminModel
{

    public class RemarkAttribute : Attribute
    {
        private string _mRemark;
        private bool _needExport;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="remark"></param>
        public RemarkAttribute(string remark,bool needExport=true)
        {
            _mRemark = remark;
            _needExport = needExport;
        }

        public bool NeedExport
        {
            get
            {
                return _needExport;
            }

            set
            {
                _needExport = value;
            }
        }

        /// <summary>
        /// 属性
        /// </summary>
        public string Remark
        {
            get { return _mRemark; }
            set { _mRemark = value; }
        }
    }

    public class BindCourseCount
    {
        private string _courseName;
        private string _totalMinutes;
        private double _totalStamps;
        private double _totalPersons;
        private double _finishPersons;
        private string _percent;
        private string _vender;
        private double _planPersons;

        [Remark("课程名称")]
        public string CourseName
        {
            get { return _courseName; }

            set { _courseName = value; }
        }

        [Remark("学习总时长")]
        public string TotalMinutes
        {
            get { return _totalMinutes; }

            set { _totalMinutes = value; }
        }

        [Remark("学习总次数")]
        public double TotalStamps
        {
            get { return _totalStamps; }

            set { _totalStamps = value; }
        }

        [Remark("已学习人数")]
        public double TotalPersons
        {
            get { return _totalPersons; }

            set { _totalPersons = value; }
        }

        [Remark("完成百分比")]
        public string Percent
        {
            get { return _percent; }

            set { _percent = value; }
        }

        [Remark("经销商")]
        public string Vender
        {
            get { return _vender; }

            set { _vender = value; }
        }

        [Remark("已完成人数")]
        public double FinishPersons
        {
            get { return _finishPersons; }

            set { _finishPersons = value; }
        }
        [Remark("应学习人数")]
        public double PlanPersons
        {
            get { return _planPersons; }

            set { _planPersons = value; }
        }
    }
}
