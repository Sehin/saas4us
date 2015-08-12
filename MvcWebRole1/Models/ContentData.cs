using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class ContentData
    {
        [Key]
        public int ID_CD { get; set; }
        public string URL { get; set; }
        // DATA_TYPE:
        // 0 - img
        // 1 - audio
        // 2 - video
        public int DATA_TYPE { get; set; }
        public int ID_CO { get; set; }
        public ContentData(int contentId, string url, int type)
        {
            this.ID_CO = contentId;
            this.URL = url;
            this.DATA_TYPE = type;
        }
        public ContentData()
        {

        }
    }
}