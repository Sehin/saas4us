using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class Tag
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }

        public Tag(String name)
        {
            this.name = name;
        }
        public Tag()
        { }
    }
}