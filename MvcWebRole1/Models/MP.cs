using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class MarkProgram
    {
        [Key]
        public int ID_PR { get; set; }
        public int ID_USER { get; set; }
        public String name { get; set; }
        public MarkProgram() { }
        public MarkProgram(String name)
        {
            this.name = name;
        }
    }
    public class Mission
    {
        [Key]
        public int ID_M { get; set; }
        public int ID_PR { get; set; }
        public int type { get; set; }
        public Mission() { }
        public Mission(int ID_PR, int type)
        {
            this.ID_PR = ID_PR;
            this.type = type;
        }
    }
    
    public class Action
    {
        [Key]
        public int ID_ACTION { get; set; }
        public int ID_PR { get; set; }
        public int type { get; set; }
        public Action() { }
        public Action(int ID_PR, int type)
        {
            this.ID_PR = ID_PR;
            this.type = type;
        }
    }
    public class Arrow
    {
        [Key]
        public int ID_AR { get; set; }
        public int ID_FROM { get; set; }
        public int ID_TO { get; set; }
        public DateTime time { get; set; }
        public Arrow() { }
        public Arrow(int ID_FROM, int ID_TO, DateTime time)
        {
            this.ID_FROM = ID_FROM;
            this.ID_TO = ID_TO;
            this.time = time;
        }
    }

}