/*****************************************************************
 * Copyright (C) Knights Warrior Corporation. All rights reserved.
 * 
 * Author:   圣殿骑士（Knights Warrior） 
 * Email:    KnightsWarrior@msn.com
 * Website:  http://www.cnblogs.com/KnightsWarrior/       http://knightswarrior.blog.51cto.com/
 * Create Date:  5/8/2010 
 * Usage:
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace KnightsWarriorAutoupdater
{
    #region The delegate
    public delegate void ShowHandler();
    #endregion

    public class AutoUpdater : IAutoUpdater
    {
        #region The private fields
        private Config config = null;
        private bool bNeedRestart = false;
        private bool bDownload = false;
        List<DownloadFileInfo> downloadFileListTemp = null;
        private string serverConfigUrl = "";
        private WebClient clientDownload = null;
        List<DownloadFileInfo> downloadConfigList = new List<DownloadFileInfo>(); //配置文件
        private bool exeHasUpdate = false;  //exe是否需要更新
        #endregion

        #region The public event
        public event ShowHandler OnShow;
        #endregion

        #region The constructor of AutoUpdater
        public AutoUpdater(bool course=false)
        {
            config = Config.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, course == true ? ConstFile .COURSEFILENAME: ConstFile.FILENAME));
        }
        #endregion

        #region The public method
        public void Update()
        {
            if (!config.Enabled)
                return;
            Dictionary<string, RemoteFile> listRemotFile = ParseRemoteXml(config.ServerUrl);
            serverConfigUrl = config.ConfigUrl; //服务器配置文件
            List<DownloadFileInfo> downloadList = new List<DownloadFileInfo>();
            
            List<DownloadFileInfo> downloadNewList = new List<DownloadFileInfo>();//保存服务器上新增文件，用于更新本地配置文件用
            foreach (LocalFile file in config.UpdateFileList)
            {
                if (listRemotFile.ContainsKey(file.Path))
                {
                    RemoteFile rf = listRemotFile[file.Path];
                    Version v1 = new Version(rf.LastVer);
                    Version v2 = new Version(file.LastVer);
                    if (v1 > v2)
                    {
                        downloadList.Add(new DownloadFileInfo(rf.Url, file.Path, rf.LastVer, rf.Size,rf.UpdateTime,rf.Name));
                        file.LastVer = rf.LastVer;
                        file.Size = rf.Size;
                        if (rf.NeedRestart)
                            bNeedRestart = true;
                        bDownload = true;
                        if (rf.Path.Contains(".exe"))
                            exeHasUpdate = true;
                    }
                    listRemotFile.Remove(file.Path);
                }
            }
            foreach (RemoteFile file in listRemotFile.Values)
            {
                downloadList.Add(new DownloadFileInfo(file.Url, file.Path, file.LastVer, file.Size,file.UpdateTime,file.Name));
                downloadNewList.Add(new DownloadFileInfo(file.Url, file.Path, file.LastVer, file.Size, file.UpdateTime,file.Name));
                bDownload = true;
                if (file.NeedRestart)
                    bNeedRestart = true;
                if (file.Path.Contains(".exe"))
                    exeHasUpdate = true;
            }
            downloadFileListTemp = downloadList;
            if (bDownload)
            {
                downloadConfigList.Add(new DownloadFileInfo(config.ConfigUrl, ConstFile.FILENAME, "1.0.0", 10,"2016.06.02","全新明锐"));
                DownloadConfirm dc = new DownloadConfirm(downloadList);

                if (OnShow != null)
                    OnShow();

                if (DialogResult.OK == dc.ShowDialog())
                    {
                        StartDownload(downloadList);
                    }

            }
        }

        public void RollBack()
        {
            foreach (DownloadFileInfo file in downloadFileListTemp)
            {
                string tempUrlPath = CommonUnitity.GetFolderUrl(file);
                string oldPath = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(tempUrlPath))
                    {
                        oldPath = Path.Combine(CommonUnitity.SystemBinUrl + tempUrlPath.Substring(1), file.FileName);
                    }
                    else
                    {
                        oldPath = Path.Combine(CommonUnitity.SystemBinUrl, file.FileName);
                    }

                    if (oldPath.EndsWith("_"))
                        oldPath = oldPath.Substring(0, oldPath.Length - 1);

                    MoveFolderToOld(oldPath + ".old", oldPath);

                }
                catch (Exception ex)
                {
                    //log the error message,you can use the application's log code
                }
            }
        }
        public List<DownloadFileInfo> downloadCourseList = new List<DownloadFileInfo>();
        public List<DownloadFileInfo> downloadCourseListMore = new List<DownloadFileInfo>();//局部更新
        public List<DownloadFileInfo> CourseUpdate(out List<DownloadFileInfo> dlCourseM)
        {
            dlCourseM = new List<DownloadFileInfo>();
            if (!config.Enabled)
                return null;
            Dictionary<string, RemoteFile> listRemotFile = ParseRemoteXml(config.CourseUrl); //注意服务器防火墙端口是否开通入站规则
            serverConfigUrl = config.ConfigUrl; //服务器配置文件
            //List<DownloadFileInfo> downloadNewList = new List<DownloadFileInfo>();//保存服务器上新增文件，用于更新本地配置文件用
            foreach (LocalFile file in config.UpdateFileList)
            {
                if (listRemotFile.ContainsKey(file.Path))  //修改
                {
                    RemoteFile rf = listRemotFile[file.Path];
                    Version v1 = new Version(rf.LastVer);  //服务器版本号
                    Version v2 = new Version(file.LastVer);//本地版本号
                    if (v1 > v2)
                    {
                        //这里需要区分是局部更新还是整体课件的更新
                        //局部更新：服务器版本号不为整数 比如 1.1，2.3等，第二位数字不为0。那么就需要迭代从服务器下载各个版本的文件覆盖客户端文件
                        //整体更新：服务器版本号为整数， 比如 1.0，2.0等，第二位数字为0.   那么就是从服务器上下载最新版的课件，不需要逐个版本下载
                        if (v1.Minor == 0) //服务器版本号第二位数为0
                        {
                            //整体下载更新包
                            downloadCourseList.Add(new DownloadFileInfo(rf.Url, file.Path, rf.LastVer, rf.Size, rf.UpdateTime, rf.Name));
                            file.LastVer = rf.LastVer;
                            file.Size = rf.Size;
                            if (rf.NeedRestart)
                                bNeedRestart = true;
                        }
                        else
                        { 
                            //局部更新
                            //注意配置文件的版本号 lastver  一定为两位数，这样有利于转为double数字处理！！
                            for (decimal d = Convert.ToDecimal(file.LastVer); ; )
                            {
                                if (d < Convert.ToDecimal(rf.LastVer))
                                {
                                    dlCourseM.Add(new DownloadFileInfo(rf.Url, file.Path, (d + (decimal)0.1).ToString(), rf.Size, rf.UpdateTime, rf.Name));
                                    d = d + (decimal)0.1;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        bDownload = true;
                    }
                    listRemotFile.Remove(file.Path);
                }
            }
            foreach (RemoteFile file in listRemotFile.Values) //新增，不存在局部文件更新
            {
                downloadCourseList.Add(new DownloadFileInfo(file.Url, file.Path, file.LastVer, file.Size,file.UpdateTime,file.Name));
                //downloadNewList.Add(new DownloadFileInfo(file.Url, file.Path, file.LastVer, file.Size));
                bDownload = true;
            }


            return downloadCourseList;
        }

        #endregion

        #region The private method
        string newfilepath = string.Empty;
        private void MoveFolderToOld(string oldPath, string newPath)
        {
            if (File.Exists(oldPath) && File.Exists(newPath))
            {
                System.IO.File.Copy(oldPath, newPath, true);
            }
        }

        private void StartDownload(List<DownloadFileInfo> downloadList)
        {
 
            DownloadProgress dp = new DownloadProgress(downloadList);
            if (dp.ShowDialog() == DialogResult.OK)
            {
                //
                if (DialogResult.Cancel == dp.ShowDialog())
                {
                    return;
                }
                //Update successfully?
                //这里更新没有把后添加的资源也更新进配置文件，所以另找方法。
                //config.SaveConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.FILENAME));
                //服务器上放置一配置文件，更新文件和服务器xml内容相同，下载服务器上配置文件替换客户端上的配置文件
                clientDownload = new WebClient();
                //Added the function to support proxy
                clientDownload.Proxy = System.Net.WebProxy.GetDefaultProxy();
                clientDownload.Proxy.Credentials = CredentialCache.DefaultCredentials;
                clientDownload.Credentials = System.Net.CredentialCache.DefaultCredentials;
                //End added
                try
                {
                    string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.FILENAME);
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    DownloadFileInfo file = this.downloadConfigList[0];
                    //clientDownload.DownloadFileAsync(new Uri(config.ConfigUrl), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.FILENAME), file);
                    //clientDownload.DownloadFile(config.ConfigUrl, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.FILENAME));
                    byte[] byteArr = clientDownload.DownloadData(config.ConfigUrl);
                    //byte[] byteArr = clientDownload.DownloadData("http://139.196.58.114:8083//11.txt");
                    FileStream fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.FILENAME), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Write(byteArr, 0, byteArr.Length);
                    fs.Close();
                }
                catch (Exception ex)
                { 
                    
                }
                if (bNeedRestart)
                {
                    //Delete the temp folder
                    Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.TEMPFOLDERNAME), true);

                    MessageBox.Show(ConstFile.APPLYTHEUPDATE, ConstFile.MESSAGETITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start(Application.StartupPath + "\\updateSKOD.exe",""); //用另外一个exe程序来更新本身exe程序,第二个参数是服务器路径
                    Environment.Exit(0);
                    //Application.Exit();
                    //CommonUnitity.RestartApplication();
                }
            }
        }

        private Dictionary<string, RemoteFile> ParseRemoteXml(string xml)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xml);
            Dictionary<string, RemoteFile> list = new Dictionary<string, RemoteFile>();
            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                list.Add(node.Attributes["path"].Value, new RemoteFile(node));
            }
            return list;
        }

      
        #endregion

    }

}
