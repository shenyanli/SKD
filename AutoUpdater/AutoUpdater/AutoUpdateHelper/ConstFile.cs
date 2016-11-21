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
using System.Linq;
using System.Text;

namespace KnightsWarriorAutoupdater
{
    public class ConstFile
    {
        public const string TEMPFOLDERNAME = "TempFolder";
        public const string CONFIGFILEKEY = "config_";
        public const string FILENAME = "AutoUpdater.config";
        public const string COURSEFILENAME = "CourseUpdater.xml";//2016-08-03 shenyanli 修改后缀名为xml
        public const string CONFIGNAME = "AutoUpdater.txt";
        public const string ROOLBACKFILE = "SKD_Client.exe";
        public const string MESSAGETITLE = "自动更新";
        public const string CANCELORNOT = "客户端程序正在更新，您确认取消吗?";
        public const string APPLYTHEUPDATE = "程序需要重启才能生效，请点击确定按钮重启客户端。";
        public const string NOTNETWORK = "程序更新失败，请尝试再次更新！";
    }
}
