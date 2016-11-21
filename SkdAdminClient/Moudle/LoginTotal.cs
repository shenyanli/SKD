using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkdAdminClient.Moudle
{
  public  class LoginTotal
  {
      private string _vender;
      private string _userName;
      private string _userAccount;
      private double _totalCounts;
      private string _totalMinutes;
      private string _lastLoginDate;
      private string _lastLogoutDate;
      private string _status;

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
