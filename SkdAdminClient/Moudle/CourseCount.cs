using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkdAdminClient.Moudle
{
  public  class CourseCount
  {
      private string _courseName;
      private string _totalMinutes;
      private double _totalStamps;
      private double _totalPersons;
      private double _finishPersons;
      private string _percent;
      private string _vender;

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

        public double TotalStamps
        {
            get
            {
                return _totalStamps;
            }

            set
            {
                _totalStamps = value;
            }
        }

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

        public double FinishPersons
        {
            get
            {
                return _finishPersons;
            }

            set
            {
                _finishPersons = value;
            }
        }
    }
}
