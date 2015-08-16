using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class Group
    {
        [Key]
        public int ID { get; set; }
        public String ID_GROUP { get; set; }
        public int ID_AC { get; set; }
        public Group(String id_gr, int id_ac)
        {
            this.ID_GROUP = id_gr;
            this.ID_AC = id_ac;
        }
        public Group()
        {

        }
    }
}
