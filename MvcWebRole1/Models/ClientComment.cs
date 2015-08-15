using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class ClientComment
    {
        [Key]
        public int ID_CLC { get; set; }
        public int ID_CIG { get; set; }
        public int ID_CL { get; set; }
        public int ID_COMMENT { get; set; }
        public ClientComment()
        {

        }
        public ClientComment(int ID_CIG, int ID_CL, int ID_COMMENT)
        {
            this.ID_CIG = ID_CIG;
            this.ID_CL = ID_CL;
            this.ID_COMMENT = ID_COMMENT;
        }
    }
}