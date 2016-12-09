namespace SkdAdminModel
{
  public  class BindTestCount
  {
      private string _courseName = "";
      private string _vender = "";
      private string _title = "";
      private string _type = "";
      private string _items = "";
      private string _standardAnswer = "";
      private double _totalCount = 0;
      private double _rightCount = 0;
      private string _percent = "";

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

        [Remark("经销商")]
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

        [Remark("测试题目")]
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        [Remark("题目类型")]
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

        [Remark("答案选项")]
        public string Items
        {
            get
            {
                return _items;
            }

            set
            {
                _items = value;
            }
        }
        [Remark("标准答案")]
        public string StandardAnswer
        {
            get
            {
                return _standardAnswer;
            }

            set
            {
                _standardAnswer = value;
            }
        }

        [Remark("总答题次数")]
        public double TotalCount
        {
            get
            {
                return _totalCount;
            }

            set
            {
                _totalCount = value;
            }
        }

        [Remark("答对次数")]
        public double RightCount
        {
            get
            {
                return _rightCount;
            }

            set
            {
                _rightCount = value;
            }
        }

        [Remark("正确百分比")]
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
