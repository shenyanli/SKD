using System.Collections.Generic;

namespace SkdAdminModel
{
    public  class GlobalData
    {
        public static string UserAccount="";//登录人员帐号
        public static string UserName = "";//登陆人员姓名
        public static Privilege PrivilegeLevel =(Privilege)1;//经销商权限级别,默认是一般学员
        public static string Area = "";//经销商区域
        public static string Vender="";//经销商名称
        public static List<string> Venders = new List<string>();//当前权限下的经销商Id_Name
        public static List<string> RboList = new List<string>();
        public static Dictionary<string, string> CourseReleaseDate = new Dictionary<string, string>();//2018-11-08
        public static Dictionary<string, string> CourseNameMaps = new Dictionary<string, string>();
    }

    public enum Privilege
    {
        Student = 0,
        VenderAdmin = 1,
        AreaAdmin = 2,
        SuperAdmin
    }
}
