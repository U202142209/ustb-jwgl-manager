using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwglqProMax_Frontend
{
    class Configration
    {
        // 服务器的根地址
        public static string BASICURL = "http://127.0.0.1:8000/api";
        // public static string BASICURL = "http://jwglq.wobushidalao.top/api";

        public static string Cookie = "";
        public static string Name = "";

        public static string LogFilePath = "cookies.txt";
        public static string jx0502zbid = "";
        public static string Token = "";
        // 当前的软件版本
        public static string version = "1.0.0";
        // 抢课时间间隔
        public static int interval = 1000;
        // 抢课提醒
        public static bool IsEmailMessageAlert = false;
        // 超时时间
        public static int TIMEOUT = 5000;
        // 是否等待上一次结束
        public static bool waitingtStop = true;
        // 是否已经登录
        public static bool Logined = false;
        // 教务系统的sis认证地址
        public static string JwSisBaseUrl = "https://sis.ustb.edu.cn";
    }
}
