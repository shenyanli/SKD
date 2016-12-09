namespace SkdAdminModel
{
  public  class BindLoginTotal
  {
      private string _vender;
      private string _userName;
      private string _userAccount;
      private double _totalCounts;
      private string _totalMinutes;
      private string _lastLoginDate;
      private string _lastLogoutDate;
      private string _status;


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

        [Remark("用户名称")]
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
        [Remark("用户帐号")]
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
        [Remark("登录总次数")]
        public double LoginCounts
        {
            get
            {
                return _totalCounts;
            }

            set
            {
                _totalCounts = value;
            }
        }
        [Remark("登录总时长")]
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
        [Remark("上次登入时间")]
        public string LastLoginDate
        {
            get
            {
                return _lastLoginDate;
            }

            set
            {
                _lastLoginDate = value;
            }
        }
        [Remark("上次登出时间")]
        public string LastLogoutDate
        {
            get
            {
                return _lastLogoutDate;
            }

            set
            {
                _lastLogoutDate = value;
            }
        }
        [Remark("状态")]
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
    }
}
