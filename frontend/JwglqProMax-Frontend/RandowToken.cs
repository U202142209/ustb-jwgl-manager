using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace JwglqProMax_Frontend
{
    class RandowToken
    {
        public static string random_token = "";
        public static string sid= "";
        public static string jsession_id = "";
        public static string server_id = "";
        public static void CreateRandowToken(JObject res)
        {
            RandowToken.random_token = (string)res["data"]["random_token"];
            RandowToken.sid = (string)res["data"]["sid"];
            RandowToken.jsession_id = (string)res["data"]["jsession_id"];
            RandowToken.server_id = (string)res["data"]["server_id"];
        }

        public static Dictionary<string,string> GetPostParams()
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            res.Add("sid", RandowToken.sid);
            res.Add("rand_token", RandowToken.random_token);
            res.Add("jsession_id", RandowToken.jsession_id);
            res.Add("server_id", RandowToken.server_id);
            return res;
        }
    }
}
