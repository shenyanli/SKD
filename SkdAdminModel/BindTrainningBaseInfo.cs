namespace SkdAdminModel
{
   public class BindTrainningBaseInfo
   {
       private double _totalPersons = 0;
       private double _planPersons = 0;
       private double _rightPersons = 0;
       private string _vender = "";
       string _courseName = "";
       private string _recordName = "";
       private string _id = "";
       private string _content = "";
       private string _percent ="";

        [Remark("经销商",false)]
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
        public string RecordName
        {
            get
            {
                return _recordName;
            }

            set
            {
                _recordName = value;
            }
        }
        [Remark("虚拟实训步骤ID")]
        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        [Remark("操作内容")]
        public string Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
            }
        }
        [Remark("已操作人数")]
        public double TotalPersons
        {
            get
            {
                return _totalPersons;
            }

            set
            {
                _totalPersons = value;
            }
        }
        [Remark("应操作人数")]
        public double PlanPersons
        {
            get
            {
                return _planPersons;
            }

            set
            {
                _planPersons = value;
            }
        }
        [Remark("操作正确人数")]
        public double RightPersons
        {
            get
            {
                return _rightPersons;
            }

            set
            {
                _rightPersons = value;
            }
        }
        [Remark("正确率")]
        public string Percent
        {
            get
            {
                return _percent;
            }

            set
            {
                _percent = value;
            }
        }


    }
}
