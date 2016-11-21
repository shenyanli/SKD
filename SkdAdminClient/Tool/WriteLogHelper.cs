using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkdAdminClient.Tool
{
    /// <summary>
    /// 简单自定义输出Log
    /// </summary>
    public class WriteLogHelper
    {
        /// <summary>
        /// 简单Log文档
        /// </summary>
        /// <param name="msg">记入信息</param>
        /// <param name="path">写入路径</param>
        public void WriteLog(string msg, string path)
        {
            string fileName = @"\" + System.DateTime.Now.ToString("yyyyMMdd") + "log.txt";
            string time = System.DateTime.Now.ToLongTimeString();
            using (StreamWriter sw=new StreamWriter(path+fileName,true))
            {
                sw.WriteLine(msg);
                sw.WriteLine("--**"+ time + "**----------------------------");
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// 简单Log文档（当前程序目录）
        /// </summary>
        /// <param name="msg"></param>
        public void WriteLogs(string msg)
        {
            WriteLog(msg,Environment.CurrentDirectory.ToString());
        }
    }
}
