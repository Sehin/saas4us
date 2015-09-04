﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public MarkProgram(String name,int ID_USER)
        {
            this.name = name;
            this.ID_USER = ID_USER;
        }
    }
    public class T1Trigger
    {
        [Key]
        public int ID_TT1 { get; set; }
        public int ID_PR { get; set; }
        public int CL_TYPE { get; set; }
        public int CL_AGE { get; set; }
        public int CL_SEX { get; set; }
        public int CL_AGE_SIGN { get; set; }
    }
    public class T3Trigger
    {
        [Key]
        public int ID_TT3 { get; set; }
        public int ID_PR { get; set; }
        public String TOKEN { get; set; }
        public T3Trigger() { }
        public T3Trigger(int ID_PR, String TOKEN)
        {
            this.ID_PR = ID_PR;
            this.TOKEN = TOKEN;
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
        public int ID_PR { get; set; }
        public Action() { }
        public Action(int type, int ID_PR)
        {
            this.type = type;
            this.ID_PR = ID_PR;
        }
    }

   public class T1Action
   {
        [Key]
        public int ID_T1A { get; set; }
        public int TYPE { get; set; }
        public int ID_ACTION { get; set; }
        public T1Action() { }
        public T1Action(int TYPE, int ID_ACTION)
        {
            this.TYPE = TYPE;
            this.ID_ACTION = ID_ACTION;
        }
   }

    [Table("T2Actions")]
    public class T2Action
    {
        [Key]
        public int ID_T2A { get; set; }
        public int ID_CO { get; set; }
        public int ID_ACTION { get; set; }
        public T2Action() { }
        public T2Action(int ID_CO, int ID_ACTION)
        {
            this.ID_CO = ID_CO;
            this.ID_ACTION = ID_ACTION;
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
    public class T2Arrow
    {
        [Key]
        public int ID_T2AR { get; set; }
        public int ID_ARROW { get; set; }
        public int HOURS { get; set; }
        public T2Arrow() { }
        public T2Arrow(int ID_ARROW, int HOURS)
        {
            this.ID_ARROW = ID_ARROW;
            this.HOURS = HOURS;
        }


    }

}