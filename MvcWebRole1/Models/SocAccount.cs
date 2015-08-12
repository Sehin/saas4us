using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace MvcWebRole1.Models
{
    public class SocAccount
    {
        [Key, Required]
        public int ID_AC { get; set; }
        public int SOCNET_TYPE { get; set; }
        public string TOKEN { get; set; }
        public int ID_USER { get; set; }
    }
    public class socInfo
    {
        public socInfo(string name, string url, int type)
        {
            this.name = name; this.url = url; this.type = type;
        }
        public string name;
        public string url;
        public int type;
    }

    public static class Facebook
    {
        public static String getUserName(string token)
        {
            String name;
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;

            name = wc.DownloadString("https://graph.facebook.com/me?access_token=" + token);
            JObject test = JObject.Parse(name);

            return (string)test["name"];
        }

        public static String getUserURL(string token)
        {
            String name;
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;

            name = wc.DownloadString("https://graph.facebook.com/me?access_token=" + token);
            JObject test = JObject.Parse(name);

            return (string)test["link"];
        }
    }
}