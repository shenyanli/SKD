namespace SkdAdminModel
{
   public class BindLoginDetail
    {
        private string _vender;
        private string _userName;
        private string _userAccount;
       private string _loginDate;
       private string _logoutDate;
       private string _stayTime;

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

        [Remark("登录时间")]
        public string LoginDate
       {
           get { return _loginDate; }
           set { _loginDate = value; }
       }
        [Remark("登出时间")]
        public string LogoutDate
       {
           get { return _logoutDate; }
           set { _logoutDate = value; }
       }

        [Remark("在线时长")]
        public string StayTime
        {
            get
            {
                return _stayTime;
            }

            set
            {
                _stayTime = value;
            }
        }
    }
}
