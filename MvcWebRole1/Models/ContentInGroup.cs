using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class ContentInGroup
    {
        [Key]
        public int ID_CIG { get; set; }
        public int ID_GROUP { get; set; }
        public int ID_CO { get; set; }
        public int ID_POST { get; set; }
        public DateTime POST_TIME { get; set; }
        public int POST_VIEWS { get; set; }
        public int POST_COMMENTS { get; set; }
        public ContentInGroup()
        {

        }
        public ContentInGroup(int ID_GROUP, int ID_CONTENT, int ID_POST, DateTime POST_TIME)
        {
            this.ID_GROUP = ID_GROUP;
            this.ID_CO = ID_CONTENT;
            this.ID_POST = ID_POST;
            this.POST_TIME = POST_TIME;
        }
    }
}