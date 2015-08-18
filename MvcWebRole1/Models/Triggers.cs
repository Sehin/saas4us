using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class Trigger
    {
        [Key]
        public int ID_TR { get; set; }
        public int ID_PR { get; set; }
        public int type { get; set; }
        public Trigger() { }
        public Trigger(int ID_PR, int type)
        {
            this.ID_PR = ID_PR;
            this.type = type;
        }
    }
    public class T1Trigger
    {
        [Key]
        public int ID_TT1 { get; set; }
        public int ID_TR { get; set; }
        public int CL_TYPE { get; set; }
        public int CL_AGE { get; set; }
        public int CL_SEX { get; set; }
        public int CL_AGE_SIGN { get; set; }
    }
}