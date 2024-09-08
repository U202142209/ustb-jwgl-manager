using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace JwglqProMax_Frontend
{
    class NetWorkService
    {
        public static HttpWebRequest GetPostHttpWebRequest(string url,int timeout= 60000)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.KeepAlive = false;
            req.Headers.Add("Authorization", "Bearer " + Configration.Token); // 添加 Authorization 头部字段
            req.Timeout = timeout; // 设置超时时间为一分钟
            return req;

        }
        /// 使用C# 发起Post网络请求,本程序没有异常处理，需要自己判断
        /// <param name="url">请求的api地址</param>
        public static string Post(string url)
        {
            string result = "";
            HttpWebRequest req = NetWorkService.GetPostHttpWebRequest(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// 使用post方式，携带参数，获取全部字符串
        /// <param name="url">请求后台地址</param>
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = NetWorkService.GetPostHttpWebRequest(url);
            req.ContentType = "application/x-www-form-urlencoded";

            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }

            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());

            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// 发起get请求并获取json数据
        /// <param name="url请求地址"></param>
        /// <returns>Json的字符串形式</returns>
        public string GetJson(string url)
        {
            //访问https需加上这句话 
            //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            //访问http（不需要加上面那句话） 
            WebClient wc = new WebClient();
            // 添加User-Agent头部信息
            wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            wc.Headers.Add("Cookie", Configration.Cookie);
            wc.Credentials = CredentialCache.DefaultCredentials;
            wc.Encoding = Encoding.UTF8;
            string returnText = wc.DownloadString(url);
            //Response.Write(returnText); 
            return returnText;
        }

        public JObject get(string url)
        {
            return JObject.Parse(GetJson(url));
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   // 总是接受  
            return true;
        }

        /// <summary>
        /// 抢课的函数
        /// </summary>
        /// <param name="jx0404id">通知单</param>
        /// <param name="jx02id">课程编号</param>
        /// <returns></returns>
        //public JObject getClass(string jx0404id, string jx02id)
        //{
        //    string url = $"https://jwgl.ustb.edu.cn/xsxk/xsxkoper?jx0404id={jx0404id}&dqjx0502zbid={Config.jx0502zbid}&yjx02id={jx02id}&jx02id={jx02id}";
        //    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
        //    WebClient wc = new WebClient();
        //    wc.Headers.Add("Referer", $"https://jwgl.ustb.edu.cn/xsxk/getkcxxlist.do?dqjx0502zbid={Config.jx0502zbid}&jx02id={jx02id}");
        //    // wc.Headers.Add("Accept", "*/*");
        //    wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        //    wc.Headers.Add("Cookie", Config.Cookie);
        //    wc.Credentials = CredentialCache.DefaultCredentials;
        //    wc.Encoding = Encoding.UTF8;
        //    return JObject.Parse(wc.DownloadString(url));
        //}
        public JObject getClass(string jx0404id, string jx02id)
        {
            string url = $"https://jwgl.ustb.edu.cn/xsxk/xsxkoper?jx0404id={jx0404id}&dqjx0502zbid={Configration.jx0502zbid}&yjx02id={jx02id}&jx02id={jx02id}";

            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            WebClient wc = new WebClient();
            wc.Headers.Add("Referer", $"https://jwgl.ustb.edu.cn/xsxk/getkcxxlist.do?dqjx0502zbid={Configration.jx0502zbid}&jx02id={jx02id}");
            wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            wc.Headers.Add("Cookie", Configration.Cookie);
            wc.Credentials = CredentialCache.DefaultCredentials;
            wc.Encoding = Encoding.UTF8;

            // Set timeout to 3 seconds
            using (var cts = new CancellationTokenSource(Configration.TIMEOUT))
            {
                try
                {
                    byte[] result = wc.DownloadData(url);
                    string json = Encoding.UTF8.GetString(result);
                    return JObject.Parse(json);
                }
                catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
                {
                    // Timeout occurred, return null
                    return null;
                }
            }
        }
    }
}
