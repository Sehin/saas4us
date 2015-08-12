using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class Content
    {
        [Key]
        public int ID_CO { get; set; }
        public int ID_USER { get; set; }
        public string CONTENT_TEXT { get; set; }
        public string CONTENT_TITLE { get; set; }
        /* Content types:
         * 0 - article
         * 1 - img
         * 2 - video
         */
        public int CONTENT_TYPE { get; set; }
        public Content(int id_user, string text, string title, int type)
        {
            this.ID_USER = id_user;
            this.CONTENT_TEXT = text;
            this.CONTENT_TITLE = title;
            this.CONTENT_TYPE = type;
        }
        public Content(int ID_CO, int id_user, string text, string title, int type)
        {
            this.ID_CO = ID_CO;
            this.ID_USER = id_user;
            this.CONTENT_TEXT = text;
            this.CONTENT_TITLE = title;
            this.CONTENT_TYPE = type;
        }

        public Content()
        { }
    }
}