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
        public int type { get; set; }
        public Action() { }
        public Action(int type)
        {
            this.type = type;
        }
    }

    public class Arrows
    {
        [Key]
        public int ID_ARROW { get; set; }
        public int ID_FROM { get; set; }
        public int ID_TO { get; set; }
        public int ID_PR { get; set; }
        public int TYPE { get; set; }
        public Arrows() { }
        public Arrows(int ID_FROM, int ID_TO, int ID_PR)
        {
            this.ID_FROM = ID_FROM;
            this.ID_TO = ID_TO;
            this.ID_PR = ID_PR;
        }
    }

    public class T1Arrow
    {
        [Key]
        public int ID_T1AR { get; set; }
        public int ID_ARROW { get; set; }
        public double CHANCE { get; set; }
        
        public T1Arrow() { }
        public T1Arrow(int ID_ARROW, float CHANCE)
        {
            this.ID_ARROW = ID_ARROW;
            this.CHANCE = CHANCE;
        }
    }

}