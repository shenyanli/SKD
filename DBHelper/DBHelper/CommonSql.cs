using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBHelper
{
  public   class CommonSql
    {
        public static void RecordErrorInfo(string userAccount, string courseName, string errMsg, string errNum, string errMethod)
        {
            errMsg = errMsg.Replace("'", "''");
            errNum= errNum.Replace("'", "''");
            string sql =
                $"insert into LogTable (userAccount,courseName,errorProcedure,errorNumber,errorMessage)values('{userAccount}','{courseName}','{errMethod}','{errNum}','{errMsg}')";
         
            SQLHelper.ExcuteSQL(sql);
        }

        public static string Now()
        {
            string sql = "select getdate()";
            DateTime now = (DateTime)DBHelper.SQLHelper.ExcuteScalarSQL(sql);
            return now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
