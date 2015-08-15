using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class ClientInMP
    {
         [Key]
        public int ID_CIMP { get; set; }
        public int ID_CL { get; set; }
        public int ID_MP { get; set; }
        public int ID_ACTION { get; set; }
        public ClientInMP()
        {

        }
        public ClientInMP(int ID_MP, int ID_CL, int ID_ACTION)
        {
            this.ID_MP = ID_MP;
            this.ID_CL = ID_CL;
            this.ID_ACTION = ID_ACTION;
        }
    }
}