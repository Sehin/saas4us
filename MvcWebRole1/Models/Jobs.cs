using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class Job
    {
        [Key]
        public int ID_JOB { get; set; }
        public String ID { get; set; }
        public int ID_JC { get; set; }
        public Job()
        {

        }
        public Job(String ID, int ID_JC)
        {
            this.ID = ID;
            this.ID_JC = ID_JC;
        }
    }
    public class JobCollection
    {
        [Key]
        public int ID_JC { get; set; }
        public String ID { get; set; }
        public JobCollection()
        {

        }
        public JobCollection(String ID)
        {
            this.ID = ID;
        }
    }
}