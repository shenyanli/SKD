namespace SkdAdminClient.Moudle
{
    public class TrainningRecord
    {
        private string _userName = "";
        private string _userAccount = "";
        private string _courseName = "";
        private string _trainningRecordName = "";
        private double _score = 0;
        private string _totalMinutes = "";
        private string _detail = "";

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
    }
}