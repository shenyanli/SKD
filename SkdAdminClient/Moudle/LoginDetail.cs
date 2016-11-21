using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkdAdminClient.Moudle
{
   public class LoginDetail
    {
        private string _vender;
        private string _userName;
        private string _userAccount;
       private string _loginDate;
       private string _logoutDate;
       private string _stayTime;
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

       public string LoginDate
       {
           get { return _loginDate; }
           set { _loginDate = value; }
       }

       public string LogoutDate
       {
           get { return _logoutDate; }
           set { _logoutDate = value; }
       }

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
