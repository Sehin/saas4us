using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class TagsInContent
    {
        [Key]
        public int ID { get; set; }
        public int ID_CO { get; set; }
        public int ID_TAG { get; set; }

        public TagsInContent(int ID_CO, int ID_TAG)
        {
            this.ID_CO = ID_CO;
            this.ID_TAG = ID_TAG;
        }
        public TagsInContent()
        { }
    }
}