using System.Collections.Generic;

namespace SkdAdminModel
{
    public  class GolableData
    {
        public static string UserAccount="";//登录人员帐号
        public static string UserName = "";//登陆人员姓名
        public static Privilege PrivilegeLevel =(Privilege)1;//经销商权限级别,默认是一般学员
        public static string Area = "";//经销商区域
        public static string Vender="";//经销商名称
        public static List<string> Venders=new List<string>( );//区域管理员区域下的经销商
       // public static WindowUpdateData MainWindowUpdateData ;
    }

    public enum Privilege
    {
         Student=1,
         VenderAdmin,
         AreaAdmin,
         SuperAdmin
    }
}
