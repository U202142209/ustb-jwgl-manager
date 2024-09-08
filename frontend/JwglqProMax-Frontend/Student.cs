using Newtonsoft.Json.Linq;

namespace JwglqProMax_Frontend
{
    class Student
    {
        public static string name = "";
        public static string personlnfo_class = "";
        public static string personlnfo_grade = "";
        public static string personlnfo_major = "";
        public static string email = "";
        /// <summary>
        /// 设置登录成功的参数
        /// </summary>
        /// <param name="res"></param>
        public static int SetLoginParames(JObject res)
        {
            Configration.Cookie = (string)res["data"]["cookie_str"];
            Configration.Token = (string)res["data"]["data"]["token"];
            Configration.Logined = true;
            Student.name = (string)res["data"]["data"]["name"];
            JObject personInfo = JObject.Parse(res["data"]["data"]["personInfo"].ToString());
            Student.personlnfo_class = (string)personInfo["class"];
            Student.personlnfo_grade = (string)personInfo["grade"];
            Student.personlnfo_major = (string)personInfo["major"];
            // 向sqlite数据库中写入必要的数据，登录日志
            return new DatabaseManager().InsertLoginLog(Student.name, Configration.Cookie, Configration.Token);
        }
        public static string getEmailByRequest()
        {
            JObject res = JObject.Parse(NetWorkService.Post(Configration.BASICURL + "/getMyEmail/"));
            if ((int)res["code"] == 200)
            {
                return (string)res["data"]["email"];
            }
            return "";
        }
    }
}
