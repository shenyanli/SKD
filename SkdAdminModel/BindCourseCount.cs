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
        private string _venderCode;
        private string _vender;
        private string _rbo;
        private string _courseName;
        private string _totalMinutes;
        private double _totalStamps;
        private double _totalPersons;
        private double _finishPersons;
        private string _percent;
        private double _planPersons;
        private int _venderTotalCounts = 0;
        private int _venderBeginCounts = 0;
        private int _venderFinishCounts = 0;
        private string _venderPercent;
        private string _releaseDate = "";

   
        [Remark("售后代码")]
        public string VenderCode
        {
            get
            {
                return _venderCode;
            }

            set
            {
                _venderCode = value;
            }
        }

        [Remark("经销商")]
        public string Vender
        {
            get { return _vender; }

            set { _vender = value; }
        }
        [Remark("RBO")]
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

        [Remark("已学习账号数")]
        public double TotalPersons
        {
            get { return _totalPersons; }

            set { _totalPersons = value; }
        }

        [Remark("完成百分比",false)]//2018-11-08 不导出
        public string Percent
        {
            get { return _percent; }

            set { _percent = value; }
        }


        [Remark("已完成账号数")]
        public double FinishPersons
        {
            get { return _finishPersons; }

            set { _finishPersons = value; }
        }
        [Remark("应学习人数",false)]//2018-11-08 不导出
        public double PlanPersons
        {
            get { return _planPersons; }

            set { _planPersons = value; }
        }
 

        [Remark("经销商总数",false)]//2018-11-08 不导出
        public int VenderTotalCounts
        {
            get { return _venderTotalCounts; }

            set { _venderTotalCounts = value; }
        }
        //[Remark("已开始经销商数量")] 2018-11-08
        [Remark("已学习经销商数量")]
        public int VenderBeginCounts
        {
            get { return _venderBeginCounts; }

            set { _venderBeginCounts = value; }
        }
        [Remark("已完成经销商数量",false)]//2018-11-08 不导出
        public int VenderFinishCounts
        {
            get { return _venderFinishCounts; }

            set { _venderFinishCounts = value; }
        }

        [Remark("已学习经销商占比",false)]//2018-11-08 不导出
        public string VenderPercent
        {
            get { return _venderPercent; }
            set { _venderPercent = value; }
        }

        [Remark("课件发布日期")]//2018-11-08 显示课件发布日期
        public string ReleaseDate
        {
            get
            {
                return _releaseDate;
            }

            set
            {
                _releaseDate = value;
            }
        }
    }
}
