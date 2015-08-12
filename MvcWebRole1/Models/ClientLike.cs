using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class ClientLike
    {
        [Key]
        public int ID_CLL { get; set; }
        public int ID_CIG { get; set; }
        public int ID_CL { get; set; }
        public ClientLike()
        {

        }
        public ClientLike(int ID_CIG,int ID_CL)
        {
            this.ID_CIG = ID_CIG;
            this.ID_CL = ID_CL;
        }
    }
}