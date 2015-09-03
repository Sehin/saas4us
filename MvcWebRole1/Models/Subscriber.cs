using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class Subscriber
    {
        [Key]
        public int ID_SUBSCRIBER { get; set; }
        public string EMAIL { get; set; }
        public Subscriber()
        {

        }
        public Subscriber(string Email)
        {
            this.EMAIL = Email;
        }
    }
}